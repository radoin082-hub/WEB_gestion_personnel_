using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using GestionPersonnel.Models.Fonctions;

namespace GestionPersonnel.Storages.FonctionsStorages
{
    public class FonctionStorage
    {
        private readonly string _connectionString;

        public FonctionStorage(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Fonction>> GetAll()
        {
            var fonctions = new List<Fonction>();
            string query = "SELECT FonctionID, NomFonction FROM Fonctions"; 

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                await connection.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        fonctions.Add(new Fonction
                        {
                            FonctionID = reader.GetInt32(0),
                            NomFonction = reader.GetString(1)
                        });
                    }
                }
            }
            return fonctions;
        }

        public async Task Add(Fonction fonction)
        {
            string query = "INSERT INTO Fonctions (NomFonction) VALUES (@NomFonction)"; 

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@NomFonction", fonction.NomFonction);
                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task Update(Fonction fonction)
        {
            string query = "UPDATE Fonctions SET NomFonction = @NomFonction WHERE FonctionID = @FonctionID"; // Ensure this matches your table name

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@NomFonction", fonction.NomFonction);
                command.Parameters.AddWithValue("@FonctionID", fonction.FonctionID);
                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task Delete(int fonctionId)
        {
            try
            {
                // Check if the Fonction is referenced by any employee
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Count employees referencing this function
                    string checkQuery = "SELECT COUNT(*) FROM Employes WHERE FonctionID = @FonctionID";
                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@FonctionID", fonctionId);
                    int count = (int)await checkCommand.ExecuteScalarAsync();

                    if (count > 0)
                    {
                        throw new InvalidOperationException("The function cannot be deleted because it is referenced by employees.");
                    }

                    // Delete the Fonction
                    string deleteQuery = "DELETE FROM Fonctions WHERE FonctionID = @FonctionID";
                    SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection);
                    deleteCommand.Parameters.AddWithValue("@FonctionID", fonctionId);
                    await deleteCommand.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while deleting the function: {ex.Message}");
            }
        }



        public async Task<Fonction> GetById(int fonctionId)
        {
            Fonction fonction = null;
            string query = "SELECT FonctionID, NomFonction FROM Fonctions WHERE FonctionID = @FonctionID"; // Ensure this matches your table name

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@FonctionID", fonctionId);
                await connection.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        fonction = new Fonction
                        {
                            FonctionID = reader.GetInt32(0),
                            NomFonction = reader.GetString(1)
                        };
                    }
                }
            }
            return fonction;
        }
    }
}
