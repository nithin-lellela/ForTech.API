namespace ForTech.API.Models
{
    public class AuthResponseModel
    {
        public ResponseCode ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public object DataSet { get; set; }
    }
}

public enum ResponseCode
{
    Ok = 1,
    Error = 2,
}