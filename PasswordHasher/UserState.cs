namespace PE.DesktopApplication.TestHub.BLL
{
    public class UserState
    {
        private static UserState _instance;
        public string Name { get; private set; }
        public string Login { get; private set; }
        public string Role { get; private set; }
        public int Id { get; set; }

        private UserState() { }

        // Метод для отримання єдиного екземпляра (синглтон)
        public static UserState Instance => _instance ??= new UserState();

        public void SetUserState(string name, string login, string role, int id)
        {
            Name = name;
            Login = login;
            Role = role;
            Id = id;
        }
        public void ClearState()
        {
            Name = null;
            Login = null;
            Role = null;
            Id = -1;
        }

        // Метод для перевірки, чи користувач автентифікований
        public bool IsAuthenticated() => !string.IsNullOrEmpty(Login);
    }
}
