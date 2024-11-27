using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PE.DesktopApplication.TestHub.DAL
{

    [Table("tests")]
    public class Test
    {
        [Key]
        public int test_id { get; set; }
        public required string title { get; set; }
        public int tutor_id { get; set; }
        public required string status { get; set; }
        public required string area { get; set; }
        public required int mins_to_pass { get; set; }
    }
}
