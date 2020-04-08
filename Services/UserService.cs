using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using DRMAPI.Data;
using DRMAPI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DRMAPI.Services
{
    public class UserService : IUserService
    {
        private readonly GroceryContext _groceryContext;
        private const string GroceryJwtSecret = "JwtSecret__GroceryDRM";
        private readonly GroceryDb _groceryDb;

        public UserService(GroceryContext groceryContext, IConfiguration configuration)
        {
            _groceryContext = groceryContext;
            _groceryDb = new GroceryDb();
        }

        public GroceryAppState Authenticate(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return null;

            var groceryAppState = JsonConvert.DeserializeObject<GroceryAppState>(_groceryDb.GetAppStateByEmail(email));

            if (groceryAppState.User == null)
                return null;

            if (!VerifyPasswordHash(password, groceryAppState.User.PasswordHash, groceryAppState.User.PasswordSalt))
                return null;

            // authentication successful
            return groceryAppState;
        }

        public User Create(User user)
        {
            // validation
            if (string.IsNullOrWhiteSpace(user.Password))
                throw new Exception("Password is required");

            if (_groceryContext.Users.Any(x => x.Email == user.Email))
                throw new Exception("Email address \"" + user.Email + "\" is already registered");

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(user.Password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _groceryContext.Users.Add(user);
            _groceryContext.SaveChanges();

            return user;
        }

        public string GetToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable(GroceryJwtSecret));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Email)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }

        public User GetByEmail(string email)
        {
            return _groceryContext.Users.Find(email);
        }

        // private helper methods
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }
    }
}
