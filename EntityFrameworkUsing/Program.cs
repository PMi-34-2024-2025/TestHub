using Microsoft.EntityFrameworkCore;
using Npgsql;
using DotNetEnv;

namespace PE.DesktopApplication.TestHub.DAL
{

    public class TestingSystemContext : DbContext
    {
        private static string dbHost;
        private static string dbUser;
        private static string dbPass;
        private static string dbPort;
        private static string dbName;

        static TestingSystemContext()
        {
            // Load the .env file
            DotNetEnv.Env.Load();

            // Read the environment variables
            dbHost = Environment.GetEnvironmentVariable("DB_HOST");
            dbUser = Environment.GetEnvironmentVariable("DB_USER");
            dbPass = Environment.GetEnvironmentVariable("DB_PASS");
            dbPort = Environment.GetEnvironmentVariable("DB_PORT");
            dbName = Environment.GetEnvironmentVariable("DB_NAME");
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Attempt> Attempts { get; set; }
        public DbSet<TestStatus> TestStatuses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Configure the PostgreSQL database connection
            Console.WriteLine($"Server={dbHost};Port={dbPort};Database={dbName};UserId={dbUser};Password={dbPass}");
            optionsBuilder.UseNpgsql($"Server={dbHost};Port={dbPort};Database={dbName};UserId={dbUser};Password={dbPass}");
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            /// Створення таблиць за допомогою ADO.NET
            ////string connectionString = "Host=localhost;Username=postgres;Password=130505;Database=TestHubV2"; // Заміни на свої дані
            ////string connectionString = "Server=localhost//PostgreSQL 16;Database=TestHubV2;Trusted_Connection=true";
            ///
            //string dbHost = Environment.GetEnvironmentVariable("DB_HOST");
            //string dbUser= Environment.GetEnvironmentVariable("DB_USER");
            //string dbPass = Environment.GetEnvironmentVariable("DB_PASS");
            //string dbPort = Environment.GetEnvironmentVariable("DB_PORT");
            //string dbName = Environment.GetEnvironmentVariable("DB_NAME");

            //string connectionString = $"Host={dbHost};Port=4321;Database=TestHubV2;User Id={dbUser};Password=130505;";
            //string connectionString = "Host=testhubv2.cdccsc0ye5z9.eu-north-1.rds.amazonaws.com;Port=5432;Database=TestHubV2;User Id=postgres;Password=QeiW7PkY1bOqp3C;";
            //using (var connection = new NpgsqlConnection(connectionString))
            //{
            //    connection.Open();
            //    CreateTables(connection);
            //}


            /// Додавання тестових даних до таблиць використовуючи entity framework
            //using (var context = new TestingSystemContext())
            //{
            //    SeedData(context); // Додавання тестових даних
            //}
            //Console.WriteLine("Successfull!");
        }

        public static void SeedData(TestingSystemContext context)
        {
            var random = new Random();

            // Генерація користувачів
            var users = Enumerable.Range(1, 50).Select(i => new User
            {
                name = $"User {i}",
                login = $"user{i}@example.com",
                password_hash = "hashedpassword",
                role = i % 4 == 0 ? "admin" : (i % 3 == 0 ? "tutor" : (i % 2 == 0 ? "student" : "supervisor"))
            }).ToList();

            context.Users.AddRange(users);
            context.SaveChanges();

            // Генерація тестів
            var tutors = users.Where(u => u.role == "tutor").ToList();
            var tests = Enumerable.Range(1, 20).Select(i => new Test
            {
                title = $"Test {i}",
                tutor_id = tutors[random.Next(tutors.Count)].user_id,
                status = "pending",
                area = $"Area {i}",
                mins_to_pass = random.Next(15, 120)
            }).ToList();

            context.Tests.AddRange(tests);
            context.SaveChanges();

            var questions = tests.SelectMany(test => Enumerable.Range(1, random.Next(5, 10)).Select(i => new Question
            {
                test_id = test.test_id,
                question_text = $"Question {i} for Test {test.test_id}"
            })).ToList();

            context.Questions.AddRange(questions);
            context.SaveChanges();

            var answers = questions.SelectMany(question => Enumerable.Range(1, random.Next(2, 5)).Select(i => new Answer
            {
                question_id = question.question_id,
                answer_text = $"Answer {i} for Question {question.question_id}",
                is_correct = random.Next(0, 2) == 0
            })).ToList();

            context.Answers.AddRange(answers);
            context.SaveChanges();

            var students = users.Where(u => u.role == "student").ToList();
            var attempts = students.SelectMany(student => tests.Take(random.Next(3, 10)).Select(test => new Attempt
            {
                test_id = test.test_id,
                user_id = student.user_id,
                attempt_number = random.Next(1, 5),
                start_time = DateTime.UtcNow.AddMinutes(-random.Next(30, 120)),
                end_time = DateTime.UtcNow,
                score = random.Next(50, 100),
                user_report = "Example user report"
            })).ToList();

            context.Attempts.AddRange(attempts);
            context.SaveChanges();

            // Генерація статусів тестів
            var admins = users.Where(u => u.role == "admin").ToList();
            var testStatuses = tests.Select(test => new TestStatus
            {
                test_id = test.test_id,
                admin_id = admins[random.Next(admins.Count)].user_id,
                status = "pending",
                date_changed = DateTime.UtcNow
            }).ToList();

            context.TestStatuses.AddRange(testStatuses);
            context.SaveChanges();
            //var random = new Random();

            //// Додаємо користувачів
            //for (int i = 1; i <= 50; i++)
            //{
            //    context.Users.Add(new User
            //    {
            //        name = $"Student {i}",
            //        login = $"student{i}@example.com",
            //        password_hash = "hashedpassword",
            //        role = i % 3 == 0 ? "admin" : i % 2 == 0 ? "tutor" : "student"
            //    });
            //}
            //context.SaveChanges();

            //var tutors = context.Users.Where(u => u.role == "tutor").ToList();
            //var students = context.Users.Where(u => u.role == "student").ToList();
            //var admins = context.Users.Where(u => u.role == "admin").ToList();

            ////Додаємо тести
            //for (int i = 1; i <= 50; i++)
            //{
            //    var tutor = tutors[random.Next(tutors.Count)];
            //    context.Tests.Add(new Test
            //    {
            //        title = $"Test {i}",
            //        tutor_id = tutor.user_id,
            //        status = "pending",
            //        area = $"Area{i}",
            //        mins_to_pass = i * 50
            //    });
            //}
            //context.SaveChanges();

            //var tests = context.Tests.ToList();

            //// Додаємо питання та відповіді
            //foreach (var test in tests)
            //{
            //    for (int j = 1; j <= 5; j++)
            //    {
            //        var question = new Question
            //        {
            //            test_id = test.test_id,
            //            question_text = $"Question {j} for {test.title}"
            //        };
            //        context.Questions.Add(question);
            //        context.SaveChanges();

            //        for (int k = 1; k <= 4; k++)
            //        {
            //            context.Answers.Add(new Answer
            //            {
            //                question_id = question.question_id,
            //                answer_text = $"Answer {k} for {question.question_text}",
            //                is_correct = k == 1
            //            });
            //        }
            //        context.SaveChanges();
            //    }
            //}

            ////Додаємо спроби тестів для студентів
            //foreach (var student in students)
            //{
            //    foreach (var test in tests)
            //    {
            //        context.Attempts.Add(new Attempt
            //        {
            //            test_id = test.test_id,
            //            user_id = student.user_id,
            //            attempt_number = random.Next(1, 5),
            //            start_time = DateTime.SpecifyKind(DateTime.Now.AddMinutes(-random.Next(30, 120)), DateTimeKind.Utc),
            //            end_time = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
            //            score = random.Next(50, 100)
            //        });
            //    }
            //}
            //context.SaveChanges();

            //// Додаємо результати тестів
            //var attempts = context.Attempts.ToList();
            ////foreach (var attempt in attempts)
            ////{
            ////    context.TestResults.Add(new TestResult
            ////    {
            ////        attempt_id = attempt.attempt_id,
            ////        date_passed = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
            ////    });
            ////}
            ////context.SaveChanges();

            //// Додаємо помилки (Mistakes)
            ////foreach (var attempt in attempts)
            ////{
            ////    var questions = context.Questions.Where(q => q.test_id == attempt.test_id).ToList();
            ////    foreach (var question in questions)
            ////    {
            ////        if (random.Next(0, 2) == 1) // Випадковий вибір, чи зроблена помилка
            ////        {
            ////            context.Mistakes.Add(new Mistake
            ////            {
            ////                attempt_id = attempt.attempt_id,
            ////                answer_id = .question_id
            ////            });
            ////        }
            ////    }
            ////}
            ////context.SaveChanges();

            //// Додаємо статуси тестів (TestStatuses)
            //foreach (var test in tests)
            //{
            //    var admin = admins[random.Next(admins.Count)]; // Випадковий адміністратор
            //    context.TestStatuses.Add(new TestStatus
            //    {
            //        test_id = test.test_id,
            //        admin_id = admin.user_id,
            //        status = random.Next(0, 2) == 0 ? "approved" : "rejected", // Випадковий статус
            //        date_changed = DateTime.SpecifyKind(DateTime.Now.AddDays(-random.Next(0, 10)), DateTimeKind.Utc)
            //    });
            //}
            //context.SaveChanges();

            Console.WriteLine("Test data including Mistakes and TestStatuses added successfully.");
        }


        static void CreateTables(NpgsqlConnection connection)
        {
            using (var command = new NpgsqlCommand())
            {
                command.Connection = connection;

                // 1. Створення таблиці Users
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        user_id SERIAL PRIMARY KEY,
                        name VARCHAR(100) NOT NULL,
                        login VARCHAR(100) UNIQUE NOT NULL,
                        password_hash VARCHAR(255) NOT NULL,
                        role VARCHAR(10) CHECK(role IN ('student', 'tutor', 'admin', 'supervisor')) NOT NULL
                    );";
                command.ExecuteNonQuery();

                // 2. Створення таблиці Tests
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Tests (
                        test_id SERIAL PRIMARY KEY,
                        title VARCHAR(255) NOT NULL,
                        tutor_id INT REFERENCES Users(user_id),
                        status VARCHAR(15) CHECK(status IN ('pending', 'on_review', 'approved', 'rejected')) NOT NULL,
                        area TEXT NOT NULL,
                        mins_to_pass INT
                    );";
                command.ExecuteNonQuery();

                // 3. Створення таблиці Questions
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Questions (
                        question_id SERIAL PRIMARY KEY,
                        test_id INT REFERENCES Tests(test_id),
                        question_text TEXT NOT NULL
                    );";
                command.ExecuteNonQuery();

                // 4. Створення таблиці Answers
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Answers (
                        answer_id SERIAL PRIMARY KEY,
                        question_id INT REFERENCES Questions(question_id),
                        answer_text TEXT NOT NULL,
                        is_correct BOOLEAN NOT NULL
                    );";
                command.ExecuteNonQuery();

                // 5. Створення таблиці Attempts
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Attempts (
                        attempt_id SERIAL PRIMARY KEY,
                        test_id INT REFERENCES Tests(test_id),
                        user_id INT REFERENCES Users(user_id),
                        attempt_number INT NOT NULL,
                        start_time TIMESTAMP WITHOUT TIME ZONE,
                        end_time TIMESTAMP WITHOUT TIME ZONE,
                        score DECIMAL(5, 2),
                        user_report TEXT
                    );";
                command.ExecuteNonQuery();

                // 8. Створення таблиці TestStatuses
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS TestStatuses (
                        status_id SERIAL PRIMARY KEY,
                        test_id INT REFERENCES Tests(test_id),
                        admin_id INT REFERENCES Users(user_id),
                        status VARCHAR(15) CHECK(status IN ('pending', 'approved', 'rejected')) NOT NULL,
                        date_changed TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                    );";
                command.ExecuteNonQuery();

                Console.WriteLine("All tables created successfully.");
            }
        }
    }
}
