using GestionPersonnel.Models.CoefficientTravail;
using System.Data.SqlClient;
using System.Data;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace GestionPersonnel.Storages.CoefficientTravailStorages
{
    public class CoefficientTravailStorage
    {
        private readonly string _connectionString;

        public CoefficientTravailStorage(string connectionString)
        {
            _connectionString = connectionString;
        }
        private const string SelectByEmployeIdAndDateQuery =
    "SELECT * FROM CoefficientTravail WHERE EmployeID = @EmployeID AND Date = @Date";

        private const string SelectAllQuery = "SELECT * FROM CoefficientTravail";
        private const string SelectByIdQuery = "SELECT * FROM CoefficientTravail WHERE CoefficientID = @id";
        private const string InsertQuery = "INSERT INTO CoefficientTravail (EmployeID, Date, JourneeCoefficient, HeuresSupplementairesCoefficient) " +
                                           "VALUES (@EmployeID, @Date, @JourneeCoefficient, @HeuresSupplementairesCoefficient); SELECT SCOPE_IDENTITY();";
        private const string UpdateQuery = "UPDATE CoefficientTravail SET EmployeID = @EmployeID, Date = @Date, " +
                                           "JourneeCoefficient = @JourneeCoefficient, HeuresSupplementairesCoefficient = @HeuresSupplementairesCoefficient " +
                                           "WHERE CoefficientID = @CoefficientID;";
        private const string DeleteQuery = "DELETE FROM CoefficientTravail WHERE CoefficientID = @CoefficientID;";
        private const string SelectTotalHeuresSupplementairesByMonthQuery = @"
    SELECT SUM(HeuresSupplementairesCoefficient) AS TotalHeuresSupplementaires
    FROM CoefficientTravail
    WHERE EmployeID = @EmployeID AND
          YEAR(Date) = @Year AND
          MONTH(Date) = @Month";

        private static CoefficientTravail GetCoefficientTravailFromDataRow(DataRow row)
        {
            return new CoefficientTravail
            {
                CoefficientID = (int)row["CoefficientID"],
                EmployeID = (int)row["EmployeID"],
                Date = (DateTime)row["Date"],
                JourneeCoefficient = (decimal)row["JourneeCoefficient"],
                HeuresSupplementairesCoefficient = (decimal)row["HeuresSupplementairesCoefficient"]
            };
        }

        public async Task<List<CoefficientTravail>> GetAll()
        {
            await using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(SelectAllQuery, connection);

            var dataTable = new DataTable();
            var da = new SqlDataAdapter(cmd);

            await connection.OpenAsync();
            da.Fill(dataTable);

            return (from DataRow row in dataTable.Rows select GetCoefficientTravailFromDataRow(row)).ToList();
        }

        public async Task<CoefficientTravail> GetById(int coefficientId)
        {
            await using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(SelectByIdQuery, connection);
            cmd.Parameters.AddWithValue("@id", coefficientId);

            var dataTable = new DataTable();
            var da = new SqlDataAdapter(cmd);

            await connection.OpenAsync();
            da.Fill(dataTable);

            if (dataTable.Rows.Count == 0)
                throw new KeyNotFoundException($"CoefficientTravail with ID {coefficientId} not found.");

            return GetCoefficientTravailFromDataRow(dataTable.Rows[0]);
        }

        public async Task<int> Add(CoefficientTravail coefficientTravail)
        {
            await using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(InsertQuery, connection);

            cmd.Parameters.AddWithValue("@EmployeID", coefficientTravail.EmployeID);
            cmd.Parameters.AddWithValue("@Date", coefficientTravail.Date);
            cmd.Parameters.AddWithValue("@JourneeCoefficient", coefficientTravail.JourneeCoefficient);
            cmd.Parameters.AddWithValue("@HeuresSupplementairesCoefficient", coefficientTravail.HeuresSupplementairesCoefficient);

            await connection.OpenAsync();
            var id = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(id);
        }

        public async Task Update(CoefficientTravail coefficientTravail)
        {
            await using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(UpdateQuery, connection);

            cmd.Parameters.AddWithValue("@EmployeID", coefficientTravail.EmployeID);
            cmd.Parameters.AddWithValue("@Date", coefficientTravail.Date);
            cmd.Parameters.AddWithValue("@JourneeCoefficient", coefficientTravail.JourneeCoefficient);
            cmd.Parameters.AddWithValue("@HeuresSupplementairesCoefficient", coefficientTravail.HeuresSupplementairesCoefficient);
            cmd.Parameters.AddWithValue("@CoefficientID", coefficientTravail.CoefficientID);

            await connection.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task Delete(int coefficientId)
        {
            await using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(DeleteQuery, connection);
            cmd.Parameters.AddWithValue("@CoefficientID", coefficientId);

            await connection.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
        public async Task<List<CoefficientTravail>> GetByEmployeIdAndDate(int employeId, DateTime date)
        {
            await using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(SelectByEmployeIdAndDateQuery, connection);

            cmd.Parameters.AddWithValue("@EmployeID", employeId);
            cmd.Parameters.AddWithValue("@Date", date);

            var dataTable = new DataTable();
            var da = new SqlDataAdapter(cmd);

            await connection.OpenAsync();
            da.Fill(dataTable);

            return (from DataRow row in dataTable.Rows select GetCoefficientTravailFromDataRow(row)).ToList();
        }
        public async Task<decimal> GetTotalHeuresSupplementairesByMonth(int employeId, DateTime Date)
        {
            await using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(SelectTotalHeuresSupplementairesByMonthQuery, connection);

            cmd.Parameters.AddWithValue("@EmployeID", employeId);
            cmd.Parameters.AddWithValue("@Year", Date.Year);
            cmd.Parameters.AddWithValue("@Month", Date.Month);

            await connection.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();

            return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
        }


    }
}