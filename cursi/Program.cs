using Dapper;
using System.Data.SqlClient;
using cursi.DAL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace cursi
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var db = new AppDbContext();
            db.Database.EnsureCreated();

            Console.WriteLine("===== КІНОТЕАТР =====");
            MainMenu(db);
        }

        static void MainMenu(AppDbContext db)
        {
            while (true)
            {
                Console.WriteLine("\n1. Додати фільм");
                Console.WriteLine("2. Додати зал");
                Console.WriteLine("3. Додати сеанс");
                Console.WriteLine("4. Додати вільні місця");
                Console.WriteLine("5. Показати фільми/зали/сеанси/вільні місця");
                Console.WriteLine("6. Вийти");
                Console.WriteLine("7. Favorite Movies"); 

                Console.Write("\nВведіть номер: ");
                var cmd = Console.ReadLine();

                switch (cmd)
                {
                    case "1": AddMovie(db); break;
                    case "2": AddHall(db); break;
                    case "3": AddSession(db); break;
                    case "4": AddSeats(db); break;
                    case "5": ShowAll(db); break;
                    case "6": return;
                    case "7": Topfilms(); break; 
                    default: Console.WriteLine("Невірний вибір!"); break;
                }
            }
        }

        static void Topfilms()
        {
            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=cinema;Integrated Security=True;Connect Timeout=30;"; 

            using var connection = new SqlConnection(connectionString);
            connection.Open();

            while (true)
            {
                Console.WriteLine("\n--- FAVORITE MOVIES ---");
                Console.WriteLine("1 — Add 3 movies");
                Console.WriteLine("2 — Show all movies");
                Console.WriteLine("3 — Delete all movies");
                Console.WriteLine("4 — Back");

                Console.Write("Choose: ");
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    var movies = new[]
                    {
                        new Topfilms { Title = "Веном" },
                        new Topfilms { Title = "Человек-паук" },
                        new Topfilms { Title = "Дедпул 3" }
                    };

                    foreach (var m in movies)
                        connection.Execute("INSERT INTO Topfilms (Title) VALUES (@Title)", m);

                    Console.WriteLine("3 movies added!");
                }
                else if (choice == "2")
                {
                    var allMovies = connection.Query<Topfilms>("SELECT * FROM Topfilms").ToList();
                    Console.WriteLine("\n--- MOVIES ---");
                    foreach (var m in allMovies)
                        Console.WriteLine($"{m.Id}. {m.Title}");
                }
                else if (choice == "3")
                {
                    connection.Execute("DELETE FROM Topfilms");
                    Console.WriteLine("All movies deleted");
                }
                else if (choice == "4")
                    return;
                else
                    Console.WriteLine("Invalid choice");
            }
        }

        static void AddMovie(AppDbContext db)
        {
            Console.Write("Назва фільму: ");
            var title = Console.ReadLine();

            Console.Write("Тривалість (хв): ");
            var dur = int.Parse(Console.ReadLine());

            Console.Write("Формат (2D/3D): ");
            var format = Console.ReadLine();

            Console.Write("Ціна квитка: ");
            var price = decimal.Parse(Console.ReadLine());

            db.Movies.Add(new Movie { Title = title, DurationMin = dur, Format = format, Price = price });
            db.SaveChanges();
            Console.WriteLine("Фільм додано!");
        }

        static void AddHall(AppDbContext db)
        {
            Console.Write("Назва залу: ");
            var name = Console.ReadLine();

            Console.Write("Кількість місць: ");
            var count = int.Parse(Console.ReadLine());

            db.Halls.Add(new Hall { Name = name, SeatsCount = count });
            db.SaveChanges();
            Console.WriteLine("Зал додано!");
        }

        static void AddSession(AppDbContext db)
        {
            Console.Write("ID фільму: ");
            var movieId = int.Parse(Console.ReadLine());
            var movie = db.Movies.FirstOrDefault(m => m.Id == movieId);
            if (movie == null) { Console.WriteLine("Фільм не знайдено!"); return; }

            Console.Write("ID залу: ");
            var hallId = int.Parse(Console.ReadLine());
            var hall = db.Halls.FirstOrDefault(h => h.Id == hallId);
            if (hall == null) { Console.WriteLine("Зал не знайдено!"); return; }

            Console.Write("Дата та час сеансу (yyyy-MM-dd HH:mm): ");
            var dt = DateTime.Parse(Console.ReadLine());

            db.Sessions.Add(new Session { MovieId = movieId, HallId = hallId, StartTime = dt });
            db.SaveChanges();
            Console.WriteLine("Сеанс додано!");
        }

        static void AddSeats(AppDbContext db)
        {
            Console.Write("ID сеансу: ");
            var sessionId = int.Parse(Console.ReadLine());
            var session = db.Sessions.FirstOrDefault(s => s.Id == sessionId);
            if (session == null) { Console.WriteLine("Сеанс не знайдено!"); return; }

            Console.Write("Кількість місць для додавання: ");
            var count = int.Parse(Console.ReadLine());

            for (int i = 1; i <= count; i++)
            {
                db.Seats.Add(new Seat { SessionId = sessionId, SeatNumber = i, IsFree = true });
            }

            db.SaveChanges();
            Console.WriteLine("Місця додано!");
        }

        static void ShowAll(AppDbContext db)
        {
            Console.WriteLine("\n=== ФІЛЬМИ ===");
            foreach (var m in db.Movies.ToList())
                Console.WriteLine($"{m.Id}. {m.Title} ({m.Format}) — {m.Price} грн");

            Console.WriteLine("\n=== ЗАЛИ ===");
            foreach (var h in db.Halls.ToList())
                Console.WriteLine($"{h.Id}. {h.Name} — {h.SeatsCount} місць");

            Console.WriteLine("\n=== СЕАНСИ ===");
            foreach (var s in db.Sessions.Include(s => s.Movie).Include(s => s.Hall))
            {
                Console.WriteLine($"{s.Id}. {s.Movie?.Title} — {s.Hall?.Name} — {s.StartTime}");
            }

            Console.WriteLine("\n=== ВІЛЬНІ МІСЦЯ ===");
            foreach (var seat in db.Seats.Where(x => x.IsFree).ToList())
                Console.WriteLine($"Сеанс {seat.SessionId}, місце {seat.SeatNumber}");
        }
    }
}
