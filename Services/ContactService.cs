using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DRMAPI.Data;
using DRMAPI.Models;

namespace DRMAPI.Services
{
    public class ContactService : IContactService
    {
        private readonly DRMContext _context;

        public ContactService(DRMContext context)
        {
            _context = context;
        }

        public void AddContact(Contact contact)
        {
            _context.Contacts.Add(contact);
            _context.SaveChanges();
        }
    }
}
