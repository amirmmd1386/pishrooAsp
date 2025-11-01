using System.ComponentModel.DataAnnotations;

namespace pishrooAsp.Models.Products
{
	public class ProductTranslation
	{
		public Guid Id { get; set; }

		[Required, MaxLength(200)]
		public string Title { get; set; }

		//[MaxLength(1000)]
		public string? Description { get; set; }

		[Required]
		public Guid ProductId { get; set; }
		public Product Product { get; set; }

		[Required]
		public int LangId { get; set; }
		public Lang Lang { get; set; }
	}

}
