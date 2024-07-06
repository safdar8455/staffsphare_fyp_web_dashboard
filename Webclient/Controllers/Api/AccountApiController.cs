using Ems.BusinessTracker.Common;
using System.Linq;
using System.Web.Http;
using Ems.AttendanceTracking.Interfaces;
using Ems.AttendanceTracking.Mappers;
using Webclient.Models;

namespace Webclient.Controllers.Api
{
    public class AccountApiController : ApiController
    {
        private readonly IUserCredential _userCredential;
        private readonly ICompany _companyRepository;

        public AccountApiController()
        {
            _userCredential = AttendanceUnityMapper.GetInstance<IUserCredential>();
            _companyRepository = AttendanceUnityMapper.GetInstance<ICompany>();
        }

        /// <summary>
        /// for vehicle login
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult LoginApps([FromBody]PortalLoginModel model)
        {
            if (ModelState.IsValid)
            {
                var password = CryptographyHelper.CreateMD5Hash(model.Password);
                var user = _userCredential.Get(model.UserName, password);
                if (user == null)
                    return Ok(new { Success = false, message = "Invalid userid/password" });

                if (user.UserTypeId == (int)UserType.SuperAdmin)
                    return Ok(new { Success = false, message = "You have no permission" });

                //var driverInfo = _driverInfo.GetByLoginID(model.UserName);
                //if (driverInfo == null)
                //    return Ok(new { Success = false, message = "You have no permission" });

                var companyDetails = _companyRepository.GetCompanyList().FirstOrDefault(x => x.Id == user.CompanyId && x.IsActive.HasValue && x.IsActive.Value);
                if (companyDetails == null)
                    return Ok(new { Success = false, Token=string.Empty, Message = "Your company now disabled.Please contact to administrator." });

                return Ok(new
                {
                    Success = true,
                    Token = TokenManager.GenerateToken(model.UserName, user.Id, user.CompanyId),
                    UserName = user.FullName,
                    user.UserTypeId,

                });
            }

            return BadRequest();
        }

    }
}
