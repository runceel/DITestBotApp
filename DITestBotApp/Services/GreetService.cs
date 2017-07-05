namespace DITestBotApp.Services
{
    public interface IGreetService
    {
        string GetMessage();
    }

    public class GreetService : IGreetService
    {
        public string GetMessage()
        {
            return "これはDIしたサービスから返されたメッセージです。";
        }
    }
}