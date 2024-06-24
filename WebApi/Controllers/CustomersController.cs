using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Interfaces;
using Business.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        //Inject customer service via constructor
        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // GET: api/customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerModel>>> Get()
        {
            var customers = await _customerService.GetAllAsync();
            return Ok(customers);
        }

        //GET: api/customers/1
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerModel>> GetById(int id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

        //GET: api/customers/products/1
        [HttpGet("products/{id}")]
        public async Task<ActionResult<CustomerModel>> GetByProductId(int id)
        {
            var customers = await _customerService.GetCustomersByProductIdAsync(id);
            return Ok(customers);
        }

        // POST: api/customers
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CustomerDTO value)
        {
            if(value == null || string.IsNullOrEmpty(value.Name) || string.IsNullOrEmpty(value.Surname) || value.BirthDate >= DateTime.Now || value.DiscountValue < 0 || value.BirthDate <= new DateTime(1900, 1, 1, 1, 1, 1, DateTimeKind.Utc))
            {
                return BadRequest();
            }
            var customerModel = new CustomerModel
            {
                Id = value.Id,
                Name = value.Name,
                BirthDate = value.BirthDate,
                Surname = value.Surname,
                DiscountValue = value.DiscountValue,
            };
            await _customerService.AddAsync(customerModel);
            return CreatedAtAction(nameof(GetById), new { id = value.Id }, value);
        }

        // PUT: api/customers/1
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int Id, [FromBody] CustomerDTO value)
        {
			if (value == null || string.IsNullOrEmpty(value.Name) || string.IsNullOrEmpty(value.Surname) || value.BirthDate >= DateTime.Now || value.DiscountValue < 0|| value.BirthDate <= new DateTime(1900, 1, 1, 1, 1, 1, DateTimeKind.Utc))
			{
				return BadRequest();
			}
			if (Id != value.Id)
            {
                return BadRequest();
            }

			var customerModel = new CustomerModel
			{
				Id = value.Id,
				Name = value.Name,
				BirthDate = value.BirthDate,
				Surname = value.Surname,
				DiscountValue = value.DiscountValue,
			};

			await _customerService.UpdateAsync(customerModel);
            return NoContent();
        }

        // DELETE: api/customers/1
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _customerService.DeleteAsync(id);
            return NoContent();
        }
    }
    public class CustomerDTO
    {
		public int Id { get; set; }
		public string? Name { get; set; }
		public string? Surname { get; set; }
		public DateTime BirthDate { get; set; }
		public int DiscountValue { get; set; }

	}
}
