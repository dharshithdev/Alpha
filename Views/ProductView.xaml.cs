using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Alpha.Models;
using Alpha.Services;

namespace Alpha.Views
{
    public partial class ProductView : Window
    {
        private ProductService? _productService;
        private List<Product> _allProducts = new List<Product>();

        public ProductView()
        {
            try
            {
                InitializeComponent();

                // Check if user is logged in
                if (AppState.CurrentUser == null)
                {
                    MessageBox.Show("Please login first.", "Access Denied",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    Close();
                    return;
                }

                // Check permissions
                if (!AppState.HasPermission(UserRoles.Manager, UserRoles.Inventory))
                {
                    MessageBox.Show("Access Denied! Only Managers and Inventory Staff can manage products.",
                                  "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Close();
                    return;
                }

                // Initialize service
                try
                {
                    _productService = new ProductService();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to initialize database: {ex.Message}", "Error",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                    return;
                }

                // Load products
                LoadProducts();
                LoadCategories();

                // Update UI based on role
                UpdateUIForRole();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Product Management: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void UpdateUIForRole()
        {
            if (AppState.IsCashier)
            {
                AddBtn.IsEnabled = false;
                AddBtn.Content = "🔒 View Only";
            }
        }

        private void LoadProducts()
        {
            try
            {
                if (_productService == null) return;

                _allProducts = _productService.GetAllProducts() ?? new List<Product>();
                ProductsGrid.ItemsSource = _allProducts;
                UpdateStatus($"Showing {_allProducts.Count} products");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadCategories()
        {
            try
            {
                if (_productService == null) return;

                CategoryFilter.Items.Clear();
                CategoryFilter.Items.Add("All Categories");

                var categories = _productService.GetAllCategories() ?? new List<string>();
                foreach (var category in categories)
                {
                    if (!string.IsNullOrEmpty(category))
                    {
                        CategoryFilter.Items.Add(category);
                    }
                }

                CategoryFilter.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading categories: {ex.Message}");
            }
        }

        private void FilterProducts()
        {
            try
            {
                string searchTerm = SearchBox.Text?.Trim().ToLower() ?? "";
                string selectedCategory = CategoryFilter.SelectedItem?.ToString() ?? "All Categories";

                var filtered = _allProducts ?? new List<Product>();

                // Apply search
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    filtered = filtered.Where(p =>
                        (p.Name?.ToLower().Contains(searchTerm) ?? false) ||
                        (p.Barcode != null && p.Barcode.ToLower().Contains(searchTerm)) ||
                        (p.Category != null && p.Category.ToLower().Contains(searchTerm))
                    ).ToList();
                }

                // Apply category filter
                if (selectedCategory != "All Categories")
                {
                    filtered = filtered.Where(p => p.Category == selectedCategory).ToList();
                }

                ProductsGrid.ItemsSource = filtered;
                UpdateStatus($"Showing {filtered.Count} of {_allProducts.Count} products");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error filtering products: {ex.Message}");
            }
        }

        private void UpdateStatus(string message)
        {
            try
            {
                StatusText.Text = message;
                CountText.Text = $"Total: {_allProducts?.Count ?? 0} products";
            }
            catch { }
        }

        private void ProductsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle selection if needed
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new ProductDialog();
                dialog.Owner = this;

                if (dialog.ShowDialog() == true)
                {
                    LoadProducts(); // Refresh
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (AppState.IsCashier)
                {
                    MessageBox.Show("Cashiers cannot edit products.", "Access Denied",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var button = sender as Button;
                if (button?.Tag == null) return;

                int productId = Convert.ToInt32(button.Tag);
                if (_productService == null) return;

                var product = _productService.GetProductById(productId);
                if (product == null)
                {
                    MessageBox.Show("Product not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var dialog = new ProductDialog(product);
                dialog.Owner = this;

                if (dialog.ShowDialog() == true)
                {
                    LoadProducts(); // Refresh
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (AppState.IsCashier)
                {
                    MessageBox.Show("Cashiers cannot delete products.", "Access Denied",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var button = sender as Button;
                if (button?.Tag == null) return;

                int productId = Convert.ToInt32(button.Tag);

                var result = MessageBox.Show("Are you sure you want to delete this product?\nThis action cannot be undone!",
                                            "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes && _productService != null)
                {
                    if (_productService.DeleteProduct(productId))
                    {
                        MessageBox.Show("Product deleted successfully.", "Success",
                                      MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadProducts(); // Refresh
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting product: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterProducts();
        }

        private void CategoryFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterProducts();
        }

        private void ClearSearchBtn_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "";
            CategoryFilter.SelectedIndex = 0;
            LoadProducts();
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadProducts();
            LoadCategories();
        }

        protected override void OnClosed(EventArgs e)
        {
            _productService?.Dispose();
            base.OnClosed(e);
        }
    }
}