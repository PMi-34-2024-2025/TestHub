using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using PE.DesktopApplication.TestHub.BLL;
using PE.DesktopApplication.TestHub.DAL;

namespace PE.DesktopApplication.TestHub.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainTestingWindow : Window
    {
        public static double ReturnMark(QuestionWrapper question, List<int> userAnswers)
        {
            if (userAnswers.Count == 0)
            {
                return 0;
            }
            else if (userAnswers.Count == 1 && question.correctAnswers.Count == 1)
            {
                if (userAnswers[0] == question.correctAnswers[0])
                    return 1;
                else
                    return 0;
            }
            else
            {
                double sum = 0;
                int numOfCorrect = question.correctAnswers.Count; 
                foreach (int answer in userAnswers)
                {
                    if (question.correctAnswers.Contains(answer))
                    {
                        sum += 1.0 / numOfCorrect;
                    }
                    else
                    {
                        sum -= 1.0 / numOfCorrect;
                    }
                }
                return Math.Max(0, sum);
            }

        }
        private string? title;
        private string? area;
        private List<QuestionWrapper>questionWrappers = new List<QuestionWrapper>();
        private int minsToPass;
        private DateTime start;
        private DateTime end;
        private int id;
        private DispatcherTimer timer;
        private TimeSpan timeLeft;
        public MainTestingWindow(int id)
        {
            start = DateTime.UtcNow;
            this.id = id;
            minsToPass = DBInteraction.GetTestFromDB(id).mins_to_pass;
            title = DBInteraction.GetTestTitle(id);
            InitializeComponent();
            TestTitle.Content = title;
            AddElementsDependsOnRole();
            LoadDataAsync(id);
            if(UserState.Instance.Role == "user")
                StartTimer(minsToPass);
            else
            {
                TimerTextBox.Visibility = Visibility.Collapsed;
            }
            //imitateGettingFromDB();
        }
       
        private void StartTimer(int totalMinutes)
        {
            timeLeft = TimeSpan.FromMinutes(totalMinutes);
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timeLeft = timeLeft.Subtract(TimeSpan.FromSeconds(1));
            TimerTextBox.Text = $"Залишилось часу: {timeLeft:hh\\:mm\\:ss}";
            if (timeLeft.TotalMinutes <= 5.02 && timeLeft.TotalMinutes > 4.98) // Приблизно 5 хвилин
            {
                MessageBox.Show("Залишилось 5 хвилин!", "Увага", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            if (timeLeft.TotalSeconds <= 0)
            {
                timer.Stop();
                MessageBox.Show("Час вийшов!", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AddElementsDependsOnRole()
        {
            switch (UserState.Instance.Role)
            {
                case "student":
                    Reject.Visibility = Visibility.Collapsed;//add Button to pass the tests (handler) (get all marks and open the window to show the result (maybe with animation)) + like addition add part to see results
                    Accept.Visibility = Visibility.Collapsed;
                    resultsGrid.Visibility = Visibility.Collapsed;
                    break;
                case "tutor":
                    Reject.Visibility = Visibility.Collapsed;//add Button to pass the tests (handler) (get all marks and open the window to show the result (maybe with animation)) + like addition add part to see results
                    Accept.Visibility = Visibility.Collapsed;
                    Finish.Visibility = Visibility.Collapsed;
                    LoadTeacherControls();
                    break;     //add table of users who passed it
                case "admin":
                    Finish.Visibility = Visibility.Collapsed;
                    resultsGrid.Visibility = Visibility.Collapsed;
                    break;     //add 2 buttons - reject and accept and it handlers
            }
        }

        private void imitateGettingFromDB()
        {
            title = "Sample Test";
            area = "General Knowledge";
            minsToPass = 15;

            questionWrappers.Add(new QuestionWrapper
            {
                QuestionText = "What is 2 + 2?",
                Answers = new List<AnswerWrapper>
                    {
                        new AnswerWrapper { AnswerText = "3", IsCorrect = false },
                        new AnswerWrapper { AnswerText = "4", IsCorrect = true },
                        new AnswerWrapper { AnswerText = "5", IsCorrect = false }
                    }
            });
            questionWrappers.Add(new QuestionWrapper
            {
                QuestionText = "What is the capital city?",
                Answers = new List<AnswerWrapper>
            {
                new AnswerWrapper { AnswerText = "Berlin", IsCorrect = true },
                new AnswerWrapper { AnswerText = "Paris", IsCorrect = true },
                new AnswerWrapper { AnswerText = "Madrid", IsCorrect = true }
            }
            });
            questionWrappers.Add(new QuestionWrapper
            {
                QuestionText = "What is the square root of 16?",
                Answers = new List<AnswerWrapper>
            {
                new AnswerWrapper { AnswerText = "2", IsCorrect = false },
                new AnswerWrapper { AnswerText = "4", IsCorrect = true },
                new AnswerWrapper { AnswerText = "8", IsCorrect = false }
            }
            });
            questionWrappers.Add(new QuestionWrapper
            {
                QuestionText = "What is the largest planet in the solar system?",
                Answers = new List<AnswerWrapper>
                {
                    new AnswerWrapper { AnswerText = "Earth", IsCorrect = false },
                    new AnswerWrapper { AnswerText = "Mars", IsCorrect = false },
                    new AnswerWrapper { AnswerText = "Jupiter", IsCorrect = true }
                }
            });
            questionWrappers.Add(new QuestionWrapper
            {
                QuestionText = "Which element has the chemical symbol 'O'?",
                Answers = new List<AnswerWrapper>
            {
                new AnswerWrapper { AnswerText = "Oxygen", IsCorrect = true },
                new AnswerWrapper { AnswerText = "Gold", IsCorrect = false },
                new AnswerWrapper { AnswerText = "Osmium", IsCorrect = false }
            }
            });
            

            // Генерація обгорток і заповнення полів
            foreach (var question in questionWrappers)
            {
                AddTestQuestion(question);
            }
        }

        private void LoadDataAsync(int id)
        {
            var allContent = DBInteraction.GetTestByIdAsync(id);
            if(allContent == null)
            {
                MessageBox.Show("There is no info about test #" +  id);
                return;
            }
            title = allContent.Title;
            area = allContent.Area;
            minsToPass = allContent.MinutesToPass;
            questionWrappers = allContent.Questions.Select(
                q => new QuestionWrapper()
                {
                    QuestionText = q.QuestionText,
                    Answers = q.Answers.Select(
                        a => new AnswerWrapper()
                        {
                            AnswerText = a.AnswerText,
                            IsCorrect = a.IsCorrect,
                        }).ToList()
                }).ToList();
            foreach(QuestionWrapper questionWrapper in questionWrappers)
            {
                AddTestQuestion(questionWrapper);
            }
        }

        private void AddTestQuestion(QuestionWrapper question)
        {
            question.SetFields();
            UserControl2 test = new UserControl2(question.QuestionText, question.variantsText, question.correctAnswers);
            MainContent.Children.Add(test);
        }

        private void LoadTeacherControls()
        {
            resultsGrid.Items.Clear();
            var attempts = DBInteraction.getAttempts(id);
            foreach (var attempt in attempts)
            {
                resultsGrid.Items.Add(new
                {
                    StudentName = DBInteraction.GetUserFromDB(attempt.user_id).name,
                    TimeSpent = (attempt.end_time - attempt.start_time),
                    Score = attempt.score,
                    AttemptId = attempt.attempt_id
                });
            }
        }

        private void RowButton_Click(object sender,  RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                try
                {
                    var context = button.DataContext;
                    int attemptId = (int)context.GetType().GetProperty("AttemptId").GetValue(context);
                    var attempt = DBInteraction.GetAttemptById(attemptId);
                    string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TestHubDownloads");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string filePath = Path.Combine(folderPath, $"Attempt_{attemptId}.txt");
                    File.WriteAllText(filePath, $"Результати спроби ID {attemptId}\nСтудент: {DBInteraction.GetUserFromDB(attempt.user_id).name}\nОцінка: {attempt.score}\n {attempt.user_report}");

                    MessageBox.Show($"Файл успішно збережено в {filePath}", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка при збереженні файлу: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Не вдалося визначити спробу для завантаження.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }



        private void LoadResultsForTestWrapper(string testName)
        {
            if (resultsGrid == null) return;

            var testResults = new List<TestResult>
            {
                new TestResult { StudentName = "Іван Іванов", TimeSpent = "30 хвилин", Score = "80" },
                new TestResult { StudentName = "Олена Петрова", TimeSpent = "45 хвилин", Score = "95" },
                new TestResult { StudentName = "Микола Сидоров", TimeSpent = "25 хвилин", Score = "70" }
            };

            resultsGrid.ItemsSource = testResults;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if(UserState.Instance.Role == "user")
            {
                MessageBoxResult result = MessageBox.Show("Ви впевнені, що хочете завершити тест?", "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true; 
                }
                else
                {
                    end = DateTime.UtcNow;
                    double totalScore = 0;
                    double maxScore = questionWrappers.Count;
                    string report = "";

                    for (int i = 0; i < questionWrappers.Count; i++)
                    {
                        var questionData = questionWrappers[i];
                        var userAnswerControl = (UserControl2)MainContent.Children[i];
                        var userAnswers = userAnswerControl.GetUserAnswers();
                        double mark = ReturnMark(questionData, userAnswers);
                        totalScore += mark;
                        report += userAnswerControl.GetTestReport();
                    }
                    DBInteraction.AddAttemptToDB(
                        DBInteraction.GetUserFromDB(UserState.Instance.Id), DBInteraction.GetTestFromDB(id), 1, start, end, (decimal)totalScore, report);
                    ShowingResultWindow resultWindow = new ShowingResultWindow(totalScore, maxScore);

                    resultWindow.ShowDialog();
                }
            }
            else
            {
                new MainWindowWithTests().Show();
            }
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            var testStatus = DBInteraction.GetTestStatusFromDB(id);
            if (testStatus == null || testStatus.status == "pending")
            {
                DBInteraction.UpdateTestStatus(id, "approved", DBInteraction.GetUserFromDB(UserState.Instance.Id));
                Close();
            }
            else
            {
                MessageBox.Show("Інший адмін вас випередив!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                Close();
            }
        }

        private void Reject_Click(object sender, RoutedEventArgs e)
        {
            var testStatus = DBInteraction.GetTestStatusFromDB(id);
            if (testStatus == null || testStatus.status == "pending")
            {
                DBInteraction.UpdateTestStatus(id, "rejected", DBInteraction.GetUserFromDB(UserState.Instance.Id));
                Close();
            }
            else
            {
                MessageBox.Show("Інший адмін вас випередив!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                Close();
            }
        }
    }

    // Клас для збереження результатів
    
}
