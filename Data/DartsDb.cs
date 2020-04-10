using DRMAPI.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DRMAPI.Data
{
    public class DartsDb
    {
        private string _connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DartsDRM");

        public async Task<User> GetUserByEmail(string emailAddress)
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            await using (var cmd = new NpgsqlCommand(@"
                SELECT user_id, email, username, password_hash, password_salt 
                FROM public.users 
                WHERE email = @emailAddress", conn))
            {
                cmd.Parameters.AddWithValue("emailAddress", emailAddress);
                await using var reader = await cmd.ExecuteReaderAsync();
                User user = new User();
                while (await reader.ReadAsync())
                {
                    int colIdx = 0;
                    user.Id = reader.GetInt32(colIdx++);
                    user.Email = reader.GetString(colIdx++);
                    user.Username = reader.GetString(colIdx++);
                    user.PasswordHash = (byte[])reader.GetValue(colIdx++);
                    user.PasswordSalt = (byte[])reader.GetValue(colIdx++);
                    return user;
                }
            }
            return null;
        }

        public async Task CreateUser(User user)
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            await using (var cmd = new NpgsqlCommand(@"
                INSERT INTO public.users(email, username, password_hash, password_salt) 
                VALUES(@email, @username, @passwordHash, @passwordSalt)", conn))
            {
                cmd.Parameters.AddWithValue("email", user.Email);
                cmd.Parameters.AddWithValue("username", user.Username);
                cmd.Parameters.AddWithValue("passwordHash", user.PasswordHash);
                cmd.Parameters.AddWithValue("passwordSalt", user.PasswordSalt);
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
