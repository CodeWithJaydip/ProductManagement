using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using ProductManagement.Data.Repository;
using ProductManagement.Models;

namespace ProductManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsAPIController : ControllerBase
    {

        private readonly ILogger<ProductsAPIController> _logger;
        private readonly IProductRepository _productRepository;

        public ProductsAPIController(IProductRepository productRepository, ILogger<ProductsAPIController> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        // GET: api/ProductsAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _productRepository.GetAllProductsAsync();
            return Ok(products);
        }

        // GET: api/ProductsAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);

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
                _logger.LogWarning("Product ID in the route does not match the ID in the product data. Route ID: {Id}, Product ID: {ProductId}", id, product.ProductId);
                return BadRequest("Product ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model validation failed for product update. ModelState: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                var existingProduct = await _productRepository.UpdateProductAsync(product);

                if (existingProduct)
                {
                    _logger.LogWarning("Product not found for update. Product ID: {ProductId}", id);
                    return NotFound("Product not found.");
                }
                _logger.LogInformation("Product updated successfully. Product ID: {ProductId}", product.ProductId);
                return Ok(existingProduct);
            }
            catch (Exception ex)
            {
                // Log the exception details
                _logger.LogError(ex, "An error occurred while updating the product. Product ID: {ProductId}, Product Data: {@Product}", id, product);
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
                var createdProduct = await _productRepository.AddProductAsync(product);
                return CreatedAtAction("GetProduct", new { id = createdProduct.ProductId }, product);
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
                var deleteSuccess = await _productRepository.DeleteProductAsync(id);
                if (!deleteSuccess)
                {
                    return NotFound("Product not found.");
                }

                return Ok("Product deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the product. Product ID: {ProductId}", id);
                return StatusCode(500, "An error occurred while deleting the product. Please try again later.");
            }
        }
    }
}
