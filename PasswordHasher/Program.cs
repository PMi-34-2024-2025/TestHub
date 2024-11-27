namespace PE.DesktopApplication.TestHub.BLL
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(PasswordHasher.HashPassword("super1234visor"));
            //Console.WriteLine(PasswordHasher.PasswordHasher.VerifyPassword("1234", "ugPHH+J4n5meBcO4hW11KqEuVzgqIOcAHJ3/hK6Wk6m/GF0xuNniKkcnEfsOZdqY"));
        }
    }
}