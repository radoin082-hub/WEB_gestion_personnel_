using GestionPersonnel.Models.Employees;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPersonnel.Storages.EmployeesStorages
{
    public class EmployeStorage
    {
        private readonly string _connectionString;

        public EmployeStorage(string connectionString)
        {
            _connectionString = connectionString;
        }

        private const string _selectAllQuery = @"
            SELECT E.EmployeID, E.Nom, E.Prenom, E.DateDeNaissance, E.NSecuriteSocial, E.Adresse, E.GroupSanguin, 
                   E.NTelephone, E.FonctionID, E.DateEntree, E.DateSortie, E.SitiationFamiliale, 
                   E.Photo, F.NomFonction
            FROM Employes E
            INNER JOIN Fonctions F ON E.FonctionID = F.FonctionID
            WHERE E.status = 1";
        private const string _selectByIdQuery = @"
            SELECT E.EmployeID, E.Nom, E.Prenom, E.DateDeNaissance, E.NSecuriteSocial, E.Adresse, E.GroupSanguin, 
                   E.NTelephone, E.FonctionID, E.DateEntree, E.DateSortie, E.SitiationFamiliale, 
                   E.Photo, F.NomFonction
            FROM Employes E
            INNER JOIN Fonctions F ON E.FonctionID = F.FonctionID
            WHERE E.EmployeID = @id";
        private const string _insertQuery = "INSERT INTO Employes (Nom, Prenom, DateDeNaissance, NSecuriteSocial, Adresse, GroupSanguin, NTelephone, FonctionID, DateEntree, DateSortie, SitiationFamiliale, Photo) VALUES (@Nom, @Prenom, @DateDeNaissance, @NSecuriteSocial, @Adresse, @GroupSanguin, @NTelephone, @FonctionID, @DateEntree, @DateSortie, @SitiationFamiliale, @Photo); SELECT SCOPE_IDENTITY();";
        private const string _updateQuery = "UPDATE Employes SET Nom = @Nom, Prenom = @Prenom, DateDeNaissance = @DateDeNaissance, NSecuriteSocial = @NSecuriteSocial, Adresse = @Adresse, GroupSanguin = @GroupSanguin, NTelephone = @NTelephone, FonctionID = @FonctionID, DateEntree = @DateEntree, DateSortie = @DateSortie, SitiationFamiliale = @SitiationFamiliale, Photo = @Photo WHERE EmployeID = @EmployeID;";
        private const string _deleteQuery = "UPDATE Employes SET status = 0, DateSortie = @DateSortie WHERE EmployeID = @EmployeID;";

        private const string _selectByFunctionIdQuery = @"
            SELECT E.EmployeID, E.Nom, E.Prenom, E.DateDeNaissance, E.NSecuriteSocial, E.Adresse, E.GroupSanguin, 
                   E.NTelephone, E.FonctionID, E.DateEntree, E.DateSortie, E.SitiationFamiliale, 
                   E.Photo, F.NomFonction
            FROM Employes E
            INNER JOIN Fonctions F ON E.FonctionID = F.FonctionID
            WHERE E.FonctionID = @FonctionID AND E.status = 1";

        private const string _selectEmployeeIdByNameAndFunctionQuery = @"
    SELECT E.EmployeID
    FROM Employes E
    INNER JOIN Fonctions F ON E.FonctionID = F.FonctionID
    WHERE E.Nom = @Nom AND E.Prenom = @Prenom AND F.NomFonction = @NomFonction";

     


        private static Employee GetEmployeFromDataRow(DataRow row)
        {
            return new Employee
            {
                EmployeID = (int)row["EmployeID"],
                Nom = (string)row["Nom"],
                Prenom = (string)row["Prenom"],
                DateDeNaissance = (DateTime)row["DateDeNaissance"],
                NSecuriteSocial = (string)row["NSecuriteSocial"],
                Adresse = (string)row["Adresse"],
                GroupSanguin = (string)row["GroupSanguin"],
                NTelephone = (string)row["NTelephone"],
                FonctionID = Convert.ToInt32(row["FonctionID"]),
                DateEntree = (DateTime)row["DateEntree"],
                DateSortie = row["DateSortie"] != DBNull.Value ? (DateTime)row["DateSortie"] : (DateTime?)null,
                SitiationFamiliale = (string)row["SitiationFamiliale"],
                Photo = row["Photo"] as byte[],
                FonctionName = row["NomFonction"].ToString()
               
            };
        }

        public async Task<List<Employee>> GetAll()
        {
            await using var connection = new SqlConnection(_connectionString);
            SqlCommand cmd = new(_selectAllQuery, connection);

            DataTable dataTable = new();
            SqlDataAdapter da = new(cmd);

            await connection.OpenAsync();
            da.Fill(dataTable);

            return (from DataRow row in dataTable.Rows select GetEmployeFromDataRow(row)).ToList();
        }

        public async Task<Employee?> GetById(int id)
        {
            await using var connection = new SqlConnection(_connectionString);

            SqlCommand cmd = new(_selectByIdQuery, connection);
            cmd.Parameters.AddWithValue("@id", id);

            DataTable dataTable = new();
            SqlDataAdapter da = new(cmd);

            await connection.OpenAsync();
            da.Fill(dataTable);

            return dataTable.Rows.Count == 0 ? null : GetEmployeFromDataRow(dataTable.Rows[0]);
        }

        public async Task Add(Employee employe)
        {
            await using var connection = new SqlConnection(_connectionString);
            SqlCommand cmd = new(_insertQuery, connection);
            cmd.Parameters.AddWithValue("@Nom", employe.Nom);
            cmd.Parameters.AddWithValue("@Prenom", employe.Prenom);
            cmd.Parameters.AddWithValue("@DateDeNaissance", employe.DateDeNaissance);
            cmd.Parameters.AddWithValue("@NSecuriteSocial", employe.NSecuriteSocial);
            cmd.Parameters.AddWithValue("@Adresse", employe.Adresse);
            cmd.Parameters.AddWithValue("@GroupSanguin", employe.GroupSanguin);
            cmd.Parameters.AddWithValue("@NTelephone", employe.NTelephone);
            cmd.Parameters.AddWithValue("@FonctionID", employe.FonctionID);
            cmd.Parameters.AddWithValue("@DateEntree", employe.DateEntree);
            cmd.Parameters.AddWithValue("@DateSortie", employe.DateSortie ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@SitiationFamiliale", employe.SitiationFamiliale);
            cmd.Parameters.AddWithValue("@Photo", employe.Photo ?? (object)DBNull.Value);

            await connection.OpenAsync();
            var id = await cmd.ExecuteScalarAsync();
            employe.EmployeID = Convert.ToInt32(id);
        }

        public async Task Update(Employee employe)
        {
            await using var connection = new SqlConnection(_connectionString);
            SqlCommand cmd = new(_updateQuery, connection);
            cmd.Parameters.AddWithValue("@Nom", employe.Nom);
            cmd.Parameters.AddWithValue("@Prenom", employe.Prenom);
            cmd.Parameters.AddWithValue("@DateDeNaissance", employe.DateDeNaissance);
            cmd.Parameters.AddWithValue("@NSecuriteSocial", employe.NSecuriteSocial);
            cmd.Parameters.AddWithValue("@Adresse", employe.Adresse);
            cmd.Parameters.AddWithValue("@GroupSanguin", employe.GroupSanguin);
            cmd.Parameters.AddWithValue("@NTelephone", employe.NTelephone);
            cmd.Parameters.AddWithValue("@FonctionID", employe.FonctionID);
            cmd.Parameters.AddWithValue("@DateEntree", employe.DateEntree);
            cmd.Parameters.AddWithValue("@DateSortie", employe.DateSortie ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@SitiationFamiliale", employe.SitiationFamiliale);
            cmd.Parameters.AddWithValue("@Photo", employe.Photo ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@EmployeID", employe.EmployeID);

            await connection.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task Delete(int id)
        {
            await using var connection = new SqlConnection(_connectionString);
            SqlCommand cmd = new(_deleteQuery, connection);
            cmd.Parameters.AddWithValue("@EmployeID", id);
            cmd.Parameters.AddWithValue("@DateSortie", DateTime.Now);

            await connection.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<int> GetTotalNumberOfEmployees()
        {
            await using var connection = new SqlConnection(_connectionString);
            SqlCommand cmd = new("CalculerNombreTotalEmployes", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            var param = new SqlParameter("@nombreTotalEmployes", SqlDbType.Int);
            param.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(param);

            await connection.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            return (int)cmd.Parameters["@nombreTotalEmployes"].Value;
        }

        public async Task<decimal> GetTotalSalaryForMonth(DateTime month)
        {
            try
            {
                await using var connection = new SqlConnection(_connectionString);
                SqlCommand cmd = new("CalculerTotalSalairesDansUnMois", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@mois", month);

                var param = new SqlParameter("@totalSalaires", SqlDbType.Decimal);
                param.Direction = ParameterDirection.Output;
                param.Precision = 10;
                param.Scale = 2;
                cmd.Parameters.Add(param);

                await connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                // Check if the output parameter value is DBNull or NULL
                if (cmd.Parameters["@totalSalaires"].Value == DBNull.Value || cmd.Parameters["@totalSalaires"].Value == null)
                {
                    return 0; // Or handle DBNull/NULL case as per your application logic
                }

                return Convert.ToDecimal(cmd.Parameters["@totalSalaires"].Value);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching the total salary for the month.", ex);
            }
        }

        public async Task<List<Employee>> GetEmployeesByFunctionId(int fonctionId)
        {
            await using var connection = new SqlConnection(_connectionString);
            SqlCommand cmd = new(_selectByFunctionIdQuery, connection);
            cmd.Parameters.AddWithValue("@FonctionID", fonctionId);

            DataTable dataTable = new();
            SqlDataAdapter da = new(cmd);

            await connection.OpenAsync();
            da.Fill(dataTable);

            return (from DataRow row in dataTable.Rows select GetEmployeFromDataRow(row)).ToList();
        }
        public async Task<int?> GetEmployeeIdByName(string nom, string prenom, string nomFonction)
        {
            await using var connection = new SqlConnection(_connectionString);
            SqlCommand cmd = new(_selectEmployeeIdByNameAndFunctionQuery, connection);
            cmd.Parameters.AddWithValue("@Nom", nom);
            cmd.Parameters.AddWithValue("@Prenom", prenom);
            cmd.Parameters.AddWithValue("@NomFonction", nomFonction);

            await connection.OpenAsync();

            var result = await cmd.ExecuteScalarAsync();

            // Check if result is DBNull
            if (result == DBNull.Value || result == null)
            {
                return null; // Employee not found
            }

            return Convert.ToInt32(result); // Return the EmployeeID
        }

    }
}
