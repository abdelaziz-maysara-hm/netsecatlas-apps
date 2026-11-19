using NetSecAtlas.Ui;

namespace LockoutTracer;

static class Program
{
    [STAThread]
    static int Main(string[] args) => AtlasApp.Run(args, () => new MainWindow());
}
