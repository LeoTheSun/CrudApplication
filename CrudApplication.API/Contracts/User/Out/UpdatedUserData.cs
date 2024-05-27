namespace CrudApplication.API.Contracts.User.Out
{
    public class UpdatedUserData
    {
        public UpdatedUserData(int code, string status, string detail)
        {
            Code = code;
            Status = status;
            Detail = detail;
        }

        public UpdatedUserData()
        {
            Code = -1;
            Status = string.Empty;
            Detail = string.Empty;
        }


        public int Code { get; set; }
        public string Status { get; set; }
        public string Detail { get; set; }
    }
}