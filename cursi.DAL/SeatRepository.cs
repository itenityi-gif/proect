using System.Linq;
using System.Collections.Generic;

namespace cursi.DAL
{
    public class SeatRepository
    {
        private readonly AppDbContext _db;
        public SeatRepository(AppDbContext db) => _db = db;

        public void Add(Seat seat)
        {
            _db.Seats.Add(seat);
            _db.SaveChanges();
        }

        public Seat GetById(int id) => _db.Seats.FirstOrDefault(s => s.Id == id);

        public List<Seat> GetAll() => _db.Seats.ToList();

        public List<Seat> GetFreeSeatsBySession(int sessionId) =>
            _db.Seats.Where(s => s.SessionId == sessionId && s.IsFree).ToList();
    }
}
