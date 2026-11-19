using System.Globalization;
using System.Windows;
using System.Windows.Threading;
using NetSecAtlas.Core;

namespace NetSecAtlas.Ui;

public static class AtlasApp
{
    public static string[] Args { get; private set; } = Array.Empty<string>();
    public static string? ScreenshotPath { get; private set; }
    public static bool Demo { get; private set; }

    public static string? Arg(string name)
    {
        int i = Array.IndexOf(Args, name);
        return i >= 0 && i + 1 < Args.Length ? Args[i + 1] : null;
    }

    /// <summary>Entry point shared by every app. Options: --lang ar|en, --demo, --screenshot file.png</summary>
    public static int Run(string[] args, Func<AtlasWindow> create)
    {
        Args = args;
        var lang = Arg("--lang") ?? Settings.Load().Lang ?? (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar" ? "ar" : "en");
        Tr.Ar = lang == "ar";
        ScreenshotPath = Arg("--screenshot");
        Demo = args.Contains("--demo") || ScreenshotPath != null;
        var app = new Application { ShutdownMode = ShutdownMode.OnMainWindowClose };
        app.DispatcherUnhandledException += (_, e) =>
        {
            if (ScreenshotPath != null) { Console.Error.WriteLine(e.Exception); Environment.Exit(3); }
            MessageBox.Show(e.Exception.Message, "NetSec Atlas", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        };
        var w = create();
        if (ScreenshotPath != null)
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(double.TryParse(Arg("--delay"), out var d) ? d : 4) };
            timer.Tick += (_, _) => { timer.Stop(); try { Screenshot.Save(w, ScreenshotPath); } finally { app.Shutdown(); } };
            w.Loaded += (_, _) => timer.Start();
        }
        return app.Run(w);
    }
}

/// <summary>Per-user settings (language only). Stored in %APPDATA%\NetSecAtlas\settings.txt; nothing else is saved.</summary>
public sealed class Settings
{
    public string? Lang { get; set; }
    static string File => System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NetSecAtlas", "settings.txt");
    public static Settings Load()
    {
        try { var s = System.IO.File.Exists(File) ? System.IO.File.ReadAllText(File).Trim() : ""; return new Settings { Lang = s is "ar" or "en" ? s : null }; }
        catch { return new Settings(); }
    }
    public void Save()
    {
        try { Directory.CreateDirectory(System.IO.Path.GetDirectoryName(File)!); System.IO.File.WriteAllText(File, Lang ?? ""); } catch { }
    }
}
