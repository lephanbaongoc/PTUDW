using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyClass.Model
{
    [Table("Users")]
    public class users
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        public string Password { get; set; }

        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        public string Phone { get; set; }

        public string Img { get; set; }

        [Required]
        public string Role { get; set; }

        [Required]
        public string Gender { get; set; }

        public string Address { get; set; }

        public int CreateBy { get; set; }

        public DateTime CreateAt { get; set; }

        public int UpdateBy { get; set; }

        public DateTime UpdateAt { get; set; }

        public int Status { get; set; }

    }
}
