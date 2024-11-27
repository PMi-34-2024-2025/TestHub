using System.Windows;
using System.Windows.Controls;

namespace PE.DesktopApplication.TestHub.WPF
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public string topic;
        MainWindowWithTests parent;
        public UserControl1(string title, string description, int id, string role = "student", string status = "pending", string topic = "Всі", MainWindowWithTests parent = null)
        {
            InitializeComponent();
            Status.Content = status;
            if(role != "tutor")
            {
                Status.Visibility = Visibility.Collapsed;
            }
            Title.Content = title;
            Description.Content = description;
            ID.Content = "ID: " + id;
            srartTestButton.Tag = id;
            srartTestButton.Content = role == "student" ? "Почати тест" : role == "admin" ? "Інспектувати" : "Інфо";
            this.topic = topic;
            this.parent = parent;
        }

        public void srartTestButton_Click(object sender, RoutedEventArgs e)
        {
            parent.isNavigatingToNext = true;
            int id = (int)((Button)sender).Tag;
            MainTestingWindow window = new MainTestingWindow(id);
            //TestingPassWindow.MainWindow window = new TestingPassWindow.MainWindow();
            window.Show();
            parent.Close();
        }

        //rebuild this controler and depends on the different role will be different fields visibility.
    }

}
