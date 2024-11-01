using GestionPersonnel.Models.SalairesBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace GestionPersonnel.Storages.SalairesBaseStorages
{
    public class SalaireBaseStorage
    {
        private readonly string _connectionString;

        public SalaireBaseStorage(string connectionString)
        {
            _connectionString = connectionString;
        }

        private const string SelectAllQuery = "SELECT * FROM SalairesBase";

        private const string SelectByIdQuery = "SELECT * FROM SalairesBase WHERE IdSalaireBase = @id";
       
        private const string InsertQuery = "INSERT INTO SalairesBase (SalaireBase, TypePaiementID, EmployeID) " +
                                           "VALUES (@SalaireBase, @TypePaiementID, @EmplyeId); SELECT SCOPE_IDENTITY();";
       
        private const string UpdateQuery = "UPDATE SalairesBase SET SalaireBase = @SalaireBase, TypePaiementID = @TypePaiementID, " +
                                           "EmployeID = @EmplyeId WHERE IdSalaireBase = @IdSalaireBase;";
        
        private const string DeleteQuery = "DELETE FROM SalairesBase WHERE IdSalaireBase = @IdSalaireBase;";
        
        private const string SelectByEmployeeIdQuery = "SELECT * FROM SalairesBase WHERE EmployeID = @employeeId";

        private static SalairesBase GetSalairesBaseFromDataRow(DataRow row)
        {
            return new SalairesBase
            {
                IdSalaireBase = (int)row["IdSalaireBase"],
                SalaireBase = (decimal)row["SalaireBase"],
                TypePaiementID = (int)row["TypePaiementID"],
                EmplyeId = (int)row["EmployeID"]
            };
        }

        public async Task<List<SalairesBase>> GetAll()
        {
            await using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(SelectAllQuery, connection);

            var dataTable = new DataTable();
            var da = new SqlDataAdapter(cmd);

            await connection.OpenAsync();
            da.Fill(dataTable);

            return (from DataRow row in dataTable.Rows select GetSalairesBaseFromDataRow(row)).ToList();
        }

        public async Task<SalairesBase> GetById(int idSalaireBase)
        {
            await using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(SelectByIdQuery, connection);
            cmd.Parameters.AddWithValue("@id", idSalaireBase);

            var dataTable = new DataTable();
            var da = new SqlDataAdapter(cmd);

            await connection.OpenAsync();
            da.Fill(dataTable);

            if (dataTable.Rows.Count == 0)
                throw new KeyNotFoundException($"SalairesBase with ID {idSalaireBase} not found.");

            return GetSalairesBaseFromDataRow(dataTable.Rows[0]);
        }

        public async Task<List<SalairesBase>> GetByEmployeeId(int employeeId)
        {
            await using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(SelectByEmployeeIdQuery, connection);
            cmd.Parameters.AddWithValue("@employeeId", employeeId);

            var dataTable = new DataTable();
            var da = new SqlDataAdapter(cmd);

            await connection.OpenAsync();
            da.Fill(dataTable);

            return (from DataRow row in dataTable.Rows select GetSalairesBaseFromDataRow(row)).ToList();
        }

        public async Task<int> Add(SalairesBase salairesBase)
        {
            await using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(InsertQuery, connection);

            cmd.Parameters.AddWithValue("@SalaireBase", salairesBase.SalaireBase);
            cmd.Parameters.AddWithValue("@TypePaiementID", salairesBase.TypePaiementID);
            cmd.Parameters.AddWithValue("@EmplyeId", salairesBase.EmplyeId);

            await connection.OpenAsync();
            var id = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(id);
        }



        public async Task Update(SalairesBase salairesBase)
        {
            await using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(UpdateQuery, connection);

            cmd.Parameters.AddWithValue("@SalaireBase", salairesBase.SalaireBase);
            cmd.Parameters.AddWithValue("@TypePaiementID", salairesBase.TypePaiementID);
            cmd.Parameters.AddWithValue("@EmplyeId", salairesBase.EmplyeId);
            cmd.Parameters.AddWithValue("@IdSalaireBase", salairesBase.IdSalaireBase);

            await connection.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task Delete(int idSalaireBase)
        {
            await using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(DeleteQuery, connection);
            cmd.Parameters.AddWithValue("@IdSalaireBase", idSalaireBase);

            await connection.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }


        


    }
}
