using GestionPersonnel.Models.Dettes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using GestionPersonnel.Models.Dettes;
using GestionPersonnel.Models.Avances;
using GestionPersonnel.Storages.DettesStorages;
using GestionPersonnel.Storages.AvancesStorages;

namespace GestionPersonnel.Storages.DettesStorages
{
    public class DetteStorage
    {
        private readonly string _connectionString;

        public DetteStorage(string connectionString)
        {
            _connectionString = connectionString;
        }

        private const string SelectAllQuery = "SELECT * FROM Dettes";
        private const string SelectByIdQuery = "SELECT * FROM Dettes WHERE DetteID = @id";
        private const string InsertQuery = "INSERT INTO Dettes (EmployeID, Montant, Date) " +
                                           "VALUES (@EmployeID, @Montant, @Date); SELECT SCOPE_IDENTITY();";
        private const string UpdateQuery = "UPDATE Dettes SET EmployeID = @EmployeID, Montant = @Montant, " +
                                           "Date = @Date WHERE DetteID = @DetteID;";
        private const string DeleteQuery = "DELETE FROM Dettes WHERE DetteID = @DetteID;";

        private static Dette GetDetteFromDataRow(DataRow row)
        {
            return new Dette
            {
                DetteID = (int)row["DetteID"],
                EmployeID = (int)row["EmployeID"],
                Montant = (decimal)row["Montant"],
                Date = (DateTime)row["Date"]
            };
        }

        public async Task<List<Dette>> GetAll()
        {
            await using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(SelectAllQuery, connection);

            var dataTable = new DataTable();
            var da = new SqlDataAdapter(cmd);

            await connection.OpenAsync();
            da.Fill(dataTable);

            return (from DataRow row in dataTable.Rows select GetDetteFromDataRow(row)).ToList();
        }

        public async Task<List<Dette>> GetByEmployeId(int employeId)
        {
            if (employeId <= 0)
                throw new ArgumentException("Invalid employee ID.", nameof(employeId));

            var dettes = new List<Dette>();

            await using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT * FROM Dettes WHERE EmployeID = @EmployeID", connection);
            cmd.Parameters.AddWithValue("@EmployeID", employeId);

            var dataTable = new DataTable();
            var da = new SqlDataAdapter(cmd);

            await connection.OpenAsync();
            da.Fill(dataTable);

            foreach (DataRow row in dataTable.Rows)
            {
                dettes.Add(GetDetteFromDataRow(row));
            }

            return dettes;
        }



        public async Task<int> Add(Dette dette)
        {
            await using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(InsertQuery, connection);

            cmd.Parameters.AddWithValue("@EmployeID", dette.EmployeID);
            cmd.Parameters.AddWithValue("@Montant", dette.Montant);
            cmd.Parameters.AddWithValue("@Date", dette.Date);
         

            await connection.OpenAsync();
            var id = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(id);
        }

        public async Task Update(Dette dette)
        {
            await using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(UpdateQuery, connection);

            cmd.Parameters.AddWithValue("@EmployeID", dette.EmployeID);
            cmd.Parameters.AddWithValue("@Montant", dette.Montant);
            cmd.Parameters.AddWithValue("@Date", dette.Date);
            cmd.Parameters.AddWithValue("@DetteID", dette.DetteID);

            await connection.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task Delete(int detteId)
        {
            await using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(DeleteQuery, connection);
            cmd.Parameters.AddWithValue("@DetteID", detteId);

            await connection.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }


        public async Task<List<PaimentsInfo>> GetEmployeeDebtDetails()
        {
            List<PaimentsInfo> paimentsInfos = new List<PaimentsInfo>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand("GetEmployeeDebtDetails", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    await connection.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var paimentinfo = new PaimentsInfo
                            {
                                EmployeID = Convert.ToInt32(reader["EmployeID"]),
                                Nom = reader["Nom"].ToString(),
                                Prenom = reader["Prenom"].ToString(),
                                NomFonction = reader["NomFonction"].ToString(),
                                TotaleDette = Convert.ToDecimal(reader["TotaleDette"]),
                                MontantRetrait = Convert.ToDecimal(reader["MontantRetrait"]),
                                TotaleAvances = Convert.ToDecimal(reader["TotaleAvances"])

                            };

                            paimentsInfos.Add(paimentinfo);
                        }
                    }
                }
            }

            return paimentsInfos;
        }
        public async Task<decimal> GetTotalDettes()
        {
            decimal totalDettes = 0;

            await using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("GetTotalDettes", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            await connection.OpenAsync();

            var result = await cmd.ExecuteScalarAsync();
            if (result != DBNull.Value)
            {
                totalDettes = Convert.ToDecimal(result);
            }

            return totalDettes;
        }
    }
}