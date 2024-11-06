using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using ProductManagement.Models;

namespace ProductManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsAPIController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsAPIController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ProductsAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        // GET: api/ProductsAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/ProductsAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> EditProduct(int id, Product product)
        {
            if (id != product.ProductId)
            {
                //_logger.LogWarning("Product ID in the route does not match the ID in the product data. Route ID: {Id}, Product ID: {ProductId}", id, product.ProductId);
                return BadRequest("Product ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                //_logger.LogWarning("Model validation failed for product update. ModelState: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                var existingProduct = await _context.Products.FindAsync(id);

                if (existingProduct == null)
                {
                    //_logger.LogWarning("Product not found for update. Product ID: {ProductId}", id);
                    return NotFound("Product not found.");
                }

                // Update product fields
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.QuantityInStock = product.QuantityInStock;
                existingProduct.DiscountPercentage = product.DiscountPercentage;
                existingProduct.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                //_logger.LogInformation("Product updated successfully. Product ID: {ProductId}", product.ProductId);

                return Ok(existingProduct);
            }
            catch (Exception ex)
            {
                // Log the exception details
                //_logger.LogError(ex, "An error occurred while updating the product. Product ID: {ProductId}, Product Data: {@Product}", id, product);

                // Return a generic error response
                return StatusCode(500, "An error occurred while updating the product. Please try again later.");
            }
        }

        // POST: api/ProductsAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState); // Return validation errors to the client
                }
                product.CreatedAt = DateTime.Now;
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetProduct", new { id = product.ProductId }, product);
            }
            catch (Exception)
            {

                return StatusCode(500, "An error occurred while creating the product. Please try again later.");
            }
            
        }

        // DELETE: api/ProductsAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    //_logger.LogWarning("Product not found for deletion. Product ID: {ProductId}", id);
                    return NotFound("Product not found.");
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                //_logger.LogInformation("Product deleted successfully. Product ID: {ProductId}", id);
                return Ok("Product deleted successfully.");
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An error occurred while deleting the product. Product ID: {ProductId}", id);
                return StatusCode(500, "An error occurred while deleting the product. Please try again later.");
            }
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
