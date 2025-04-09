using Microsoft.AspNetCore.SignalR;
using PHDFinalVersion.AppDBContext;
using PHDFinalVersion.ChatHubb;
using PHDFinalVersion.Email_Verification;
using PHDFinalVersion.Models;

namespace PHDFinalVersion.ChatHub
{
    public class SendAndReceiveMessage
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IEmailVerification _emailVerification;
        private readonly ApplicationDBContext _applicationDBContext;
       

        public SendAndReceiveMessage(IEmailVerification emailVerification, ApplicationDBContext applicationDBContext)
        {
            _emailVerification = emailVerification;
            _applicationDBContext = applicationDBContext;
        }

        public SenderReceiverMessage SendMessage1(SendMessages formData)
        {
            string Filename = "";
            string FilePath = "";
            string FileType = "";
            if (formData.File != null)
            {

                var FileExtension = Path.GetExtension(formData.File.FileName).ToLower();

                var FileNamePDF = Guid.NewGuid().ToString() + Path.GetExtension(formData.File.FileName);

                Filename = FileNamePDF;
                if (FileExtension == ".pdf")
                {
                    FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files", FileNamePDF);
                    FileType = "D";
                }

                if (FileExtension == ".mp4")
                {
                    FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Vedio", FileNamePDF);
                    FileType = "V";
                }

                if (FileExtension == ".jpg" || FileExtension == ".jpeg" || FileExtension == "png" || FileExtension == ".gif")
                {
                    FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", FileNamePDF);
                    FileType = "I";
                }

                var DirectoryPDF = Path.GetDirectoryName(FilePath);

                if (!Directory.Exists(DirectoryPDF))
                {
                    Directory.CreateDirectory(DirectoryPDF);

                }
                using (var FilePDF = new FileStream(FilePath, FileMode.Create))
                {
                    formData.File.CopyToAsync(FilePDF);
                }
            }

            string messageType = "";

            if (!string.IsNullOrEmpty(formData.MessageText) && (formData.File == null || formData.File.Length == 0))
            {
                messageType = "T";
            }
            else if (string.IsNullOrEmpty(formData.MessageText) && formData.File != null && formData.File.Length > 0)
            {
                messageType = "F";
            }
            else if (!string.IsNullOrEmpty(formData.MessageText) && formData.File != null && formData.File.Length > 0)
            {
                messageType = "B";
            }



            var MessageDataDB = new Message()
            {
                SenderId = formData.SenderId,
                ReceiverId = formData.ReceiverId,
                MessageType = messageType,
                MessageText = formData.MessageText,
                FileType = FileType,
                FilePath = FilePath,
                Timestamp = DateTime.Now,
                GroupId = 5,



            };
            _applicationDBContext.Message.Add(MessageDataDB);
            _applicationDBContext.SaveChanges();

            var MessageData = _applicationDBContext.Message.Where(a => a.SenderId == formData.SenderId && a.ReceiverId == formData.ReceiverId).
                              OrderByDescending(a => a.MessageId).FirstOrDefault();

            var response = new SenderReceiverMessage()
            {
                SenderId = MessageData.SenderId,
                ReceiverId = MessageData.ReceiverId,
                MessageType = MessageData.MessageType,
                MessageText = MessageData.MessageText,
                FileType = MessageData.FileType,
                FilePath = MessageData.FilePath,
                Flag = "Sender"

            };

            var ResponseReceive = new SenderReceiverMessage()
            {
                SenderId = MessageData.SenderId,
                ReceiverId = MessageData.ReceiverId,
                MessageType = MessageData.MessageType,
                MessageText = MessageData.MessageText,
                FileType = MessageData.FileType,
                FilePath = MessageData.FilePath,
                Flag = "Receiver"

            };






            return ResponseReceive;


        }
    }
}
