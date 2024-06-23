using Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IReceiptRepository : IRepository<Receipt>
    {
        Task<IEnumerable<Receipt>> GetAllWithDetailsAsync();

        Task<Receipt> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Receipt>> GetByIdWithDetails2Async(int id);


        Task<IEnumerable<Receipt>> GetReceiptsByPeriodAsync(DateTime startDate, DateTime endDate);


    }
}
