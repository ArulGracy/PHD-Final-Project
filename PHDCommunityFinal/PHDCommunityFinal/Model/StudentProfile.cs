using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PHDCommunityFinal.Model
{
    public class StudentProfile
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
        public int Verified { get; set; } =0;

        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        [Required]
        public DateTime? ModifiedOn { get; set; } = DateTime.Now;

        [Required]
        public int Enabled { get; set; } = 1;
    }
}

