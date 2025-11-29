using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;

namespace cursi.DAL
{
    public class SessionRepository
    {
        private readonly AppDbContext _db;
        public SessionRepository(AppDbContext db) => _db = db;

        public void Add(Session session)
        {
            _db.Sessions.Add(session);
            _db.SaveChanges();
        }

        public Session GetById(int id) => _db.Sessions
            .Include(s => s.Movie)
            .Include(s => s.Hall)
            .FirstOrDefault(s => s.Id == id);

        public List<Session> GetAll() => _db.Sessions
            .Include(s => s.Movie)
            .Include(s => s.Hall)
            .ToList();
    }
}
