using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace smartpalika.Models
{
    public class CreateRoleVM
    {
       
        public Guid Id { get; set; }
        [Required]
        public string RoleName { get; set; }
    }
}
