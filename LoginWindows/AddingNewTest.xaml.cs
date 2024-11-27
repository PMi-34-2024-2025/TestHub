using System.ComponentModel;
using System.Windows;
using PE.DesktopApplication.TestHub.BLL;
using PE.DesktopApplication.TestHub.DAL;

namespace PE.DesktopApplication.TestHub.WPF
{
    /// <summary>
    /// Interaction logic for AddingNewTest.xaml
    /// </summary>
    public partial class AddingNewTest : Window
    {
        public AddingNewTest()
        {
            InitializeComponent();
        }
        private void BtnAddQuestion_Click(object sender, RoutedEventArgs e)
        {
            var questionControl = new QuestionControl();
            questionsPanel.Children.Add(questionControl);
        }

        private void BtnCreateTest_Click(object sender, RoutedEventArgs e)
        {
            string testName = txtTestName.Text.Trim();
            if (string.IsNullOrEmpty(testName))
            {
                MessageBox.Show("Введіть назву тесту.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string areaName = txtAreaName.Text.Trim();
            if (string.IsNullOrEmpty(areaName))
            {
                MessageBox.Show("Введіть предметну область тесту.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int minutesToPass;
            try
            {
                minutesToPass = int.Parse(minsToPass.Text);
                if (minutesToPass > 300 || minutesToPass < 5)
                    throw new Exception("invalid time");
            }
            catch (Exception)
            {
                MessageBox.Show("Кількість хвилин повинна бути числом від 5 до 300.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (questionsPanel.Children.Count == 0)
            {
                MessageBox.Show("Тест не містить жодного питання.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var questions = new List<(string QuestionText, Dictionary<string, bool> Answers)>();
            foreach (var child in questionsPanel.Children)
            {
                if (child is QuestionControl questionControl)
                {
                    var questionData = questionControl.GetQuestionData();

                    if (string.IsNullOrEmpty(questionData.QuestionText?.Trim()) || !questionData.Answers.Any())
                        continue;

                    var validAnswers = questionData.Answers
                        .Where(a => !string.IsNullOrEmpty(a.Key?.Trim()))
                        .ToDictionary(a => a.Key, a => a.Value);

                    if (validAnswers.Count == 0)
                    {
                        MessageBox.Show($"Питання \"{questionData.QuestionText}\" не має жодної відповіді. Будь ласка, додайте відповіді.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (!validAnswers.Values.Any(a => a))
                    {
                        MessageBox.Show($"Питання \"{questionData.QuestionText}\" не має жодної правильної відповіді. Будь ласка, виберіть хоча б одну правильну відповідь.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    questions.Add((questionData.QuestionText, validAnswers));
                }
            }

            if (!questions.Any())
            {
                MessageBox.Show("Тест не містить жодного валідного питання.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                int testId = DBInteraction.AddTestWithQuestionsAsync(
                    DBInteraction.GetUserFromDB(UserState.Instance.Id), testName, areaName, minutesToPass, questions);
                if (testId > 0)
                {
                    MessageBox.Show("Тест успішно створено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Не вдалося створити тест. Спробуйте знову.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Сталася помилка під час створення тесту: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            new MainWindowWithTests().Show();
        }
    }
}
