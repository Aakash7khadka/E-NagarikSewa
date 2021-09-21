using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace smartpalika.Models
{
    public class AppointmentUserDetails
    {
        [Key]
        public Guid ID { get; set; }
       
        public string ServiceType { get; set; }
        public string Date { get; set; }
        public string priority { get; set; }

        public string Time { get; set; }
        public string Provider { get; set; }
        public bool isAvailable { get; set; }

        
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
