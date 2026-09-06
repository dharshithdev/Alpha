using System;

namespace Alpha.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Role { get; set; } = "Cashier";
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? LastLoginDate { get; set; }

        // Helper properties
        public string DisplayName => $"{FullName} ({Username})";
        public bool IsManager => Role == UserRoles.Manager;
        public bool IsInventoryStaff => Role == UserRoles.Inventory;
        public bool IsCashier => Role == UserRoles.Cashier;
    }

    public static class UserRoles
    {
        public const string Manager = "Manager";
        public const string Inventory = "Inventory";
        public const string Cashier = "Cashier";

        public static readonly string[] AllRoles = { Manager, Inventory, Cashier };
    }
}