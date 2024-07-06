using Microsoft.Practices.Unity;
using System;
using Ems.AttendanceTracking.DataAccess;
using Ems.AttendanceTracking.Interfaces;

namespace Ems.AttendanceTracking.Mappers
{
    public class AttendanceUnityMapper
    {
        private static IUnityContainer _container;

        public static void RegisterMappings(IUnityContainer container)
        {
            _container = container;

            container.RegisterType<IUserCredential, UserCredentialDataAccess>(new ContainerControlledLifetimeManager());
            container.RegisterType<ISetupInputHelp, SetupInputHelpDataAccess>(new ContainerControlledLifetimeManager());
            container.RegisterType<IDepartment, DepartmentDataAccess>(new ContainerControlledLifetimeManager());
            container.RegisterType<IDesignation, DesignationDataAccess>(new ContainerControlledLifetimeManager());
            container.RegisterType<ILeavePolicy, LeavePolicyDataAccess>(new ContainerControlledLifetimeManager());
            container.RegisterType<INoticeBoard, NoticeBoardDataAccess>(new ContainerControlledLifetimeManager());
            container.RegisterType<ITask, TaskDataAccess>(new ContainerControlledLifetimeManager());
            container.RegisterType<IEmployee, EmployeeDataAccess>(new ContainerControlledLifetimeManager());
            container.RegisterType<IAttendance, AttendanceDataAccess>(new ContainerControlledLifetimeManager());
            container.RegisterType<ILeave, LeaveDataAccess>(new ContainerControlledLifetimeManager());
            container.RegisterType<IAttendanceReport, AttendanceReportDataAccess>(new ContainerControlledLifetimeManager());
            container.RegisterType<ICompany, CompanyDataAccess>(new ContainerControlledLifetimeManager());
            container.RegisterType<IHoliday, HolidayDataAccess>(new ContainerControlledLifetimeManager());

        }

        public static T GetInstance<T>()
        {
            try
            {
                return _container.Resolve<T>();
            }
            catch (ResolutionFailedException exception)
            {

            }
            return default(T);
        }
    }
}
