using Dapper;
using System.Data.SqlClient;
using cursi.DAL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Diagnostics;

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
                Console.WriteLine("5. Показати все");
                Console.WriteLine("6. Вийти");
                Console.WriteLine("7. Favorite Movies");
                Console.WriteLine("8. Bagatopotokovist");
                Console.WriteLine("9. Noviy proses");

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
                    case "8": MultiThreadReport(); break;
                    case "9": RunNewProcess(); break;
                    default: Console.WriteLine("Невірний вибір!"); break;
                }
            }
        }

        static void MultiThreadReport()
        {
            Console.WriteLine("\n--- Bagatopotokovist ---");

            Thread t1 = new Thread(() =>
            {
                using var db = new AppDbContext();
                Console.WriteLine($"Movies: {db.Movies.Count()}");
            });

            Thread t2 = new Thread(() =>
            {
                using var db = new AppDbContext();
                Console.WriteLine($"Halls: {db.Halls.Count()}");
            });

            Thread t3 = new Thread(() =>
            {
                using var db = new AppDbContext();
                Console.WriteLine($"Sessions: {db.Sessions.Count()}");
            });

            t1.Start();
            t2.Start();
            t3.Start();

            t1.Join();
            t2.Join();
            t3.Join();
        }

        static void RunNewProcess()
        {
            string exePath = Environment.ProcessPath;

            Process.Start(new ProcessStartInfo
            {
                FileName = exePath,
                UseShellExecute = true
            });

            Console.WriteLine("Program started in new process!");
        }

        static void Topfilms()
        {
            string connectionString =
                "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=cinema;Integrated Security=True;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();

            while (true)
            {
                Console.WriteLine("\n--- FAVORITE MOVIES ---");
                Console.WriteLine("1 — Add 3 movies");
                Console.WriteLine("2 — Show all movies");
                Console.WriteLine("3 — Delete all movies");
                Console.WriteLine("4 — Back");

                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    var movies = new[]
                    {
                        new Topfilms { Title = "Веном" },
                        new Topfilms { Title = "Человек-паук" },
                        new Topfilms { Title = "Дедпул 3" }
                    };

                    connection.Execute(
                        "INSERT INTO Topfilms (Title) VALUES (@Title)", movies);

                    Console.WriteLine("3 movies added!");
                }
                else if (choice == "2")
                {
                    var allMovies =
                        connection.Query<Topfilms>("SELECT * FROM Topfilms").ToList();

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

            db.Movies.Add(new Movie
            {
                Title = title,
                DurationMin = dur,
                Format = format,
                Price = price
            });

            db.SaveChanges();
            Console.WriteLine("Фільм додано!");
        }

        static void AddHall(AppDbContext db)
        {
            Console.Write("Назва залу: ");
            var name = Console.ReadLine();

            Console.Write("Кількість місць: ");
            var count = int.Parse(Console.ReadLine());

            db.Halls.Add(new Hall
            {
                Name = name,
                SeatsCount = count
            });

            db.SaveChanges();
            Console.WriteLine("Зал додано!");
        }

        static void AddSession(AppDbContext db)
        {
            Console.Write("ID фільму: ");
            var movieId = int.Parse(Console.ReadLine());

            var movie = db.Movies.FirstOrDefault(m => m.Id == movieId);
            if (movie == null) return;

            Console.Write("ID залу: ");
            var hallId = int.Parse(Console.ReadLine());

            var hall = db.Halls.FirstOrDefault(h => h.Id == hallId);
            if (hall == null) return;

            Console.Write("Дата та час (yyyy-MM-dd HH:mm): ");
            var dt = DateTime.Parse(Console.ReadLine());

            db.Sessions.Add(new Session
            {
                MovieId = movieId,
                HallId = hallId,
                StartTime = dt
            });

            db.SaveChanges();
            Console.WriteLine("Сеанс додано!");
        }

        static void AddSeats(AppDbContext db)
        {
            Console.Write("ID сеансу: ");
            var sessionId = int.Parse(Console.ReadLine());

            Console.Write("Кількість місць: ");
            var count = int.Parse(Console.ReadLine());

            for (int i = 1; i <= count; i++)
            {
                db.Seats.Add(new Seat
                {
                    SessionId = sessionId,
                    SeatNumber = i,
                    IsFree = true
                });
            }

            db.SaveChanges();
            Console.WriteLine("Місця додано!");
        }

        static void ShowAll(AppDbContext db)
        {
            Console.WriteLine("\n=== ФІЛЬМИ ===");
            foreach (var m in db.Movies)
                Console.WriteLine($"{m.Id}. {m.Title}");

            Console.WriteLine("\n=== ЗАЛИ ===");
            foreach (var h in db.Halls)
                Console.WriteLine($"{h.Id}. {h.Name}");

            Console.WriteLine("\n=== СЕАНСИ ===");
            foreach (var s in db.Sessions.Include(s => s.Movie).Include(s => s.Hall))
                Console.WriteLine($"{s.Id}. {s.Movie.Title} — {s.Hall.Name}");

            Console.WriteLine("\n=== ВІЛЬНІ МІСЦЯ ===");
            foreach (var seat in db.Seats.Where(x => x.IsFree))
                Console.WriteLine($"Сеанс {seat.SessionId}, місце {seat.SeatNumber}");
        }
    }
}
