using CRUDEF.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRUDEF.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InvoiceController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var invoices = await _context.Invoices
                .Include(invoice => invoice.InvoiceDetails)
                .ThenInclude(detail => detail.Product)
                .OrderByDescending(invoice => invoice.Id)
                .Select(invoice => new InvoiceListViewModel
                {
                    Id = invoice.Id,
                    InvoiceNo = invoice.InvoiceId,
                    TotalPrice = invoice.TotalPrice,
                    Details = invoice.InvoiceDetails.Select(detail => new InvoiceDetailListViewModel
                    {
                        ProductName = detail.Product.Name,
                        UnitPrice = detail.Product.Price,
                        Quantity = detail.Quantity,
                        TotalPrice = detail.TotalPrice
                    }).ToList()
                })
                .ToListAsync();

            return View(invoices);
        }

        [HttpGet]
        public async Task<IActionResult> AddInvoice()
        {
            var model = new InvoiceCreateViewModel
            {
                InvoiceNo = GenerateInvoiceNo(),
                Details = new List<InvoiceDetailInputModel>
                {
                    new InvoiceDetailInputModel { Quantity = 1 }
                }
            };

            await LoadProductsAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddInvoice(InvoiceCreateViewModel model)
        {
            model.Details = model.Details
                .Where(detail => detail.ProductId > 0 || detail.Quantity > 0)
                .ToList();

            if (!model.Details.Any())
            {
                ModelState.AddModelError(string.Empty, "Please add at least one invoice detail row.");
            }

            if (model.Details.Any(detail => detail.ProductId <= 0))
            {
                ModelState.AddModelError(string.Empty, "Please select a product for every detail row.");
            }

            if (model.Details.Any(detail => detail.Quantity <= 0))
            {
                ModelState.AddModelError(string.Empty, "Quantity must be greater than zero.");
            }

            var productIds = model.Details.Select(detail => detail.ProductId).Distinct().ToList();
            var products = await _context.Products
                .Where(product => productIds.Contains(product.Id))
                .ToDictionaryAsync(product => product.Id);

            if (products.Count != productIds.Count)
            {
                ModelState.AddModelError(string.Empty, "One or more selected products could not be found.");
            }

            if (!ModelState.IsValid)
            {
                await LoadProductsAsync(model);
                return View(model);
            }

            var invoice = new Invoice
            {
                InvoiceId = string.IsNullOrWhiteSpace(model.InvoiceNo) ? GenerateInvoiceNo() : model.InvoiceNo.Trim()
            };

            foreach (var row in model.Details)
            {
                var product = products[row.ProductId];
                var lineTotal = product.Price * row.Quantity;

                invoice.InvoiceDetails.Add(new InvoiceDetail
                {
                    ProductId = product.Id,
                    Quantity = row.Quantity,
                    TotalPrice = lineTotal
                });

                invoice.TotalPrice += lineTotal;
            }

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadProductsAsync(InvoiceCreateViewModel model)
        {
            model.Products = await _context.Products
                .OrderBy(product => product.Name)
                .Select(product => new ProductOptionViewModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price
                })
                .ToListAsync();
        }

        private static string GenerateInvoiceNo()
        {
            return $"INV-{DateTime.Now:yyyyMMddHHmmss}";
        }
    }
}
