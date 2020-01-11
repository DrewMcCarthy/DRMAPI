using DRMAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DRMAPI.Services
{
    public interface IContactService
    {
        void AddContact(Contact contact);
    }
}
