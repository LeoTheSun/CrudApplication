namespace CrudApplication.Core.Models
{
    public class User
    {
        public User(Guid guid, string login, string password, string name, int gender, DateTime? birthday, bool admin, DateTime createdOn, string createdBy, DateTime modifiedOn, string modifiedBy, DateTime? revokedOn, string? revokedBy)
        {
            Guid = guid;
            Login = login;
            Password = password;
            Name = name;
            Gender = gender;
            Birthday = birthday;
            Admin = admin;
            CreatedOn = createdOn;
            CreatedBy = createdBy;
            ModifiedOn = modifiedOn;
            ModifiedBy = modifiedBy;
            RevokedOn = revokedOn;
            RevokedBy = revokedBy;
        }


        public Guid Guid { get; }
        public string Login { get; }
        public string Password { get; }
        public string Name { get; }
        public int Gender { get; }
        public DateTime? Birthday { get; }
        public bool Admin { get; }
        public DateTime CreatedOn { get; }
        public string CreatedBy { get; }
        public DateTime ModifiedOn { get; }
        public string ModifiedBy { get; }
        public DateTime? RevokedOn { get; }
        public string? RevokedBy { get; }
    }
}