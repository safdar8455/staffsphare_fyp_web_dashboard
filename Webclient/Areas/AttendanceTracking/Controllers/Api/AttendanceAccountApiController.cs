using Ems.BusinessTracker.Common;
using System.Linq;
using System.Web.Http;
using Ems.AttendanceTracking.Interfaces;
using Ems.AttendanceTracking.Mappers;
using Webclient.Controllers.Api;
using Webclient.Models;

namespace Webclient.Areas.AttendanceTracking.Controllers.Api
{
    public class AttendanceAccountApiController : ApiController
    {
        private readonly IUserCredential _userCredential;
        private readonly ICompany _companyRepository;

        public AttendanceAccountApiController()
        {
            _userCredential = AttendanceUnityMapper.GetInstance<IUserCredential>();
            _companyRepository = AttendanceUnityMapper.GetInstance<ICompany>();
        }

        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult Login([FromBody]PortalLoginModel model)
        {
            if (ModelState.IsValid)
            {
                var password = CryptographyHelper.CreateMD5Hash(model.Password);
                var user = _userCredential.Get(model.UserName, password);
                if (user == null)
                    return Ok(new { Success = false, message = "Invalid userid/password" });

                if (user.UserTypeId != (int)UserType.Employee && user.UserTypeId!= (int)UserType.Admin)
                    return Ok(new { Success = false, message = "You have no permission" });

                var companyDetails = _companyRepository.GetCompanyList().FirstOrDefault(x => x.Id == user.CompanyId && x.IsActive.HasValue && x.IsActive.Value);
                if (companyDetails == null)
                    return Ok(new { Success = false, Token=string.Empty, Message = "Your company now disabled.Please contact to administrator." });

                return Ok(new
                {
                    Success = true,
                    Token = TokenManager.GenerateToken(model.UserName, user.Id, user.CompanyId),
                    UserName = user.FullName,
                    user.UserTypeId,
                    UserId=user.Id,
                    HasAccessQrCodeScan=user.HasAccessQrCodeScan
                });
            }

            return BadRequest();
        }

    }
}
