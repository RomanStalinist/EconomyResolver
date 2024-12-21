using System.ComponentModel;
using System.Windows;

namespace EconomyResolver.Views
{
    public partial class MainWindow
    {
        public MainWindow() => InitializeComponent();

        private void Window_Closing(object sender, CancelEventArgs e)
            => e.Cancel = MessageBox.Show("Вы уверены, что хотите закрыть программу?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes;
    }
}
