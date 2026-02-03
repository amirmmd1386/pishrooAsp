using Kavenegar;
using Microsoft.Extensions.Configuration;

public class KavenegarSmsSender : ISmsSender
{
	private readonly string _apiKey;
	private readonly string _sender;

	public KavenegarSmsSender(IConfiguration config)
	{
		_apiKey = config["Kavenegar:ApiKey"] ?? "5269784E645955614833434A474C6667694B425832794D746A4235596969434B457353706B72722B79746F3D";
		_sender = config["Kavenegar:Sender"] ?? "90006210";
	
	}

	public Task<string> SendAsync(string mobile, string message)
	{
		var api = new KavenegarApi(_apiKey);

		var result = api.Send(_sender, mobile, message);

		return Task.FromResult(result.StatusText);
	}

}
