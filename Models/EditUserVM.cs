using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace smartpalika.Models
{
    public class EditUserVM
    {
        [Key]
        public string ID { get; set; }
        public string Email { get; set; }
        [Display(Name ="Full Name")]
        public string FullName { get; set; }
        public string Address { get; set; }
        [DataType(DataType.PhoneNumber)]
        [Display(Name ="Phone Number")]


       
        public string PhoneNumber { get; set; }
        public byte[] ProfileImage { get; set; }

        
    }
}
