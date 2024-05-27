namespace CrudApplication.API.Contracts.User.In
{
    public class CreateUserData
    {
        public CreateUserData(string login, string password, string name, int gender, DateTime? birthday, bool admin)
        {
            Login = login;
            Password = password;
            Name = name;
            Gender = gender;
            Birthday = birthday;
            Admin = admin;
        }

        public CreateUserData()
        {
            Login = string.Empty;
            Password = string.Empty;
            Name = string.Empty;
            Gender = -1;
            Birthday = null;
            Admin = false;
        }


        public string Login { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public int Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public bool Admin { get; set; }
    }
}