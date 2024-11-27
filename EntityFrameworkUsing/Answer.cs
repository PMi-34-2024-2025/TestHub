using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PE.DesktopApplication.TestHub.DAL
{
    [Table("answers")]
    public class Answer
    {
        [Key]
        public int answer_id { get; set; }
        public int question_id { get; set; }
        public required string answer_text { get; set; }
        public bool is_correct { get; set; }
    }
}
