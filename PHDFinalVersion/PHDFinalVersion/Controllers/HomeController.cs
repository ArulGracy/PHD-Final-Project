using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using PHDFinalVersion.AppDBContext;
using PHDFinalVersion.Email_Verification;
using PHDFinalVersion.Models;
using static System.Net.WebRequestMethods;

namespace PHDFinalVersion.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IEmailVerification _emailVerification;
        private readonly ApplicationDBContext _applicationDBContext;

        public HomeController(IEmailVerification emailVerification,ApplicationDBContext applicationDBContext)
        {
            _emailVerification = emailVerification;
            _applicationDBContext = applicationDBContext;
        }
        public IActionResult LandingPage()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignIn(LoginModel loginModel)
        {
            try
            {

                TempData["Message"] = "";

                if (loginModel == null || string.IsNullOrEmpty(loginModel.Username) || string.IsNullOrEmpty(loginModel.Password))
                {
                    TempData["Message"] = "Please provide both username and password.";
                    return View();
                }
                var LoginData=_applicationDBContext.StudentProfile.FirstOrDefault(a => a.Email==loginModel.Username.ToString() &&  a.Password==loginModel.Password.ToString());
                if (LoginData!=null)
                {
                    var GroupData = _applicationDBContext.Group.ToList();

                    var ProfileData = (from StudToStud in _applicationDBContext.StudentToStudent
                                       join
                                     StudProf in _applicationDBContext.StudentProfile on StudToStud.StudentID2 equals StudProf.StudentID
                                       where StudToStud.StudentID1 == LoginData.StudentID
                                       select new StudentProfileTable
                                       {
                                           StudentID = StudProf.StudentID,
                                           Name = StudProf.Name,
                                           Email = StudProf.Email,                                          
                                           ProfilePic=StudProf.ProfilePic,
                                           Institution=StudProf.Institution,
                                           FieldOfStudy=StudProf.FieldOfStudy,
                                           ResearchTopic=StudProf.ResearchTopic,
                                           Programstartyear=StudProf.Programstartyear,
                                           Linkdinprofile=StudProf.Linkdinprofile,
                                           Password=StudProf.Password,
                                           Verified=StudProf.Verified,
                                           CreatedOn=StudProf.CreatedOn,
                                           ModifiedOn=StudProf.ModifiedOn,
                                           Enabled=StudProf.Enabled
                                       }).ToList();
                    TempData["FriendStudProfile"]=JsonConvert.SerializeObject(ProfileData);


                    var UserData = _applicationDBContext.StudentProfile
                                   .Where(a => a.Email == loginModel.Username.ToString() && a.Password == loginModel.Password.ToString()).ToList();
                    TempData["StudentStudProfile"]=JsonConvert.SerializeObject(UserData);



                    TempData["GroupData"] = JsonConvert.SerializeObject(GroupData);

                    return RedirectToAction("ChatingPage", "MessageChat");
                }
                else
                {
                    TempData["Message"] = "Invalid user name and password";
                    return View();
                }


            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message.ToString();
                return View();
            }
        }


        [HttpPost]
        public  IActionResult SignUp(StudentProfile studentProfile)
        {
            string Filename="";
            TempData["PopupMessage"] = "";
            try
            {
               var StudProf= _applicationDBContext.StudentProfile.FirstOrDefault(a => a.Email==studentProfile.Email && a.Enabled==1);
               if(StudProf==null)
                {
                    if (studentProfile.ProfilePic!=null && studentProfile.ProfilePic.Length > 0)
                    {
                        var UniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(studentProfile.ProfilePic.FileName);

                        var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", UniqueFileName);
                        Filename = filepath;
                        var uploadDirectory= Path.GetDirectoryName(filepath);
                        if (!Directory.Exists(uploadDirectory))
                        {
                            Directory.CreateDirectory(uploadDirectory);

                        }
                        using(var file= new FileStream(filepath, FileMode.Create))
                        {
                            studentProfile.ProfilePic.CopyToAsync(file);
                        }
                    }

                    StudentProfileTable studentProfileTable = new StudentProfileTable()
                    {
                        Name = studentProfile.Name,
                        Email = studentProfile.Email,
                        ProfilePic = Filename,
                        Institution = studentProfile.Institution,
                        FieldOfStudy = studentProfile.FieldOfStudy,
                        ResearchTopic = studentProfile.ResearchTopic,
                        Programstartyear = studentProfile.Programstartyear,
                        Linkdinprofile = studentProfile.Linkdinprofile,
                        Password = studentProfile.Password1,
                        Verified = 0,
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                        Enabled = 1
                    };
                    _applicationDBContext.StudentProfile.Add(studentProfileTable);
                    _applicationDBContext.SaveChanges();
                    var NewCreatedOne= _applicationDBContext.StudentProfile.OrderByDescending(a =>a.StudentID).FirstOrDefault();
                    var OTPNew = new Random().Next(100000,999999).ToString("D6");
                    var OTPValidateObj = new OTPValidationRequest
                    {
                        StuID = NewCreatedOne.StudentID,
                        Email = NewCreatedOne.Email,
                        Otp = OTPNew,
                    };
                    var query = "INSERT INTO OTPValidation (StuID,Email, Otp) VALUES (@StudID,@Email, @OTP)";
                    _applicationDBContext.Database.ExecuteSqlRawAsync(query,
                        new SqlParameter("@StudID", OTPValidateObj.StuID),
                        new SqlParameter("@Email", OTPValidateObj.Email),
                        new SqlParameter("@OTP", OTPValidateObj.Otp));
                     _applicationDBContext.SaveChangesAsync();
                    _emailVerification.SendEmail("prakashbharathi1306@gmail.com", "OTP", "<h1>" + OTPValidateObj.Otp + "</h1>");
                    HttpContext.Session.SetString("StudProfile", JsonConvert.SerializeObject(NewCreatedOne));
                    TempData["PopupMessage"] = "Created Successfully!";
                }
                else
                {
                    _applicationDBContext.Database.ExecuteSqlRawAsync("DELETE FROM OTPValidation WHERE Email = {0} AND StuID = {1}", StudProf.Email, StudProf.StudentID);
                    _applicationDBContext.SaveChangesAsync();

                    string OTPExist = new Random().Next(100000, 999999).ToString("D6");

                    var OTPValidateObj1 = new OTPValidationRequest
                    {
                        StuID = StudProf.StudentID,
                        Email = StudProf.Email,
                        Otp = OTPExist,
                    };
                    var query = "INSERT INTO OTPValidation (StuID,Email, Otp) VALUES (@StudID,@Email, @OTP)";
                     _applicationDBContext.Database.ExecuteSqlRawAsync(query,
                        new SqlParameter("@StudID", OTPValidateObj1.StuID),
                        new SqlParameter("@Email", OTPValidateObj1.Email),
                        new SqlParameter("@OTP", OTPValidateObj1.Otp));
                     _applicationDBContext.SaveChangesAsync();
                    HttpContext.Session.SetString("StudProfile", JsonConvert.SerializeObject(StudProf));
                    _emailVerification.SendEmail("prakashbharathi1306@gmail.com", "Account is Already Created,OTP", "<h1>" + OTPValidateObj1.Otp + "</h1>");
                    TempData["PopupMessage"] = "Account Already Exist for this Email ID,Kinldy check your mail ID";
                }

                

            }
            catch(Exception Ex)
            {
                
                    TempData["PopupMessage"] = "Error" + Ex.Message.ToString();
                

            }
            return View();

        }



        [HttpGet]
        public IActionResult SignUp()
        {
            
            return View();
        }

        [HttpGet]
        public IActionResult OTPScreen()
        {
            return View();
        }
        [HttpPost]
        public IActionResult OTPScreen([FromForm] string otp)
        {
            TempData["OTPMessage"] = "";
            
            if (!string.IsNullOrEmpty(otp))
            {
                StudentProfileTable Stu = JsonConvert.DeserializeObject<StudentProfileTable>(HttpContext.Session.GetString("StudProfile"));
                OTPValidationRequest OTPCheck = new OTPValidationRequest()
                {
                    Email = Stu.Email,
                    StuID = Stu.StudentID,
                    Otp = otp
                };
                var OTPCheckValidation = _applicationDBContext.OTPValidation.FirstOrDefault(e => e.Email == OTPCheck.Email &&
                e.StuID == OTPCheck.StuID && e.Otp == OTPCheck.Otp);
                if (OTPCheckValidation != null)
                {

                    var query = "UPDATE StudentProfile SET Verified=1 WHERE Email=@Email AND StudentID=@StudID";
                    _applicationDBContext.Database.ExecuteSqlRawAsync(query,
                        new SqlParameter("@Email", OTPCheckValidation.Email),
                        new SqlParameter("@StudID", OTPCheckValidation.StuID));
                     _applicationDBContext.SaveChangesAsync();
                    return RedirectToAction("SignIn", "Home");
                }
                else
                {
                    TempData["OTPMessage"] = "Invalide OTP";
                    return View();

                }
            }
            else
            {
                TempData["OTPMessage"] = "Enter the  OTP";
                return View();
            }

        }

        public IActionResult ForgotPassWord()
        {
            return View();
        }
         
        public IActionResult ForgotPassWord([FromForm] string EmailID)
        {
            TempData["ForgotPassWordMessage"] = "";
            if (!string.IsNullOrEmpty(EmailID))
            {
                var EmailDetails = _applicationDBContext.StudentProfile.FirstOrDefault(a=> a.Email==EmailID && a.Verified==1 && a.Enabled==1);
                if (EmailDetails != null)
                {
                    _emailVerification.SendEmail("prakashbharathi1306@gmail.com", "OTP", "<h1>" + EmailDetails.Password + "</h1>");
                    return RedirectToAction("SignIn", "Home");
                }
                else
                {

                    TempData["ForgotPassWorEdMessage"] = "Enter the valid EmailID";
                    return View();
                }

            }
            else
            {
                TempData["ForgotPassWordMessage"] ="Email ID Cannot be Empty";
                return View();
            }
           
        }
    }
}
