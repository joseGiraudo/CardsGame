using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsLibrary.DTOs.Auth
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Email obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Password obligatorio")]
        public string Password { get; set; }

    }
}
