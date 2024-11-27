using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace PE.DesktopApplication.TestHub.DAL
{
    [Table("attempts")]
    public class Attempt
    {
        [Key]
        public int attempt_id { get; set; }
        public int test_id { get; set; }
        public int user_id { get; set; }
        public int attempt_number { get; set; }
        public DateTime start_time { get; set; }
        public DateTime? end_time { get; set; }
        public decimal? score { get; set; }
        public string? user_report { get; set; }
    }
}
