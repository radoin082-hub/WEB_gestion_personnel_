using GestionPersonnel.Models.Pointage;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPersonnel.Storages.PointagesStorages
{
    public class PointageStorage
    {
        private readonly string _connectionString;

        public PointageStorage(string connectionString)
        {
            _connectionString = connectionString;
        }
        private const string _selectAllQuery = "SELECT * FROM Pointage";

        private const string _selectByIdAndDateQuery = "SELECT * FROM Pointage WHERE EmployeID = @id AND Date = @date";

        private const string _insertQuery = "INSERT INTO Pointage (EmployeID, Date, HeureEntree, HeureSortie, HeuresTravaillees) VALUES (@EmployeID, @Date, @HeureEntree, @HeureSortie, @HeuresTravaillees); SELECT SCOPE_IDENTITY();";
        private const string _updateQuery = "UPDATE Pointage SET HeuresTravaillees = @HeuresTravaillees, Remarque = @Remarque WHERE PointageID = @PointageID;";
        private const string _deleteQuery = "DELETE FROM Pointage WHERE PointageID = @PointageID;";

      
        private static Pointage GetPointageFromDataRow(DataRow row)
        {
            return new Pointage
            {
                PointageID = (int)row["PointageID"],
                EmployeID = (int)row["EmployeID"],
                Date = (DateTime)row["Date"],
                HeureEntree = (TimeSpan)row["HeureEntree"],
                HeureSortie = (TimeSpan)row["HeureSortie"],
                HeuresTravaillees = (decimal)row["HeuresTravaillees"],
                Remarque = row["Remarque"] == DBNull.Value ? null : (string)row["Remarque"]
            };
        }

        public async Task<List<Pointage>> GetAll()
        {
            await using var connection = new SqlConnection(_connectionString);
            SqlCommand cmd = new(_selectAllQuery, connection);

            DataTable dataTable = new();
            SqlDataAdapter da = new(cmd);

            connection.Open();
            da.Fill(dataTable);

            return (from DataRow row in dataTable.Rows select GetPointageFromDataRow(row)).ToList();
        }


        public async Task<Pointage?> GetByIdAndDate(int id, DateOnly date)
        {
            await using var connection = new SqlConnection(_connectionString);

            SqlCommand cmd = new(_selectByIdAndDateQuery, connection);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@date", date.ToDateTime(TimeOnly.MinValue));

            DataTable dataTable = new();
            SqlDataAdapter da = new(cmd);

            await connection.OpenAsync().ConfigureAwait(false);
            da.Fill(dataTable);

            return dataTable.Rows.Count == 0 ? null : GetPointageFromDataRow(dataTable.Rows[0]);
        }



        public async Task Add(Pointage pointage)
        {
            await using var connection = new SqlConnection(_connectionString);
            SqlCommand cmd = new(_insertQuery, connection);
            cmd.Parameters.AddWithValue("@EmployeID", pointage.EmployeID);
            cmd.Parameters.AddWithValue("@Date", pointage.Date);
            cmd.Parameters.AddWithValue("@HeureEntree", pointage.HeureEntree);
            cmd.Parameters.AddWithValue("@HeureSortie", pointage.HeureSortie);
            cmd.Parameters.AddWithValue("@HeuresTravaillees", pointage.HeuresTravaillees);

            connection.Open();
            var id = await cmd.ExecuteScalarAsync();
            pointage.PointageID = Convert.ToInt32(id);
        }

        public async Task Update(Pointage pointage)
        {
            await using var connection = new SqlConnection(_connectionString);
            SqlCommand cmd = new(_updateQuery, connection);

            cmd.Parameters.AddWithValue("@HeuresTravaillees", pointage.HeuresTravaillees);
            cmd.Parameters.AddWithValue("@Remarque", pointage.Remarque);
            cmd.Parameters.AddWithValue("@PointageID", pointage.PointageID);

            await connection.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }



        public async Task Delete(int id)
        {
            await using var connection = new SqlConnection(_connectionString);
            SqlCommand cmd = new(_deleteQuery, connection);
            cmd.Parameters.AddWithValue("@PointageID", id);

            connection.Open();
            await cmd.ExecuteNonQueryAsync();
        }

    }
}
