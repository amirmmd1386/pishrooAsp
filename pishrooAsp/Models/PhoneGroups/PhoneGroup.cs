// Models/PhoneGroup.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pishrooAsp.Models.GroupSms
{
	public class PhoneGroup
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public List<PhoneNumber> PhoneNumbers { get; set; } = new List<PhoneNumber>();
		public DateTime CreatedAt { get; set; } = DateTime.Now;
		public DateTime UpdatedAt { get; set; } = DateTime.Now;
	}

	public class PhoneNumber
	{
		[Key]
		public int Id { get; set; }
		public string Number { get; set; }
		public string Name { get; set; }
		public bool IsActive { get; set; } = true;

		// این خط را اضافه کنید:
		public int PhoneGroupId { get; set; }

		// و این خط را هم اضافه کنید (اختیاری اما توصیه شده):
		[ForeignKey("PhoneGroupId")]
		public PhoneGroup? PhoneGroup { get; set; }
	}
}