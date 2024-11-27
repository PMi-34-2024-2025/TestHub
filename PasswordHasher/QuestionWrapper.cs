using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PE.DesktopApplication.TestHub.BLL
{
    public class QuestionWrapper
    {
        public string? QuestionText { get; set; }
        public List<AnswerWrapper>? Answers { get; set; }
        public List<int> correctAnswers;
        public List<string> variantsText;
        public void SetFields()
        {
            int n = Answers.Count;
            correctAnswers = new List<int>(n);
            variantsText = new List<string>(n);
            for (int i = 0; i < n; i++)
            {
                variantsText.Add(Answers[i].AnswerText);
                if (Answers[i].IsCorrect)
                {
                    correctAnswers.Add(i);
                }
            }
        }
    }
}
