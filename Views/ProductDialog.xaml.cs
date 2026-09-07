using System;
using System.Windows;
using System.Windows.Input;
using Alpha.Models;
using Alpha.Services;

namespace Alpha.Views
{
    public partial class ProductDialog : Window
    {
        private readonly ProductService _productService;
        private readonly Product _product;
        private readonly bool _isEditMode;

        public ProductDialog(Product product = null)
        {
            InitializeComponent();
            _productService = new ProductService();

            // Allow moving the window by dragging
            this.MouseDown += (s, e) => { if (e.LeftButton == MouseButtonState.Pressed) DragMove(); };

            if (product != null)
            {
                _isEditMode = true;
                _product = product;
                TitleText.Text = "✏️ Edit Product";
                LoadProductData();
            }
            else
            {
                _isEditMode = false;
                _product = new Product();
                TitleText.Text = "➕ Add New Product";
            }
        }

        private void LoadProductData()
        {
            BarcodeBox.Text = _product.Barcode;
            NameBox.Text = _product.Name;
            CategoryBox.Text = _product.Category;
            PurchasePriceBox.Text = _product.PurchasePrice.ToString("0.00");
            SellingPriceBox.Text = _product.SellingPrice.ToString("0.00");
            QuantityBox.Text = _product.Quantity.ToString();
            MinStockBox.Text = _product.MinStockLevel.ToString();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                ShowError("Product Name is required.");
                NameBox.Focus();
                return;
            }

            if (!decimal.TryParse(SellingPriceBox.Text, out decimal sellingPrice) || sellingPrice <= 0)
            {
                ShowError("Please enter a valid Selling Price.");
                SellingPriceBox.Focus();
                return;
            }

            if (!int.TryParse(QuantityBox.Text, out int quantity) || quantity < 0)
            {
                ShowError("Please enter a valid Quantity.");
                QuantityBox.Focus();
                return;
            }

            if (!int.TryParse(MinStockBox.Text, out int minStock) || minStock < 0)
            {
                ShowError("Please enter a valid Minimum Stock level.");
                MinStockBox.Focus();
                return;
            }

            // Parse purchase price (optional)
            decimal purchasePrice = 0;
            if (!string.IsNullOrWhiteSpace(PurchasePriceBox.Text))
            {
                if (!decimal.TryParse(PurchasePriceBox.Text, out purchasePrice) || purchasePrice < 0)
                {
                    ShowError("Please enter a valid Purchase Price.");
                    PurchasePriceBox.Focus();
                    return;
                }
            }

            try
            {
                _product.Barcode = string.IsNullOrWhiteSpace(BarcodeBox.Text) ? null : BarcodeBox.Text.Trim();
                _product.Name = NameBox.Text.Trim();
                _product.Category = string.IsNullOrWhiteSpace(CategoryBox.Text) ? null : CategoryBox.Text.Trim();
                _product.Description = null; // No description field
                _product.PurchasePrice = purchasePrice;
                _product.SellingPrice = sellingPrice;
                _product.Quantity = quantity;
                _product.MinStockLevel = minStock;

                if (_isEditMode)
                {
                    if (_productService.UpdateProduct(_product))
                    {
                        MessageBox.Show("Product updated successfully!", "Success",
                                      MessageBoxButton.OK, MessageBoxImage.Information);
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        ShowError("Failed to update product. Please try again.");
                    }
                }
                else
                {
                    int newId = _productService.AddProduct(_product);
                    if (newId > 0)
                    {
                        MessageBox.Show("Product added successfully!", "Success",
                                      MessageBoxButton.OK, MessageBoxImage.Information);
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        ShowError("Failed to add product. Please try again.");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error: {ex.Message}");
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ShowError(string message)
        {
            ErrorText.Text = message;
            ErrorText.Visibility = Visibility.Visible;
        }

        private void HideError()
        {
            ErrorText.Visibility = Visibility.Collapsed;
        }

        // Enter key triggers Save
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SaveBtn_Click(sender, e);
            }
            else if (e.Key == Key.Escape)
            {
                CancelBtn_Click(sender, e);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _productService?.Dispose();
            base.OnClosed(e);
        }
    }
}