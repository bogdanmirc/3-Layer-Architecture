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
    public class ReceiptRepository : Repository<Receipt>, IReceiptRepository
    {
        public ReceiptRepository(TradeMarketDbContext context) : base(context) { }

        public async Task<IEnumerable<Receipt>> GetAllWithDetailsAsync()
        {
            return await _context.Receipts
                .Include(r => r.Customer)
                .Include(r => r.ReceiptDetails)
                .ThenInclude(rd => rd.Product)
                .ThenInclude(p => p.Category)
                .ToListAsync();
        }

        public async Task<Receipt> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Receipts.Include(r => r.Customer)
                .Include(r => r.Customer)
                .Include(r => r.ReceiptDetails)
                .ThenInclude(rd => rd.Product)
                .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Receipt>> GetByIdWithDetails2Async(int id)
        {
            return await _context.Receipts.Include(r => r.Customer)
                .Include(r => r.ReceiptDetails)
                .ThenInclude(rd => rd.Product)
                .ThenInclude(p => p.Category)
                .Where(r => r.Id == id)
                .ToListAsync();
        }



        public async Task<IEnumerable<Receipt>> GetReceiptsByPeriodAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Receipts
                .Include(r => r.Customer)
                .Include(r => r.ReceiptDetails)
                .ThenInclude(rd => rd.Product)
                .Where(r => r.OperationDate >= startDate && r.OperationDate <= endDate)
                .ToListAsync();
        }
    }
}
