using System;

namespace Alpha.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Barcode { get; set; } // For scanning
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int Quantity { get; set; }
        public int MinStockLevel { get; set; } = 10;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Helper properties
        public decimal ProfitMargin => SellingPrice - PurchasePrice;
        public bool IsLowStock => Quantity <= MinStockLevel;
        public bool IsOutOfStock => Quantity <= 0;
        public string DisplayNameWithStock => $"{Name} ({Quantity} in stock)";
    }
}