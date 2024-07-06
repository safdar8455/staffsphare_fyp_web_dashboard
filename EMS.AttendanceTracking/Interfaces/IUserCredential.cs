using Ems.BusinessTracker.Common;
using Ems.BusinessTracker.Common.Models;
using System.Collections.Generic;
namespace Ems.AttendanceTracking.Interfaces
{
    public interface IUserCredential
    {
        ResponseModel Save(UserCredentialModel model);
        List<UserCredentialModel> GetAllUser();
        List<UserCredentialModel> GetAllUser(int cId);
        List<UserCredentialModel> GetAll(UserCredentialModel searchModel);
        ResponseModel Update(UserCredentialModel model);
        UserCredentialModel Get(string username, string password);

        ResponseModel ChangePassword(string userInitial, string newPassword);
        ResponseModel ChangePasswordByPk(string userKey, string newPassword);
        UserCredentialModel GetProfileDetails(string userId);
        UserCredentialModel GetByLoginID(string loginID);
        UserCredentialModel GetByLoginID(string loginID, UserType uType);
        UserCredentialModel GetByLoginID(string loginID, string password, UserType uType);
        UserCredentialModel GetUserFullInfo(string userId);
        ResponseModel updateDeviceIdentifier(DeviceInformation model);
        ResponseModel RemoveDevice(string uId);
        ResponseModel ResetUserPass(string uId);
    }
}