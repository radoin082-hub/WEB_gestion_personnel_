using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace GestionPersonnel.Storages.Storages.PostesStorages
{
    public class PosteStorage
    {
        private readonly string _connectionString;

        public PosteStorage(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task InsererDonneesPoste(string idPoste, int idEquipe, DateTime date, List<int> idEmployes)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Insert into PosteComplete and retrieve the generated ID
                string insertPosteCompleteQuery = @"
            INSERT INTO [db_aa9d4f_gestionpersonnel].[dbo].[PosteComplete] ([IdPoste], [IdEquipe], [Date])
            VALUES (@IdPoste, @IdEquipe, @Date);
            SELECT SCOPE_IDENTITY();";

                int idPosteComplete;
                using (SqlCommand command = new SqlCommand(insertPosteCompleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@IdPoste", idPoste);
                    command.Parameters.AddWithValue("@IdEquipe", idEquipe);
                    command.Parameters.AddWithValue("@Date", date);

                    idPosteComplete = Convert.ToInt32(await command.ExecuteScalarAsync());
                }

                // Insert into EmployePoste for each employee
                string insertEmployePosteQuery = @"
            INSERT INTO [db_aa9d4f_gestionpersonnel].[dbo].[EmployePoste] ([IdEmploye], [Date])
            VALUES (@IdEmploye, @Date);";

                foreach (int idEmploye in idEmployes)
                {
                    using (SqlCommand command = new SqlCommand(insertEmployePosteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@IdEmploye", idEmploye);
                        command.Parameters.AddWithValue("@Date", date);
                        await command.ExecuteNonQueryAsync();
                    }
                }

                // Calculate and update the total posts for each employee
                string updateOrInsertTotalePostesQuery = @"
            MERGE [db_aa9d4f_gestionpersonnel].[dbo].[TotalePostes] AS target
            USING (
                SELECT [IdEmploye], COUNT(*) AS TotalePostes
                FROM [db_aa9d4f_gestionpersonnel].[dbo].[EmployePoste]
                WHERE YEAR([Date]) = @Year AND MONTH([Date]) = @Month
                GROUP BY [IdEmploye]
            ) AS source
            ON target.IdEmploye = source.IdEmploye
               AND MONTH(target.[Date]) = @Month
               AND YEAR(target.[Date]) = @Year
            WHEN MATCHED THEN
                UPDATE SET target.TotalePostes = source.TotalePostes
            WHEN NOT MATCHED THEN
                INSERT (IdEmploye, [Date], TotalePostes)
                VALUES (source.IdEmploye, DATEFROMPARTS(@Year, @Month,1), source.TotalePostes);";

                using (SqlCommand command = new SqlCommand(updateOrInsertTotalePostesQuery, connection))
                {
                    command.Parameters.AddWithValue("@Year", date.Year);
                    command.Parameters.AddWithValue("@Month", date.Month);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }


    }
}
