namespace cursi.DAL
{
    public class Seat
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public int SeatNumber { get; set; }
        public bool IsFree { get; set; }
    }
}
