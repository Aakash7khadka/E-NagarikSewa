using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace smartpalika.Models
{
    public class UserAttendanceVM
    {
        public string Username { get; set; }
        public bool isPresent { get; set; }
        public string date { get; set; }
        public string time { get; set; }
    }
}
