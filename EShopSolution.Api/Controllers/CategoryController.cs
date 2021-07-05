using EShopSolution.Application.Catalog.Categories;
using EShopSolution.ViewModels.Catalog.Categories;
using EShopSolution.ViewModels.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EShopSolution.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        // GET: api/<CategoryController>
        [HttpGet]
        public async Task<Object> Get([FromQuery] string languageId)
        {
            return await _categoryService.GetAll(languageId);
        }

        // GET api/<CategoryController>/5
        //[HttpGet("{languageId}/{id}")]
        [HttpGet("/getById")]
        public async Task<ApiResult<CategoryVm>> Get([FromQuery] string languageId , [FromQuery] int id)
        {
            return await _categoryService.GetById(languageId, id);
        }

        // POST api/<CategoryController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<CategoryController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CategoryController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
