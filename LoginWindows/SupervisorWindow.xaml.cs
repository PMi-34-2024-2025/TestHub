using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PE.DesktopApplication.TestHub.WPF
{
    /// <summary>
    /// Interaction logic for SupervisorWindow.xaml
    /// </summary>
    public partial class SupervisorWindow : Window
    {
        private bool isNavigatingToNext;
        public SupervisorWindow()
        {
            isNavigatingToNext = false;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            isNavigatingToNext = true;
            new AddAdminWindow().Show();
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            isNavigatingToNext = true;
            new DeleteAdminWindow().Show();
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (!isNavigatingToNext)
            {
                new MainLoginWindow().Show();
            }
        }
    }
}
