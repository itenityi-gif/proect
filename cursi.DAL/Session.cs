using System.Collections.Generic;

namespace cursi.DAL
{
    public class Session
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int HallId { get; set; }
        public DateTime StartTime { get; set; }

        
        public Movie Movie { get; set; }
        public Hall Hall { get; set; }
        public List<Seat> Seats { get; set; } = new List<Seat>();
    }
}
