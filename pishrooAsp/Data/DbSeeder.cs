using pishrooAsp.Data;

public static class DbSeeder
{
	public static void Seed(AppDbContext context)
	{
		if (!context.Users.Any())
		{
			context.Users.AddRange(
				new User
				{
					Username = "admin",
					PasswordHash = PasswordHelper.Hash("123456"),
					Role = "Admin"
				},
				new User
				{
					Username = "limited",
					PasswordHash = PasswordHelper.Hash("654321"),
					Role = "Limited"
				}
			);
			context.SaveChanges();
		}
	}
}
