namespace CRUDEF.Models
{
    public class InvoiceDetail
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        // Navigation properties
        public Invoice Invoice { get; set; }
        public Product Product { get; set; }
    }
}
