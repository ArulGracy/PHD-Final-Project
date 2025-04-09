using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PHDFinalVersion.Models
{
    public class LoginModel
    {
        public string? Username { get; set; } = string.Empty;

        public string? Password { get; set; } = string.Empty;
    }
    public class StudentProfile
    {


        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public IFormFile ProfilePic { get; set; }

        [Required]
        public string Institution { get; set; } = string.Empty;

        [Required]
        public string FieldOfStudy { get; set; } = string.Empty;

        [Required]
        public string ResearchTopic { get; set; } = string.Empty;

        [Required]
        public string Programstartyear { get; set; } = string.Empty;

        [Required]
        public string Linkdinprofile { get; set; } = string.Empty;


        [Required]
        public string Password1 { get; set; } = string.Empty;

        [Required]
        public string Password2 { get; set; } = string.Empty;


    }

    public class StudentProfileTable
    {


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StudentID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string ProfilePic { get; set; } = string.Empty;

        [Required]
        public string Institution { get; set; } = string.Empty;

        [Required]
        public string FieldOfStudy { get; set; } = string.Empty;

        [Required]
        public string ResearchTopic { get; set; } = string.Empty;

        [Required]
        public string Programstartyear { get; set; } = string.Empty;

        [Required]
        public string Linkdinprofile { get; set; } = string.Empty;


        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public int Verified { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        [Required]
        public DateTime? ModifiedOn { get; set; } = null;

        [Required]
        public int Enabled { get; set; } = 1;


        public virtual ICollection<StudentsGroup> StudentStudentsGroups { get; set; }

        public virtual ICollection<Message> Message1 { get; set; }

        public virtual ICollection<Message> Message2 { get; set; }


        public virtual ICollection<StudentToStudent> StudentToStudent1 { get; set; }

        public virtual ICollection<StudentToStudent> StudentToStudent2 { get; set; }

    }

    public class OTPValidationRequest
    {
        public int StuID { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
    }


    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string MessageType { get; set; }
        public string MessageText { get; set; }
        public string FileType { get; set; }
        public string FilePath { get; set; }
        public DateTime Timestamp { get; set; }
        public int? GroupId { get; set; }

        public virtual Group Groups { get; set; }

        public virtual StudentProfileTable Receiver { get; set; }

        public virtual StudentProfileTable Sender { get; set; }

    }

    public class Group
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GroupId { get; set; }
        public string GroupName { get; set; }

        public virtual ICollection<StudentsGroup> GroupStudentsGroups { get; set; }

        public virtual ICollection<Message> MessageGroups { get; set; }




    }

    public class StudentsGroup
    {
        public int GroupId { get; set; }

        public int StudentId { get; set; }


        public virtual StudentProfileTable StudentGroupProfile { get; set; }

        public virtual Group StudentGroupGroup { get; set; }

    }

    public class StudentToStudent
    {
        public int StudentID1 { get; set; }

        public int StudentID2 { get; set; }


        public StudentProfileTable Student1 { get; set; }

        public StudentProfileTable Student2 { get; set; }
    }

    public class AllModelData
    {
        public List<Group> GroupData { get; set; }

        public List<StudentProfileTable> UserStudentProfileData { get; set; }

        public List<StudentProfileTable> FriendStudentProfileData { get; set; }
    }

    public class MessageRequest
    {
        public int StudentID { get; set; }
        public int UserID { get; set; }

    }

    public class SendMessages
    {
        public int SenderId { get; set; }

        public int ReceiverId { get; set; }

        public string MessageText { get; set; } = string.Empty;

        public IFormFile? File { get; set; }
    }


    public class SenderReceiverMessage
    {
        public int SenderId { get; set; }

        public int ReceiverId { get; set; }
        
        public string MessageType { get; set; }

        public string MessageText { get; set; }

        public string FileType { get; set; }

        public string FilePath { get; set; }

        public string Flag { get; set; }
    }
}