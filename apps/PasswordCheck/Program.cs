using NetSecAtlas.Ui;

namespace PasswordCheck;

static class Program
{
    [STAThread]
    static int Main(string[] args) => AtlasApp.Run(args, () => new MainWindow());
}
