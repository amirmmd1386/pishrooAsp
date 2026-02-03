using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pishrooAsp.Models.MessageTemplate
{
	public class SentMessage
	{
		[Key]
		public int Id { get; set; }

		[Column(TypeName = "nvarchar(max)")]
		public string? MessageContent { get; set; } // Nullable کن

		[Required]
		[MaxLength(200)]
		public string Recipient { get; set; } = string.Empty;

		[Column(TypeName = "nvarchar(max)")]
		public string? Subject { get; set; } // Nullable کن

		public DateTime SentDate { get; set; } = DateTime.Now;

		[MaxLength(50)]
		public string Status { get; set; } = "Pending";

		[MaxLength(50)]
		public string MessageType { get; set; } = "Email";

		// Foreign Keys - Nullable کن
		public int? CompanyId { get; set; }
		public int? TemplateId { get; set; }

		// Navigation Properties
		public virtual pishrooAsp.Models.Company.Company? Company { get; set; }
		public virtual MessageTemplate? Template { get; set; }

		[Column(TypeName = "nvarchar(max)")]
		public string UsedTokensJson { get; set; } = "{}";
	}
}