using EasyWinFormLibrary.Extension;
using EasyWinFormLibrary.WinAppNeeds;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace EasyWinFormLibrary.Data
{
    public static class AuthUserInfo
    {
        public static string UserID { get; set; }
        public static string Username { get; set; }
        public static string Fullname { get; set; }
        public static string LoginCountID { get; set; }

        public static bool IsFullAdmin = false;
        public static bool IsAdmin = false;

        private static List<string> UserActions = new List<string>();
        private static List<string> UserRoles = new List<string>();
        private static List<string> UserPermissions = new List<string>();

        public enum PermissionType { Insert, Update, Delete, View }

        public static void SetUserActions(DataTable UserActionsDataTable)
        {
            bool NeedReturn = false;
            if (UserActionsDataTable.AsEnumerable().Where(row => row.Field<string>("action_name") == "Admin").Count() > 0)
            {
                IsAdmin = true;
                NeedReturn = true;
            }
            if (UserActionsDataTable.AsEnumerable().Where(row => row.Field<string>("action_name").Contains("Full Admin")).Count() > 0)
            {
                IsFullAdmin = true;
                NeedReturn = true;
            }

            if (NeedReturn)
                return;

            UserActions = UserActionsDataTable.AsEnumerable().Select(row => row.Field<string>("action_name")).ToList();
        }
        public static void SetUserRolesList(DataTable RolesListDataTable)
        {
            if (IsAdmin) return;

            UserRoles = RolesListDataTable.AsEnumerable().Select(row => row.Field<string>("role_name")).ToList();
            foreach (DataRow row in RolesListDataTable.Rows)
            {
                UserPermissions.Add($"{(row["btn_insert"].ToString() == "True" ? "1" : "0")}{(row["btn_update"].ToString() == "True" ? "1" : "0")}{(row["btn_delete"].ToString() == "True" ? "1" : "0")}{(row["btn_view"].ToString() == "True" ? "1" : "0")}");
            }
        }

        public static bool HasAction(string ActionName, bool ShowErrorMessge = true)
        {
            if (IsAdmin || IsFullAdmin)
                return true;

            bool Check = UserActions.Any(s => s == ActionName);

            if (!Check && ShowErrorMessge)
                AdvancedAlert.ShowAlert("ببورە، ڕێگەپێدراونیت بۆ ئەنجامدانی ئەم کردارە", "عذرًا، ليس لديك الإذن للقيام بهذا الإجراء", "Sorry, you are not authorized to perform this action", AdvancedAlert.AlertType.Warning);

            return Check;
        }
        public static bool HasRole(string RoleName, bool ShowErrorMessge = true)
        {
            if (IsAdmin || IsFullAdmin)
                return true;

            bool Check = UserRoles.Any(s => s == RoleName);

            if (!Check && ShowErrorMessge)
                AdvancedAlert.ShowAlert("ببورە، ڕێگەپێدراونیت بۆ چوونەناو ئەم بەشە", "عذرا، غير مسموح لك بالدخول إلى هذه الصفحة", "Sorry, you are not allowed to access this section", AdvancedAlert.AlertType.Warning);

            return Check;
        }
        public static bool HasPermission(string RoleName, PermissionType type, bool ShowErrorMessge = true)
        {
            if (IsAdmin || IsFullAdmin)
                return true;

            bool Check = false;
            switch (type)
            {
                case PermissionType.Insert:
                    Check = UserPermissions[UserRoles.IndexOf(RoleName)][0] == '1' ? true : false;
                    break;
                case PermissionType.Update:
                    Check = UserPermissions[UserRoles.IndexOf(RoleName)][1] == '1' ? true : false;
                    break;
                case PermissionType.Delete:
                    Check = UserPermissions[UserRoles.IndexOf(RoleName)][2] == '1' ? true : false;
                    break;
                case PermissionType.View:
                    Check = UserPermissions[UserRoles.IndexOf(RoleName)][3] == '1' ? true : false;
                    break;
            }

            if (!Check && ShowErrorMessge)
                AdvancedAlert.ShowAlert("ببورە، ڕێگەپێدراونیت بۆ ئەنجامدانی ئەم کردارە", "عذرًا، ليس لديك الإذن للقيام بهذا الإجراء", "Sorry, you are not authorized to perform this action", AdvancedAlert.AlertType.Warning);

            return Check;
        }

        public static async void Register()
        {
            SetUserActions((await SqlDatabaseActions.GetDataAsync($"SELECT (SELECT action_name FROM tbl_actions WHERE action_id=action_id_fk) AS action_name FROM tbl_user_actions WHERE user_id_fk={UserID}")).Data);
            SetUserRolesList((await SqlDatabaseActions.GetDataAsync($"SELECT (SELECT role_name FROM tbl_roles WHERE role_id=role_id_fk) AS role_name, btn_insert, btn_update, btn_delete, btn_view FROM tbl_user_roles WHERE user_id_fk={UserID}")).Data);
            LoginCountID = ((await SqlDatabaseActions.GetSingleValueAsync($"SELECT ISNULL(MAX(count_id),0) FROM tbl_check_user WHERE user_id_fk={UserID}")).Value.TextToInt() + 1).ToString();

            await SqlDatabaseActions.ExecuteCommandAsync("INSERT INTO tbl_check_user VALUES (@maxUserID, @userIdFk, @countId)",
                new SqlParameter[] {
                new SqlParameter("@maxUserID", (await SqlDatabaseActions.GetMaxNumberAsync("tbl_check_user", "id")).MaxNumber),
                new SqlParameter("@userIdFk", UserID),
                new SqlParameter("@countId", LoginCountID)
            });
        }
        public static async void RemoveUser(bool isDoubleUser)
        {
            await SqlDatabaseActions.ExecuteCommandAsync($"DELETE FROM tbl_user_actions WHERE user_id_fk=@userIdFk {(isDoubleUser ? "AND count_id=@countID" : "")}",
                new SqlParameter[] {
                    new SqlParameter("@userIdFk", UserID),
                    new SqlParameter("@countID", LoginCountID)
                });

            UserActions.Clear();
            UserRoles.Clear();
            UserPermissions.Clear();
            IsAdmin = IsFullAdmin = false;
        }
    }
}
