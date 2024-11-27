namespace PE.DesktopApplication.TestHub.BLL
{
    public class TestData
    {
        public int TestId { get; set; }
        public string Title { get; set; }
        public int TutorId { get; set; }
        public string Status { get; set; }
        public string Area { get; set; }
        public int MinutesToPass { get; set; }
        public List<QuestionData> Questions { get; set; }
    }
}
