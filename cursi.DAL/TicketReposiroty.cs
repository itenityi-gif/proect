using System.Linq;
using System.Collections.Generic;

namespace cursi.DAL
{
    public class TicketRepository
    {
        private readonly AppDbContext _db;
        public TicketRepository(AppDbContext db) => _db = db;

        public void Add(Ticket ticket)
        {
            _db.Tickets.Add(ticket);
            _db.SaveChanges();
        }

        public Ticket GetById(int id) => _db.Tickets.FirstOrDefault(t => t.Id == id);

        public List<Ticket> GetAll() => _db.Tickets.ToList();
    }
}
