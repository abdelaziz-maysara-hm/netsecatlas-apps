using NetSecAtlas.Ui;

namespace CertExpiry;

static class Program
{
    [STAThread]
    static int Main(string[] args) => AtlasApp.Run(args, () => new MainWindow());
}
