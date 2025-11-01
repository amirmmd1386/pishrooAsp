using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pishrooAsp.Models.ProductRequest
{
	public class message
	{
		public int Id { get; set; }
		public string Message { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		// ✅ ارتباط با درخواست‌ها از طریق جدول رابط
		public ICollection<ProductRequestMessage> ProductRequestMessages { get; set; } = new List<ProductRequestMessage>();
	}
}
