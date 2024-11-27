using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PE.DesktopApplication.TestHub.DAL
{
    [Table("teststatuses")]
    public class TestStatus
    {
        [Key]
        public int status_id { get; set; }
        public int test_id { get; set; }
        public int admin_id { get; set; }
        public required string status { get; set; }
        public DateTime date_changed { get; set; }
    }
}
