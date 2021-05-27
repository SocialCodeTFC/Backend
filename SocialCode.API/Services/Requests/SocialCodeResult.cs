namespace SocialCode.API.Services.Requests
{
    public class SocialCodeResult<T>
    {
        public SocialCodeResult()
        {
            
        }
        public T Value { get; set; }
        
        public SocialCodeError Error { get; set; }

        public string ErrorMsg { get; set; }
        
        public bool IsValid()
        {
            return !(Value is null);
        }
        
    }
}