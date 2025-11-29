using System.Linq;
using System.Collections.Generic;

namespace cursi.DAL
{
    public class HallRepository
    {
        private readonly AppDbContext _db;
        public HallRepository(AppDbContext db) => _db = db;

        public void Add(Hall hall)
        {
            _db.Halls.Add(hall);
            _db.SaveChanges();
        }

        public Hall GetById(int id) => _db.Halls.FirstOrDefault(h => h.Id == id);

        public List<Hall> GetAll() => _db.Halls.ToList();
    }
}
