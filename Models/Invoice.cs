using System;
using System.Collections.Generic;

namespace Alpha.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } // Auto-generated
        public int CustomerId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime? DueDate { get; set; }

        // Financials
        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal AmountDue => GrandTotal - AmountPaid;

        public string PaymentStatus { get; set; } // Paid, Pending, Overdue
        public string PaymentMethod { get; set; } // Cash, Card, UPI
        public string Notes { get; set; }

        public int CreatedBy { get; set; } // User ID
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Navigation properties
        public virtual Customer Customer { get; set; }
        public virtual User CreatedByUser { get; set; }
        public virtual List<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();

        // Helper properties
        public bool IsPaid => PaymentStatus == "Paid";
        public bool IsPending => PaymentStatus == "Pending";
        public bool IsOverdue => PaymentStatus == "Overdue";
        public int TotalItems => Items?.Count ?? 0;
    }

    public class InvoiceItem
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal Total => (UnitPrice * Quantity) - Discount;

        // Navigation properties
        public virtual Product Product { get; set; }
        public virtual Invoice Invoice { get; set; }
    }
}