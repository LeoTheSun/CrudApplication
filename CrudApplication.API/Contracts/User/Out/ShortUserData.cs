namespace CrudApplication.API.Contracts.User.Out
{
    public class ShortUserData
    {
        public ShortUserData(string name, int gender, DateTime? birthday, bool active)
        {
            Name = name;
            Gender = gender;
            Birthday = birthday;
            Active = active;
        }

        public ShortUserData()
        {
            Name = string.Empty;
            Gender = -1;
            Birthday = null;
            Active = false;
        }


        public string Name { get; set; }
        public int Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public bool Active { get; set; }
    }
}