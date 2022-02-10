using LibraryApiApplication.Model;
using Microsoft.EntityFrameworkCore;

namespace LibraryApiApplication.DbContext
{
    public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
    {public DbSet<BookLog> BookLog { get; set; }
        public DbSet<Book> Book { get; set; }
        public DbSet<Member> Member { get; set; }
        
        public AppDbContext (DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        
    }
}