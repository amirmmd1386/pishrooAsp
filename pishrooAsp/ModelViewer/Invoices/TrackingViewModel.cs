using pishrooAsp.Models.Invoice;
using System.ComponentModel.DataAnnotations;

namespace pishrooAsp.ModelViewer.Invoices
{
	public class TrackingViewModel
	{
		[Required(ErrorMessage = "کد پیگیری الزامی است")]
		[Display(Name = "کد پیگیری / شماره سفارش")]
		public string TrackingCode { get; set; }

		public List<Invoice> Invoices { get; set; } = new List<Invoice>();
		public bool ShowResults { get; set; }
	}
}
