public class UserLoginLog
{
	public int Id { get; set; }

	public int UserId { get; set; }
	public User User { get; set; }

	public DateTime LoginAt { get; set; } = DateTime.UtcNow;
	public string Ip { get; set; }
}
