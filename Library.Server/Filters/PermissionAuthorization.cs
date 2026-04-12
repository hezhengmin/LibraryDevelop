using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Library.Server.Authorization
{
    /// <summary>
    /// 權限常數定義
    /// </summary>
    public static class Permissions
    {
        /// <summary>
        /// 圖書管理權限
        /// </summary>
        public static class Books
        {
            public const string View = "Books.View";
            public const string Create = "Books.Create";
            public const string Edit = "Books.Edit";
            public const string Delete = "Books.Delete";
        }

        /// <summary>
        /// 使用者管理權限
        /// </summary>
        public static class Users
        {
            public const string View = "Users.View";
            public const string Create = "Users.Create";
            public const string Edit = "Users.Edit";
            public const string Delete = "Users.Delete";
            public const string ResetPassword = "Users.ResetPassword";
        }

        /// <summary>
        /// 角色管理權限
        /// </summary>
        public static class Roles
        {
            public const string View = "Roles.View";
            public const string Create = "Roles.Create";
            public const string Edit = "Roles.Edit";
            public const string Delete = "Roles.Delete";
        }

        /// <summary>
        /// 系統管理權限
        /// </summary>
        public static class System
        {
            public const string Menu = "System.Menu";
            public const string MenuCreate = "System.Menu.Create";
            public const string MenuUpdate = "System.Menu.Update";
            public const string MenuDelete = "System.Menu.Delete";
        }
    }
}

