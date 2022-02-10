using System.Collections.Generic;

namespace LibraryApiApplication.Model
{
    public class Member:ModelBase
    {
        
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Occupation { get; set; }
        public string Email { get; set; }
        
        public virtual ICollection<BookLog> BookLog { get; set; }
    }
}