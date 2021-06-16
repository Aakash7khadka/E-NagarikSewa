using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace smartpalika.Models
{
    public class RoleUsersVM
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public bool IsSelected { get; set; }
    }
}
