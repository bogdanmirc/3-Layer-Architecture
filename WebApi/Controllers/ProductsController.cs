using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Interfaces;
using Business.Models;
using Business.Validation;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductModel>>> Get()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductModel>> GetById(int id)
        {
			try
			{
				var product = await _productService.GetByIdAsync(id);
				return Ok(product);
			}
			catch (MarketException)
			{
				return NotFound();
			}
		}

        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<ProductCategoryModel>>> GetCategories()
        {
            var categories = await _productService.GetAllProductCategoriesAsync();
            return Ok(categories);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ProductDTO value)
        {
			if (value == null || string.IsNullOrEmpty(value.CategoryName))
			{
				return BadRequest();
			}
            var productModel = new ProductModel
            {
                Id= value.Id,
                CategoryName= value.CategoryName,
                Price= value.Price,
                ProductCategoryId=value.ProductCategoryId,
                ProductName= value.ProductName,
            };
			await _productService.AddAsync(productModel);
            return CreatedAtAction(nameof(GetById), new { id = value.Id }, value);
        }

        [HttpPost("categories")]
        public async Task<ActionResult> PostCategory([FromBody] ProductCategoryModel value)
        {
            await _productService.AddCategoryAsync(value);
            return CreatedAtAction(nameof(GetCategories), new { id = value.Id }, value);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] ProductModel value)
        {
            if (id != value.Id)
            {
                return BadRequest();
            }

            await _productService.UpdateAsync(value);
            return NoContent();
        }

        [HttpPut("categories/{id}")]
        public async Task<ActionResult> PutCategory(int id, [FromBody] ProductCategoryModel value)
        {
            if (id != value.Id)
            {
                return BadRequest();
            }

            await _productService.UpdateCategoryAsync(value);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _productService.DeleteAsync(id);
            return NoContent();
        }

        [HttpDelete("categories/{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            await _productService.RemoveCategoryAsync(id);
            return NoContent();
        }


		public class ProductDTO
		{
			public int Id { get; set; }
			public int ProductCategoryId { get; set; }
			public string CategoryName { get; set; }
			public string ProductName { get; set; }
			public decimal Price { get; set; }

		}
	}
}
