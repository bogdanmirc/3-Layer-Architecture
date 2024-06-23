using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Interfaces;
using Business.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceiptsController : ControllerBase
    {
        private readonly IReceiptService _receiptService;

        public ReceiptsController(IReceiptService receiptService)
        {
            _receiptService = receiptService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReceiptModel>>> Get()
        {
            var receipts = await _receiptService.GetAllAsync();
            return Ok(receipts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReceiptModel>> GetById(int id)
        {
            var receipt = await _receiptService.GetByIdAsync(id);
            if (receipt == null)
            {
                return NotFound();
            }
            return Ok(receipt);
        }

        [HttpGet("{id}/details")]
        public async Task<ActionResult<IEnumerable<ReceiptDetailModel>>> GetDetails(int id)
        {
            var details = await _receiptService.GetReceiptDetailsAsync(id);
            return Ok(details);
        }

        [HttpGet("{id}/sum")]
        public async Task<ActionResult<decimal>> GetSum(int id)
        {
            var sum = await _receiptService.ToPayAsync(id);
            return Ok(sum);
        }

        [HttpGet("period")]
        public async Task<ActionResult<IEnumerable<ReceiptModel>>> GetByPeriod(DateTime startDate, DateTime endDate)
        {
            var receipts = await _receiptService.GetReceiptsByPeriodAsync(startDate, endDate);
            return Ok(receipts);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ReceiptModel value)
        {
            await _receiptService.AddAsync(value);
            return CreatedAtAction(nameof(GetById), new { id = value.Id }, value);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] ReceiptModel value)
        {
            if (id != value.Id)
            {
                return BadRequest();
            }

            await _receiptService.UpdateAsync(value);
            return NoContent();
        }

        [HttpPut("{id}/products/add/{productId}/{quantity}")]
        public async Task<ActionResult> AddProduct(int id, int productId, int quantity)
        {
            await _receiptService.AddProductAsync(productId, id, quantity);
            return NoContent();
        }

        [HttpPut("{id}/products/remove/{productId}/{quantity}")]
        public async Task<ActionResult> RemoveProduct(int id, int productId, int quantity)
        {
            await _receiptService.RemoveProductAsync(productId, id, quantity);
            return NoContent();
        }

        [HttpPut("{id}/checkout")]
        public async Task<ActionResult> Checkout(int id)
        {
            await _receiptService.CheckOutAsync(id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _receiptService.DeleteAsync(id);
            return NoContent();
        }
    }
}
