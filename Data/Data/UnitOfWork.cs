using Data.Interfaces;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Data.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TradeMarketDbContext _context;

        public ICustomerRepository CustomerRepository { get; }
        public IPersonRepository PersonRepository { get; }
        public IProductRepository ProductRepository { get; }
        public IProductCategoryRepository ProductCategoryRepository { get; }
        public IReceiptRepository ReceiptRepository { get; }
        public IReceiptDetailRepository ReceiptDetailRepository { get; }

        public UnitOfWork(TradeMarketDbContext context)
        {
            _context = context;
            CustomerRepository = new CustomerRepository(context);
            PersonRepository = new PersonRepository(context);
            ProductRepository = new ProductRepository(context);
            ProductCategoryRepository = new ProductCategoryRepository(context);
            ReceiptRepository = new ReceiptRepository(context);
            ReceiptDetailRepository = new ReceiptDetailRepository(context);
        }

        

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
