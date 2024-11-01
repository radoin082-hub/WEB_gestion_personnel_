using GestionPersonnel.Models.TypeDePaiment;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPersonnel.Storages.TypeDePaimentStorages
{
    public class TypeDePaiementStorage
    {
        private readonly string _connectionString;

        public TypeDePaiementStorage(string connectionString)
        {
            _connectionString = connectionString;
        }

        private const string _selectAllQuery = "SELECT * FROM TypesDePaiement";

        private const string _selectByIdQuery = "SELECT * FROM TypesDePaiement WHERE TypePaiementID = @id";
        private const string _insertQuery = "INSERT INTO TypesDePaiement (NomTypePaiement) VALUES (@NomTypePaiement); SELECT SCOPE_IDENTITY();";
        private const string _updateQuery = "UPDATE TypesDePaiement SET NomTypePaiement = @NomTypePaiement WHERE TypePaiementID = @TypePaiementID;";
        private const string _deleteQuery = "DELETE FROM TypesDePaiement WHERE TypePaiementID = @TypePaiementID;";

        
        private static TypeDePaiement GetTypeDePaiementFromDataRow(DataRow row)
        {
            return new TypeDePaiement
            {
                TypePaiementID = (int)row["TypePaiementID"],
                NomTypePaiement = (string)row["NomTypePaiement"]
            };
        }

        public async Task<List<TypeDePaiement>> GetAll()
        {
            await using var connection = new SqlConnection(_connectionString);
            SqlCommand cmd = new(_selectAllQuery, connection);

            DataTable dataTable = new();
            SqlDataAdapter da = new(cmd);

            connection.Open();
            da.Fill(dataTable);

            return (from DataRow row in dataTable.Rows select GetTypeDePaiementFromDataRow(row)).ToList();
        }

        public async Task<TypeDePaiement?> GetById(int id)
        {
            await using var connection = new SqlConnection(_connectionString);

            SqlCommand cmd = new(_selectByIdQuery, connection);
            cmd.Parameters.AddWithValue("@id", id);

            DataTable dataTable = new();
            SqlDataAdapter da = new(cmd);

            connection.Open();
            da.Fill(dataTable);

            return dataTable.Rows.Count == 0 ? null : GetTypeDePaiementFromDataRow(dataTable.Rows[0]);
        }

        public async Task Add(TypeDePaiement typeDePaiement)
        {
            await using var connection = new SqlConnection(_connectionString);
            SqlCommand cmd = new(_insertQuery, connection);
            cmd.Parameters.AddWithValue("@NomTypePaiement", typeDePaiement.NomTypePaiement);

            connection.Open();
            var id = await cmd.ExecuteScalarAsync();
            typeDePaiement.TypePaiementID = Convert.ToInt32(id);
        }

        public async Task Update(TypeDePaiement typeDePaiement)
        {
            await using var connection = new SqlConnection(_connectionString);
            SqlCommand cmd = new(_updateQuery, connection);
            cmd.Parameters.AddWithValue("@NomTypePaiement", typeDePaiement.NomTypePaiement);
            cmd.Parameters.AddWithValue("@TypePaiementID", typeDePaiement.TypePaiementID);

            connection.Open();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task Delete(int id)
        {
            await using var connection = new SqlConnection(_connectionString);
            SqlCommand cmd = new(_deleteQuery, connection);
            cmd.Parameters.AddWithValue("@TypePaiementID", id);

            connection.Open();
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
