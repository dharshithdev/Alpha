using Alpha.Models;
using Alpha.Views;
using System;
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

            // Show dashboard content by default
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
                    ProductsBtn.Visibility = Visibility.Visible;
                    InventoryBtn.Visibility = Visibility.Visible;
                    InvoicesBtn.Visibility = Visibility.Visible;
                    CustomersBtn.Visibility = Visibility.Visible;
                    ReportsBtn.Visibility = Visibility.Visible;
                    UsersBtn.Visibility = Visibility.Visible;
                    break;

                case "Inventory":
                    ProductsBtn.Visibility = Visibility.Visible;
                    InventoryBtn.Visibility = Visibility.Visible;
                    break;

                case "Cashier":
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

        // ========== NAVIGATION METHODS ==========

        private void ShowDashboard()
        {
            SetActiveButton(DashboardBtn);

            // Create a simple dashboard content
            var dashboardContent = new StackPanel
            {
                Margin = new Thickness(25),
                VerticalAlignment = VerticalAlignment.Top
            };

            dashboardContent.Children.Add(new TextBlock
            {
                Text = "📊 Dashboard Overview",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D3748")),
                Margin = new Thickness(0, 0, 0, 15)
            });

            dashboardContent.Children.Add(new TextBlock
            {
                Text = "Welcome to Alpha Billing System!\n\nQuick Stats (Coming Soon):\n• Today's Sales: $0.00\n• Total Customers: 0\n• Products in Stock: 0\n• Pending Invoices: 0\n\nLow Stock Alerts:\nNo products with low stock.\n\nRecent Activity:\nNo recent activity.\n\nSelect a feature from the sidebar to get started.",
                FontSize = 14,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4A5568")),
                TextWrapping = TextWrapping.Wrap
            });

            MainContent.Content = dashboardContent;
        }

        private void DashboardBtn_Click(object sender, RoutedEventArgs e)
        {
            ShowDashboard();
        }

        private void ProductsBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetActiveButton(ProductsBtn);

                // Load Product Page in the ContentControl
                var productPage = new ProductPage();
                MainContent.Content = productPage;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Products: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InventoryBtn_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(InventoryBtn);

            var content = new StackPanel { Margin = new Thickness(25) };
            content.Children.Add(new TextBlock
            {
                Text = "📋 Inventory Management",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D3748")),
                Margin = new Thickness(0, 0, 0, 15)
            });
            content.Children.Add(new TextBlock
            {
                Text = "Stock Management Page (Coming Soon!)\n\nFeatures:\n• Receive new stock (Stock In)\n• Remove stock (Stock Out)\n• Update quantities\n• View stock history\n• Low stock alerts\n• Stock takes\n• Remove expired items",
                FontSize = 14,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4A5568")),
                TextWrapping = TextWrapping.Wrap
            });
            MainContent.Content = content;
        }

        private void InvoicesBtn_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(InvoicesBtn);

            var content = new StackPanel { Margin = new Thickness(25) };
            content.Children.Add(new TextBlock
            {
                Text = "🧾 Invoice Management",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D3748")),
                Margin = new Thickness(0, 0, 0, 15)
            });
            content.Children.Add(new TextBlock
            {
                Text = "Invoice Page (Coming Soon!)\n\nFeatures:\n• Create new invoice\n• Scan barcodes\n• View invoice history\n• Print invoices\n• Record payments\n• Track overdue invoices\n• Customer balance updates",
                FontSize = 14,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4A5568")),
                TextWrapping = TextWrapping.Wrap
            });
            MainContent.Content = content;
        }

        private void CustomersBtn_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(CustomersBtn);

            var content = new StackPanel { Margin = new Thickness(25) };
            content.Children.Add(new TextBlock
            {
                Text = "👤 Customer Management",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D3748")),
                Margin = new Thickness(0, 0, 0, 15)
            });
            content.Children.Add(new TextBlock
            {
                Text = "Customer Page (Coming Soon!)\n\nFeatures:\n• View customer list\n• Add new customers\n• Edit customer details\n• Delete customers\n• View purchase history\n• Track balances\n• Customer search",
                FontSize = 14,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4A5568")),
                TextWrapping = TextWrapping.Wrap
            });
            MainContent.Content = content;
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

            var content = new StackPanel { Margin = new Thickness(25) };
            content.Children.Add(new TextBlock
            {
                Text = "📈 Reports Dashboard",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D3748")),
                Margin = new Thickness(0, 0, 0, 15)
            });
            content.Children.Add(new TextBlock
            {
                Text = "Reports Page (Coming Soon!)\n\nAvailable Reports:\n• Daily Sales Report\n• Weekly Sales Summary\n• Monthly Revenue\n• Top Selling Products\n• Customer Purchase History\n• Stock Value Report\n• Profit & Loss Statement\n• Tax Report\n\n⚠️ Access: Manager Only",
                FontSize = 14,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4A5568")),
                TextWrapping = TextWrapping.Wrap
            });
            MainContent.Content = content;
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

            var content = new StackPanel { Margin = new Thickness(25) };
            content.Children.Add(new TextBlock
            {
                Text = "👥 User Management",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D3748")),
                Margin = new Thickness(0, 0, 0, 15)
            });
            content.Children.Add(new TextBlock
            {
                Text = "User Management Page (Coming Soon!)\n\nFeatures:\n• View all users\n• Add new users\n• Edit user details\n• Assign roles\n• Enable/Disable users\n• Reset passwords\n\nRoles Available:\n1. Manager - Full access\n2. Inventory - Stock management\n3. Cashier - Sales only\n\n⚠️ Access: Manager Only",
                FontSize = 14,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4A5568")),
                TextWrapping = TextWrapping.Wrap
            });
            MainContent.Content = content;
        }

        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to logout?",
                                        "Logout Confirmation",
                                        MessageBoxButton.YesNo,
                                        MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                AppState.Logout();
                var login = new MainWindow();
                login.Show();
                this.Close();
            }
        }
    }
}