using System;
using System.Data.SqlClient;

namespace TelegramBot
{
    // Работа с базой данных переводов
    class Database
    {
        static string connectionString = @"Data Source = (localdb)\MSSQLLocalDB; Initial Catalog = master; Integrated Security = True";
        static SqlConnection sqlConnection = new SqlConnection(connectionString);

        // Создание базы данных TelegramBotDB
        public static void CreateDatabase()
        {
            string createDatabase = "IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'TelegramBotDB') CREATE DATABASE TelegramBotDB;";
            SqlCommand createDatabaseCommand = new SqlCommand(createDatabase, sqlConnection);

            string createTable = "IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LocalBuffer') " +
                "CREATE TABLE LocalBuffer\r\n(\r\n\tTranslationID INT PRIMARY KEY IDENTITY NOT NULL," +
                "\r\n\tOriginal NVARCHAR(100) NOT NULL,\r\n\tTranslation NVARCHAR(100) NOT NULL\r\n)";
            SqlCommand createTableCommand = new SqlCommand(createTable, sqlConnection);

            try
            {
                sqlConnection.Open();
                createDatabaseCommand.ExecuteNonQuery();
                sqlConnection.ChangeDatabase("TelegramBotDB");

                try
                {
                    createTableCommand.ExecuteNonQuery();
                    Console.WriteLine("База данных TelegramBotDB создана\n");
                }
                catch (Exception)
                {
                    throw;
                }

                createTableCommand.ExecuteNonQuery();
                Console.WriteLine("База данных TelegramBotDB подключена\n");
            }
            catch (SqlException sqlExceprion)
            {
                Console.WriteLine("Базы данных не существует");
                Console.WriteLine(sqlExceprion.Message);
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        // Добавление данных в базу данных
        public static void AddData(string original, string translation)
        {
            string insertValue = $"INSERT INTO LocalBuffer VALUES (N'{original}', N'{translation}');";
            SqlCommand insertValueCommand = new SqlCommand(insertValue, sqlConnection);

            try
            {
                sqlConnection.Open();
                sqlConnection.ChangeDatabase("TelegramBotDB");
                insertValueCommand.ExecuteNonQuery();
            }
            catch (SqlException sqlExceprion)
            {
                Console.WriteLine(sqlExceprion.Message);
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        // Проверка на наличие перевода в базе данных
        public static bool IsCheckTranslation(string message)
        {
            bool found = false;
            string isCheckTranslation = $"SELECT Translation FROM LocalBuffer WHERE Original = N'{message}'";
            SqlCommand IsCheckTranslationCommand = new SqlCommand(isCheckTranslation, sqlConnection);
            IsCheckTranslationCommand.Parameters.AddWithValue("@message", message);

            try
            {
                sqlConnection.Open();
                sqlConnection.ChangeDatabase("TelegramBotDB");
                SqlDataReader reader = IsCheckTranslationCommand.ExecuteReader();

                if (reader.Read())
                {
                    found = true;
                }
            }
            catch (SqlException sqlExceprion)
            {
                Console.WriteLine(sqlExceprion.Message);
            }
            finally
            {
                sqlConnection.Close();
            }

            if (found)
            {
                Console.WriteLine("Текст найден в базе данных");
                return true;
            }
            else
            {
                Console.WriteLine("Текст не найден в базе данных");
                return false;
            }
        }

        // Получение перевода с базы данных
        public static string GetData(string original)
        {
            string translation = $"SELECT Translation FROM LocalBuffer WHERE Original = N'{original}'";
            SqlCommand translationCommand = new SqlCommand(translation, sqlConnection);
            translationCommand.Parameters.AddWithValue("@original", original);

            try
            {
                sqlConnection.Open();
                sqlConnection.ChangeDatabase("TelegramBotDB");
                SqlDataReader reader = translationCommand.ExecuteReader();

                if (reader.Read())
                {
                    return reader["Translation"].ToString();
                }
                else
                {
                    return "Name not found";
                }
            }
            catch (SqlException sqlExceprion)
            {
                Console.WriteLine(sqlExceprion.Message);
            }
            finally
            {
                sqlConnection.Close();
            }

            return "Name not found";
        }
    }
}