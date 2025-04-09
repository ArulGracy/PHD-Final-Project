using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using PHDFinalVersion.AppDBContext;
using PHDFinalVersion.ChatHubb;
using PHDFinalVersion.Email_Verification;
using PHDFinalVersion.Models;

namespace PHDFinalVersion.Controllers
{
    public class MessageChatController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IEmailVerification _emailVerification;
        private readonly ApplicationDBContext _applicationDBContext;
       

        public MessageChatController(IEmailVerification emailVerification, ApplicationDBContext applicationDBContext)
        {
            _emailVerification = emailVerification;
            _applicationDBContext = applicationDBContext;
            

        }

        

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ChatingPage()
        {
            var GroupRecord = JsonConvert.DeserializeObject<List<Group>>(TempData["GroupData"].ToString());

            var FriendsProfileData = JsonConvert.DeserializeObject<List<StudentProfileTable>>(TempData["FriendStudProfile"].ToString());

            var UserProfileData = JsonConvert.DeserializeObject<List<StudentProfileTable>>(TempData["StudentStudProfile"].ToString());

            var AllModelData = new AllModelData
            {
                GroupData = GroupRecord,
                FriendStudentProfileData = FriendsProfileData,
                UserStudentProfileData = UserProfileData
            };
            return View(AllModelData);
        }


        [HttpPost]
        public IActionResult GetIndividualMessage([FromBody] MessageRequest request)
        {


            int SenderID = request.StudentID;
            int UserID = request.UserID;

            var IndividualMessage = _applicationDBContext.Message.Where(a => (a.SenderId == UserID && a.ReceiverId == SenderID) || (a.SenderId == SenderID && a.ReceiverId == UserID)).
                                  OrderBy(m => m.Timestamp)
                                  .Select(m => new
                                  {
                                      m.SenderId,
                                      m.ReceiverId,
                                      m.MessageType,
                                      m.MessageText,
                                      m.FileType,
                                      m.FilePath,
                                      Flag = m.ReceiverId == SenderID ? "Sender" : "Receiver"

                                  });
            var response = JsonConvert.SerializeObject(IndividualMessage);

            return Json(response);
        }

        
        //public SenderReceiverMessage SendMessage(SendMessages formData)
        //{
        //    string Filename = "";
        //    string FilePath="";
        //    string FileType = "";
        //    if (formData.File!= null)
        //    {

        //        var  FileExtension=Path.GetExtension(formData.File.FileName).ToLower();
                
        //        var FileNamePDF = Guid.NewGuid().ToString() + Path.GetExtension(formData.File.FileName);

        //        Filename=FileNamePDF;
        //        if (FileExtension == ".pdf")
        //        {
        //              FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files", FileNamePDF);
        //              FileType = "D";
        //        }

        //        if (FileExtension == ".mp4")
        //        {
        //            FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Vedio", FileNamePDF);
        //            FileType = "V";
        //        }

        //        if (FileExtension == ".jpg" || FileExtension == ".jpeg" || FileExtension == "png" || FileExtension == ".gif")
        //        {
        //            FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", FileNamePDF);
        //            FileType = "I";
        //        }

        //        var DirectoryPDF=Path.GetDirectoryName(FilePath);

        //        if (!Directory.Exists(DirectoryPDF))
        //        {
        //            Directory.CreateDirectory(DirectoryPDF);

        //        }
        //        using(var FilePDF= new FileStream(FilePath, FileMode.Create))
        //        {
        //            formData.File.CopyToAsync(FilePDF);
        //        }    
        //    }

        //    string messageType = "";

        //    if (!string.IsNullOrEmpty(formData.MessageText) && (formData.File == null || formData.File.Length == 0))
        //    {
        //        messageType = "T";
        //    }
        //    else if (string.IsNullOrEmpty(formData.MessageText) && formData.File != null && formData.File.Length > 0)
        //    {
        //        messageType = "F";
        //    }
        //    else if (!string.IsNullOrEmpty(formData.MessageText) && formData.File != null && formData.File.Length > 0)
        //    {
        //        messageType = "B";
        //    }



        //    var MessageDataDB = new Message()
        //    {
        //        SenderId = formData.SenderId,
        //        ReceiverId = formData.ReceiverId,
        //        MessageType = messageType,
        //        MessageText = formData.MessageText,
        //        FileType = FileType,
        //        FilePath = FilePath,
        //        Timestamp = DateTime.Now,
        //        GroupId = 5,



        //    };
        //    _applicationDBContext.Message.Add(MessageDataDB);
        //    _applicationDBContext.SaveChanges();

        //    var MessageData= _applicationDBContext.Message.Where(a=> a.SenderId==formData.SenderId && a.ReceiverId==formData.ReceiverId).
        //                      OrderByDescending(a => a.MessageId).FirstOrDefault();

        //    var response = new SenderReceiverMessage()
        //    {
        //        SenderId = MessageData.SenderId,
        //        ReceiverId = MessageData.ReceiverId,
        //        MessageType = MessageData.MessageType,
        //        MessageText = MessageData.MessageText,
        //        FileType = MessageData.FileType,
        //        FilePath = MessageData.FilePath,
        //        Flag =  "Sender" 

        //    };

        //    var ResponseReceive = new SenderReceiverMessage()
        //    {
        //        SenderId = MessageData.SenderId,
        //        ReceiverId = MessageData.ReceiverId,
        //        MessageType = MessageData.MessageType,
        //        MessageText = MessageData.MessageText,
        //        FileType = MessageData.FileType,
        //        FilePath = MessageData.FilePath,
        //        Flag = "Receiver"

        //    };



            


        //    return ResponseReceive;


        //}
    }
}
