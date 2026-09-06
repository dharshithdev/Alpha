using System;
using System.Windows;
using Alpha.Models;
using Alpha.Services;
using Alpha.Views;

namespace Alpha
{
    public partial class MainWindow : Window
    {
        private readonly DatabaseService _databaseService;

        public MainWindow()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            this.Loaded += (s, e) => UsernameBox.Focus();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.Trim();
            string password = PasswordBox.Password;

            // Validate input
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ShowError("Please enter both username and password");
                return;
            }

            // Disable button and show loading
            LoginButton.IsEnabled = false;
            LoginButton.Content = "Signing in...";
            HideError();

            try
            {
                // Simulate async operation (database call)
                await System.Threading.Tasks.Task.Delay(500);

                // Get user from database
                var user = _databaseService.GetUser(username, password);

                if (user != null)
                {
                    // Store user in global state
                    AppState.CurrentUser = user;

                    // Update last login
                    _databaseService.UpdateLastLogin(user.Id);

                    // Open dashboard
                    var dashboard = new DashboardView();
                    dashboard.Show();

                    // Close login window
                    this.Close();
                }
                else
                {
                    ShowError("Invalid username or password. Please try again.");
                    PasswordBox.Password = "";
                    PasswordBox.Focus();
                }
            }
            catch (Exception ex)
            {
                ShowError($"An error occurred: {ex.Message}");
            }
            finally
            {
                // Re-enable button
                LoginButton.IsEnabled = true;
                LoginButton.Content = "Sign In";
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
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

        protected override void OnClosed(EventArgs e)
        {
            _databaseService?.Dispose();
            base.OnClosed(e);
        }
    }
}