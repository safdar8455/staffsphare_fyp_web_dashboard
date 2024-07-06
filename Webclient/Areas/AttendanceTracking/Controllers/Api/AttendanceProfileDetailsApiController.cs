using Ems.BusinessTracker.Common;
using System.Web.Http;
using Ems.AttendanceTracking.Interfaces;
using Ems.AttendanceTracking.Mappers;
using Webclient.Controllers.Api;
using Webclient.Models;
using Ems.BusinessTracker.Common.Models;

namespace Webclient.Areas.AttendanceTracking.Controllers.Api
{
    public class AttendanceProfileDetailsApiController : BaseApiController
    {
        private readonly IUserCredential _userCredential;

        public AttendanceProfileDetailsApiController()
        {
            _userCredential = AttendanceUnityMapper.GetInstance<IUserCredential>();
        }


        [HttpGet]
        public ProfileViewModel GetUserClaims()
        {
            var dd = _userCredential.GetProfileDetails(this.UserId);
            ProfileViewModel model = new ProfileViewModel()
            {
                Id = dd.Id,
                UserName = dd.LoginID,
                PhoneNumber = dd.ContactNo,
                Email = dd.Email,
                Gender = "Male",
                UserFullName = dd.FullName,
                CompanyName=dd.CompanyName,
                Address = dd.Address,
                HasAccessQrCodeScan=dd.HasAccessQrCodeScan,
                UserType = dd.UserTypeId == (int)UserType.Admin ? "admin" : "user",
                UniqueDeviceIdentifier=dd.UniqueDeviceIdentifier,
                IsMultipleDevieAllow=dd.IsMultipleDevieAllow,

            };
            return model;
        }
        [HttpPost]
        public IHttpActionResult updateDeviceIdentifier([FromBody] DeviceInformation model)
        {
            model.userId = this.UserId;
            var response = _userCredential.updateDeviceIdentifier(model);
            return Ok(response);
        }
    }
}
