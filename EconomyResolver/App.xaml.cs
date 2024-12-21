using EconomyResolver.BusinessLogic;
using System.Windows;

namespace EconomyResolver
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            МенеджерЛицензий.УстановитьЛицензию();
        }
    }
}
