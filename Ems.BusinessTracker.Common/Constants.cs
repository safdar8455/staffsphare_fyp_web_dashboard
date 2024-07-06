
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Ems.BusinessTracker.Common
{
    public class Constants
    {
        public const double SixtyOneDaySecond = 5270400.00;
        public const double SixtyDaySecond = 5184000.00;
        public const double ThirtyOneDaySecond = 2678400.00;
        public const double ThirtyDaySecond = 2592000.00;
        public const double ElevenDaySecond = 950400.00;
        public const double TenDaySecond = 864000.00;
        public const double TwoDaySecond = 172800.00;
        public const double OneDaySecond = 86400.00;
        public const string ConnectionStringName = "DataModel";
        public const string LocalFilePath = "/UploadFiles/";
        public static string CurrentUser = "A5151013-DF9F-4234-B0BA-7556A035011E";
        public const string DateFormat = "dd/MM/yyyy";
        public const string TimeFormat = "hh:mm tt";
        public const string DateTimeFormat = "dd/MM/yyyy hh:m   m:ss tt";
        public const string DateSeparator = "/";
        public const string ServerDateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        public const string DateLongFormat = "dd-MMM-yyyy";
        public const string DateTimeLongFormat = "dd-MMM-yyyy hh:mm";
        public const string ResetPassword = "123456";
    }
    public enum MonthList
    {
        January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12
    }
    public static class DateTimeConverter
    {
        public static DateTime ToZoneTime(this DateTime t)
        {
            string bnTimeZoneKey = "Bangladesh Standard Time";
            TimeZoneInfo bdTimeZone = TimeZoneInfo.FindSystemTimeZoneById(bnTimeZoneKey);
            return TimeZoneInfo.ConvertTimeFromUtc((t), bdTimeZone);

        }
    }

    public enum UserType
    {
        SuperAdmin = 1,
        Admin = 2,
        Operator = 3,
        Driver = 4,
        Employee=5
    }

    public enum InputHelpType
    {
        [Description("Nationality")]
        Nationality = 1,
        [Description("Gender")]
        Gender = 2,
        [Description("Sponsor Company")]
        SponsorCompany = 4,
        [Description("Working Location")]
        WorkingLocation = 5,
        [Description("Mother Tongue")]
        MotherTongue = 6,
        [Description("Religion")]
        Religion = 7,
        [Description("Country")]
        Country = 8,
        [Description("Home Airport")]
        HomeAireport = 9,
        [Description("Employee Group")]
        EmployeeGroup = 10,
        [Description("Project")]
        Project = 11,
        [Description("Marital Status")]
        MaritalStatus = 12

    }

    public enum EmployeeStatus
    {
        [Description("Active")]
        Active = 1,
        [Description("In Active")]
        InActive = 2
    }
    public enum CompanyAttachmentType
    {
        CompanyRegistration = 1,
        EstablishmentCard = 2,
        TradeAndMunicipalLicense = 3,
        Others = 4
    }
    public class MenuModel
    {
        public string path { get; set; }
        public string title { get; set; }
        public string icon { get; set; }
        public string cssClass { get; set; }
        public List<MenuItemModel> children { get; set; }
    }

    public class MenuItemModel
    {
        public string path { get; set; }
        public string title { get; set; }
        public string icon { get; set; }
        public string cssClass { get; set; }
    }

    public  class MenuCollection
    {

        public static List<MenuModel> GetMenu(int userTypeId)
        {
            switch (userTypeId)
            {
                case (int)UserType.SuperAdmin:
                    return GetSuperAdminMenu();
                case (int)UserType.Admin:
                    return GetAdminMenu();
                case (int)UserType.Operator:
                    return GetOperatorMenu();
                case (int)UserType.Driver:
                    return GetOperatorMenu();
                default:
                   return new List<MenuModel>();
            }
        }

        private static List<MenuModel> GetSuperAdminMenu()
        {
            var list = new List<MenuModel>();

            list.Add(new MenuModel { path = "/dashboard", title = "Dashboard", icon = "developer_board", cssClass = "dashboard-menu-icon", children = new List<MenuItemModel>() });
            list.Add(new MenuModel { path = "/live-tracking", title = "Vehicle Location", icon = "edit_location", cssClass = "live-menu-icon", children = new List<MenuItemModel>() });
            list.Add(new MenuModel { path = "/vehicle-list", title = "Vehicles", icon = "directions_car", cssClass = "vehicle-menu-icon", children = new List<MenuItemModel>() });

            list.Add(new MenuModel
            {
                path = "/settings",
                title = "Settings",
                icon = "settings",
                cssClass = "settings-menu-icon",
                children = new List<MenuItemModel>
                {
                    new MenuItemModel { path = "/settings/company-list", title = "Company List", icon = "list", cssClass = "nav-item-sub" },
                    new MenuItemModel { path = "/settings/user-list", title = "User List", icon = "contact_mail", cssClass = "nav-item-sub" },
                    new MenuItemModel { path = "/settings/menu-permission", title = "Menu Permissions", icon = "lock_open", cssClass = "nav-item-sub" },
                    new MenuItemModel { path = "/settings/vehicle-type-setup", title = "Vehicle Type Setup", icon = "note_add", cssClass = "nav-item-sub" },
                    new MenuItemModel { path = "/settings/brand-type-setup", title = "Brand Type Setup", icon = "note_add", cssClass = "nav-item-sub" },
                    new MenuItemModel { path = "/settings/brand-setup", title = "Brand Setup", icon = "note_add", cssClass = "nav-item-sub" },
                }
            });

            list.Add(new MenuModel { path = "/login", title = "Logout", icon = "logout", cssClass = "logout-menu-icon", children = new List<MenuItemModel>() });

            return list;
        }

        private static List<MenuModel> GetAdminMenu()
        {
            var list = new List<MenuModel>();

            list.Add(new MenuModel { path = "/dashboard", title = "Dashboard", icon = "developer_board", cssClass = "dashboard-menu-icon", children = new List<MenuItemModel>() });
            list.Add(new MenuModel { path = "/live-tracking", title = "Vehicle Location", icon = "edit_location", cssClass = "live-menu-icon", children = new List<MenuItemModel>() });
            list.Add(new MenuModel { path = "/vehicle-list", title = "Vehicles", icon = "directions_car", cssClass = "vehicle-menu-icon", children = new List<MenuItemModel>() });

            list.Add(new MenuModel
            {
                path = "/maintenance",
                title = "Maintenance Details",
                icon = "build",
                cssClass = "maintenance-menu-icon",
                children = new List<MenuItemModel>
                {
                    new MenuItemModel { path = "/maintenance/driver-details", title = "Driver Details", icon = "accessible", cssClass = "nav-item-sub" },
                    new MenuItemModel { path = "/maintenance/handover-takeover", title = "HandOver/TakeOver Details", icon = "commute", cssClass = "nav-item-sub" },
                    new MenuItemModel { path = "/maintenance/maintenance-cost", title = "Maintenance Cost", icon = "attach_money", cssClass = "nav-item-sub" },
                    new MenuItemModel { path = "/maintenance/vehicle-violations", title = "Violation on Vehicle", icon = "calendar_today", cssClass = "nav-item-sub" },
                    new MenuItemModel { path = "/maintenance/fuel-consumption", title = "Fuel Consumption", icon = "description", cssClass = "nav-item-sub" }
                }
            });

            list.Add(new MenuModel
            {
                path = "/settings",
                title = "Settings",
                icon = "settings",
                cssClass = "settings-menu-icon",
                children = new List<MenuItemModel>
                {
                    new MenuItemModel { path = "/settings/user-list", title = "User List", icon = "contact_mail", cssClass = "nav-item-sub" },
                    new MenuItemModel { path = "/settings/vehicle-owner-company", title = "Vehicle Owner Company", icon = "note_add", cssClass = "nav-item-sub" },
                    new MenuItemModel { path = "/settings/vehicle-user-company", title = "Vehicle User Company", icon = "note_add", cssClass = "nav-item-sub" },
                    new MenuItemModel { path = "/settings/company-project-setup", title = "Company Project Setup", icon = "note_add", cssClass = "nav-item-sub" }

                }
            });

            list.Add(new MenuModel { path = "/login", title = "Logout", icon = "logout", cssClass = "logout-menu-icon", children = new List<MenuItemModel>() });

            return list;
        }

        private static List<MenuModel> GetOperatorMenu()
        {
            var list = new List<MenuModel>();

            list.Add(new MenuModel { path = "/dashboard", title = "Dashboard", icon = "developer_board", cssClass = "dashboard-menu-icon", children = new List<MenuItemModel>() });
            list.Add(new MenuModel { path = "/live-tracking", title = "Vehicle Location", icon = "edit_location", cssClass = "live-menu-icon", children = new List<MenuItemModel>() });
            list.Add(new MenuModel { path = "/vehicle-list", title = "Vehicles", icon = "directions_car", cssClass = "vehicle-menu-icon", children = new List<MenuItemModel>() });

            list.Add(new MenuModel
            {
                path = "/maintenance",
                title = "Maintenance Details",
                icon = "build",
                cssClass = "maintenance-menu-icon",
                children = new List<MenuItemModel>
                {
                    new MenuItemModel { path = "/maintenance/driver-details", title = "Driver Details", icon = "accessible", cssClass = "nav-item-sub" },
                    new MenuItemModel { path = "/maintenance/handover-takeover", title = "HandOver/TakeOver Details", icon = "commute", cssClass = "nav-item-sub" },
                    new MenuItemModel { path = "/maintenance/maintenance-cost", title = "Maintenance Cost", icon = "attach_money", cssClass = "nav-item-sub" },
                    new MenuItemModel { path = "/maintenance/vehicle-violations", title = "Violation on Vehicle", icon = "calendar_today", cssClass = "nav-item-sub" },
                    new MenuItemModel { path = "/maintenance/fuel-consumption", title = "Fuel Consumption", icon = "description", cssClass = "nav-item-sub" }
                }
            });

            list.Add(new MenuModel { path = "/login", title = "Logout", icon = "logout", cssClass = "logout-menu-icon", children = new List<MenuItemModel>() });

            return list;
        }
    }
}
