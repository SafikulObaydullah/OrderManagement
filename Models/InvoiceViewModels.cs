using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CRUDEF.Models
{
    public class InvoiceCreateViewModel
    {
        public string? InvoiceNo { get; set; }
        public List<InvoiceDetailInputModel> Details { get; set; } = new();

        [BindNever]
        public List<ProductOptionViewModel> Products { get; set; } = new();
    }

    public class InvoiceDetailInputModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class ProductOptionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public class InvoiceListViewModel
    {
        public int Id { get; set; }
        public string InvoiceNo { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public List<InvoiceDetailListViewModel> Details { get; set; } = new();
    }

    public class InvoiceDetailListViewModel
    {
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
