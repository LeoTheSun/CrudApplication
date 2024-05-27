namespace CrudApplication.API.Contracts
{
    public class Error
    {
        public Error(int code, string status, string detail)
        {
            Code = code;
            Status = status;
            Detail = detail;
        }

        public Error()
        {
            Code = -1;
            Status = string.Empty;
            Detail = string.Empty;
        }


        public int Code { get; set; }
        public string Status { get; set; }
        public string Detail { get; set; }


        public string ToLog()
        {
            return $"Error:\n    Code: {Code}\n    Status: {Status}\n    Detail: {Detail}";
        }
    }
}