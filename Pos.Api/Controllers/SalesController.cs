using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pos.Application.Dtos;
using Pos.Domain.Entities;
using Pos.Domain.Entities.Enums;
using Pos.Infrastructure.Data;

namespace Pos.Api.Controllers
{
    [ApiController]
    [Route("api/sales")]
    public class SalesController : ControllerBase
    {
        private readonly PosDbContext _dbContext;
        private readonly IConfiguration _config;

        public SalesController(PosDbContext dbContext, IConfiguration config)
        {
            _dbContext = dbContext;
            _config = config;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSaleDto dto)
        {
            var vatRate = _config.GetValue<decimal>("Taxes:VatRate");

            var products = await _dbContext.Products
                //.Include(p => p.Inventory)
                //.Where(p => dto.Items.Select(i => i.ProductId).Contains(p.Id))
                .ToListAsync();

            if (products.Count != dto.Items.Count)
                return BadRequest("Producto invalido.");

            var sale = new Sale
            {
                Folio = $"POS-{DateTime.UtcNow:yyyyMMddHHmmss}"
            };

            foreach (var item in dto.Items)
            {
                var product = products.First(p => p.Id == item.ProductId);

                var lineTotal = product.Price * item.Quantity;

                var iepsRate = product.IepsRate;
                var factor = 1m + vatRate + iepsRate;

                var lineSubtotal = (product.Price / factor) * item.Quantity;

                var lineIva = lineSubtotal * vatRate;
                var lineIeps = lineSubtotal * iepsRate;

                sale.Items.Add(new SaleItem
                {
                    ProductId = product.Id,
                    SkuSnapshot = product.Sku,
                    NameSnapshot = product.Name,
                    UnitPrice = product.Price,
                    Quantity = item.Quantity,
                    LineSubtotal = Math.Round(lineSubtotal, 2),
                    LineIeps = Math.Round(lineIeps, 2)
                });

                sale.Subtotal += lineSubtotal;
                sale.TaxIva += lineIva;
                sale.TaxIeps += lineIeps;

                sale.Total += lineTotal;
            }

            sale.Subtotal = Math.Round(sale.Subtotal, 2);
            sale.TaxIva = Math.Round(sale.TaxIva, 2);
            sale.TaxIeps = Math.Round(sale.TaxIeps, 2);
            sale.Total = Math.Round(sale.Total, 2);

            _dbContext.Sales.Add(sale);
            await _dbContext.SaveChangesAsync();

            return Ok(new SaleDto
            {
                Id = sale.Id,
                Folio = sale.Folio,
                Status = sale.Status.ToString(),
                Subtotal = sale.Subtotal,
                TaxIva = sale.TaxIva,
                TaxIeps = sale.TaxIeps,
                Total = sale.Total
            });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var sale = await _dbContext.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .FirstOrDefaultAsync(s => s.Id == id);

            if (sale is null) return NotFound("La venta no existe.");

            var dto = new SaleDetailsDto
            {
                Id = sale.Id,
                Folio = sale.Folio,
                Status = sale.Status.ToString(),
                Subtotal = sale.Subtotal,
                TaxIva = sale.TaxIva,
                TaxIeps = sale.TaxIeps,
                Total = sale.Total,
                Items = sale.Items.Select(i => new SaleItemDetailsDto
                {
                    ProductId = i.ProductId,
                    Sku = i.SkuSnapshot,
                    Name = i.NameSnapshot,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    LineSubtotal = i.LineSubtotal,
                    LineIeps = i.LineIeps
                }).ToList(),
                Payments = sale.Payments
                    .OrderBy(p => p.CreatedAt)
                    .Select(p => new PaymentDetailsDto
                    {
                        Method = p.Method.ToString(),
                        Amount = p.Amount,
                        Status = p.Status.ToString(),
                        Reference = p.Reference,
                        AuthCode = p.AuthCode,
                        Message = p.Message,
                        CreatedAt = p.CreatedAt
                    }).ToList()
            };

            return Ok(dto);
        }

        [HttpPost("{id:int}/payments")]
        public async Task<IActionResult> Pay(int id, CreatePaymentDto dto)
        {
            if (dto.Payments is null || dto.Payments.Count == 0)
                return BadRequest("No se proporciono forma de pago.");

            await using var tx = await _dbContext.Database.BeginTransactionAsync();

            var sale = await _dbContext.Sales
                .Include(s => s.Items)
                .Include(s => s.Payments)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale is null) return NotFound("La venta no existe.");
            if (sale.Status == SaleStatus.CANCELED) return BadRequest("La venta esta cancelada.");
            if (sale.Status == SaleStatus.PAID) return BadRequest("La venta ya esta pagada.");

            var vatRate = _config.GetValue<decimal>("Taxes:VatRate");

            sale.Subtotal = 0;
            sale.TaxIeps = 0;

            foreach (var item in sale.Items)
            {
                var lineSubtotal = item.UnitPrice * item.Quantity;
                sale.Subtotal += lineSubtotal;
                sale.TaxIeps += item.LineIeps;
                item.LineSubtotal = lineSubtotal;
            }

            sale.TaxIva = sale.Subtotal * vatRate;
            sale.Total = sale.Subtotal + sale.TaxIva + sale.TaxIeps;

            decimal approvedTotalThisRequest = 0;
            decimal approvedCashThisRequest = 0;

            foreach (var p in dto.Payments)
            {
                if (p.Amount <= 0) return BadRequest("El monto del pago debe ser mayor a cero.");

                var methodOk = Enum.TryParse<PaymentMethod>(p.Method, ignoreCase: true, out var method);
                if (!methodOk) return BadRequest($"{p.Method} es un metodo de pago invalido. Use CASH o CARD");

                var status = PaymentStatus.APPROVED;
                string message = "Approved";
                string? authCode = null;

                if (method == PaymentMethod.CARD)
                {
                    if (string.IsNullOrWhiteSpace(p.Reference))
                    {
                        status = PaymentStatus.DECLINED;
                        message = "Referencia de tarjeta es requerido.";
                    }
                    else if (p.Reference.Contains("FAIL", StringComparison.OrdinalIgnoreCase))
                    {
                        status = PaymentStatus.DECLINED;
                        message = "Tarjeta declinada por el simulador";
                    }
                    else
                    {
                        status = PaymentStatus.APPROVED;
                        authCode = $"A{DateTime.UtcNow:HHmmss}";
                        message = "Tarjeta aprovada por el simulador";
                    }

                    if (status == PaymentStatus.DECLINED)
                    {
                        sale.Payments.Add(new Payment
                        {
                            Method = method,
                            Amount = p.Amount,
                            Reference = p.Reference,
                            Status = status,
                            Message = message
                        });

                        await _dbContext.SaveChangesAsync();
                        await tx.CommitAsync();

                        return Ok(new PaySaleResultDto
                        {
                            SaleId = sale.Id,
                            Status = sale.Status.ToString(),
                            Total = sale.Total,
                            ApprovedTotal = sale.Payments
                                .Where(x => x.Status == PaymentStatus.APPROVED)
                                .Sum(x => x.Amount),
                            Change = 0,
                            Message = message
                        });
                    }
                }

                if (method == PaymentMethod.CASH)
                {
                    status = PaymentStatus.APPROVED;
                    message = "Efectivo aceptado.";
                }

                sale.Payments.Add(new Payment
                {
                    Method = method,
                    Amount = p.Amount,
                    Reference = p.Reference,
                    Status = status,
                    AuthCode = authCode,
                    Message = message
                });

                if (status == PaymentStatus.APPROVED)
                {
                    approvedTotalThisRequest += p.Amount;
                    if (method == PaymentMethod.CASH) approvedCashThisRequest += p.Amount;
                }
            }

            var approvedTotalOverall = sale.Payments
                .Where(x => x.Status == PaymentStatus.APPROVED)
                .Sum(x => x.Amount);

            if (approvedTotalOverall < sale.Total)
            {
                await _dbContext.SaveChangesAsync();
                await tx.CommitAsync();

                return Ok(new PaySaleResultDto
                {
                    SaleId = sale.Id,
                    Status = sale.Status.ToString(),
                    Total = sale.Total,
                    ApprovedTotal = approvedTotalOverall,
                    Change = 0,
                    Message = $"Falta por pagar {sale.Total - approvedTotalOverall}."
                });
            }

            var productIds = sale.Items.Select(i => i.ProductId).Distinct().ToList();

            var inventories = await _dbContext.Inventories
                .Where(inv => productIds.Contains(inv.ProductId))
                .ToDictionaryAsync(inv => inv.ProductId);

            foreach (var item in sale.Items)
            {
                if (!inventories.TryGetValue(item.ProductId, out var inv))
                    return BadRequest($"Inventario no capturado para producto: {item.ProductId}");

                if (inv.Stock < item.Quantity)
                    return BadRequest($"No hay stock para producto: {item.ProductId}. Stock={inv.Stock}, required={item.Quantity}");
            }

            foreach (var item in sale.Items)
            {
                inventories[item.ProductId].Stock -= item.Quantity;
            }

            sale.Status = SaleStatus.PAID;
            sale.PaidAt = DateTime.UtcNow;

            var approvedCashOverall = sale.Payments
                .Where(x => x.Status == PaymentStatus.APPROVED && x.Method == PaymentMethod.CASH)
                .Sum(x => x.Amount);

            var change = approvedCashOverall > sale.Total ? (approvedCashOverall - sale.Total) : 0;

            await _dbContext.SaveChangesAsync();
            await tx.CommitAsync();

            return Ok(new PaySaleResultDto
            {
                SaleId = sale.Id,
                Status = sale.Status.ToString(),
                Total = sale.Total,
                ApprovedTotal = approvedTotalOverall,
                Change = change,
                Message = "Venta pagada correctamente."
            });
        }

        [HttpPost("{id:int}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            await using var tx = await _dbContext.Database.BeginTransactionAsync();

            var sale = await _dbContext.Sales
                .Include(s => s.Items)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale is null) return NotFound();
            if (sale.Status == SaleStatus.CANCELED) return BadRequest("Venta ya esta canceleda.");

            if (sale.Status == SaleStatus.PAID)
            {
                var productIds = sale.Items.Select(i => i.ProductId).Distinct().ToList();

                var inventories = await _dbContext.Inventories
                    .Where(inv => productIds.Contains(inv.ProductId))
                    .ToDictionaryAsync(inv => inv.ProductId);

                foreach (var item in sale.Items)
                {
                    if (!inventories.TryGetValue(item.ProductId, out var inv))
                        return BadRequest($"No se encontro inventario para el producto: {item.ProductId}");

                    inv.Stock += item.Quantity;
                }
            }

            sale.Status = SaleStatus.CANCELED;
            sale.CanceledAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            await tx.CommitAsync();

            return Ok(new { sale.Id, Status = sale.Status.ToString() });
        }
    }
}
