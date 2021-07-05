using EShopSolution.ViewModels.Catalog.ProductImages;
using EShopSolution.ViewModels.Catalog.Products;
using EShopSolution.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShopSolution.Application.Catalog.Products
{
    public interface IProductService
    {
        Task<ApiResult<int>> Create(ProductCreateRequest request);
        Task<ApiResult<int>> Update(ProductUpdateRequest request);
        Task<ApiResult<int>> Delete(ProductDeleteRequest request);
        Task<ApiResult<ProductVm>> GetById(int productId, string languageId);
        Task<ApiResult<int>> UpdatePice(int productId, decimal newPrice);
        Task<ApiResult<bool>> UpdateStock(int productId, int addedQuantity);
        Task AddViewCount(int productId);
        Task<ApiResult<PagedResult<ProductVm>>> GetAllPaging(GetManageProductPagingRequest request);
        Task<ApiResult<int>> AddImage(int productId, ProductImageCreateRequest request);
        Task<ApiResult<int>> RemoveImage(int imageId);
        Task<ApiResult<int>> UpdateImage(int imageId, ProductImageUpdateRequest request);
        Task<ApiResult<ProductImageViewModel>> GetImageById(int imageId);
        Task<ApiResult<List<ProductImageViewModel>>> GetListImages(int productId);
        Task<ApiResult<PagedResult<ProductVm>>> GetAllByCategoryId(string languageId, GetPublicProductPagingRequest request);
        Task<ApiResult<bool>> CategoryAssign(int id, CategoryAssignRequest request);
        Task<ApiResult<List<ProductVm>>> GetFeaturedProducts(string languageId, int take);

        Task<ApiResult<List<ProductVm>>> GetLatestProducts(string languageId, int take);

    }
}
