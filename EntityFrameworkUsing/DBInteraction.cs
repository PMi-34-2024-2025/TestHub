using Microsoft.EntityFrameworkCore;
using PE.DesktopApplication.TestHub.BLL;
using static System.Net.Mime.MediaTypeNames;

namespace PE.DesktopApplication.TestHub.DAL
{
    public class DBInteraction : IDisposable
    {
        private static readonly TestingSystemContext _context = new TestingSystemContext();
        private bool _disposed = false;

        public static void AddUserToDB(string name, string login, string password, string role)
        {
            _context.Users.Add(new User
            {
                name = name,
                login = login,
                password_hash = PasswordHasher.HashPassword(password),
                role = role
            });
            _context.SaveChanges();
        }

        public static User GetUserFromDB(string login, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.login == login);
            if (user != null && PasswordHasher.VerifyPassword(password, user.password_hash))
                return user;
            else
                return null;
        }

        public static void SupervisorDeleteUserByLogin(string login)
        {
            _context.Users.Remove(SupervisorGetUserFromDB(login));
            _context.SaveChanges();
        }

        public static User SupervisorGetUserFromDB(string login)
        {
            return _context.Users.FirstOrDefault(u => u.login == login);
        }

        public static TestStatus GetTestStatusFromDB(int id)
        {
            return _context.TestStatuses.FirstOrDefault(t => t.status_id == id);
        }

        public static User GetUserFromDB(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.user_id == id);
            return user;
        }

        public static void UpdateTestStatus(int testId, string newStatus, User user)
        {
            try
            {
                var test = _context.Tests.FirstOrDefault(t => t.test_id == testId);

                if (test != null)
                {
                    test.status = newStatus;
                    _context.SaveChanges();
                }
                AddTestStatusToDB(user, test, newStatus, DateTime.UtcNow);
            }
            catch (Exception ex)
            {

            }
        }

        public static Test GetTestFromDB(int id)
        {
            var test = _context.Tests.FirstOrDefault(t => t.test_id == id);
            return test;
        }

        public static void AddTestToDB(User tutor, string title, string area, int minsToPass)
        {
            _context.Tests.Add(new Test
            {
                title = title,
                tutor_id = tutor.user_id,
                status = "pending",
                area = area,
                mins_to_pass = minsToPass
            });
            _context.SaveChanges();
        }
        public static void AddTestPartToDB(Test test, string questionText, Dictionary<string, bool> variants)
        {
            var question = new Question
            {
                test_id = test.test_id,
                question_text = questionText
            };
            _context.Questions.Add(question);
            _context.SaveChanges();
            bool allVariantsIncorrect = variants.Values.All(v => v == false);

            for (int i = 0; i < variants.Count; i++)
            {
                _context.Answers.Add(new Answer
                {
                    question_id = question.question_id,
                    answer_text = variants.ElementAt(i).Key,
                    is_correct = variants.ElementAt(i).Value
                });
            }
            if (allVariantsIncorrect)
            {
                _context.Answers.Add(new Answer
                {
                    question_id = question.question_id,
                    answer_text = "Усі варіанти вище некоректні",
                    is_correct = true
                });
            }

            _context.SaveChanges();
        }


        public static void AddAttemptToDB(User student, Test test, int attempt, DateTime start, DateTime finish, decimal score, string report)
        {
            _context.Attempts.Add(new Attempt
            {
                test_id = test.test_id,
                user_id = student.user_id,
                attempt_number = attempt,
                start_time = start,
                end_time = finish,
                score = score,
                user_report = report
            });
            _context.SaveChanges();
        }

        public static List<Attempt> getAttempts(int id)
        {
            return _context.Attempts.ToList();
        }

        public static string GetTestTitle(int id)
        {
            return _context.Tests.FirstOrDefault(t => t.test_id == id).title;
        }

        public static Attempt GetAttemptById(int id)
        {
            return _context.Attempts.FirstOrDefault(a => a.attempt_id == id);
        }

        public static List<string> GetAllAreas()
        {
            var areas = _context.Tests
                    .Select(test => test.area)
                    .Distinct()
                    .OrderBy(area => area)
                    .ToList();

            return areas;
        }
        public static void AddTestStatusToDB(User admin, Test test, string status, DateTime dateOfChange)
        {
            _context.TestStatuses.Add(new TestStatus
            {
                test_id = test.test_id,
                admin_id = admin.user_id,
                status = status,
                date_changed = dateOfChange
            });
            _context.SaveChanges();
        }

        public static TestData GetTestByIdAsync(int testId)
        {
            var testData = (from test in _context.Tests
                            join question in _context.Questions on test.test_id equals question.test_id
                            join answer in _context.Answers on question.question_id equals answer.question_id
                            where test.test_id == testId
                            select new
                            {
                                TestId = test.test_id,
                                Title = test.title,
                                TutorId = test.tutor_id,
                                Status = test.status,
                                Area = test.area,
                                MinutesToPass = test.mins_to_pass,
                                QuestionId = question.question_id,
                                QuestionText = question.question_text,
                                AnswerId = answer.answer_id,
                                AnswerText = answer.answer_text,
                                IsCorrect = answer.is_correct
                            }).ToList();

            if (!testData.Any())
                return null;

            // Перетворення на об'єкт TestData
            var groupedData = new TestData
            {
                TestId = testData.First().TestId,
                Title = testData.First().Title,
                TutorId = testData.First().TutorId,
                Status = testData.First().Status,
                Area = testData.First().Area,
                MinutesToPass = testData.First().MinutesToPass,
                Questions = testData
                    .GroupBy(x => new { x.QuestionId, x.QuestionText })
                    .Select(qGroup => new QuestionData
                    {
                        QuestionId = qGroup.Key.QuestionId,
                        QuestionText = qGroup.Key.QuestionText,
                        Answers = qGroup.Select(a => new AnswerData
                        {
                            AnswerId = a.AnswerId,
                            AnswerText = a.AnswerText,
                            IsCorrect = a.IsCorrect
                        }).ToList()
                    }).ToList()
            };

            return groupedData;
        }

        public static List<TestInfo> GetTestListForUserAsync(User user)
        {
            IQueryable<Test> query = _context.Tests;
            if (_context.Tests.Count() < 0)
            {
                return null;
            }
            if (user.role == "tutor")
            {
                query = query.Where(t => t.tutor_id == user.user_id);
            }
            else if (user.role == "student")
            {
                var completedTestIds = _context.Attempts
                    .Where(a => a.user_id == user.user_id)
                    .Select(a => a.test_id)
                    .Distinct();
                query = query.Where(t => t.status == "approved" && !completedTestIds.Contains(t.test_id));
            }
            else if (user.role == "admin")
            {
                query = query.Where(t => t.status == "pending");
            }
            var testList = query
                .Select(t => new TestInfo
                {
                    Id = t.test_id,
                    Title = t.title,
                    Description = t.area,
                    Status = t.status,
                    Role = user.role,
                    Area = t.area
                })
                .ToList();
            return testList;
        }

        public static int AddTestWithQuestionsAsync(User tutor, string title, string area, int minsToPass, List<(string questionText, Dictionary<string, bool> variants)> questionsData)
        {
            var newTest = new Test
            {
                title = title,
                tutor_id = tutor.user_id,
                status = "pending",
                area = area,
                mins_to_pass = minsToPass
            };

            _context.Tests.Add(newTest);
            _context.SaveChanges();

            int testId = newTest.test_id;
            AddTestStatusToDB(tutor, newTest, "pending", DateTime.UtcNow);
            foreach (var (questionText, variants) in questionsData)
            {
                var newQuestion = new Question
                {
                    test_id = testId,
                    question_text = questionText
                };
                _context.Questions.Add(newQuestion);
                _context.SaveChanges();

                int questionId = newQuestion.question_id;
                foreach (var pair in variants)
                {
                    var newAnswer = new Answer
                    {
                        question_id = questionId,
                        answer_text = pair.Key,
                        is_correct = pair.Value
                    };

                    _context.Answers.Add(newAnswer);
                }
            }

            _context.SaveChanges();
            return testId;
        }


        public static void AddAttemptWithResultsAsync(User student, Test test, int attemptNumber, int score, DateTime startTime, DateTime endTime, string attemptData)
        {
            var newAttempt = new Attempt
            {
                test_id = test.test_id,
                user_id = student.user_id,
                attempt_number = attemptNumber,
                start_time = startTime,
                end_time = endTime,
                score = score,
                user_report = attemptData
            };

            _context.Attempts.Add(newAttempt);
            _context.SaveChanges();
        }



        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context?.Dispose();
                }
                _disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~DBInteraction()
        {
            Dispose(false);
        }
    }
}






