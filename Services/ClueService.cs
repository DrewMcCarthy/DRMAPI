using DRMAPI.Data;
using DRMAPI.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DRMAPI.Services
{
    public class ClueService : IClueService
    {
        private TriviaDRMContext _triviaDRM;

        public ClueService(TriviaDRMContext triviaDRM)
        {
            _triviaDRM = triviaDRM;
        }

        public IEnumerable<Clue> GetClues()
        {
            return _triviaDRM.Clues;
            //foreach(var clue in _triviaDRM.Clues)
            //{
            //    var paramClueId = new Npgsql.NpgsqlParameter("p_clueId", clue.ClueId);

            //    clue.Revealed = false;
            //    clue.OtherAnswers = _triviaDRM.Query<string>("SELECT fn_similar_answers({0})", paramClueId);
            //}
        }
    }
}
