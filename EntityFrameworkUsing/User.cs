using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PE.DesktopApplication.TestHub.DAL
{
    [Table("users")]
    public class User
    {
        [Key]
        public int user_id { get; set; }
        public required string name { get; set; }
        public required string login { get; set; }
        public required string password_hash { get; set; }
        public required string role { get; set; }
    }
}
