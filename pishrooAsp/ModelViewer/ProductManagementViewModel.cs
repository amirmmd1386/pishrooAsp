using pishrooAsp.Models.Products;

namespace pishrooAsp.ModelViewer
{
	public class ProductManagementViewModel
	{
		public ProductCreateViewModel CreateViewModel { get; set; }
		public List<Product> Products { get; set; }
	}

}
