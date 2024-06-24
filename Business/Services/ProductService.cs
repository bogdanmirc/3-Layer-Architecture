using AutoMapper;
using Business.Interfaces;
using Business.Models;
using Business.Validation;
using Data.Entities;
using Data.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }



        public async Task AddAsync(ProductModel model)
        {

            if (model == null)
            {
                throw new MarketException("Product model cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(model.ProductName))
            {
                throw new MarketException("Product name cannot be empty.");
            }

            if (model.Price < 0)
            {
                throw new MarketException("Product price cannot be negative.");
            }


            var product = _mapper.Map<Product>(model);
            await _unitOfWork.ProductRepository.AddAsync(product);
            await _unitOfWork.SaveAsync();
        }




        public async Task AddCategoryAsync(ProductCategoryModel categoryModel)
        {
            if (categoryModel == null)
            {
                throw new MarketException("Category model cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(categoryModel.CategoryName))
            {
                throw new MarketException("Category name cannot be empty.");
            }
            var productCategory = _mapper.Map<ProductCategory>(categoryModel);
            await _unitOfWork.ProductCategoryRepository.AddAsync(productCategory);
            await _unitOfWork.SaveAsync();
        }




        public async Task DeleteAsync(int modelId)
        {
            await _unitOfWork.ProductRepository.DeleteByIdAsync(modelId);
            await _unitOfWork.SaveAsync();
        }




        public async Task<IEnumerable<ProductModel>> GetAllAsync()
        {
            var products = await _unitOfWork.ProductRepository.GetAllWithDetailsAsync();
            return _mapper.Map<IEnumerable<ProductModel>>(products);
        }

        public async Task<IEnumerable<ProductCategoryModel>> GetAllProductCategoriesAsync()
        {
            var productCategories = await _unitOfWork.ProductCategoryRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductCategoryModel>>(productCategories);
        }

        public async Task<IEnumerable<ProductModel>> GetByFilterAsync(FilterSearchModel filterSearch)
        {
            var allProducts = await _unitOfWork.ProductRepository.GetAllWithDetailsAsync();

            List<Product> filtered;
            if (filterSearch.CategoryId is null)
            {
                filtered = allProducts.Where(p => p.Price >= filterSearch.MinPrice).ToList();
                return _mapper.Map<List<ProductModel>>(filtered);
            }

            if (filterSearch.MinPrice is null && filterSearch.MaxPrice is null)
            {
                filtered = allProducts.Where(p => p.Category.Id == filterSearch.CategoryId).ToList();
                return _mapper.Map<List<ProductModel>>(filtered);
            }

            if (filterSearch.MaxPrice is null) 
            {
               filtered = allProducts.Where(p => p.Category.Id == filterSearch.CategoryId && p.Price >= filterSearch.MinPrice).ToList();
                return _mapper.Map<List<ProductModel>>(filtered);
            }

            if (filterSearch.MinPrice is null)
            {
                filtered = allProducts.Where(p => p.Category.Id == filterSearch.CategoryId && p.Price <= filterSearch.MaxPrice).ToList();
                return _mapper.Map<List<ProductModel>>(filtered);
            }

            filtered = allProducts.Where(p => p.Category.Id == filterSearch.CategoryId && (filterSearch.MinPrice <= p.Price && filterSearch.MaxPrice >= p.Price)).ToList();

            return _mapper.Map<List<ProductModel>>(filtered);
        }





        public async Task<ProductModel> GetByIdAsync(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdWithDetailsAsync(id);
            if (product == null)
            {
                throw new MarketException($"Product with ID {id} does not exist.");
            }
            return _mapper.Map<ProductModel>(product);
        }





        public async Task RemoveCategoryAsync(int categoryId)
        {
            await _unitOfWork.ProductCategoryRepository.DeleteByIdAsync(categoryId);
            await _unitOfWork.SaveAsync();
        }



        public async Task UpdateAsync(ProductModel model)
        {
            if (string.IsNullOrWhiteSpace(model.ProductName))
            {
                throw new MarketException("Product name cannot be empty.");
            }

            if (model.Price < 0)
            {
                throw new MarketException("Product price cannot be negative.");
            }
            var product = _mapper.Map<Product>(model);
            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveAsync();
        }


        public async Task UpdateCategoryAsync(ProductCategoryModel categoryModel)
        {
            if (string.IsNullOrWhiteSpace(categoryModel.CategoryName))
            {
                throw new MarketException("Category name cannot be empty.");
            }
            var productCategory = _mapper.Map<ProductCategory>(categoryModel);
            _unitOfWork.ProductCategoryRepository.Update(productCategory);
            await _unitOfWork.SaveAsync();
        }
    }
}
