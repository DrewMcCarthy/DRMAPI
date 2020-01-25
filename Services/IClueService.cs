using System.Collections.Generic;
using DRMAPI.Models;

namespace DRMAPI.Services
{
    public interface IClueService
    {
        IEnumerable<Clue> GetClues();
    }
}