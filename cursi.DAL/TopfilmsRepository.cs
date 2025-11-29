using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;

namespace cursi.DAL
{
    public class TopfilmsRepository
    {
        private readonly string _connStr =
            "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=cinema;Integrated Security=True;";
        public void Add(Topfilms movie)
        {
            string sql = "INSERT INTO Topfilms (Title) VALUES (@Title)";
            using var db = new SqlConnection(_connStr);
            db.Execute(sql, movie);
        }

        public void AddMany(IEnumerable<Topfilms> movies)
        {
            string sql = "INSERT INTO Topfilms (Title) VALUES (@Title)";
            using var db = new SqlConnection(_connStr);
            db.Execute(sql, movies);
        }

        public IEnumerable<Topfilms> GetAll()
        {
            using var db = new SqlConnection(_connStr);
            return db.Query<Topfilms>("SELECT * FROM Topfilms").ToList();
        }

        public void DeleteAll()
        {
            using var db = new SqlConnection(_connStr);
            db.Execute("DELETE FROM Topfilms");
            db.Execute("DBCC CHECKIDENT ('Topfilms', RESEED, 0)");
        }
    }
}