using DRMAPI.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DRMAPI.Models
{
    public class User
    {
        public User()
        {
        }

        [JsonConstructor]
        public User(int id, string email, string passwordHash, string passwordSalt)
        {
            Id = id;
            Email = email;
            PasswordHash = DataUtils.HexStringToByteArray(passwordHash);
            PasswordSalt = DataUtils.HexStringToByteArray(passwordSalt);
        }

        public int Id { get; set; }
        public string Email { get; set; }
        public string Password {get;set;}
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string JwtToken { get; set; }
    }
}
