using GestionPersonnel.Models.Equipe;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace GestionPersonnel.Storages.EquipeStorages
{
    public class EquipeStorage
    {
        private readonly string _connectionString;

        public EquipeStorage(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Constantes pour les requêtes SQL
        private const string SelectAllQuery = "SELECT * FROM Equipes Where Status=1";
        private const string SelectByIdQuery = "SELECT * FROM Equipes WHERE EquipeID = @id";
        private const string InsertQuery = "INSERT INTO Equipes (NomEquipe, ChefEquipeID) VALUES (@NomEquipe, @ChefEquipeID); SELECT SCOPE_IDENTITY();";
        private const string UpdateQuery = "UPDATE Equipes SET NomEquipe = @NomEquipe, ChefEquipeID = @ChefEquipeID WHERE EquipeID = @EquipeID;";
        private const string DeleteQuery = "UPDATE Equipes SET Status = 0 WHERE EquipeID = @EquipeID;";
        private const string UpdateChefEquipeQuery = "UPDATE Equipes SET ChefEquipeID = @ChefEquipeID WHERE EquipeID = @EquipeID;";

        // Méthode pour mapper une DataRow à un objet Equipe
        private static Equipe GetEquipeFromDataRow(DataRow row)
        {
            return new Equipe
            {
                EquipeID = (int)row["EquipeID"],
                NomEquipe = row["NomEquipe"].ToString(),
                ChefEquipeID = (int)row["ChefEquipeID"]
            };
        }

        // Méthode pour récupérer toutes les Equipes depuis la base de données
        public async Task<List<Equipe>> GetAll()
        {
            await using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(SelectAllQuery, connection);

            var dataTable = new DataTable();
            var da = new SqlDataAdapter(cmd);

            await connection.OpenAsync();
            da.Fill(dataTable);

            return (from DataRow row in dataTable.Rows select GetEquipeFromDataRow(row)).ToList();
        }

        // Méthode pour récupérer une Equipe par son ID
        public async Task<Equipe> GetById(int equipeId)
        {
            await using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(SelectByIdQuery, connection);
            cmd.Parameters.AddWithValue("@id", equipeId);

            var dataTable = new DataTable();
            var da = new SqlDataAdapter(cmd);

            await connection.OpenAsync();
            da.Fill(dataTable);

            if (dataTable.Rows.Count == 0)
                throw new KeyNotFoundException($"Equipe with ID {equipeId} not found.");

            return GetEquipeFromDataRow(dataTable.Rows[0]);
        }

        // Méthode pour ajouter une nouvelle Equipe à la base de données
        public async Task<int> Add(Equipe equipe)
        {
            if (equipe == null) throw new ArgumentNullException(nameof(equipe));

            await using var connection = new SqlConnection(_connectionString);
            await using var cmd = new SqlCommand(InsertQuery, connection);

            cmd.Parameters.AddWithValue("@NomEquipe", equipe.NomEquipe ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ChefEquipeID", equipe.ChefEquipeID);

            try
            {
                await connection.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
            catch (SqlException ex)
            {
                // Log exception details here
                throw new Exception("An error occurred while adding the team.", ex);
            }
        }

        // Méthode pour mettre à jour une Equipe dans la base de données
        public async Task Update(Equipe equipe)
        {
            await using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(UpdateQuery, connection);

            cmd.Parameters.AddWithValue("@NomEquipe", equipe.NomEquipe);
            cmd.Parameters.AddWithValue("@ChefEquipeID", equipe.ChefEquipeID);
            cmd.Parameters.AddWithValue("@EquipeID", equipe.EquipeID);

            await connection.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
        public async Task UpdateChefEquipeById(int equipeId, int chefEquipeId)
        {
            await using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(UpdateChefEquipeQuery, connection);

            cmd.Parameters.AddWithValue("@EquipeID", equipeId);
            cmd.Parameters.AddWithValue("@ChefEquipeID", chefEquipeId);

            await connection.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        // Méthode pour supprimer une Equipe de la base de données par son ID
        public async Task Delete(int equipeId)
        {
            await using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(DeleteQuery, connection);
            cmd.Parameters.AddWithValue("@EquipeID", equipeId);

            await connection.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<EquipesInfos>> GetEquipePostesInfoAsync()
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var query = @"
SELECT 
    E.EquipeID, 
    E.NomEquipe, 
    Emp.Nom AS ChefEquipeNom,
    COUNT(CASE 
            WHEN MONTH(P.Date) = @CurrentMonth AND YEAR(P.Date) = @CurrentYear THEN P.IdPosteComplete 
            ELSE NULL 
          END) AS NombreTotalDesPostes
FROM Equipes E
JOIN Employes Emp ON E.ChefEquipeID = Emp.EmployeID
LEFT JOIN PosteComplete P ON E.EquipeID = P.IdEquipe
WHERE E.Status = 1
GROUP BY E.EquipeID, E.NomEquipe, Emp.Nom;";

            var resultList = new List<EquipesInfos>();

            await using var connection = new SqlConnection(_connectionString);
            await using var cmd = new SqlCommand(query, connection);

            // Add parameters for the current month and year with explicit types
            cmd.Parameters.Add(new SqlParameter("@CurrentMonth", SqlDbType.Int) { Value = currentMonth });
            cmd.Parameters.Add(new SqlParameter("@CurrentYear", SqlDbType.Int) { Value = currentYear });

            await connection.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var equipePostesInfo = new EquipesInfos
                {
                    EquipeID = reader.GetInt32(0),
                    NomEquipe = reader.GetString(1),
                    ChefEquipeNom = reader.GetString(2),
                    NombreTotalDesPostes = reader.IsDBNull(3) ? 0 : reader.GetInt32(3)
                };

                resultList.Add(equipePostesInfo);
            }

            return resultList;
        }



    }
}
