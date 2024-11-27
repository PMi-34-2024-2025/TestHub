using PE.DesktopApplication.TestHub.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Interaction logic for DeleteAdminWindow.xaml
    /// </summary>
    public partial class DeleteAdminWindow : Window
    {
        public DeleteAdminWindow()
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

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            string login = Login.Text;
            if (string.IsNullOrEmpty(login))
            {
                MessageBox.Show("Будь ласка, заповніть полe!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var user = DBInteraction.SupervisorGetUserFromDB(login);
            if (user == null)
            {
                MessageBox.Show("Не існує користувача з таким логіном", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else
            {
                MessageBoxResult result = MessageBox.Show($"Ви впевнені, що хочете видалити адміна {login}?", "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    
                    DBInteraction.SupervisorDeleteUserByLogin(login);
                    Close();
                }
            }
        }
    }
}
