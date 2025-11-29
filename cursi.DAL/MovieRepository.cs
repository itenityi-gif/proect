using System.Linq;
using System.Collections.Generic;

namespace cursi.DAL
{
    public class MovieRepository
    {
        private readonly AppDbContext _db;
        public MovieRepository(AppDbContext db) => _db = db;

        public void Add(Movie movie)
        {
            _db.Movies.Add(movie);
            _db.SaveChanges();
        }

        public Movie GetById(int id) => _db.Movies.FirstOrDefault(m => m.Id == id);

        public List<Movie> GetAll() => _db.Movies.ToList();
    }
}
