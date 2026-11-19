using NetSecAtlas.Ui;

namespace PortCheck;

static class Program
{
    [STAThread]
    static int Main(string[] args) => AtlasApp.Run(args, () => new MainWindow());
}
