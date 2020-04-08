using DRMAPI.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DRMAPI.Data
{
    public class GroceryDb
    {
        private string _connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__GroceryDRM");

        public string GetAppState(int userId)
        {
            var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using (var cmd = new NpgsqlCommand("SELECT public.fn_getAppState(@id)", conn))
            {
                cmd.Parameters.AddWithValue("id", userId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //Console.WriteLine(reader.GetString(0));
                        return reader.GetString(0);
                    }
                }

            }
            return null;
        }

        public string GetAppStateByEmail(string emailAddress)
        {
            var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using (var cmd = new NpgsqlCommand("SELECT public.fn_getAppStateByEmail(@emailAddress)", conn))
            {
                cmd.Parameters.AddWithValue("emailAddress", emailAddress);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                       return reader.GetString(0);
                    }
                }

            }
            return null;
        }

        public string GetUserByEmail(string emailAddress)
        {
            var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using (var cmd = new NpgsqlCommand("SELECT public.fn_getUserByEmail(@emailAddress)", conn))
            {
                cmd.Parameters.AddWithValue("emailAddress", emailAddress);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //Console.WriteLine(reader.GetString(0));
                        return reader.GetString(0);
                    }
                }

            }
            return null;
        }

        public void UpdateGroceryList(GroceryList groceryList)
        {
            var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            var transaction = conn.BeginTransaction();

            using (var deleteCmd = new NpgsqlCommand(
                "DELETE " +
                "FROM public.list_items " +
                "WHERE list_id = @listId", conn, transaction))
            {
                deleteCmd.Parameters.AddWithValue("listId", groceryList.ListId);
                deleteCmd.ExecuteNonQuery();
            }
                     

            using (var cmd = new NpgsqlCommand(
                "INSERT INTO public.list_items(list_id, item_name, is_active)" +
                "VALUES(@listId, @itemName, @isActive)"
                , conn, transaction))
            {
                cmd.Parameters.AddWithValue("listId", groceryList.ListId);
                cmd.Parameters.Add("itemName", NpgsqlTypes.NpgsqlDbType.Varchar);
                cmd.Parameters.Add("isActive", NpgsqlTypes.NpgsqlDbType.Boolean);

                foreach (var listItem in groceryList.ListItems)
                {
                    cmd.Parameters.First(p => p.ParameterName == "itemName").Value = listItem.ItemName;
                    cmd.Parameters.First(p => p.ParameterName == "isActive").Value = listItem.IsActive;
                    cmd.ExecuteNonQuery();
                }
            }

            transaction.Commit();
        }

        public void AddItemToList(int listId, GroceryListItem groceryListItem)
        {
            var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using (var cmd = new NpgsqlCommand(
                "INSERT INTO public.list_items(list_id, item_name, is_active)" +
                "VALUES(@listId, @itemName, @isActive)", conn))
            {
                cmd.Parameters.AddWithValue("listId", listId);
                cmd.Parameters.AddWithValue("itemName", groceryListItem.ItemName);
                cmd.Parameters.AddWithValue("isActive", groceryListItem.IsActive);
                cmd.ExecuteNonQuery();
            }

        }
    }
}
