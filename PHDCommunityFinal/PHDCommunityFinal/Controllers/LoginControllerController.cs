using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PHDCommunityFinal.ApplicationDBContext1;
using PHDCommunityFinal.LoginMail;
using PHDCommunityFinal.Model;
using System.Data.Entity;
using System.Text;

namespace PHDCommunityFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginControllerController : ControllerBase
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IEmailVerification emailVerifications;
        private readonly ApplicationDBContext applicationDBContext;

        public LoginControllerController(IEmailVerification emailVerification, ApplicationDBContext applicationDBContexts)
        {
            emailVerifications = emailVerification;
            applicationDBContext = applicationDBContexts;
        }

        [HttpPost(Name = "ProfileCreation")]
        public async Task<IActionResult> PostStudentProfile(StudentProfile studentProfile)
        {
            try
            {
                await applicationDBContext.StudentProfileFinal.AddAsync(studentProfile);
                await applicationDBContext.SaveChangesAsync();
                var LatestIdenty = await applicationDBContext.StudentProfileFinal.OrderByDescending(e => e.StudentID).FirstOrDefaultAsync();
                string Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(LatestIdenty.StudentID.ToString()));
                string Link = "https://helloworld.com/?loginverification =" + Encoded;
                emailVerifications.SendEmail("nehileans1@gmail.com", "Phd Student Information", "<h1>" + Link + "</h1>");

                return Ok("Email sent successfully. Go and check your email.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpGet(Name = "LoginVerification")]
        public async Task<IActionResult> LoginVerification([FromQuery] string StudenID)
        {
            try
            {
                string decodedString = Encoding.UTF8.GetString(Convert.FromBase64String(StudenID));
                int StudentID = int.Parse(decodedString);
                var StudentProfile = await applicationDBContext.StudentProfileFinal.FirstOrDefaultAsync(a => a.StudentID == StudentID);
                if (StudentProfile != null)
                {
                    bool response = true;
                    //var response = new
                    //{
                    //    Message = "Verified Successfully",
                    //    StudID = StudentProfile.StudentID,
                    //    Email = StudentProfile.Email

                    //};
                    return Ok(response);
                }
                else
                {
                    bool response = false;
                    //var response = new
                    //{
                    //    Message = "Student Not Found",
                    //    StudID = "",
                    //    Email = ""

                    //};
                    return Ok(response);
                }


            }


            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }
    }
}
