using System.Windows;
using System.Windows.Controls;

namespace PE.DesktopApplication.TestHub.WPF
{
    /// <summary>
    /// Interaction logic for QuestionControl.xaml
    /// </summary>
    public partial class QuestionControl : UserControl
    {
        public QuestionControl()
        {
            InitializeComponent();
        }
        private void BtnAddAnswer_Click(object sender, RoutedEventArgs e)
        {
            var answerControl = new AnswerControl();
            answersPanel.Children.Add(answerControl);
        }

        public (string QuestionText, Dictionary<string, bool> Answers) GetQuestionData()
        {
            string questionText = txtQuestion.Text.Trim();
            var answers = new Dictionary<string, bool>();

            foreach (var child in answersPanel.Children)
            {
                if (child is AnswerControl answerControl)
                {
                    var answerData = answerControl.GetAnswerData();
                    if (!string.IsNullOrEmpty(answerData.AnswerText?.Trim()))
                    {
                        answers.Add(answerData.AnswerText, answerData.IsCorrect);
                    }
                }
            }
            return (questionText, answers);
        }
    }
}
