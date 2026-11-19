using NetSecAtlas.Ui;

namespace PingMonitor;

static class Program
{
    [STAThread]
    static int Main(string[] args) => AtlasApp.Run(args, () => new MainWindow());
}
