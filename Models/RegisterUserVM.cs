using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace smartpalika.Models
{
    public class RegisterUserVM
    {
      
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name ="Full Name")]
        public string FullName { get; set; }

        [Required]
        public string Address { get; set; }
        [Required]
        [Display(Name ="Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Profile Photo")]
        public byte[] ProfileImage { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage ="Passwords are not matching..")]
        public string ConfirmPassword { get; set; }
    }
}
