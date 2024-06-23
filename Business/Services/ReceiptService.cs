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
    public class ReceiptService : IReceiptService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReceiptService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task AddAsync(ReceiptModel model)
        {
            var receipt = _mapper.Map<Receipt>(model);
            await _unitOfWork.ReceiptRepository.AddAsync(receipt);
            await _unitOfWork.SaveAsync();
        }



        public async Task AddProductAsync(int productId, int receiptId, int quantity)
        {

            // Step 1: Retrieve the receipt by its ID, including its details
            var receipt = await _unitOfWork.ReceiptRepository.GetByIdWithDetailsAsync(receiptId);

            // Step 2: Check if the receipt exists
            if (receipt == null)
            {
                throw new MarketException($"Receipt with ID {receiptId} not found.");
            }

            // Step 3: Ensure receipt details are initialized
            receipt.ReceiptDetails ??= new List<ReceiptDetail>();

            if(receipt.ReceiptDetails.Any(rd => rd.ProductId == productId))
            {
                var rd = receipt.ReceiptDetails.SingleOrDefault(rd => rd.ProductId == productId);
                rd.Quantity += quantity;
                await _unitOfWork.SaveAsync();
                return;
            }

            // Step 4: Retrieve the product by its ID
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(productId);

            // Step 5: Check if the product exists
            if (product == null)
            {
                throw new MarketException($"Product with ID {productId} not found.");
            }

            // Step 6: Ensure customer is not null and calculate discount unit price
            if (receipt.Customer == null)
            {
                throw new MarketException($"Customer associated with receipt ID {receiptId} not found.");
            }

            // Step 7: Find the receipt detail with the specified product ID
            var receiptDetail = receipt.ReceiptDetails.SingleOrDefault(rd => rd.ProductId == productId);

            if (receiptDetail == null)
            {
                // Step 8: Create a new receipt detail if the product is not already in the receipt
                receiptDetail = new ReceiptDetail
                {
                    ProductId = productId,
                    ReceiptId = receiptId,
                    Quantity = quantity,
                    UnitPrice = product.Price,
                    DiscountUnitPrice = product.Price - (product.Price * receipt.Customer.DiscountValue / 100m)
                };
                await _unitOfWork.ReceiptDetailRepository.AddAsync(receiptDetail);
            }
            else
            {
                // Step 9: Update the quantity if the product is already in the receipt
                receiptDetail.Quantity += quantity;
                _unitOfWork.ReceiptDetailRepository.Update(receiptDetail);
            }

            // Step 10: Save the changes to the database
            await _unitOfWork.SaveAsync();
        }






        public async Task CheckOutAsync(int receiptId)
        {

            var receipt = await _unitOfWork.ReceiptRepository.GetByIdAsync(receiptId);

            if (receipt == null)
            {
                throw new MarketException($"Receipt with ID {receiptId} not found.");
            }
            receipt.IsCheckedOut = true;
            await _unitOfWork.SaveAsync();
        }




        public async Task DeleteAsync(int modelId)
        {
            var receipt = await _unitOfWork.ReceiptRepository.GetByIdWithDetailsAsync(modelId);

            if (receipt == null)
            {
                throw new MarketException($"Receipt with ID {modelId} not found.");
            }

            foreach (var detail in receipt.ReceiptDetails)
            {
                _unitOfWork.ReceiptDetailRepository.Delete(detail);
            }
            await _unitOfWork.ReceiptRepository.DeleteByIdAsync(modelId);
            await _unitOfWork.SaveAsync();
        }




        public async Task<IEnumerable<ReceiptModel>> GetAllAsync()
        {
            var receipt = await _unitOfWork.ReceiptRepository.GetAllWithDetailsAsync();
            return _mapper.Map<IEnumerable<ReceiptModel>>(receipt);
        }

        public async Task<ReceiptModel> GetByIdAsync(int id)
        {
            var receipt = await _unitOfWork.ReceiptRepository.GetByIdWithDetailsAsync(id);
            return _mapper.Map<ReceiptModel>(receipt);
        }




        public async Task<IEnumerable<ReceiptDetailModel>> GetReceiptDetailsAsync(int receiptId)
        {
            var receipts = await _unitOfWork.ReceiptRepository.GetByIdWithDetailsAsync(receiptId);
            //var receipt = receipts.FirstOrDefault();
            if (receipts == null)
            {
                throw new MarketException($"Receipt with ID {receiptId} not found.");
            }
            

            var receiptDetailModels = _mapper.Map<IEnumerable<ReceiptDetailModel>>(receipts.ReceiptDetails);
            return receiptDetailModels;
        }




        public async Task<IEnumerable<ReceiptModel>> GetReceiptsByPeriodAsync(DateTime startDate, DateTime endDate)
        {
            var receipts = await _unitOfWork.ReceiptRepository.GetAllWithDetailsAsync();
            var receiptsByDate = receipts.Where(r => r.OperationDate >= startDate && r.OperationDate <= endDate);
            return _mapper.Map<IEnumerable<ReceiptModel>>(receiptsByDate);
        }

        public async Task RemoveProductAsync(int productId, int receiptId, int quantity)
        {
            var receipt = await _unitOfWork.ReceiptRepository.GetByIdWithDetailsAsync(receiptId);

            if (receipt == null)
            {
                throw new MarketException($"Receipt with ID {receiptId} not found.");
            }

            var receiptDetail = receipt.ReceiptDetails.FirstOrDefault(rd => rd.ProductId == productId);

            if (receiptDetail == null)
            {
                throw new MarketException($"Product with ID {productId} not found in receipt.");
            }

            receiptDetail.Quantity -= quantity;

            if (receiptDetail.Quantity <= 0)
            {
                _unitOfWork.ReceiptDetailRepository.Delete(receiptDetail);
            }

            await _unitOfWork.SaveAsync();
        }

        public async Task<decimal> ToPayAsync(int receiptId)
        {
            // Step 1: Retrieve the receipt by its ID, including its details
            var receipt = await _unitOfWork.ReceiptRepository.GetByIdWithDetailsAsync(receiptId);

            // Step 2: Check if the receipt exists
            if (receipt == null)
            {
                throw new MarketException($"Receipt with ID {receiptId} not found.");
            }

            // Step 3: Calculate the total amount to pay
            decimal totalAmount = receipt.ReceiptDetails
                .Sum(detail => detail.DiscountUnitPrice * detail.Quantity);

            // Step 4: Return the calculated total amount
            return totalAmount;
        }

        public async Task UpdateAsync(ReceiptModel model)
        {
            {
                var receipt = _mapper.Map<Receipt>(model);
                _unitOfWork.ReceiptRepository.Update(receipt);
                await _unitOfWork.SaveAsync();
            }
        }
    }
}
