using AutoMapper;
using Business.Interfaces;
using Business.Models;
using Business.Validation;
using Data.Data;
using Data.Entities;
using Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class CustomerService : ICustomerService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task AddAsync(CustomerModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Name) || model.BirthDate >= DateTime.Now || model.BirthDate <= new DateTime(1900, 1, 1, 1, 1, 1, DateTimeKind.Utc))
            {
                throw new MarketException();
            } 
            var customer = _mapper.Map<Customer>(model);
            await _unitOfWork.CustomerRepository.AddAsync(customer);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(int modelId)
        {
            await _unitOfWork.CustomerRepository.DeleteByIdAsync(modelId);
            await _unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<CustomerModel>> GetAllAsync()
        {
            var customers = await _unitOfWork.CustomerRepository.GetAllWithDetailsAsync();
            return _mapper.Map<IEnumerable<CustomerModel>>(customers);
        }

        public async Task<CustomerModel> GetByIdAsync(int id)
        {
            var customer = await _unitOfWork.CustomerRepository.GetByIdWithDetailsAsync(id);

            return _mapper.Map<CustomerModel>(customer);
        }

        public async Task<IEnumerable<CustomerModel>> GetCustomersByProductIdAsync(int productId)
        {
            var customers = await _unitOfWork.CustomerRepository.GetAllWithDetailsAsync();
            var expectedCustomer = new List<Customer>();
            foreach (var customer in customers)
            {
                foreach (var receipt in customer.Receipts)
                {
                    foreach (var receiptDetails in receipt.ReceiptDetails)
                    {
                        var product2 = receiptDetails.ProductId;
                        var x = product2;
                        if(product2 == productId)
                        {
                            expectedCustomer.Add(customer);
                        }
                    }
                }
            }
            return _mapper.Map<IEnumerable<CustomerModel>>(expectedCustomer);
        }

        public async Task UpdateAsync(CustomerModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Surname) || model.BirthDate >= DateTime.Now || model.BirthDate <= new DateTime(1900, 1, 1, 1, 1, 1, DateTimeKind.Utc))
            {
                throw new MarketException();
            }
            var customer = _mapper.Map<Customer>(model);
            _unitOfWork.CustomerRepository.Update(customer);
            await _unitOfWork.SaveAsync();
        }
    }
}
