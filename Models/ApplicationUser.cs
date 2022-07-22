using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace smartpalika.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string Address { get; set; }
        public string FullName { get; set; }
        public byte[] ProfileImage { get; set; }
        public virtual ICollection<AppointmentUserDetails> Appointments { get; set; }

    }
}
