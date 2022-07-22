using System;
using System.ComponentModel.DataAnnotations;

namespace smartpalika.Models
{
    public class Appointment_All
    {

        [Key]
        public Guid ID { get; set; }

        public string ServiceType { get; set; }
        public string Date { get; set; }
        public string priority { get; set; }

        public string Time { get; set; }
        public string Provider { get; set; }
        public bool isAvailable { get; set; }
        public string Status { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
