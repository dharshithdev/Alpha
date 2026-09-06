using System;

namespace Alpha.Models
{
    public static class AppState
    {
        private static User _currentUser;
        private static bool _isInitialized = false;

        public static User CurrentUser
        {
            get { return _currentUser; }
            set
            {
                _currentUser = value;
                _isInitialized = true;
            }
        }

        public static bool IsLoggedIn => _currentUser != null && _isInitialized;

        public static string CurrentUserRole => _currentUser?.Role ?? string.Empty;

        public static bool IsManager => CurrentUser?.Role == UserRoles.Manager;
        public static bool IsInventoryStaff => CurrentUser?.Role == UserRoles.Inventory;
        public static bool IsCashier => CurrentUser?.Role == UserRoles.Cashier;

        public static bool HasPermission(params string[] allowedRoles)
        {
            if (!IsLoggedIn) return false;

            foreach (var role in allowedRoles)
            {
                if (CurrentUser.Role == role)
                    return true;
            }
            return false;
        }

        public static void Logout()
        {
            _currentUser = null;
            _isInitialized = false;
        }
    }
}