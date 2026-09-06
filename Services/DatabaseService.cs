using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using Alpha.Models;

namespace Alpha.Services
{
    public class DatabaseService : IDisposable
    {
        private readonly string _connectionString;
        private bool _disposed = false;

        public DatabaseService()
        {
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "AlphaBilling"
            );

            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            string dbPath = Path.Combine(appDataPath, "AlphaBilling.db");
            _connectionString = $"Data Source={dbPath};";

            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            // Create Users table
            string createUsers = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    Password TEXT NOT NULL,
                    FullName TEXT NOT NULL,
                    Email TEXT,
                    Role TEXT NOT NULL,
                    IsActive INTEGER DEFAULT 1,
                    CreatedDate TEXT DEFAULT CURRENT_TIMESTAMP,
                    LastLoginDate TEXT
                )";

            // Create Products table
            string createProducts = @"
                CREATE TABLE IF NOT EXISTS Products (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Barcode TEXT UNIQUE,
                    Name TEXT NOT NULL,
                    Description TEXT,
                    Category TEXT,
                    PurchasePrice REAL DEFAULT 0,
                    SellingPrice REAL DEFAULT 0,
                    Quantity INTEGER DEFAULT 0,
                    MinStockLevel INTEGER DEFAULT 10,
                    IsActive INTEGER DEFAULT 1,
                    CreatedDate TEXT DEFAULT CURRENT_TIMESTAMP,
                    UpdatedDate TEXT
                )";

            // Create Customers table
            string createCustomers = @"
                CREATE TABLE IF NOT EXISTS Customers (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    CustomerCode TEXT NOT NULL UNIQUE,
                    FullName TEXT NOT NULL,
                    Phone TEXT,
                    Email TEXT,
                    Address TEXT,
                    GSTNumber TEXT,
                    CompanyName TEXT,
                    Balance REAL DEFAULT 0,
                    IsActive INTEGER DEFAULT 1,
                    CreatedDate TEXT DEFAULT CURRENT_TIMESTAMP,
                    LastPurchaseDate TEXT
                )";

            // Create Invoices table
            string createInvoices = @"
                CREATE TABLE IF NOT EXISTS Invoices (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    InvoiceNumber TEXT NOT NULL UNIQUE,
                    CustomerId INTEGER NOT NULL,
                    InvoiceDate TEXT DEFAULT CURRENT_TIMESTAMP,
                    DueDate TEXT,
                    SubTotal REAL DEFAULT 0,
                    Discount REAL DEFAULT 0,
                    Tax REAL DEFAULT 0,
                    GrandTotal REAL DEFAULT 0,
                    AmountPaid REAL DEFAULT 0,
                    PaymentStatus TEXT DEFAULT 'Pending',
                    PaymentMethod TEXT,
                    Notes TEXT,
                    CreatedBy INTEGER NOT NULL,
                    CreatedDate TEXT DEFAULT CURRENT_TIMESTAMP,
                    UpdatedDate TEXT,
                    FOREIGN KEY(CustomerId) REFERENCES Customers(Id),
                    FOREIGN KEY(CreatedBy) REFERENCES Users(Id)
                )";

            // Create InvoiceItems table
            string createInvoiceItems = @"
                CREATE TABLE IF NOT EXISTS InvoiceItems (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    InvoiceId INTEGER NOT NULL,
                    ProductId INTEGER NOT NULL,
                    Quantity INTEGER NOT NULL,
                    UnitPrice REAL NOT NULL,
                    Discount REAL DEFAULT 0,
                    FOREIGN KEY(InvoiceId) REFERENCES Invoices(Id),
                    FOREIGN KEY(ProductId) REFERENCES Products(Id)
                )";

            // Create StockMovements table
            string createStockMovements = @"
                CREATE TABLE IF NOT EXISTS StockMovements (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ProductId INTEGER NOT NULL,
                    Quantity INTEGER NOT NULL,
                    Type TEXT NOT NULL,
                    Reference TEXT,
                    Note TEXT,
                    UserId INTEGER NOT NULL,
                    Date TEXT DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY(ProductId) REFERENCES Products(Id),
                    FOREIGN KEY(UserId) REFERENCES Users(Id)
                )";

            using var command = new SqliteCommand(createUsers, connection);
            command.ExecuteNonQuery();

            command.CommandText = createProducts;
            command.ExecuteNonQuery();

            command.CommandText = createCustomers;
            command.ExecuteNonQuery();

            command.CommandText = createInvoices;
            command.ExecuteNonQuery();

            command.CommandText = createInvoiceItems;
            command.ExecuteNonQuery();

            command.CommandText = createStockMovements;
            command.ExecuteNonQuery();

            // Insert default users if they don't exist
            InsertDefaultUsers(connection);

            connection.Close();
        }

        private void InsertDefaultUsers(SqliteConnection connection)
        {
            var users = new[]
            {
                new { Username = "manager", Password = "manager123", FullName = "Store Manager", Email = "manager@alpha.com", Role = "Manager" },
                new { Username = "inventory", Password = "inventory123", FullName = "Inventory Staff", Email = "inventory@alpha.com", Role = "Inventory" },
                new { Username = "cashier", Password = "cashier123", FullName = "Cashier Staff", Email = "cashier@alpha.com", Role = "Cashier" }
            };

            foreach (var user in users)
            {
                string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @username";
                using var checkCmd = new SqliteCommand(checkQuery, connection);
                checkCmd.Parameters.AddWithValue("@username", user.Username);

                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (count == 0)
                {
                    string insertQuery = @"
                        INSERT INTO Users (Username, Password, FullName, Email, Role)
                        VALUES (@username, @password, @fullname, @email, @role)";

                    using var insertCmd = new SqliteCommand(insertQuery, connection);
                    insertCmd.Parameters.AddWithValue("@username", user.Username);
                    insertCmd.Parameters.AddWithValue("@password", user.Password);
                    insertCmd.Parameters.AddWithValue("@fullname", user.FullName);
                    insertCmd.Parameters.AddWithValue("@email", user.Email);
                    insertCmd.Parameters.AddWithValue("@role", user.Role);
                    insertCmd.ExecuteNonQuery();
                }
            }
        }

        public User GetUser(string username, string password)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = @"
                SELECT Id, Username, FullName, Email, Role, IsActive, LastLoginDate
                FROM Users 
                WHERE Username = @username 
                AND Password = @password 
                AND IsActive = 1";

            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@password", password);

            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new User
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Username = reader["Username"].ToString(),
                    FullName = reader["FullName"].ToString(),
                    Email = reader["Email"]?.ToString(),
                    Role = reader["Role"].ToString(),
                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                    LastLoginDate = reader["LastLoginDate"] != DBNull.Value ?
                                    Convert.ToDateTime(reader["LastLoginDate"]) :
                                    (DateTime?)null
                };
            }

            connection.Close();
            return null;
        }

        public void UpdateLastLogin(int userId)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = "UPDATE Users SET LastLoginDate = CURRENT_TIMESTAMP WHERE Id = @userId";
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@userId", userId);
            command.ExecuteNonQuery();

            connection.Close();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources if any
                }
                _disposed = true;
            }
        }
    }
}