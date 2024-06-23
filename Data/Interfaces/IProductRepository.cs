using Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetAllWithDetailsAsync();

        Task<Product> GetByIdWithDetailsAsync(int id);

        Task<IEnumerable<Product>> GetByFilterAsync(int? categoryId, int? maxPrice, int? minPrice);

        Task RemoveCategoryAsync(int categoryId);

        Task UpdateCategoryAsync(ProductCategory productcCategory);

        Task<IEnumerable<Product>> GetMostPopularProductsAsync(int productsCount);
    }
}
