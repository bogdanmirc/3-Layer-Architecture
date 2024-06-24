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
			//var customer = CustomerMapper.ToEntity(model);
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
            var c = customers.Select(CustomerMapper.ToModel).ToList();
            return c;
        }

        //      public async Task<IEnumerable<CustomerModel>> GetAllAsync()
        //      {
        //          var customers = await _unitOfWork.CustomerRepository.GetAllWithDetailsAsync();
        //          try {
        //		var x = _mapper.Map< List<Customer>, List<CustomerModel>>(customers.ToList());
        //		return x;
        //	}
        //          catch(Exception ex) 
        //          { 
        //              return Enumerable.Empty<CustomerModel>();
        //          }
        //}


        public async Task<CustomerModel> GetByIdAsync(int id)
        {
            var customer = await _unitOfWork.CustomerRepository.GetByIdWithDetailsAsync(id);

            return _mapper.Map<Customer, CustomerModel>(customer);
        }

        public async Task<IEnumerable<CustomerModel>> GetCustomersByProductIdAsync(int productId)
        {
			var customers = await _unitOfWork.CustomerRepository.GetAllWithDetailsAsync();
			var expectedCustomer = customers
				.Where(customer => customer.Receipts
					.Any(receipt => receipt.ReceiptDetails
						.Any(receiptDetails => receiptDetails.ProductId == productId)))
				.ToList();

			return _mapper.Map<IEnumerable<CustomerModel>>(expectedCustomer);
		}

        public async Task UpdateAsync(CustomerModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Surname) || model.BirthDate >= DateTime.Now || model.BirthDate <= new DateTime(1900, 1, 1, 1, 1, 1, DateTimeKind.Utc))
            {
                throw new MarketException();
            }
            var customer = await _unitOfWork.CustomerRepository.GetByIdWithDetailsAsync(model.Id);
            if (customer == null) {
				customer = _mapper.Map<Customer>(model);


			}
			customer.Person.Name = model.Name;
			customer.Person.BirthDate = model.BirthDate;
			customer.Person.Surname = model.Surname;
			_unitOfWork.CustomerRepository.Update(customer);
			await _unitOfWork.SaveAsync();
        }
    }
}
