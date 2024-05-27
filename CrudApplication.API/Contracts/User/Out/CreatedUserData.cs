namespace CrudApplication.API.Contracts.User.Out
{
    public class CreatedUserData
    {
        public CreatedUserData(int code, string status, Guid guid)
        {
            Code = code;
            Status = status;
            @Guid = guid;
        }

        public CreatedUserData()
        {
            Code = -1;
            Status = string.Empty;
            Guid = Guid.Empty;
        }


        public int Code { get; set; }
        public string Status { get; set; }
        public Guid @Guid { get; set; }
    }
}