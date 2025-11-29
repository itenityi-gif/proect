using Microsoft.EntityFrameworkCore;

namespace cursi.DAL
{
    public class AppDbContext : DbContext
    {
        private string _connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=cinema;Integrated Security=True;Connect Timeout=30;";

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
