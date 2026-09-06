using Alpha.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Alpha.Views
{
    public partial class DashboardView : Window
    {
        public DashboardView()
        {
            InitializeComponent();

            // Security check
            if (!AppState.IsLoggedIn)
            {
                MessageBox.Show("Session expired. Please login again.", "Access Denied",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                Close();
                return;
            }

            // Setup UI based on user
            var user = AppState.CurrentUser;
            if (user != null)
            {
                UserNameText.Text = user.FullName;
                UserRoleText.Text = $"Role: {user.Role}";
                WelcomeText.Text = $"Welcome, {user.FullName}!";
                RoleText.Text = $"You are logged in as: {user.Role}";

                // Set role badge
                RoleBadgeText.Text = user.Role;
                SetRoleBadgeColor(user.Role);
            }

            // Setup role-based access
            SetupRoleBasedAccess();

            // Show dashboard content
            ShowDashboard();
        }

        private void SetRoleBadgeColor(string role)
        {
            switch (role)
            {
                case "Manager":
                    RoleBadge.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2C3E8A"));
                    break;
                case "Inventory":
                    RoleBadge.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#38A169"));
                    break;
                case "Cashier":
                    RoleBadge.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D69E2E"));
                    break;
                default:
                    RoleBadge.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#718096"));
                    break;
            }
        }

        private void SetupRoleBasedAccess()
        {
            var role = AppState.CurrentUserRole;

            // Set default visibility
            ProductsBtn.Visibility = Visibility.Collapsed;
            InventoryBtn.Visibility = Visibility.Collapsed;
            InvoicesBtn.Visibility = Visibility.Collapsed;
            CustomersBtn.Visibility = Visibility.Collapsed;
            ReportsBtn.Visibility = Visibility.Collapsed;
            UsersBtn.Visibility = Visibility.Collapsed;

            switch (role)
            {
                case "Manager":
                    // Show ALL features
                    ProductsBtn.Visibility = Visibility.Visible;
                    InventoryBtn.Visibility = Visibility.Visible;
                    InvoicesBtn.Visibility = Visibility.Visible;
                    CustomersBtn.Visibility = Visibility.Visible;
                    ReportsBtn.Visibility = Visibility.Visible;
                    UsersBtn.Visibility = Visibility.Visible;
                    break;

                case "Inventory":
                    // Show only inventory and products
                    ProductsBtn.Visibility = Visibility.Visible;
                    InventoryBtn.Visibility = Visibility.Visible;
                    break;

                case "Cashier":
                    // Show only sales and customers
                    InvoicesBtn.Visibility = Visibility.Visible;
                    CustomersBtn.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void SetActiveButton(Button activeButton)
        {
            // Reset all buttons
            DashboardBtn.Style = FindResource("SidebarButton") as System.Windows.Style;
            ProductsBtn.Style = FindResource("SidebarButton") as System.Windows.Style;
            InventoryBtn.Style = FindResource("SidebarButton") as System.Windows.Style;
            InvoicesBtn.Style = FindResource("SidebarButton") as System.Windows.Style;
            CustomersBtn.Style = FindResource("SidebarButton") as System.Windows.Style;
            ReportsBtn.Style = FindResource("SidebarButton") as System.Windows.Style;
            UsersBtn.Style = FindResource("SidebarButton") as System.Windows.Style;

            // Set active button
            if (activeButton != null)
            {
                activeButton.Style = FindResource("ActiveSidebarButton") as System.Windows.Style;
            }
        }

        private void ShowDashboard()
        {
            SetActiveButton(DashboardBtn);
            ContentText.Text = @"
📊 DASHBOARD OVERVIEW

Welcome to Alpha Billing System!

Quick Stats (Coming Soon):
• Today's Sales: $0.00
• Total Customers: 0
• Products in Stock: 0
• Pending Invoices: 0

Low Stock Alerts:
No products with low stock.

Recent Activity:
No recent activity.

Select a feature from the sidebar to get started.";
        }

        private void DashboardBtn_Click(object sender, RoutedEventArgs e)
        {
            ShowDashboard();
        }

        private void ProductsBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var productView = new ProductView();
                productView.Owner = this;
                productView.ShowDialog();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error opening Products: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InventoryBtn_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(InventoryBtn);
            ContentText.Text = @"
📋 INVENTORY MANAGEMENT

Stock Management Page (Coming Soon!)

Features:
• Receive new stock (Stock In)
• Remove stock (Stock Out)
• Update quantities
• View stock history
• Low stock alerts
• Stock takes
• Remove expired items

This page will be accessible to:
✅ Manager
✅ Inventory Staff
❌ Cashier";
        }

        private void InvoicesBtn_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(InvoicesBtn);
            ContentText.Text = @"
🧾 INVOICE MANAGEMENT

Invoice Page (Coming Soon!)

Features:
• Create new invoice
• Scan barcodes
• View invoice history
• Print invoices
• Record payments
• Track overdue invoices
• Customer balance updates

This page will be accessible to:
✅ Manager
✅ Cashier
❌ Inventory Staff";
        }

        private void CustomersBtn_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(CustomersBtn);
            ContentText.Text = @"
👤 CUSTOMER MANAGEMENT

Customer Page (Coming Soon!)

Features:
• View customer list
• Add new customers
• Edit customer details
• Delete customers
• View purchase history
• Track balances
• Customer search

This page will be accessible to:
✅ Manager
✅ Cashier
❌ Inventory Staff";
        }

        private void ReportsBtn_Click(object sender, RoutedEventArgs e)
        {
            // Security check - Only Managers
            if (!AppState.IsManager)
            {
                MessageBox.Show("Access Denied! Only Managers can view reports.",
                              "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SetActiveButton(ReportsBtn);
            ContentText.Text = @"
📈 REPORTS DASHBOARD

Reports Page (Coming Soon!)

Available Reports:
• Daily Sales Report
• Weekly Sales Summary
• Monthly Revenue
• Top Selling Products
• Customer Purchase History
• Stock Value Report
• Profit & Loss Statement
• Tax Report

⚠️ Access: Manager Only";
        }

        private void UsersBtn_Click(object sender, RoutedEventArgs e)
        {
            // Security check - Only Managers
            if (!AppState.IsManager)
            {
                MessageBox.Show("Access Denied! Only Managers can manage users.",
                              "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SetActiveButton(UsersBtn);
            ContentText.Text = @"
👥 USER MANAGEMENT

User Management Page (Coming Soon!)

Features:
• View all users
• Add new users
• Edit user details
• Assign roles
• Enable/Disable users
• Reset passwords

Roles Available:
1. Manager - Full access
2. Inventory - Stock management
3. Cashier - Sales only

⚠️ Access: Manager Only";
        }

        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to logout?",
                                        "Logout Confirmation",
                                        MessageBoxButton.YesNo,
                                        MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Clear user state
                AppState.Logout();

                // Open login window
                var login = new MainWindow();
                login.Show();

                // Close dashboard
                this.Close();
            }
        }
    }
}