using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace smartpalika.Models
{
    public class AppointmentVM
    {
        public string OfficerName { get; set; }
        public string service { get; set; }
        public List<string> Available_Time { get; set; }
    }
}
