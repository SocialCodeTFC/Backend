namespace SocialCode.API.Requests
{
    public class SocialCodeResult<T>
    {
        public SocialCodeResult()
        {
            
        }
        public T Value { get; set; }
        
        public SocialCodeErrorTypes ErrorTypes { get; set; }

        public string ErrorMsg { get; set; }
        
        public bool IsValid()
        {
            return !(Value is null);
        }
    }
}