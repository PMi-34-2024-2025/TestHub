using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace PE.DesktopApplication.TestHub.WPF
{
    /// <summary>
    /// Interaction logic for UserControl2.xaml
    /// </summary>
    public partial class UserControl2 : UserControl
    {
        private readonly StackPanel stackPanel;
        private readonly List<CheckBox> checkBoxes;
        private readonly List<RadioButton> radioButtons;
        private readonly List<int> correctAnswers;
        private readonly List<string> answers;
        public UserControl2(string label, List<string> answers, List<int> correctAnswers)
        {
            stackPanel = new StackPanel();
            checkBoxes = new List<CheckBox>();
            radioButtons = new List<RadioButton>();
            this.correctAnswers = correctAnswers;
            this.answers = answers;
            CreateQuestion(label);
            CreateAnswerControls();
            this.Content = stackPanel;
        }
        private void CreateQuestion(string label)
        {
            Label question = new Label()
            {
                Content = label,
                FontSize = 16,
                Margin = new Thickness(10, 0, 0, 10)
            };
            stackPanel.Children.Add(question);
        }

        private void CreateAnswerControls()
        {
            string uniqueGroupName = Guid.NewGuid().ToString();
            if (correctAnswers.Count == 1)
            {
                foreach (var answer in answers)
                {
                    RadioButton radioButton = new RadioButton
                    {
                        Content = answer,
                        GroupName = uniqueGroupName,
                        Margin = new Thickness(15, 5, 0, 5)
                    };
                    radioButtons.Add(radioButton);
                    stackPanel.Children.Add(radioButton);
                }
            }
            else if (correctAnswers.Count > 1)
            {
                foreach(var answer in answers)
                {
                    CheckBox checkBox = new CheckBox
                    {
                        Content = answer,
                        Margin = new Thickness(15, 5, 0, 5)
                    };
                    checkBoxes.Add(checkBox);
                    stackPanel.Children.Add(checkBox);
                }
            } 
        }
        public List<int> GetUserAnswers()
        {
            List<int> userAnswers = new List<int>();
            if (radioButtons.Count > 0)
            {
                for (int i = 0; i < radioButtons.Count; i++)
                {
                    if ((bool)radioButtons[i].IsChecked)
                    {
                        userAnswers.Add(i);
                    }
                }
            }
            else
            {
                for (int i = 0; checkBoxes.Count > i; i++)
                {
                    if ((bool)checkBoxes[i].IsChecked)
                    {
                        userAnswers.Add(i);
                    }
                }
            }
            return userAnswers;
        }

        public string GetTestReport()
        {
            List<int> userAnswers = GetUserAnswers();
            var report = new
            {
                Question = ((Label)stackPanel.Children[0]).Content.ToString(), 
                CorrectAnswers = GetCorrectAnswers(), 
                UserAnswers = userAnswers.ConvertAll(index => GetAnswerText(index)) 
            };
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append('\n').Append(report.Question).Append('\n').Append("Правильні відповіді:\t[").Append(string.Join(", ", report.CorrectAnswers)).Append("];\n").Append("Відповіді студента:\t[").Append(string.Join(", ", report.UserAnswers)).Append("];");

            return stringBuilder.ToString();
        }

        private List<string> GetCorrectAnswers()
        {
            List<string> result = new List<string>();
            foreach (var item in correctAnswers)
            {
                result.Add(answers[item]);
            }
            return result;
        }

        private string GetAnswerText(int index)
        {
            return answers[index];
        }
    }
}
