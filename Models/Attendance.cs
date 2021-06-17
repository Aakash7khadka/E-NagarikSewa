using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace smartpalika.Models
{
    public class Attendance
    {
        [Key]
        public int Entry_id { get; set; }
        public string UserEmail { get; set; }
        public string Entry_time { get; set; }
        
    }
}
