using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelsLibrary.Enums;

namespace ModelsLibrary.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int? CountryId { get; set; }
        public string Avatar { get; set; }
        public UserRole Role { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
