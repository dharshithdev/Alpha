using Alpha.Models;
using Alpha.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

            if (product != null)
            {
                _isEditMode = true;
                _product = product;
                TitleText.Text = "Edit Product";
                LoadProductData();
            }
            else
            {
                _isEditMode = false;
                _product = new Product();
                TitleText.Text = "Add New Product";
            }
        }

        private void LoadProductData()
        {
            BarcodeBox.Text = _product.Barcode;
            NameBox.Text = _product.Name;
            CategoryBox.Text = _product.Category;
            DescriptionBox.Text = _product.Description;
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
                _product.Description = string.IsNullOrWhiteSpace(DescriptionBox.Text) ? null : DescriptionBox.Text.Trim();
                _product.PurchasePrice = purchasePrice;
                _product.SellingPrice = sellingPrice;
                _product.Quantity = quantity;
                _product.MinStockLevel = minStock;

                if (_isEditMode)
                {
                    // Update existing product
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
                    // Add new product
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

        // Auto-select text on focus
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.SelectAll();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _productService?.Dispose();
            base.OnClosed(e);
        }
    }
}