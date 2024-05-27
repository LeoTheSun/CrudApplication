namespace CrudApplication.API.Contracts.User.In
{
    public class UpdateUserData
    {
        public UpdateUserData(string? login, string? password, string? name, int? gender, DateTime? birthday)
        {
            Login = login;
            Password = password;
            Name = name;
            Gender = gender;
            Birthday = birthday;
        }

        public UpdateUserData()
        {
            Login = null;
            Password = null;
            Name = null;
            Gender = null;
            Birthday = null;
        }


        public string? Login { get; set; }
        public string? Password { get; set; }
        public string? Name { get; set; }
        public int? Gender { get; set; }
        public DateTime? Birthday { get; set; }
    }
}