using EasyWinFormLibrary.Extension;
using EasyWinFormLibrary.WinAppNeeds;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace EasyWinFormLibrary.Data
{
    /// <summary>
    /// Provides authentication and authorization functionality for user management.
    /// Handles user permissions, roles, and actions with support for multi-level access control.
    /// Supports admin privileges and database-driven permission management.
    /// </summary>
    public static class AuthUserInfo
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the unique identifier of the current authenticated user
        /// </summary>
        public static string UserID { get; set; }

        /// <summary>
        /// Gets or sets the username of the current authenticated user
        /// </summary>
        public static string Username { get; set; }

        /// <summary>
        /// Gets or sets the full name of the current authenticated user
        /// </summary>
        public static string Fullname { get; set; }

        /// <summary>
        /// Gets or sets the login count identifier for tracking user sessions
        /// </summary>
        public static string LoginCountID { get; set; }

        /// <summary>
        /// Indicates whether the current user has full administrator privileges (highest level)
        /// </summary>
        public static bool IsFullAdmin = false;

        /// <summary>
        /// Indicates whether the current user has administrator privileges
        /// </summary>
        public static bool IsAdmin = false;

        #endregion

        #region Private Collections

        /// <summary>
        /// List of actions that the current user is authorized to perform
        /// </summary>
        private static List<string> UserActions = new List<string>();

        /// <summary>
        /// List of roles assigned to the current user
        /// </summary>
        private static List<string> UserRoles = new List<string>();

        /// <summary>
        /// List of permission strings for each role (encoded as 4-character strings: Insert-Update-Delete-View)
        /// </summary>
        private static List<string> UserPermissions = new List<string>();

        #endregion

        #region Enums

        /// <summary>
        /// Defines the types of permissions that can be checked for user authorization
        /// </summary>
        public enum PermissionType
        {
            /// <summary>Permission to create/insert new records</summary>
            Insert,
            /// <summary>Permission to modify/update existing records</summary>
            Update,
            /// <summary>Permission to remove/delete records</summary>
            Delete
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the user actions from a DataTable containing action permissions.
        /// Automatically detects admin and full admin privileges.
        /// </summary>
        /// <param name="UserActionsDataTable">DataTable containing action_name column with user's allowed actions</param>
        /// <remarks>
        /// If user has "Admin" or "Full Admin" actions, the respective flags are set and no individual actions are stored.
        /// Admin and Full Admin users have access to all functions without individual permission checks.
        /// </remarks>
        public static void SetUserActions(DataTable UserActionsDataTable)
        {
            bool NeedReturn = false;

            // Check for Admin privileges
            if (UserActionsDataTable.AsEnumerable().Where(row => row.Field<string>("action_name") == "Admin").Count() > 0)
            {
                IsAdmin = true;
                NeedReturn = true;
            }

            // Check for Full Admin privileges (highest level)
            if (UserActionsDataTable.AsEnumerable().Where(row => row.Field<string>("action_name").Contains("Full Admin")).Count() > 0)
            {
                IsFullAdmin = true;
                NeedReturn = true;
            }

            // If user is admin, no need to store individual actions
            if (NeedReturn)
                return;

            // Store individual actions for non-admin users
            UserActions = UserActionsDataTable.AsEnumerable().Select(row => row.Field<string>("action_name")).ToList();
        }

        /// <summary>
        /// Sets the user roles and their associated permissions from a DataTable.
        /// Builds permission strings for each role encoding Insert, Update, Delete, and View permissions.
        /// </summary>
        /// <param name="RolesListDataTable">DataTable containing role_name, btn_insert, btn_update, btn_delete, btn_view columns</param>
        /// <remarks>
        /// Permissions are encoded as 4-character strings where each position represents:
        /// Position 0: Insert permission (1=allowed, 0=denied)
        /// Position 1: Update permission (1=allowed, 0=denied)
        /// Position 2: Delete permission (1=allowed, 0=denied)
        /// Position 3: View permission (1=allowed, 0=denied)
        /// Admin users skip this process as they have all permissions.
        /// </remarks>
        public static void SetUserRolesList(DataTable RolesListDataTable)
        {
            // Admin users don't need role-based permissions
            if (IsAdmin) return;

            // Extract role names
            UserRoles = RolesListDataTable.AsEnumerable().Select(row => row.Field<string>("role_name")).ToList();

            // Build permission strings for each role
            foreach (DataRow row in RolesListDataTable.Rows)
            {
                UserPermissions.Add($"{(row["btn_insert"].ToString() == "True" ? "1" : "0")}{(row["btn_update"].ToString() == "True" ? "1" : "0")}{(row["btn_delete"].ToString() == "True" ? "1" : "0")}");
            }
        }

        /// <summary>
        /// Checks if the current user has permission to perform a specific action.
        /// Admin and Full Admin users automatically have access to all actions.
        /// </summary>
        /// <param name="ActionName">The name of the action to check permission for</param>
        /// <param name="ShowErrorMessge">Whether to display an error message if permission is denied (default: true)</param>
        /// <returns>True if user has permission, false otherwise</returns>
        /// <remarks>
        /// Error messages are displayed in Kurdish, Arabic, and English for accessibility.
        /// </remarks>
        public static bool HasAction(string ActionName, bool ShowErrorMessge = true)
        {
            // Admin users have access to all actions
            if (IsAdmin || IsFullAdmin)
                return true;

            // Check if user has the specific action
            bool Check = UserActions.Any(s => s == ActionName);

            // Show error message if permission denied and messages are enabled
            if (!Check && ShowErrorMessge)
                AdvancedAlert.ShowAlert("ببورە، ڕێگەپێدراونیت بۆ ئەنجامدانی ئەم کردارە", "عذرًا، ليس لديك الإذن للقيام بهذا الإجراء", "Sorry, you are not authorized to perform this action", AdvancedAlert.AlertType.Warning);

            return Check;
        }

        /// <summary>
        /// Checks if the current user has access to a specific role/section.
        /// Admin and Full Admin users automatically have access to all roles.
        /// </summary>
        /// <param name="RoleName">The name of the role to check access for</param>
        /// <param name="ShowErrorMessge">Whether to display an error message if access is denied (default: true)</param>
        /// <returns>True if user has access to the role, false otherwise</returns>
        /// <remarks>
        /// Error messages are displayed in Kurdish, Arabic, and English for accessibility.
        /// </remarks>
        public static bool HasRole(string RoleName, bool ShowErrorMessge = true)
        {
            // Admin users have access to all roles
            if (IsAdmin || IsFullAdmin)
                return true;

            // Check if user has the specific role
            bool Check = UserRoles.Any(s => s == RoleName);

            // Show error message if access denied and messages are enabled
            if (!Check && ShowErrorMessge)
                AdvancedAlert.ShowAlert("ببورە، ڕێگەپێدراونیت بۆ چوونەناو ئەم بەشە", "عذرا، غير مسموح لك بالدخول إلى هذه الصفحة", "Sorry, you are not allowed to access this section", AdvancedAlert.AlertType.Warning);

            return Check;
        }

        /// <summary>
        /// Checks if the current user has a specific permission type for a given role.
        /// Admin and Full Admin users automatically have all permissions.
        /// </summary>
        /// <param name="RoleName">The name of the role to check permissions for</param>
        /// <param name="type">The type of permission to check (Insert, Update, Delete, or View)</param>
        /// <param name="ShowErrorMessge">Whether to display an error message if permission is denied (default: true)</param>
        /// <returns>True if user has the specified permission, false otherwise</returns>
        /// <exception cref="System.ArgumentException">Thrown when RoleName is not found in user's roles</exception>
        /// <remarks>
        /// Permissions are checked using the encoded permission strings where each character position
        /// represents a different permission type. Error messages are displayed in multiple languages.
        /// </remarks>
        public static bool HasPermission(string RoleName, PermissionType type, bool ShowErrorMessge = true)
        {
            // Admin users have all permissions
            if (IsAdmin || IsFullAdmin)
                return true;

            bool Check = false;
            int roleIndex = UserRoles.IndexOf(RoleName);

            // Check if role exists for this user
            if (roleIndex == -1)
            {
                if (ShowErrorMessge)
                    AdvancedAlert.ShowAlert("ببورە، ڕێگەپێدراونیت بۆ ئەنجامدانی ئەم کردارە", "عذرًا، ليس لديك الإذن للقيام بهذا الإجراء", "Sorry, you are not authorized to perform this action", AdvancedAlert.AlertType.Warning);
                return false;
            }

            // Check specific permission type
            switch (type)
            {
                case PermissionType.Insert:
                    Check = UserPermissions[roleIndex][0] == '1' ? true : false;
                    break;
                case PermissionType.Update:
                    Check = UserPermissions[roleIndex][1] == '1' ? true : false;
                    break;
                case PermissionType.Delete:
                    Check = UserPermissions[roleIndex][2] == '1' ? true : false;
                    break;
            }

            // Show error message if permission denied and messages are enabled
            if (!Check && ShowErrorMessge)
                AdvancedAlert.ShowAlert("ببورە، ڕێگەپێدراونیت بۆ ئەنجامدانی ئەم کردارە", "عذرًا، ليس لديك الإذن للقيام بهذا الإجراء", "Sorry, you are not authorized to perform this action", AdvancedAlert.AlertType.Warning);

            return Check;
        }

        /// <summary>
        /// Registers a user by loading their actions and roles from the database and creating a login session.
        /// This method should be called during the login process to initialize user permissions.
        /// </summary>
        /// <param name="userID">The unique identifier of the user to register</param>
        /// <param name="userName"> The User name of the user to register</param>
        /// <param name="fullName"> The full name of the user to register</param>
        /// <remarks>
        /// This method performs the following operations:
        /// 1. Loads user actions from view_user_actions_by_group
        /// 2. Loads user roles and permissions from view_user_roles_by_group  
        /// 3. Generates a new login count ID for session tracking
        /// 4. Records the login session in tbl_user_check for security monitoring
        /// 
        /// The login count ID helps track multiple concurrent sessions and provides
        /// audit trail for user activities.
        /// </remarks>
        public static async Task Register(string userID, string userName, string fullName)
        {
            // Load user actions from database
            SetUserActions((await SqlDatabaseActions.GetDataAsync($"SELECT action_name FROM view_user_actions_by_group WHERE user_id={userID}")).Data);

            // Load user roles and permissions from database
            SetUserRolesList((await SqlDatabaseActions.GetDataAsync($"SELECT role_name, btn_insert, btn_update, btn_delete FROM view_user_roles_by_group WHERE user_id={userID}")).Data);

            // Generate new login count ID for session tracking
            LoginCountID = ((await SqlDatabaseActions.GetSingleValueAsync($"SELECT ISNULL(MAX(count_id),0) FROM tbl_user_check WHERE user_id_fk={userID}")).Value.TextToInt() + 1).ToString();

            // Record login session in database for security monitoring
            await SqlDatabaseActions.ExecuteCommandAsync("INSERT INTO tbl_user_check VALUES (@maxUserID, @userIdFk, @countId)",
                new SqlParameter[] {
                new SqlParameter("@maxUserID", (await SqlDatabaseActions.GetMaxNumberAsync("tbl_user_check", "id")).MaxNumber),
                new SqlParameter("@userIdFk", userID),
                new SqlParameter("@countId", LoginCountID)
            });

            UserID = userID;
            Username = userName;
            Fullname = fullName;

            SqlDatabaseConnectionConfigBuilder.SelectedDatabaseConfig.AuthUserId = userID;
        }

        /// <summary>
        /// Removes user session data from the database and clears all cached permissions.
        /// This method should be called during logout or when ending user sessions.
        /// </summary>
        /// <param name="isDoubleUser">
        /// If true, removes only the specific login session (for handling multiple concurrent logins).
        /// If false, removes all login sessions for the user.
        /// </param>
        /// <remarks>
        /// This method performs cleanup operations:
        /// 1. Removes login session records from tbl_user_check
        /// 2. Clears all cached user actions, roles, and permissions
        /// 3. Resets admin flags to false
        /// 
        /// The isDoubleUser parameter allows for granular session management,
        /// useful when users may have multiple concurrent sessions.
        /// </remarks>
        public static async void RemoveUser(bool isDoubleUser)
        {
            // Remove login session(s) from database
            await SqlDatabaseActions.ExecuteCommandAsync($"DELETE FROM tbl_user_check WHERE user_id_fk=@userIdFk {(isDoubleUser ? "AND count_id=@countID" : "")}",
                new SqlParameter[] {
                    new SqlParameter("@userIdFk", UserID),
                    new SqlParameter("@countID", LoginCountID)
                });

            // Clear all cached user data
            UserActions.Clear();
            UserRoles.Clear();
            UserPermissions.Clear();

            // Reset admin flags
            IsAdmin = IsFullAdmin = false;
        }

        #endregion
    }
}