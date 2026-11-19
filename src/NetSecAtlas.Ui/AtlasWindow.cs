using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NetSecAtlas.Core;

namespace NetSecAtlas.Ui;

/// <summary>Base window: branded header, language switch (rebuilds UI with RTL), guide link and status bar.</summary>
public abstract class AtlasWindow : Window
{
    readonly string _titleEn, _titleAr, _taglineEn, _taglineAr, _programSlug;
    readonly TextBlock _status = new() { Margin = new Thickness(12, 6, 12, 6), Foreground = U.Muted, TextTrimming = TextTrimming.CharacterEllipsis };

    protected AtlasWindow(string titleEn, string titleAr, string taglineEn, string taglineAr, string programSlug, double width = 980, double height = 720)
    {
        _titleEn = titleEn; _titleAr = titleAr; _taglineEn = taglineEn; _taglineAr = taglineAr; _programSlug = programSlug;
        Width = width; Height = height; MinWidth = 640; MinHeight = 480;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        FontFamily = new FontFamily("Segoe UI"); FontSize = 14;
        Background = U.Bg;
        try { Icon = BitmapFrame.Create(new Uri("pack://application:,,,/NetSecAtlas.Ui;component/atlas.png")); } catch { }
        Loaded += async (_, _) => { if (AtlasApp.Demo) await OnDemoAsync(); };
        Rebuild();
    }

    /// <summary>Builds the app body. Called again after a language switch, so keep state in fields.</summary>
    protected abstract UIElement BuildBody();

    /// <summary>Fills sample data for screenshots/--demo. Must not touch the network unless the feature needs it.</summary>
    protected virtual Task OnDemoAsync() => Task.CompletedTask;

    public void SetStatus(string text) => Dispatcher.Invoke(() => _status.Text = text);

    protected void Rebuild()
    {
        Title = $"{Tr.T(_titleEn, _titleAr)} — NetSec Atlas";
        FlowDirection = Tr.Ar ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
        var root = new DockPanel();

        var header = new Border { Background = U.Brand, Padding = new Thickness(18, 12, 18, 12) };
        var hg = new Grid();
        hg.ColumnDefinitions.Add(new ColumnDefinition());
        hg.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        var titles = new StackPanel();
        titles.Children.Add(new TextBlock { Text = Tr.T(_titleEn, _titleAr), FontSize = 22, FontWeight = FontWeights.SemiBold, Foreground = Brushes.White });
        titles.Children.Add(new TextBlock { Text = Tr.T(_taglineEn, _taglineAr), Foreground = new SolidColorBrush(Color.FromRgb(0xCF, 0xE3, 0xF2)), TextWrapping = TextWrapping.Wrap });
        hg.Children.Add(titles);
        var actions = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };
        actions.Children.Add(U.HeaderButton(Tr.T("Guide", "الدليل"), () => U.Open(Links.Program(_programSlug))));
        actions.Children.Add(U.HeaderButton(Tr.Ar ? "English" : "عربي", () =>
        {
            Tr.Ar = !Tr.Ar; new Settings { Lang = Tr.Lang }.Save(); Rebuild();
        }));
        actions.Children.Add(U.HeaderButton(Tr.T("About", "حول"), ShowAbout));
        Grid.SetColumn(actions, 1);
        hg.Children.Add(actions);
        header.Child = hg;
        DockPanel.SetDock(header, Dock.Top);
        root.Children.Add(header);

        var footer = new Border { BorderBrush = U.Line, BorderThickness = new Thickness(0, 1, 0, 0), Background = Brushes.White, Child = _status };
        if (_status.Parent is Border old) old.Child = null;
        footer.Child = _status;
        DockPanel.SetDock(footer, Dock.Bottom);
        root.Children.Add(footer);

        var body = BuildBody();
        root.Children.Add(new Border { Padding = new Thickness(16), Child = body });
        Content = root;
        if (string.IsNullOrEmpty(_status.Text)) _status.Text = Tr.T("Ready. No telemetry; nothing leaves this PC unless a feature says so.", "جاهز. لا توجد أي قياسات استخدام؛ لا يخرج شيء من جهازك إلا إذا ذكرت الميزة ذلك.");
    }

    void ShowAbout()
    {
        var v = typeof(AtlasWindow).Assembly.GetName().Version;
        MessageBox.Show(this, Tr.T(
            $"{_titleEn} {v?.ToString(3)}\nFree and open source (MIT) from NetSec Atlas.\nNo telemetry, no ads, no accounts.\n\nSource: https://github.com/abdelaziz-maysara-hm/netsecatlas-apps\nGuide: {Links.Program(_programSlug)}",
            $"{_titleAr} {v?.ToString(3)}\nبرنامج مجاني ومفتوح المصدر (MIT) من NetSec Atlas.\nبدون قياسات استخدام أو إعلانات أو حسابات.\n\nالمصدر: https://github.com/abdelaziz-maysara-hm/netsecatlas-apps\nالدليل: {Links.Program(_programSlug)}"),
            Tr.T("About", "حول"), MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
