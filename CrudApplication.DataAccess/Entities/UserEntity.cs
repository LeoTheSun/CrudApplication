using System.ComponentModel.DataAnnotations;


namespace CrudApplication.DataAccess.Entities
{
    public class UserEntity
    {
        public UserEntity(Guid guid, string login, string password, string name, int gender, DateTime? birthday, bool admin, DateTime createdOn, string createdBy, DateTime modifiedOn, string modifiedBy, DateTime? revokedOn, string? revokedBy)
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

        public UserEntity()
        {
            Guid = Guid.Empty;
            Login = string.Empty;
            Password = string.Empty;
            Name = string.Empty;
            Gender = -1;
            Birthday = null;
            Admin = false;
            CreatedOn = new DateTime();
            CreatedBy = string.Empty;
            ModifiedOn = new DateTime();
            ModifiedBy = string.Empty;
            RevokedOn = null;
            RevokedBy = null;
        }


        [Key]
        public Guid Guid { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public int Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public bool Admin { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? RevokedOn { get; set; }
        public string? RevokedBy { get; set; }
    }
}