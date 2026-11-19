using System.Windows;
using System.Windows.Controls;
using NetSecAtlas.Core;
using NetSecAtlas.Ui;

namespace PasswordCheck;

public sealed class MainWindow : AtlasWindow
{
    static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(15) };
    string _pw = "";
    bool _show;
    string? _breach; Level _breachLevel = Level.Info;

    public MainWindow() : base("Password Checker", "فاحص كلمات المرور",
        "Strength is estimated on this PC. The optional breach check sends only 5 characters of a hash.", "تُقدَّر القوة على جهازك. فحص التسريب الاختياري يرسل 5 أحرف فقط من البصمة.", "password-checker") { }

    protected override Task OnDemoAsync() { _pw = "Summer2026!"; _show = true; Rebuild(); return Task.CompletedTask; }

    protected override UIElement BuildBody()
    {
        Control box;
        if (_show) { var t = U.Input(_pw); t.Width = 420; t.FlowDirection = FlowDirection.LeftToRight; t.TextChanged += (_, _) => { _pw = t.Text; _breach = null; Refresh(); }; box = t; }
        else { var p = new PasswordBox { Password = _pw, Width = 420, Padding = new Thickness(6, 4, 6, 4), Margin = new Thickness(0, 0, 8, 8), FlowDirection = FlowDirection.LeftToRight };
               p.PasswordChanged += (_, _) => { _pw = p.Password; _breach = null; Refresh(); }; box = p; }
        var show = new CheckBox { Content = Tr.T("Show", "إظهار"), IsChecked = _show, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 0, 8) };
        show.Click += (_, _) => { _show = show.IsChecked == true; Rebuild(); };
        _result = new ContentControl { Content = ResultView() };
        return U.Scroll(U.Stack(
            U.Card(U.Stack(U.H(Tr.T("Password to test", "كلمة المرور المراد اختبارها")), U.Row(box, show),
                U.P(Tr.T("Nothing is saved. Tip: test a password similar to yours rather than the real one if you prefer.", "لا يتم حفظ أي شيء. تلميح: يمكنك اختبار كلمة مشابهة لكلمتك بدل الحقيقية."), U.Muted))),
            _result,
            U.Card(U.Stack(U.H(Tr.T("Has it appeared in a data breach?", "هل ظهرت في تسريب بيانات؟")),
                U.P(Tr.T("Uses the Have I Been Pwned Pwned Passwords range API (k-anonymity): the PC sends the first 5 characters of the SHA-1 hash and compares the returned list locally. The password itself never leaves this PC.",
                         "يستخدم واجهة Pwned Passwords من Have I Been Pwned بطريقة k-anonymity: يرسل الجهاز أول 5 أحرف من بصمة SHA-1 فقط ويقارن القائمة المُعادة محليًا. كلمة المرور نفسها لا تغادر جهازك."), U.Muted),
                U.Row(U.AsyncBtn(Tr.T("Check breaches", "افحص التسريبات"), CheckBreachAsync, true)),
                _breach is null ? new TextBlock() : U.FindingRow(_breachLevel, _breach, ""))),
            U.Row(U.Link(Tr.T("How to make passwords you can remember", "كيف تنشئ كلمات مرور يسهل تذكرها"), Links.Article("nist-password-guidelines-vs-old-rules")),
                  new TextBlock { Width = 20 }, U.Link(Tr.T("Start using a password manager", "ابدأ باستخدام مدير كلمات المرور"), Links.Article("password-manager-start")))));
    }

    ContentControl? _result;
    void Refresh() { if (_result != null) _result.Content = ResultView(); }

    UIElement ResultView()
    {
        var r = PasswordAnalyzer.Analyze(_pw);
        var lvl = r.Score switch { <= 1 => Level.Bad, 2 => Level.Warn, _ => Level.Ok };
        var meter = new ProgressBar { Height = 10, Maximum = 4, Value = r.Score + (r.Length > 0 ? 0.3 : 0), Foreground = U.LevelBrush(lvl), Margin = new Thickness(0, 4, 0, 10) };
        var issues = new StackPanel();
        foreach (var i in r.Issues) issues.Children.Add(new TextBlock { Text = "• " + i, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 0, 0, 3) });
        return U.Card(U.Stack(
            U.Row(U.Badge(lvl, r.Length == 0 ? "—" : r.ScoreLabel), new TextBlock { Text = Tr.T($"{r.Length} characters · ~{r.EntropyBits:N0} bits", $"{r.Length} حرفًا · ~{r.EntropyBits:N0} بت"), Foreground = U.Muted, VerticalAlignment = VerticalAlignment.Center }),
            meter,
            U.P(Tr.T($"Time to guess: {r.OnlineCrackTime} against a rate-limited login; {r.OfflineCrackTime} if a fast password hash leaks.",
                     $"زمن التخمين: {r.OnlineCrackTime} عبر صفحة دخول محدودة المحاولات؛ و{r.OfflineCrackTime} إذا تسرّبت بصمة سريعة.")),
            issues));
    }

    async Task CheckBreachAsync()
    {
        if (_pw.Length == 0) return;
        SetStatus(Tr.T("Contacting api.pwnedpasswords.com (5-character prefix only)…", "الاتصال بـ api.pwnedpasswords.com (أول 5 أحرف فقط)…"));
        try
        {
            var n = await PwnedPasswords.CheckAsync(Http, _pw);
            (_breachLevel, _breach) = n > 0
                ? (Level.Bad, Tr.T($"Found {n:N0} times in breach data. Do not use this password anywhere.", $"ظهرت {n:N0} مرة في بيانات مسربة. لا تستخدمها في أي مكان."))
                : (Level.Ok, Tr.T("Not found in known breach data. That alone does not make it strong.", "لم تظهر في البيانات المسربة المعروفة، لكن هذا وحده لا يجعلها قوية."));
            SetStatus(Tr.T("Breach check done.", "اكتمل فحص التسريب."));
        }
        catch (Exception ex) { (_breachLevel, _breach) = (Level.Unknown, Tr.T("Could not reach the service: ", "تعذر الوصول للخدمة: ") + ex.GetBaseException().Message); }
        Rebuild();
    }
}
