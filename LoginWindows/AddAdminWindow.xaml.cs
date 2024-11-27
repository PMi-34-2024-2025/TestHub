using PE.DesktopApplication.TestHub.BLL;
using PE.DesktopApplication.TestHub.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PE.DesktopApplication.TestHub.WPF
{
    /// <summary>
    /// Interaction logic for AddAdminWindow.xaml
    /// </summary>
    public partial class AddAdminWindow : Window
    {
        public AddAdminWindow()
        {
            InitializeComponent();
        }

        private void RejectButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            new SupervisorWindow().Show();
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            string login = Login.Text;
            string password = Password.Password;
            string name = "Admin";
            string role = "admin";
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
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
            if (user != null)
            {
                MessageBox.Show("Користувач із таким логіном вже існує", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else
            {
                DBInteraction.AddUserToDB(name, login, password, role);
                Close();
            }
        }
    }
}
