using NetSecAtlas.Ui;

namespace NetDoctor;

static class Program
{
    [STAThread]
    static int Main(string[] args) => AtlasApp.Run(args, () => new MainWindow());
}
