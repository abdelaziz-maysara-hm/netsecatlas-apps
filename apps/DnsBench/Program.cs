using NetSecAtlas.Ui;

namespace DnsBench;

static class Program
{
    [STAThread]
    static int Main(string[] args) => AtlasApp.Run(args, () => new MainWindow());
}
