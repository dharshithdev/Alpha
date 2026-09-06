using System;
using System.Collections.Generic;

namespace Alpha.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string CustomerCode { get; set; } // Auto-generated
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string GSTNumber { get; set; } // For India
        public string CompanyName { get; set; }
        public decimal Balance { get; set; } // Amount owed
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
        public DateTime? LastPurchaseDate { get; set; }

        // Helper properties
        public string DisplayName => string.IsNullOrEmpty(CompanyName) ?
                                     FullName : $"{FullName} ({CompanyName})";
        public bool HasBalance => Balance > 0;
    }
}