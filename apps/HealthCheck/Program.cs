using NetSecAtlas.Ui;

namespace HealthCheck;

static class Program
{
    [STAThread]
    static int Main(string[] args) => AtlasApp.Run(args, () => new MainWindow());
}
