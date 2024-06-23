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
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(TradeMarketDbContext context) : base(context) { }

        public async Task<IEnumerable<Product>> GetAllWithDetailsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ReceiptDetails)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByFilterAsync(int? categoryId, int? maxPrice, int? minPrice)
        {
            //if (maxPrice == null || minPrice == null)
            //{
            //    return await _context.Products
            //   .Include(p => p.Category)
            //   .Where(p => p.Category.Id == categoryId)
            //   .ToListAsync();
            //}
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Category.Id == categoryId && p.Price >= minPrice && p.Price <= maxPrice)
                .ToListAsync();
        }

        public async Task<Product> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ReceiptDetails)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> GetMostPopularProductsAsync(int productsCount)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ReceiptDetails)
                .OrderByDescending(p => p.ReceiptDetails.Sum(rd => rd.Quantity))
                .Take(productsCount)
                .ToListAsync();
        }

        public async Task RemoveCategoryAsync(int categoryId)
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            foreach (var product in products) 
            {
                if(product.Category.Id == categoryId)
                {
                    product.Category = null;
                }
            }
        }

        public async Task UpdateCategoryAsync(ProductCategory productcCategory)
        {
            var products = await _context.Products.Include(p => p.Category).Where(p => p.ProductCategoryId == productcCategory.Id).ToListAsync();
            foreach(var product in products)
            {
                product.Category = productcCategory;
            }
        }

    }
}
