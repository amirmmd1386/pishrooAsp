// Models/CompanySmsHistoryViewModel.cs
using System;

namespace pishrooAsp.Models
{
	public class CompanySmsHistoryViewModel
	{
		public int Id { get; set; }
		public string Message { get; set; }
		public DateTime SentAt { get; set; }
		public bool Status { get; set; }
	}
}
