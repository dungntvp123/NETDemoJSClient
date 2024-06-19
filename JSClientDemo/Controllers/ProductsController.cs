using AutoMapper;
using JSClientDemo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JSClientDemo.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductsController : ControllerBase
	{
		private readonly MySaleDBContext _context;
		private readonly IMapper _mapper;

		public ProductsController(MySaleDBContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<IActionResult> GetList()
		{
			return Ok(await _context.Products.ToListAsync());
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetById(int id)
		{
			var product = await _context.Products
				.SingleOrDefaultAsync(p => p.ProductId == id);
			if (product == null)
			{
				return NotFound();
			}

			return Ok(product);
		}

		[HttpPost]
		public async Task<IActionResult> Create(CreateProductRequest productRequest)
		{
			if (productRequest == null)
			{
				return BadRequest();
			}
			Product product = _mapper.Map<Product>(productRequest);

			_context.Products.Add(product);
			await _context.SaveChangesAsync();
			return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, product);
		}

		[HttpPut("{id:int}")]
		public async Task<IActionResult> Update(int id, UpdateProductRequest productRequest)
		{
			if (productRequest == null)
			{
				return BadRequest();
			}
			if (!_context.Products.Any(p => p.ProductId == id))
			{
				return NotFound();
			}

			Product product = _mapper.Map<Product>(productRequest);
			product.ProductId = id;
			_context.Entry(product).State = EntityState.Modified;
			await _context.SaveChangesAsync();
			return NoContent();
		}
		[HttpDelete("{id:int}")]
		public async Task<IActionResult> Delete(int id)
		{
			
			if (!_context.Products.Any(p => p.ProductId == id))
			{
				return NotFound();
			}
			var product = await _context.Products.FindAsync(id);
			if (product == null)
			{
				return NotFound();
			}
			_context.Products.Remove(product);
			await _context.SaveChangesAsync();
			return NoContent();
		}
	}

	public class ProductMappers : Profile
	{
		public ProductMappers()
		{
			CreateMap<CreateProductRequest, Product>();
			CreateMap<Product, CreateProductRequest>();
			CreateMap<UpdateProductRequest, Product>();
			CreateMap<Product, UpdateProductRequest>();

		}
	}

	public class UpdateProductRequest
	{
		public string? ProductName { get; set; }
		public decimal? UnitPrice { get; set; }
		public int? UnitsInStock { get; set; }
		public string? Image { get; set; }
		public int? CategoryId { get; set; }
	}

	public class CreateProductRequest
	{
		public string? ProductName { get; set; }
		public decimal? UnitPrice { get; set; }
		public int? UnitsInStock { get; set; }
		public string? Image { get; set; }
		public int? CategoryId { get; set; }
	}
}
