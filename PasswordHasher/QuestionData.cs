namespace PE.DesktopApplication.TestHub.BLL
{
    public class QuestionData
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public List<AnswerData> Answers { get; set; }
    }
}
