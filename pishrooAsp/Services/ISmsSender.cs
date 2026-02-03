public interface ISmsSender
{
	Task<string> SendAsync(string mobile, string message);
}
