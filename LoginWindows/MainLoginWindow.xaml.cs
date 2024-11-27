using System.Windows;
using PE.DesktopApplication.TestHub.DAL;
using PE.DesktopApplication.TestHub.BLL;
using System.ComponentModel;


namespace PE.DesktopApplication.TestHub.WPF
{
    /// <summary>
    /// Interaction logic for MainLoginWindow.xaml
    /// </summary>
    public partial class MainLoginWindow : Window
    {
        private bool isNavigatingToNext;
        public MainLoginWindow()
        {
            InitializeComponent();
            isNavigatingToNext = false;
        }

        private void LogButton_Click(object sender, RoutedEventArgs e)
        {
            string login = Login.Text;
            string password = Password.Password;
            var user = DBInteraction.GetUserFromDB(login, password);
            if (user == null)
            {
                MessageBox.Show("Користувач не знайдений або пароль некоректний", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                isNavigatingToNext = true;
                if (user.role == "supervisor")
                {
                    new SupervisorWindow().Show();
                    Close();
                }
                else
                {
                    //MessageBox.Show($"Ласкаво просимо, {user.name}!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                    UserState.Instance.SetUserState(user.name, user.login, user.role, user.user_id);
                    var window = new MainWindowWithTests();
                    window.Show();
                    Close();
                }
            }
        }

        private void RegButton_Click(object sender, RoutedEventArgs e)
        {
            isNavigatingToNext = true;
            var registerWindow = new MainRegisterWondow();
            registerWindow.Show();
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (!isNavigatingToNext)
            {
                new MainWindow().Show();
            }
        }
    }
}
