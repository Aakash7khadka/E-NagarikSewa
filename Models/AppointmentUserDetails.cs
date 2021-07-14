using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace smartpalika.Models
{
    public class AppointmentUserDetails
    {
        [Key]
        public int ID { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string ServiceType { get; set; }
        public string Date { get; set; }
        public string priority { get; set; }

        public string Provider_role { get; set; }
    }
}
