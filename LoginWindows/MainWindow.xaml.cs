using System.Windows;


namespace PE.DesktopApplication.TestHub.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void exitAppButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void startAppButton_Click(object sender, RoutedEventArgs e)
        {
            new MainLoginWindow().Show();
            Close();
        }
    }
}