using AutoMapper;
using JSClientDemo.Controllers;
using JSClientDemo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CategoriesController : ControllerBase
	{
		private readonly MySaleDBContext _context;
		private readonly IMapper _mapper;

		public CategoriesController(MySaleDBContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<IActionResult> GetList()
		{
			return Ok(await _context.Categories.ToListAsync());
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetById(int id)
		{
			var category = await _context.Categories
				.SingleOrDefaultAsync(c => c.CategoryId == id);
			if (category == null)
			{
				return NotFound();
			}
			 
			return Ok(category);
		}

		[HttpPost]
		public async Task<IActionResult> Create(CreateCategoryRequest categoryRequest)
		{
			if (categoryRequest == null)
			{
				return BadRequest();
			}
			Category category = _mapper.Map<Category>(categoryRequest);

			_context.Categories.Add(category);
			await _context.SaveChangesAsync();
			return CreatedAtAction(nameof(GetById), new { id = category.CategoryId }, category);
		}

		[HttpPut("{id:int}")]
		public async Task<IActionResult> Update(int id, UpdateCateogoryRequest categoryRequest)
		{
			if (categoryRequest == null)
			{
				return BadRequest();
			}
			if (!_context.Categories.Any(c => c.CategoryId == id))
			{
				return NotFound();
			}

			Category category = _mapper.Map<Category>(categoryRequest);
			category.CategoryId = id;
			_context.Entry(category).State = EntityState.Modified;
			await _context.SaveChangesAsync();
			return NoContent();
		}

		[HttpDelete("{id:int}")]
		public async Task<IActionResult> Delete(int id)
		{

			if (!_context.Categories.Any(c => c.CategoryId == id))
			{
				return NotFound();
			}
			var category = await _context.Categories.FindAsync(id);
			if (category == null)
			{
				return NotFound();
			}
			_context.Categories.Remove(category);
			await _context.SaveChangesAsync();
			return NoContent();
		}
	}

	public class CategoryMappers : Profile
	{
		public CategoryMappers()
		{
			CreateMap<CreateCategoryRequest, Category>();
			CreateMap<Category, CreateCategoryRequest>();
			CreateMap<UpdateCateogoryRequest, Category>();
			CreateMap<Category, UpdateCateogoryRequest>();

		}
	}


	public class UpdateCateogoryRequest
	{
		public string? CategoryName { get; set; }
	}

	public class CreateCategoryRequest
	{
		public string? CategoryName { get; set; }
	}
}
