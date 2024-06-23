using Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IReceiptDetailRepository : IRepository<ReceiptDetail>
    {
        Task<IEnumerable<ReceiptDetail>> GetAllWithDetailsAsync();

        Task<IEnumerable<ReceiptDetail>> AddProductAsync(int productId, int receiptId, int quantity);
    }

}
