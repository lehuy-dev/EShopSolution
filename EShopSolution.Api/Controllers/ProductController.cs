using EShopSolution.Application.Catalog.Products;
using EShopSolution.ViewModels.Catalog.ProductImages;
using EShopSolution.ViewModels.Catalog.Products;
using EShopSolution.ViewModels.Common;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class ProductController : ControllerBase
    {
        private IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging(GetManageProductPagingRequest request)
        {
            var data = await _productService.GetAllPaging(request);
            return Ok(data);
        }

        [HttpDelete("{productId}/{languageId}")]
        public async Task<IActionResult> GetById(int productId, string languageId)
        {
            var product = await _productService.GetById(productId, languageId);
            if(product.ResultObj == null)
            {
                BadRequest("Cannot find product");
            }
            return Ok(product.ResultObj);
        }

        [HttpGet("featured/{languageId}/{take}")]
        public async Task<IActionResult> GetFeaturedProducts(int take, string languageId)
        {
            var product = await _productService.GetFeaturedProducts(languageId, take);
            return Ok(product);
        }

        [HttpGet("latest/{languageId}/{take}")]
        public async Task<IActionResult> GetLatestProduct(int take, string languageId)
        {
            var products = await _productService.GetLatestProducts(languageId, take);
            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _productService.Create(request);
            if (result.ResultObj <= 0)
                return BadRequest();
            var product = await _productService.GetById(result.ResultObj, request.LanguageId);
            return CreatedAtAction(nameof(GetById), product);
        }

        [HttpPut("{productId}")]
        public async Task<IActionResult> Update([FromRoute] int productId, [FromForm] ProductUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            request.Id = productId;
            var affectedResult = await _productService.Update(request);
            if (affectedResult.ResultObj <= 0)
            {
                return BadRequest();
            }
            return Ok();
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> Delete(int productId)
        {
            var request = new ProductDeleteRequest();
            request.Id = productId;
            var affectedResult = await _productService.Delete(request);
            if(affectedResult.ResultObj <= 0)
            {
                return BadRequest();
            }
            return Ok();
        }

        [HttpPatch("{productId}/{newPrice}")]
        public async Task<IActionResult> UpdatePrice(int productId, int newPrice)
        {
            var result = await _productService.UpdatePice(productId, newPrice);
            if (result.ResultObj <= 0)
            {
                return BadRequest();
            }
            return Ok();
        }

        [HttpPost("{productId}/images")]
        public async Task<IActionResult> CreateImage(int productId, [FromForm] ProductImageCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _productService.AddImage(productId, request);
            if(result.ResultObj <= 0)
            {
                return BadRequest();
            }
            var productImage = await _productService.GetImageById(result.ResultObj);
            return CreatedAtAction(nameof(GetImageById), new { id = result.ResultObj}, productImage.ResultObj);
        }
        [HttpGet("{productId}/images/{imageId}")]
        public async Task<IActionResult> GetImageById(int productId, int imageId)
        {
            var image = await _productService.GetImageById(imageId);
            if(image == null)
            {
                return BadRequest("Cannt not find image");
            }
            return Ok(image);
        }

        [HttpPut("{productId}/images/{imageId}")]
        public async Task<IActionResult> UpdateImage(int imageId, [FromForm] ProductImageUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _productService.UpdateImage(imageId, request);
            if (result.ResultObj <= 0)
            {
                return BadRequest();
            }
            return Ok();

        }
        [HttpDelete("{productId}/images/{imageId}")]
        public async Task<IActionResult> RemoveImage(int imageId)
        {
            var result = await _productService.RemoveImage(imageId);
            if(result.ResultObj <= 0)
            {
                return BadRequest();
            }
            return Ok();
        }

        [HttpPut("{id}/categories")]
        public async Task<IActionResult> CategoryAssign(int id, [FromBody] CategoryAssignRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _productService.CategoryAssign(id, request);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
