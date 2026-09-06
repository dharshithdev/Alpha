using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using Alpha.Models;

namespace Alpha.Services
{
    public class ProductService : IDisposable
    {
        private readonly string _connectionString;
        private bool _disposed = false;

        public ProductService()
        {
            try
            {
                // Get the database path
                string appDataPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "AlphaBilling"
                );

                // Create directory if it doesn't exist
                if (!Directory.Exists(appDataPath))
                {
                    Directory.CreateDirectory(appDataPath);
                }

                string dbPath = Path.Combine(appDataPath, "AlphaBilling.db");
                _connectionString = $"Data Source={dbPath};";

                // Verify database exists
                if (!File.Exists(dbPath))
                {
                    throw new Exception($"Database not found at: {dbPath}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to initialize ProductService: {ex.Message}");
            }
        }

        public List<Product> GetAllProducts(bool includeInactive = false)
        {
            var products = new List<Product>();

            try
            {
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();

                string query = @"
                    SELECT Id, Barcode, Name, Description, Category, 
                           PurchasePrice, SellingPrice, Quantity, MinStockLevel, 
                           IsActive, CreatedDate, UpdatedDate
                    FROM Products";

                if (!includeInactive)
                {
                    query += " WHERE IsActive = 1";
                }

                query += " ORDER BY Name";

                using var command = new SqliteCommand(query, connection);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Barcode = reader["Barcode"]?.ToString(),
                        Name = reader["Name"]?.ToString() ?? string.Empty,
                        Description = reader["Description"]?.ToString(),
                        Category = reader["Category"]?.ToString(),
                        PurchasePrice = Convert.ToDecimal(reader["PurchasePrice"]),
                        SellingPrice = Convert.ToDecimal(reader["SellingPrice"]),
                        Quantity = Convert.ToInt32(reader["Quantity"]),
                        MinStockLevel = Convert.ToInt32(reader["MinStockLevel"]),
                        IsActive = Convert.ToBoolean(reader["IsActive"]),
                        CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                        UpdatedDate = reader["UpdatedDate"] != DBNull.Value ?
                                      Convert.ToDateTime(reader["UpdatedDate"]) :
                                      (DateTime?)null
                    });
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetAllProducts: {ex.Message}");
                throw;
            }

            return products;
        }

        public Product? GetProductById(int id)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();

                string query = @"
                    SELECT Id, Barcode, Name, Description, Category, 
                           PurchasePrice, SellingPrice, Quantity, MinStockLevel, 
                           IsActive, CreatedDate, UpdatedDate
                    FROM Products
                    WHERE Id = @id";

                using var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                using var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new Product
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Barcode = reader["Barcode"]?.ToString(),
                        Name = reader["Name"]?.ToString() ?? string.Empty,
                        Description = reader["Description"]?.ToString(),
                        Category = reader["Category"]?.ToString(),
                        PurchasePrice = Convert.ToDecimal(reader["PurchasePrice"]),
                        SellingPrice = Convert.ToDecimal(reader["SellingPrice"]),
                        Quantity = Convert.ToInt32(reader["Quantity"]),
                        MinStockLevel = Convert.ToInt32(reader["MinStockLevel"]),
                        IsActive = Convert.ToBoolean(reader["IsActive"]),
                        CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                        UpdatedDate = reader["UpdatedDate"] != DBNull.Value ?
                                      Convert.ToDateTime(reader["UpdatedDate"]) :
                                      (DateTime?)null
                    };
                }

                connection.Close();
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetProductById: {ex.Message}");
                throw;
            }
        }

        public Product? GetProductByBarcode(string barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode)) return null;

            try
            {
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();

                string query = @"
                    SELECT Id, Barcode, Name, Description, Category, 
                           PurchasePrice, SellingPrice, Quantity, MinStockLevel, 
                           IsActive, CreatedDate, UpdatedDate
                    FROM Products
                    WHERE Barcode = @barcode AND IsActive = 1";

                using var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@barcode", barcode);

                using var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new Product
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Barcode = reader["Barcode"]?.ToString(),
                        Name = reader["Name"]?.ToString() ?? string.Empty,
                        Description = reader["Description"]?.ToString(),
                        Category = reader["Category"]?.ToString(),
                        PurchasePrice = Convert.ToDecimal(reader["PurchasePrice"]),
                        SellingPrice = Convert.ToDecimal(reader["SellingPrice"]),
                        Quantity = Convert.ToInt32(reader["Quantity"]),
                        MinStockLevel = Convert.ToInt32(reader["MinStockLevel"]),
                        IsActive = Convert.ToBoolean(reader["IsActive"]),
                        CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                        UpdatedDate = reader["UpdatedDate"] != DBNull.Value ?
                                      Convert.ToDateTime(reader["UpdatedDate"]) :
                                      (DateTime?)null
                    };
                }

                connection.Close();
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetProductByBarcode: {ex.Message}");
                throw;
            }
        }

        public List<Product> SearchProducts(string searchTerm)
        {
            var products = new List<Product>();

            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAllProducts();

            try
            {
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();

                string query = @"
                    SELECT Id, Barcode, Name, Description, Category, 
                           PurchasePrice, SellingPrice, Quantity, MinStockLevel, 
                           IsActive, CreatedDate, UpdatedDate
                    FROM Products
                    WHERE IsActive = 1
                    AND (Name LIKE @search 
                         OR Barcode LIKE @search 
                         OR Category LIKE @search
                         OR Description LIKE @search)
                    ORDER BY Name";

                using var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@search", $"%{searchTerm}%");

                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Barcode = reader["Barcode"]?.ToString(),
                        Name = reader["Name"]?.ToString() ?? string.Empty,
                        Description = reader["Description"]?.ToString(),
                        Category = reader["Category"]?.ToString(),
                        PurchasePrice = Convert.ToDecimal(reader["PurchasePrice"]),
                        SellingPrice = Convert.ToDecimal(reader["SellingPrice"]),
                        Quantity = Convert.ToInt32(reader["Quantity"]),
                        MinStockLevel = Convert.ToInt32(reader["MinStockLevel"]),
                        IsActive = Convert.ToBoolean(reader["IsActive"]),
                        CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                        UpdatedDate = reader["UpdatedDate"] != DBNull.Value ?
                                      Convert.ToDateTime(reader["UpdatedDate"]) :
                                      (DateTime?)null
                    });
                }

                connection.Close();
                return products;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in SearchProducts: {ex.Message}");
                throw;
            }
        }

        public int AddProduct(Product product)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();

                string query = @"
                    INSERT INTO Products (Barcode, Name, Description, Category, 
                                         PurchasePrice, SellingPrice, Quantity, 
                                         MinStockLevel, IsActive)
                    VALUES (@barcode, @name, @description, @category, 
                            @purchasePrice, @sellingPrice, @quantity, 
                            @minStockLevel, 1);
                    SELECT last_insert_rowid();";

                using var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@barcode", product.Barcode ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@name", product.Name);
                command.Parameters.AddWithValue("@description", product.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@category", product.Category ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@purchasePrice", product.PurchasePrice);
                command.Parameters.AddWithValue("@sellingPrice", product.SellingPrice);
                command.Parameters.AddWithValue("@quantity", product.Quantity);
                command.Parameters.AddWithValue("@minStockLevel", product.MinStockLevel);

                int newId = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();

                return newId;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in AddProduct: {ex.Message}");
                throw;
            }
        }

        public bool UpdateProduct(Product product)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();

                string query = @"
                    UPDATE Products 
                    SET Barcode = @barcode,
                        Name = @name,
                        Description = @description,
                        Category = @category,
                        PurchasePrice = @purchasePrice,
                        SellingPrice = @sellingPrice,
                        Quantity = @quantity,
                        MinStockLevel = @minStockLevel,
                        UpdatedDate = CURRENT_TIMESTAMP
                    WHERE Id = @id";

                using var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@id", product.Id);
                command.Parameters.AddWithValue("@barcode", product.Barcode ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@name", product.Name);
                command.Parameters.AddWithValue("@description", product.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@category", product.Category ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@purchasePrice", product.PurchasePrice);
                command.Parameters.AddWithValue("@sellingPrice", product.SellingPrice);
                command.Parameters.AddWithValue("@quantity", product.Quantity);
                command.Parameters.AddWithValue("@minStockLevel", product.MinStockLevel);

                int rowsAffected = command.ExecuteNonQuery();
                connection.Close();

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in UpdateProduct: {ex.Message}");
                throw;
            }
        }

        public bool DeleteProduct(int id)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();

                string query = "UPDATE Products SET IsActive = 0, UpdatedDate = CURRENT_TIMESTAMP WHERE Id = @id";
                using var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                int rowsAffected = command.ExecuteNonQuery();
                connection.Close();

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in DeleteProduct: {ex.Message}");
                throw;
            }
        }

        public bool UpdateStock(int productId, int quantityChange, string type, string reference, int userId, string note = "")
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();
                using var transaction = connection.BeginTransaction();

                // Update product quantity
                string updateQuery = @"
                    UPDATE Products 
                    SET Quantity = Quantity + @change,
                        UpdatedDate = CURRENT_TIMESTAMP
                    WHERE Id = @productId";

                using var updateCmd = new SqliteCommand(updateQuery, connection, transaction);
                updateCmd.Parameters.AddWithValue("@change", quantityChange);
                updateCmd.Parameters.AddWithValue("@productId", productId);
                updateCmd.ExecuteNonQuery();

                // Log stock movement
                string logQuery = @"
                    INSERT INTO StockMovements (ProductId, Quantity, Type, Reference, Note, UserId)
                    VALUES (@productId, @quantity, @type, @reference, @note, @userId)";

                using var logCmd = new SqliteCommand(logQuery, connection, transaction);
                logCmd.Parameters.AddWithValue("@productId", productId);
                logCmd.Parameters.AddWithValue("@quantity", quantityChange);
                logCmd.Parameters.AddWithValue("@type", type);
                logCmd.Parameters.AddWithValue("@reference", reference ?? (object)DBNull.Value);
                logCmd.Parameters.AddWithValue("@note", note ?? (object)DBNull.Value);
                logCmd.Parameters.AddWithValue("@userId", userId);
                logCmd.ExecuteNonQuery();

                transaction.Commit();
                connection.Close();

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in UpdateStock: {ex.Message}");
                throw;
            }
        }

        public List<string> GetAllCategories()
        {
            var categories = new List<string>();

            try
            {
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();

                string query = "SELECT DISTINCT Category FROM Products WHERE IsActive = 1 AND Category IS NOT NULL ORDER BY Category";
                using var command = new SqliteCommand(query, connection);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var category = reader["Category"]?.ToString();
                    if (!string.IsNullOrEmpty(category))
                    {
                        categories.Add(category);
                    }
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetAllCategories: {ex.Message}");
            }

            return categories;
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