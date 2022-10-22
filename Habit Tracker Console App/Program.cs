using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Habit_Tracker_Console_App
{
    internal class Program
    {
        static string connectionString = @"Data Source=habit-Tracker.db";
        static void Main(string[] args)
        {

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText = @"CREATE TABLE IF NOT EXISTS drinking_water (
                                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        Date TEXT,
                                        Quantity INTEGER)";

                tableCmd.ExecuteNonQuery();

                connection.Close();

            }
            GetUserInput();
        }

        static void GetUserInput()
        {
            bool closeApp = false;

            while (closeApp == false)
            {
                Console.WriteLine("\nMAIN MENU");
                Console.WriteLine("\nWhat would you like to do?");
                Console.WriteLine("\nType 0 to Close Application");
                Console.WriteLine("Type 1 to View All Records");
                Console.WriteLine("Type 2 to Insert a Record");
                Console.WriteLine("Type 3 to Delete a Record");
                Console.WriteLine("Type 4 to Update a Record");
                Console.WriteLine("----------------------------");
                Console.WriteLine();

                int input = int.Parse(Console.ReadLine());

                switch (input)
                {
                    case 0:
                        Console.WriteLine("\nGoodbye!\n");
                        closeApp = true;
                        break;
                    case 1:
                        GetAllRecords();
                        break;
                    case 2:
                        Insert();
                        break;
                    case 3:
                        Delete();
                        break;
                    case 4:
                        Update();
                        break;
                    default:
                        Console.WriteLine("\nInvalid Command. Please type a number from 0 to 4.");
                        break;
                }
            }
        }

        private static void Insert()
        {
            string date = GetDateInput();
            int quantity = GetNumberInput("\n\nPlease insert number of glasses or other measure of your choise (no decimals allowed)\n");

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText = $"INSERT INTO drinking_water(date, quantity) VALUES('{date}',{quantity})";

                tableCmd.ExecuteNonQuery();

                connection.Close();

            }
        }

        private static int GetNumberInput(string message)
        {
            Console.WriteLine(message);
            string numInput = Console.ReadLine();

            if(numInput == "0") { GetUserInput(); }
            int finalInput = int.Parse(numInput);
            return finalInput;
        }

        internal static string GetDateInput()
        {
            Console.WriteLine("\n\nPlease insert the date: (Format mm-dd-yy). Type 0 to return to main menu.");

            string dateInput = Console.ReadLine();

            if(dateInput == "0")
            {
                GetUserInput();
            }
            return dateInput;
        }

        private static void GetAllRecords()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText = "SELECT * FROM drinking_water";

                List<DrinkingWater> tableData = new();

                SqliteDataReader reader = tableCmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tableData.Add(new DrinkingWater
                        {
                            Id = reader.GetInt32(0),
                            Date = DateTime.ParseExact(reader.GetString(1), "MM-dd-yy", new CultureInfo("en-US")),
                            Quantity = reader.GetInt32(2)
                        });
                    }
                } 
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("No rows found");
                    Console.WriteLine();
                }
                connection.Close();

                foreach (var row in tableData)
                {
                    Console.WriteLine();
                    Console.WriteLine($"Record {row.Id} - {row.Date.ToString("MM-dd-yyyy")} - Quantity: {row.Quantity}");
                    Console.WriteLine();
                }
            }
        }

        private static void Delete()
        {
            Console.Clear();
            GetAllRecords();

            var recordId = GetNumberInput("\n\nPlease type the ID of the record you want to delete or 0 to return to the main menu.\n");

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText = $"DELETE FROM drinking_water WHERE Id = {recordId}";

                int rowCount = tableCmd.ExecuteNonQuery();

                if(rowCount == 0)
                {
                    Console.WriteLine($"\nRecord Id {recordId} does NOT exist.\n");
                    Delete();
                }

                connection.Close();
            }
            Console.WriteLine($"\nRecord with ID {recordId} was deleted!\n");
            GetUserInput();
        }

        private static void Update()
        {
            Console.Clear();
            GetAllRecords();

            var recordId = GetNumberInput("\n\nPlease type the ID of the record you want to update or 0 to return to the main menu.\n");

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var checkCmd = connection.CreateCommand();
                checkCmd.CommandText = $"SELECT EXISTS(SELECT 1 FROM drinking_water WHERE Id = {recordId})";
                int checkQuery = Convert.ToInt32(checkCmd.ExecuteScalar());

                if(checkQuery == 0)
                {
                    Console.WriteLine($"\nRecord Id {recordId} does NOT exist.\n");
                    connection.Close();
                    Update();
                }
                string date = GetDateInput();
                int quantity = GetNumberInput("\n\nPlease insert number of glasses or other measure of your choise (no decimals allowed)\n");

                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText = $"UPDATE drinking_water SET Date = '{date}', Quantity = {quantity} WHERE Id = {recordId}";

                tableCmd.ExecuteNonQuery();

                connection.Close();

            }
        }
    }
}
