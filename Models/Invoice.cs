namespace CRUDEF.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public string InvoiceId { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
    }
}
