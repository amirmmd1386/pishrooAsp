using System.ComponentModel.DataAnnotations;

namespace pishrooAsp.Models
{
	public class Lang
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
		public string Code { get; set; }
		public bool dir { get; set; }
	}
}
