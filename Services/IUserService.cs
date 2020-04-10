using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DRMAPI.Models;

namespace DRMAPI.Services
{
    public interface IUserService
    {
        Task<User> Authenticate(string email, string password);
        Task<User> Create(User user);
        string GetToken(User user);
        Task<User> GetUserByEmail(string email);
    }
}
