using AutoMapper;
using Business.Interfaces;
using Business.Models;
using Business.Validation;
using Data.Entities;
using Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class StatisticService : IStatisticService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StatisticService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductModel>> GetCustomersMostPopularProductsAsync(int productCount, int customerId)
        {

            // Step 1: Retrieve all receipts for the specified customer
            var receipts = await _unitOfWork.ReceiptRepository.GetAllWithDetailsAsync();
            var customerReceipts = receipts.Where(r => r.CustomerId == customerId);

            // Step 2: Extract and count the occurrence of each product in the customer's receipts
            var productCounts = customerReceipts
                .SelectMany(r => r.ReceiptDetails)
                .GroupBy(rd => rd.Product)
                .Select(g => new { Product = g.Key, Count = g.Sum(rd => rd.Quantity) })
                .OrderByDescending(pc => pc.Count)
                .Take(productCount)
                .ToList();

            // Step 3: Map the selected products to ProductModel objects
            var productModels = _mapper.Map<IEnumerable<ProductModel>>(productCounts.Select(pc => pc.Product));

            // Step 4: Return the list of ProductModel objects
            return productModels;
        }

        public async Task<decimal> GetIncomeOfCategoryInPeriod(int categoryId, DateTime startDate, DateTime endDate)
        {
            // Step 1: Retrieve all receipts with details
            var receipts = await _unitOfWork.ReceiptRepository.GetAllWithDetailsAsync();

            // Step 2: Filter the receipts within the specified date range
            var filteredReceipts = receipts.Where(r => r.OperationDate >= startDate && r.OperationDate <= endDate);

            // Step 3: Filter the receipt details by the specified product category
            var receiptDetails = filteredReceipts
                .SelectMany(r => r.ReceiptDetails)
                .Where(rd => rd.Product.ProductCategoryId == categoryId)
                .ToList();

            // Step 4: Calculate the total income from the filtered receipt details
            var totalIncome = receiptDetails.Sum(rd => rd.DiscountUnitPrice * rd.Quantity);

            // Step 5: Return the calculated total income
            return totalIncome;
        }

        public async Task<IEnumerable<ProductModel>> GetMostPopularProductsAsync(int productCount)
        {

            var receiptDetails = await _unitOfWork.ReceiptDetailRepository.GetAllWithDetailsAsync();

            var productSales = receiptDetails
                .GroupBy(rd => rd.Product)
                .Select(g => new { Product = g.Key, TotalQuantitySold = g.Sum(rd => rd.Quantity) })
                .OrderByDescending(ps => ps.TotalQuantitySold)
                .Take(productCount)
                .ToList();

            var mostPopularProducts = productSales.Select(ps => ps.Product);

            return _mapper.Map<IEnumerable<ProductModel>>(mostPopularProducts);
        }

        public async Task<IEnumerable<CustomerActivityModel>> GetMostValuableCustomersAsync(int customerCount, DateTime startDate, DateTime endDate)
        {
            // Step 1: Retrieve all receipts within the specified date range
            var receipts = await _unitOfWork.ReceiptRepository.GetAllWithDetailsAsync();

            // Step 2: Group the receipts by customer and calculate the total sum of receipts for each customer
            var customerSums = receipts
                .Where(r => r.OperationDate >= startDate && r.OperationDate <= endDate)
                .GroupBy(r => r.Customer)
                .Select(g => new
                {
                    Customer = g.Key,
                    TotalSum = g.Sum(r => r.ReceiptDetails.Sum(rd => rd.DiscountUnitPrice * rd.Quantity))
                })
                .OrderByDescending(cs => cs.TotalSum)
                .Take(customerCount)
                .ToList();

            // Step 3: Map the selected customers to CustomerActivityModel objects
            var customerActivityModels = customerSums.Select(cs => new CustomerActivityModel
            {
                CustomerId = cs.Customer.Id,
                CustomerName = $"{cs.Customer.Person.Name} {cs.Customer.Person.Surname}",
                ReceiptSum = cs.TotalSum
            });
            // Step 4: Return the list of CustomerActivityModel objects
            return customerActivityModels;
        }
    }
}
