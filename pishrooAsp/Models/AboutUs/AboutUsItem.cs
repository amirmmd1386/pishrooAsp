using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace pishrooAsp.Models
{
	public class AboutUsItem
	{
		[Key]
		public int Id { get; set; }

		// اطلاعات ثابت شرکت (مشترک بین همه زبان‌ها)



		public string? Phone1 { get; set; }
		public string? Phone2 { get; set; }
		public string? Phone3 { get; set; }
		public string? Phone4 { get; set; }
		public string? Phone5 { get; set; }
		public string? Phone6 { get; set; }

		public string? Fax1 { get; set; }
		public string? Fax2 { get; set; }
		public string? Fax3 { get; set; }
		public string? Fax4 { get; set; }
		public string? Fax5 { get; set; }
		public string? Fax6 { get; set; }

		public string? Email1 { get; set; }
		public string? Email2 { get; set; }
		public string? Email3 { get; set; }
		public string? Email4 { get; set; }
		public string? Email5 { get; set; }
		public string? Email6 { get; set; }

		public string? Website { get; set; }
		public string? MapLocation { get; set; }

		public int? EstablishedYear { get; set; }
		public int? EmployeeCount { get; set; }
		

		public string? Instagram { get; set; }
		public string? LinkedIn { get; set; }
		public string? Telegram { get; set; }
		public string? WhatsApp { get; set; }
		public string? YouTube { get; set; }
		public string? Facebook { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.Now;
		public DateTime UpdatedAt { get; set; } = DateTime.Now;

		// ارتباط با ترجمه‌ها
		public ICollection<AboutUsTranslation>? Translations { get; set; }
	}
}
