using Inventory.Application.DTOs;
using Inventory.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly ProductService _productService;

    public ProductsController(ProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProducts()
    {
        return await _productService.GetProductsAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResponse>> GetProduct(int id)
    {
        var product = await _productService.GetProductAsync(id);
        if (product is null) return NotFound();
        return product;
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponse>> CreateProduct(ProductRequest request)
    {
        var (product, errorMessage) = await _productService.CreateProductAsync(request);
        if (errorMessage is not null)
        {
            return BadRequest(errorMessage);
        }

        return CreatedAtAction(nameof(GetProduct), new { id = product!.Id }, product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, ProductRequest request)
    {
        var (success, notFound, errorMessage) = await _productService.UpdateProductAsync(id, request);
        if (notFound) return NotFound();
        if (!success) return BadRequest(errorMessage);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var deleted = await _productService.DeleteProductAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
