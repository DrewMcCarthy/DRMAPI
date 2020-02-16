using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DRMAPI.Models
{
    public class User
    {
        public string Email { get; set; }
        public string Password{get;set;}
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}
