using System;

namespace LibraryApiApplication.Model
{
    public class BookLog : ModelBase
    {
        public DateTime StartTime{ get; set; }
        public DateTime EndTime{ get; set; }
        public int BookId { get; set; }
        public virtual Book Book { get; set; }
        public int MemberId { get; set; }
        public virtual  Member Member { get; set; }
        
        public string Status { get; set; }
        
    }
}