using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedisCache.API.Caching;
using RedisCache.API.Data;
using RedisCache.API.Models;

namespace RedisCache.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    private readonly ICachingService _cache;

    public ProductController(ApplicationDbContext context, ICachingService cache)
    {
        _context = context;
        _cache = cache;
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] Product product)
    {
        try
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception)
        {
            return BadRequest("Could not save product");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        try
        {
            var products = await _context.Products.ToListAsync();

            return Ok(products);
        }
        catch (Exception)
        {
            return BadRequest("Could not get product");
        }
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetAsync([FromRoute] Guid id)
    {
        try
        {
            Product? product;

            product = await _cache.GetAsync<Product>(id.ToString());

            if (product is not null)
                return Ok(product);

            product = _context.Products.FirstOrDefault(x => x.Id.Equals(id));

            if (product is null)
                return NotFound();

            await _cache.SetAsync(product.Id.ToString(), product);

            return Ok(product);
        }
        catch (Exception)
        {
            return BadRequest("Could not get product");
        }
    }
}