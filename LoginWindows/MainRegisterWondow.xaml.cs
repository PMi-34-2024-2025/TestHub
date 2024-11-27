using PE.DesktopApplication.TestHub.BLL;
using PE.DesktopApplication.TestHub.DAL;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;


namespace PE.DesktopApplication.TestHub.WPF
{
    /// <summary>
    /// Interaction logic for MainRegisterWondow.xaml
    /// </summary>
    public partial class MainRegisterWondow : Window
    {
        private bool isNavigatingToNext;
        public MainRegisterWondow()
        {
            InitializeComponent();
            isNavigatingToNext = false;
        }

        private void RegButton_Click(object sender, RoutedEventArgs e)
        {
            string login = Login.Text;
            string password = Password.Password;
            string name = LastName.Text + " " + FirstName.Text;
            string role = RoleComboBox.Text.Equals("Учень") ? "student" : "tutor";
            if(string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(role))
            {
                MessageBox.Show("Будь ласка, заповніть усі поля!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string theEmailPattern = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                                   + "@"
                                   + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))\z";
            if (!Regex.IsMatch(login, theEmailPattern))
            {
                MessageBox.Show("Логін має містити лише латинські букви і цифри та бути довжиною мінімум 4 символи", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var user = DBInteraction.GetUserFromDB(login, password);
            if(user != null)
            {
                MessageBox.Show("Користувач із таким логіном вже існує", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else
            {
                isNavigatingToNext = true;
                DBInteraction.AddUserToDB(name, login, password, role);
                user = DBInteraction.GetUserFromDB(login, password);
                UserState.Instance.SetUserState(name, login, role, user.user_id);
                //MessageBox.Show("Реєстрація успішна!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information
                var window = new MainWindowWithTests();
                //window.Owner = this;
                window.Show();
                Close();
            }
        }

        private void LogButton_Click(object sender, RoutedEventArgs e)
        {
            isNavigatingToNext = true;
            var loginWindow = new MainLoginWindow();
            loginWindow.Show();
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
