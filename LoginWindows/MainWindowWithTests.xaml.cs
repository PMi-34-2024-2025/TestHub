using PE.DesktopApplication.TestHub.BLL;
using PE.DesktopApplication.TestHub.DAL;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace PE.DesktopApplication.TestHub.WPF
{
    /// <summary>
    /// Interaction logic for MainWindowWithTests.xaml
    /// </summary>
    public partial class MainWindowWithTests : Window
    {
        public bool isNavigatingToNext = false;
        private static string defaultTextBoxText = "Вкажіть id, тему чи опис для пошуку";
        private string filter = "Усі|Усі";
        private List<TestWrapper> tests = new List<TestWrapper>();
        private List<string> topics = new List<string>();
        private int minsToPass;
        public MainWindowWithTests()
        {
            isNavigatingToNext = false;
            InitializeComponent();
            //List<string> topics = new List<string>() { "Math", "History", "Englih", "All" };
            topics = DBInteraction.GetAllAreas();
            if (UserState.Instance.Role == "tutor")
                AddButtonForTutor();//логіка для додавання кнопки перед тестами і логіка додавання тестів через допоміжні форми
            AddFilterButtons(topics);
            UploadTests();
            AddAllTests(tests);
        }

        public void AddButtonForTutor()
        {
            if (UserState.Instance.Role == "tutor")
            {
                Button addTest = new Button()
                {
                    Margin = new Thickness(5, 5, 5, 5),
                    Content = "Створити новий тест",
                    Width = 1000,
                    FontSize = 24,
                    Height = 40
                };
                addTest.Click += AddTestButton_Click;
                TestPanel.Children.Add(addTest);
            }
        }

        private void AddTestButton_Click(object sender, RoutedEventArgs e)
        {
            isNavigatingToNext = true;
            new AddingNewTest().Show();
            Close();
        }

        public void UploadTests()
        {
            tests.Clear();
            var testsFromDb = DBInteraction.GetTestListForUserAsync(DBInteraction.GetUserFromDB(UserState.Instance.Id));
            foreach (var test in testsFromDb)
            {
                tests.Add(new TestWrapper(test.Title, test.Description, test.Id, UserState.Instance.Role, test.Status, test.Area));
            }
        }

        public void UploadTestsWrapper()
        {
            tests.Add(new TestWrapper("Test1", "test1", 1));
            tests.Add(new TestWrapper("Test2", "test2", 2));
        }

        public void RebuildFormAfterFilter()
        {
            TestPanel.Children.Clear();
            if (UserState.Instance.Role == "tutor")
                AddButtonForTutor();
            if (filter.Equals("Усі|Усі") || filter.Equals("Усі|Вкажіть id, тему чи опис для пошуку"))
            {
                AddAllTests(tests);
            }
            else
            {
                List<TestWrapper> filteredPart = tests.ToList();
                string topicFilter = filter.Split("|")[0];
                string searchFilter = filter.Split("|")[1];
                if (topicFilter != "Усі")
                {
                    filteredPart = filteredPart.Where(test => test.topic.ToLower() == topicFilter.ToLower()).ToList();
                }
                if (!searchFilter.Equals("Усі"))
                {
                    filteredPart = filteredPart.Where(test =>
                                    (test.title != null && test.title.Contains(searchFilter, StringComparison.OrdinalIgnoreCase)) ||
                                    (test.description != null && test.description.Contains(searchFilter, StringComparison.OrdinalIgnoreCase)) ||
                                    test.id.ToString().Contains(searchFilter)).ToList();
                }
                AddAllTests(filteredPart);
            }
        }
        public void AddAllTests(List<TestWrapper> tests)
        {
            foreach (TestWrapper test in tests)
            {
                AddTestToForm(test);
            }
        }

        public void AddFilterButtons(List<string> topics)
        {
            TopicPanel.Children.Clear();
            TopicPanel.Children.Add(new Label()
            {
                Margin = new Thickness(3, 0, 3, 0),
                Width = 80,
                Height = 30,
                HorizontalAlignment = HorizontalAlignment.Center,
                Content = "Теми",
                FontSize = 18
            });
            foreach (var item in topics)
            {
                Button button = new Button()
                {
                    Content = item,
                    Width = 190,
                    Height = 40,
                    Margin = new Thickness(3, 3, 3, 3)
                };
                button.Click += FilterButton_Click;
                TopicPanel.Children.Add(button);
            }
            Button allFilterButton = new Button()
            {
                Content = "Усі",
                Width = 190,
                Height = 40,
                Margin = new Thickness(3, 3, 3, 3)
            };
            allFilterButton.Click += FilterButton_Click;
            TopicPanel.Children.Add(allFilterButton);
        }



        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;
            string? topic = button?.Content.ToString();
            filter = topic + "|" + filter.Split("|")[1]; 
        }

        public void AddTestToForm(string title, string description, int id, string role, string status, string area)
        {
            string statusUa = status == "pending" ? "обробляється" : status == "rejected" ? "заблокований" : "активний";
            UserControl1 test = new UserControl1(title, description, id, role, statusUa, area, this)
            {
                Margin = new Thickness(5, 5, 5, 5)
            };
            TestPanel.Children.Add(test);
        }

        public void AddTestToForm(TestWrapper test)
        {
            AddTestToForm(test.title, test.description, test.id, test.role, test.status, test.topic);
        }

        private void Filter_GotFocus(object sender, RoutedEventArgs e)
        {
            Filter.Text = "";
        }

        private void Filter_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Filter.Text.Length == 0)
            {
                Filter.Text = defaultTextBoxText;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Filter.Text.Length != 0)
            {
                filter = filter.Split("|")[0] + "|" + Filter.Text;
                RebuildFormAfterFilter();
            }
        }

        private void quitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void reloadButton_Click(object sender, RoutedEventArgs e)
        {
            UploadTests();
            RebuildFormAfterFilter();
            topics.Clear();
            topics = DBInteraction.GetAllAreas();
            AddFilterButtons(topics);
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
