using Microsoft.EntityFrameworkCore;
using PHDCommunityFinal.Model;

namespace PHDCommunityFinal.ApplicationDBContext1
{
    public class ApplicationDBContext:DbContext
    {
        public ApplicationDBContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<StudentProfile> StudentProfileFinal { get; set; }
    }
}
