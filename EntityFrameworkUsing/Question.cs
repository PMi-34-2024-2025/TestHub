using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PE.DesktopApplication.TestHub.DAL
{
    [Table("questions")]
    public class Question
    {
        [Key]
        public int question_id { get; set; }
        public int test_id { get; set; }
        public required string question_text { get; set; }
    }
}
