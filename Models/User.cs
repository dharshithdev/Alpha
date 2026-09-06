using System;

namespace Alpha.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } // Will be hashed in production
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } // Manager, Inventory, Cashier
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }

        // Helper properties
        public string DisplayName => $"{FullName} ({Username})";
        public bool IsManager => Role == "Manager";
        public bool IsInventoryStaff => Role == "Inventory";
        public bool IsCashier => Role == "Cashier";
    }

    public static class UserRoles
    {
        public const string Manager = "Manager";
        public const string Inventory = "Inventory";
        public const string Cashier = "Cashier";

        public static readonly string[] AllRoles = { Manager, Inventory, Cashier };
    }
}