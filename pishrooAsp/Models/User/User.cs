public class User
{
	public int Id { get; set; }

	public string Username { get; set; }
	public string PasswordHash { get; set; }

	public string Role { get; set; } // Admin | Limited
	public bool IsActive { get; set; } = true;

	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	public ICollection<UserLoginLog> LoginLogs { get; set; }
}
