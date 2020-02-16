using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DRMAPI.Models;

namespace DRMAPI.Services
{
    public interface IUserService
    {
        User Authenticate(string email, string password);
        User Create(User user);
        string GetToken(User user);
        User GetByEmail(string email);
    }
}
