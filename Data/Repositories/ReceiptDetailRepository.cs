using Data.Data;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class ReceiptDetailRepository : Repository<ReceiptDetail>, IReceiptDetailRepository
    {
        public ReceiptDetailRepository(TradeMarketDbContext context) : base(context) { }

        public async Task<IEnumerable<ReceiptDetail>> AddProductAsync(int productId, int receiptId, int quantity)
        {
            return await _context.ReceiptsDetails
                .Where(rd => rd.ProductId == productId && rd.ReceiptId == receiptId && rd.Quantity == quantity)
                .ToListAsync();
        }

        public async Task<IEnumerable<ReceiptDetail>> GetAllWithDetailsAsync()
        {
            return await _context.ReceiptsDetails
                .Include(rd => rd.Product)
                .ThenInclude(p => p.Category)
                .Include(rd => rd.Receipt)
                .ThenInclude(r => r.ReceiptDetails)
                .ToListAsync();
        }
    }
}
