using GestionPersonnel.Models.RapportPointage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace GestionPersonnel.Storages.RapportPointageStorages
{
    public class RapportPointageStorage
    {
        private readonly string _connectionString;

        public RapportPointageStorage(string connectionString)
        {
            _connectionString = connectionString;
        }

        private const string SelectAllQuery = "SELECT * FROM RapportPointage";
        private const string SelectByIdQuery = "SELECT * FROM RapportPointage WHERE RapportID = @id";
        private const string InsertQuery = "INSERT INTO RapportPointage (EmployeID, EquipeID, Mois, HeuresTotales, CofficientsTotales) " +
                                           "VALUES (@EmployeID, @EquipeID, @Mois, @HeuresTotales, @CofficientsTotales); SELECT SCOPE_IDENTITY();";
        private const string UpdateQuery = "UPDATE RapportPointage SET EmployeID = @EmployeID, EquipeID = @EquipeID, " +
                                           "Mois = @Mois, HeuresTotales = @HeuresTotales, CofficientsTotales = @CofficientsTotales " +
                                           "WHERE RapportID = @RapportID;";
        private const string DeleteQuery = "DELETE FROM RapportPointage WHERE RapportID = @RapportID;";
        private const string SelectTotalDaysByMonthQuery = @"
    SELECT COUNT(DISTINCT Date) AS TotalDays
    FROM CoefficientTravail
    WHERE EmployeID = @EmployeID AND
          YEAR(Date) = @Year AND
          MONTH(Date) = @Month";

        private static RapportPointage GetRapportPointageFromDataRow(DataRow row)
        {
            return new RapportPointage
            {
                RapportID = (int)row["RapportID"],
                EmployeID = (int)row["EmployeID"],
                EquipeID = (int)row["EquipeID"],
                Mois = (DateTime)row["Mois"],
                HeuresTotales = (decimal)row["HeuresTotales"],
                CofficientsTotales = (decimal)row["CofficientsTotales"]
            };
        }

        public async Task<List<RapportPointage>> GetAll()
        {
            await using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(SelectAllQuery, connection);

            var dataTable = new DataTable();
            var da = new SqlDataAdapter(cmd);

            await connection.OpenAsync();
            da.Fill(dataTable);

            return (from DataRow row in dataTable.Rows select GetRapportPointageFromDataRow(row)).ToList();
        }

        public async Task<RapportPointage> GetById(int rapportId)
        {
            await using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(SelectByIdQuery, connection);
            cmd.Parameters.AddWithValue("@id", rapportId);

            var dataTable = new DataTable();
            var da = new SqlDataAdapter(cmd);

            await connection.OpenAsync();
            da.Fill(dataTable);

            if (dataTable.Rows.Count == 0)
                throw new KeyNotFoundException($"RapportPointage with ID {rapportId} not found.");

            return GetRapportPointageFromDataRow(dataTable.Rows[0]);
        }

        public async Task<int> Add(RapportPointage rapportPointage)
        {
            await using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(InsertQuery, connection);

            cmd.Parameters.AddWithValue("@EmployeID", rapportPointage.EmployeID);
            cmd.Parameters.AddWithValue("@EquipeID", rapportPointage.EquipeID);
            cmd.Parameters.AddWithValue("@Mois", rapportPointage.Mois);
            cmd.Parameters.AddWithValue("@HeuresTotales", rapportPointage.HeuresTotales);
            cmd.Parameters.AddWithValue("@CofficientsTotales", rapportPointage.CofficientsTotales);

            await connection.OpenAsync();
            var id = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(id);
        }

        public async Task Update(RapportPointage rapportPointage)
        {
            await using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(UpdateQuery, connection);

            cmd.Parameters.AddWithValue("@EmployeID", rapportPointage.EmployeID);
            cmd.Parameters.AddWithValue("@EquipeID", rapportPointage.EquipeID);
            cmd.Parameters.AddWithValue("@Mois", rapportPointage.Mois);
            cmd.Parameters.AddWithValue("@HeuresTotales", rapportPointage.HeuresTotales);
            cmd.Parameters.AddWithValue("@CofficientsTotales", rapportPointage.CofficientsTotales);
            cmd.Parameters.AddWithValue("@RapportID", rapportPointage.RapportID);

            await connection.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task Delete(int rapportId)
        {
            await using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(DeleteQuery, connection);
            cmd.Parameters.AddWithValue("@RapportID", rapportId);

            await connection.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
        public async Task<int> GetTotalDaysByMonth(int employeId, DateTime monthYear)
        {
            await using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(SelectTotalDaysByMonthQuery, connection);

            cmd.Parameters.AddWithValue("@EmployeID", employeId);
            cmd.Parameters.AddWithValue("@Year", monthYear.Year);
            cmd.Parameters.AddWithValue("@Month", monthYear.Month);

            await connection.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();

            return result != DBNull.Value ? Convert.ToInt32(result) : 0;
        }

    }
}