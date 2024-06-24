using Business.Models;
using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
	public static class CustomerMapper
	{
		public static CustomerModel ToModel(Customer customer)
		
		{
			if (customer == null)
			{
				return null;
			}

			
			return new CustomerModel
			{
				Id = customer.Id,
				Name = customer.Person?.Name,
				Surname = customer.Person?.Surname,
				BirthDate = customer.Person?.BirthDate ?? DateTime.MinValue,
				DiscountValue = customer.DiscountValue,
				ReceiptsIds = customer.Receipts?.Select(r => r.Id).ToList() ?? new List<int>()
			};
		}

		public static Customer ToEntity(CustomerModel customerModel)
		{
			if (customerModel == null)
			{
				return null;
			}

			return new Customer
			{
				Id = customerModel.Id,
				Person = new Person
				{
					Name = customerModel.Name,
					Surname = customerModel.Surname,
					BirthDate = customerModel.BirthDate
				},
				DiscountValue = customerModel.DiscountValue,
				Receipts = customerModel.ReceiptsIds?.Select(id => new Receipt { Id = id }).ToList() ?? new List<Receipt>()
			};
		}
	}
}
