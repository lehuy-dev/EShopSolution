using EShopSolution.Application.Common;
using EShopSolution.Data.EF;
using EShopSolution.Data.Entities;
using EShopSolution.Utilities.Contants;
using EShopSolution.ViewModels.Catalog.ProductImages;
using EShopSolution.ViewModels.Catalog.Products;
using EShopSolution.ViewModels.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShopSolution.Application.Catalog.Products
{
    public class ProductService : IProductService
    {
        private readonly EShopDbContext _dbContext;
        private IStorageService _storageService;
        public ProductService(EShopDbContext dbContext, IStorageService storageService)
        {
            _dbContext = dbContext;
            _storageService = storageService;
        }
        public async Task<ApiResult<int>> AddImage(int productId, ProductImageCreateRequest request)
        {
            try
            {
                using (var transaction = await _dbContext.Database.BeginTransactionAsync())
                {
                    if (!_dbContext.Products.Any(x => x.Id == productId))
                        return new ApiErrorResult<int>("Khong tim thay san pham co id = " + productId);
                    var productImage = new ProductImage()
                    {
                        Caption = request.Caption,
                        DateCreated = DateTime.Now,
                        IsDefault = request.IsDefault,
                        ProductId = productId,
                        SortOrder = request.SortOrder
                    };
                    if (request.ImageFile != null)
                    {
                        productImage.ImagePath = await _storageService.SaveFileAsync(request.ImageFile);
                        productImage.FileSize = request.ImageFile.Length;
                    }
                    await _dbContext.ProductImages.AddAsync(productImage);
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ApiSuccessResult<int>(productImage.Id);
                }
            }
            catch(Exception e)
            {
                return new ApiErrorResult<int>(e.Message);
            }
        }

        public async Task AddViewCount(int productId)
        {
            using(var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                var product = await _dbContext.Products.FindAsync(productId);
                product.ViewCount++;

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            
        }

        public async Task<ApiResult<bool>> CategoryAssign(int id, CategoryAssignRequest request)
        {
            try
            {
                using (var transaction = await _dbContext.Database.BeginTransactionAsync())
                {
                    var product = await _dbContext.Products.FindAsync(id);
                    if (product == null)
                    {
                        return new ApiErrorResult<bool>($"San pham voi {id} khong ton tai");
                    }
                    foreach (var category in request.Categories)
                    {
                        var productInCategory = await _dbContext.ProductInCategories
                            .FirstOrDefaultAsync(x => x.CategoryId == int.Parse(category.Id) && x.ProductId == id);
                        if (productInCategory != null && !category.Selected)
                        {
                            _dbContext.ProductInCategories.Remove(productInCategory);
                        }
                        else if (productInCategory == null && category.Selected)
                        {
                            await _dbContext.ProductInCategories.AddAsync(new ProductInCategory()
                            {
                                CategoryId = int.Parse(category.Id),
                                ProductId = id
                            });
                        }
                    }
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ApiSuccessResult<bool>();
                }
            }
            catch(Exception e)
            {
                return new ApiErrorResult<bool>(e.Message);
            }
        }

        public async Task<ApiResult<int>> Create(ProductCreateRequest request)
        {
            try
            {
                using (var transaction = await _dbContext.Database.BeginTransactionAsync())
                {
                    var languages = _dbContext.Languages;
                    var translations = new List<ProductTranslation>();
                    foreach (var language in languages)
                    {
                        if (language.Id == request.LanguageId)
                        {
                            translations.Add(new ProductTranslation()
                            {
                                Name = request.Name,
                                Description = request.Description,
                                Details = request.Details,
                                SeoDescription = request.SeoDescription,
                                SeoAlias = request.SeoAlias,
                                SeoPageTitle = request.SeoTitle,
                                LanguageId = request.LanguageId
                            });
                        }
                        else
                        {
                            translations.Add(new ProductTranslation()
                            {
                                Name = SystemContants.ProductConstants.NA,
                                Description = SystemContants.ProductConstants.NA,
                                SeoAlias = SystemContants.ProductConstants.NA,
                                LanguageId = language.Id
                            });
                        }
                    }
                    var product = new Product()
                    {
                        Price = request.Price,
                        OriginalPrice = request.OriginalPrice,
                        Stock = request.Stock,
                        ViewCount = 0,
                        DateCreated = DateTime.Now,
                        ProductTranslations = translations
                    };
                    if (request.ThumbnailImage != null)
                    {
                        product.ProductImages = new List<ProductImage>()
                        {
                            new ProductImage()
                            {
                                Caption = "Thumbnail image",
                                DateCreated = DateTime.Now,
                                FileSize = request.ThumbnailImage.Length,
                                ImagePath = await _storageService.SaveFileAsync(request.ThumbnailImage),
                                IsDefault = true,
                                SortOrder = 1
                            }
                        };
                    }
                    await _dbContext.Products.AddAsync(product);
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ApiSuccessResult<int>(product.Id);
                }
            }
            catch(Exception e)
            {
                return new ApiErrorResult<int>(e.Message);
            }

        }

        public async Task<ApiResult<int>> Delete(ProductDeleteRequest request)
        {
            try
            {
                using (var transaction = await _dbContext.Database.BeginTransactionAsync())
                {
                    var product = await _dbContext.Products.FindAsync(request.Id);
                    if (product == null)
                        return new ApiErrorResult<int>("Khong tim thay san pham co id = " + request.Id);

                    //find adn delete product images 
                    var productImages = _dbContext.ProductImages.Where(x => x.ProductId == request.Id);
                    foreach (var image in productImages)
                    {
                        await _storageService.DeleteFileAsync(image.ImagePath);
                    }
                    _dbContext.ProductImages.RemoveRange(productImages);

                    //delete product category
                    var productCategories = _dbContext.ProductInCategories.Where(x => x.ProductId == request.Id);
                    _dbContext.RemoveRange(productCategories);

                    //remove product
                    _dbContext.Products.Remove(product);
                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return new ApiSuccessResult<int>();
                }
            }
            catch(Exception e)
            {
                return new ApiErrorResult<int>(e.Message);
            }
        }

        public async Task<ApiResult<PagedResult<ProductVm>>> GetAllByCategoryId(string languageId, GetPublicProductPagingRequest request)
        {
            try
            {
                if (! await _dbContext.Languages.AnyAsync(x => x.Id == languageId))
                    return new ApiErrorResult<PagedResult<ProductVm>>("Khong tim thay language co id = " + languageId);
                if (!await _dbContext.Categories.AnyAsync(x => x.Id == request.CategoryId) && request.CategoryId>0)
                    return new ApiErrorResult<PagedResult<ProductVm>>("Khong tim thay category co id = " + request.CategoryId);
                var query = from p in _dbContext.Products
                            join pc in _dbContext.ProductInCategories on p.Id equals pc.ProductId
                            join pt in _dbContext.ProductTranslations on p.Id equals pt.ProductId
                            where pt.LanguageId == languageId && pc.CategoryId == request.CategoryId
                            select (new {p,pc,pt });
                var data = await query.Select(x => new ProductVm()
                {
                    Id = x.p.Id,
                    Name = x.pt.Name,
                    DateCreated = x.p.DateCreated,
                    Description = x.pt.Description,
                    Details = x.pt.Details,
                    LanguageId = x.pt.LanguageId,
                    OriginalPrice = x.p.OriginalPrice,
                    Price = x.p.Price,
                    SeoAlias = x.pt.SeoAlias,
                    SeoDescription = x.pt.SeoDescription,
                    SeoTitle = x.pt.SeoPageTitle,
                    Stock = x.p.Stock,
                    ViewCount = x.p.ViewCount
                }).ToListAsync();

                var items = data.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize).ToList();
                var pagedResult = new PagedResult<ProductVm>()
                {
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize,
                    TotalRecords = data.Count(),
                    Items = items
                };
                return new ApiSuccessResult<PagedResult<ProductVm>>(pagedResult);
            }
            catch(Exception e)
            {
                return new ApiErrorResult<PagedResult<ProductVm>>(e.Message);
            }
        }

        public async Task<ApiResult<PagedResult<ProductVm>>> GetAllPaging(GetManageProductPagingRequest request)
        {
            try
            {
                if (!await _dbContext.Languages.AnyAsync(x => x.Id == request.LanguageId))
                    return new ApiErrorResult<PagedResult<ProductVm>>("Khong tim thay language co id = " + request.LanguageId);
                if (!await _dbContext.Categories.AnyAsync(x => x.Id == request.CategoryId) && request.CategoryId > 0)
                    return new ApiErrorResult<PagedResult<ProductVm>>("Khong tim thay category co id = " + request.CategoryId);

                var query = from p in _dbContext.Products
                            join pt in _dbContext.ProductTranslations on p.Id equals pt.ProductId
                            join pc in _dbContext.ProductInCategories on p.Id equals pc.ProductId
                            where pt.LanguageId == request.LanguageId && pc.CategoryId == request.CategoryId
                            select (new { p, pt, pc });
                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    query = query.Where(x => x.pt.Name.Contains(request.Keyword));
                }
                var data = await query.Select(x => new ProductVm()
                {
                    Id = x.p.Id,
                    Name = x.pt.Name,
                    DateCreated = x.p.DateCreated,
                    Description = x.pt.Description,
                    Details = x.pt.Details,
                    LanguageId = x.pt.LanguageId,
                    OriginalPrice = x.p.OriginalPrice,
                    Price = x.p.Price,
                    SeoAlias = x.pt.SeoAlias,
                    SeoDescription = x.pt.SeoDescription,
                    SeoTitle = x.pt.SeoPageTitle,
                    Stock = x.p.Stock,
                    ViewCount = x.p.ViewCount
                }).ToListAsync();

                var items = data.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize).ToList();
                var pagedResult = new PagedResult<ProductVm>()
                {
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize,
                    TotalRecords = data.Count(),
                    Items = items
                };
                return new ApiSuccessResult<PagedResult<ProductVm>>(pagedResult);
            }
            catch(Exception e)
            {
                return new ApiErrorResult<PagedResult<ProductVm>>(e.Message);
            }
        }

        public async Task<ApiResult<ProductVm>> GetById(int productId, string languageId)
        {
            try
            {
                var product = await _dbContext.Products.FindAsync(productId);
                if (product == null)
                    return new ApiErrorResult<ProductVm>("Khong tim thay product co id = "+productId);
                if (!await _dbContext.Languages.AnyAsync(x => x.Id == languageId))
                    return new ApiErrorResult<ProductVm>("Khong tim thay language co id = " + languageId);
                var productTransalation = await _dbContext.ProductTranslations.
                    FirstOrDefaultAsync(x => x.ProductId == productId && x.LanguageId == languageId);

                var categories = await (from c in _dbContext.Categories
                                        join pc in _dbContext.ProductInCategories on c.Id equals pc.CategoryId
                                        join ct in _dbContext.CategoryTranslations on c.Id equals ct.CategoryId
                                        where pc.ProductId == productId && ct.LanguageId == languageId
                                        select c.Name).ToListAsync();
                var image = await _dbContext.ProductImages.Where(x => x.ProductId == productId && x.IsDefault == true).FirstOrDefaultAsync();
                var productViewModel = new ProductVm()
                {
                    Id = productId,
                    DateCreated = product.DateCreated,
                    Description = productTransalation.Description,
                    LanguageId = languageId,
                    Details = productTransalation.Details,
                    Name = productTransalation.Name,
                    OriginalPrice = product.OriginalPrice,
                    Price = product.Price,
                    SeoAlias = productTransalation.SeoAlias,
                    SeoDescription = productTransalation.SeoDescription,
                    SeoTitle = productTransalation.SeoPageTitle,
                    Stock = product.Stock,
                    ViewCount = product.ViewCount,
                    Categories = categories,
                    ThumbnailImage = image != null ? image.ImagePath : "no-image.jpg"

                };
                return new ApiSuccessResult<ProductVm>(productViewModel);
            }
            catch(Exception e)
            {
                return new ApiErrorResult<ProductVm>(e.Message);
            }
        }

        public async Task<ApiResult<List<ProductVm>>> GetFeaturedProducts(string languageId, int take)
        {
            try
            {
                if (!await _dbContext.Languages.AnyAsync(x => x.Id == languageId))
                    return new ApiErrorResult<List<ProductVm>>("Khong tim thay language co id = " + languageId);
                var query = from p in _dbContext.Products
                            join pc in _dbContext.ProductInCategories on p.Id equals pc.ProductId
                            join pt in _dbContext.ProductTranslations on p.Id equals pt.ProductId
                            join pi in _dbContext.ProductImages on p.Id equals pi.ProductId
                            where pt.LanguageId == languageId && (pi == null || pi.IsDefault == true) && p.IsFeature == true
                            select (new { p, pc, pt, pi });
                var data = await query.OrderByDescending(x => x.p.DateCreated).Take(take)
               .Select(x => new ProductVm()
               {
                   Id = x.p.Id,
                   Name = x.pt.Name,
                   DateCreated = x.p.DateCreated,
                   Description = x.pt.Description,
                   Details = x.pt.Details,
                   LanguageId = x.pt.LanguageId,
                   OriginalPrice = x.p.OriginalPrice,
                   Price = x.p.Price,
                   SeoAlias = x.pt.SeoAlias,
                   SeoDescription = x.pt.SeoDescription,
                   SeoTitle = x.pt.SeoPageTitle,
                   Stock = x.p.Stock,
                   ViewCount = x.p.ViewCount,
                   ThumbnailImage = x.pi.ImagePath
               }).ToListAsync();

                return new ApiSuccessResult<List<ProductVm>>(data);
            }
            catch (Exception e)
            {
                return new ApiErrorResult<List<ProductVm>>(e.Message);
            }
        }

        public async Task<ApiResult<ProductImageViewModel>> GetImageById(int imageId)
        {
            try
            {
                var image = await _dbContext.ProductImages.FindAsync(imageId);
                if (image == null)
                    return new ApiErrorResult<ProductImageViewModel>("Khong tim thay image co id = "+imageId);
                var viewModel = new ProductImageViewModel()
                {
                    Caption = image.Caption,
                    DateCreated = image.DateCreated,
                    FileSize = image.FileSize,
                    Id = image.Id,
                    ImagePath = image.ImagePath,
                    IsDefault = image.IsDefault,
                    ProductId = image.ProductId,
                    SortOrder = image.SortOrder
                };
                return new ApiSuccessResult<ProductImageViewModel>(viewModel);
            }
            catch(Exception e)
            {
                return new ApiErrorResult<ProductImageViewModel>(e.Message);
            }
        }

        public async Task<ApiResult<List<ProductVm>>> GetLatestProducts(string languageId, int take)
        {
            try
            {
                var query = from p in _dbContext.Products
                            join pt in _dbContext.ProductTranslations on p.Id equals pt.ProductId
                            join pic in _dbContext.ProductInCategories on p.Id equals pic.ProductId into ppic
                            from pic in ppic.DefaultIfEmpty()
                            join pi in _dbContext.ProductImages on p.Id equals pi.ProductId into ppi
                            from pi in ppi.DefaultIfEmpty()
                            join c in _dbContext.Categories on pic.CategoryId equals c.Id into picc
                            from c in picc.DefaultIfEmpty()
                            where pt.LanguageId == languageId && (pi == null || pi.IsDefault == true)
                            select new { p, pt, pic, pi };

                var data = await query.OrderByDescending(x => x.p.DateCreated).Take(take)
                    .Select(x => new ProductVm()
                    {
                        Id = x.p.Id,
                        Name = x.pt.Name,
                        DateCreated = x.p.DateCreated,
                        Description = x.pt.Description,
                        Details = x.pt.Details,
                        LanguageId = x.pt.LanguageId,
                        OriginalPrice = x.p.OriginalPrice,
                        Price = x.p.Price,
                        SeoAlias = x.pt.SeoAlias,
                        SeoDescription = x.pt.SeoDescription,
                        SeoTitle = x.pt.SeoPageTitle,
                        Stock = x.p.Stock,
                        ViewCount = x.p.ViewCount,
                        ThumbnailImage = x.pi.ImagePath
                    }).ToListAsync();
                return new ApiSuccessResult<List<ProductVm>>(data);
            }
            catch (Exception e)
            {
                return new ApiErrorResult<List<ProductVm>>(e.Message);
            }
        }

        public async Task<ApiResult<List<ProductImageViewModel>>> GetListImages(int productId)
        {
            try
            {
                if (!await _dbContext.Products.AnyAsync(x => x.Id == productId))
                    return new ApiErrorResult<List<ProductImageViewModel>>("Khong tim thay product co id = " + productId);
                var result = await _dbContext.ProductImages.Where(x => x.ProductId == productId)
                    .Select(i => new ProductImageViewModel()
                    {
                        Caption = i.Caption,
                        DateCreated = i.DateCreated,
                        FileSize = i.FileSize,
                        Id = i.Id,
                        ImagePath = i.ImagePath,
                        IsDefault = i.IsDefault,
                        ProductId = i.ProductId,
                        SortOrder = i.SortOrder
                    }).ToListAsync();
                return new ApiSuccessResult<List<ProductImageViewModel>>(result);
            }
            catch(Exception e)
            {
                return new ApiErrorResult<List<ProductImageViewModel>>(e.Message);
            }
        }

        public async Task<ApiResult<int>> RemoveImage(int imageId)
        {
            try
            {
                var productImage = await _dbContext.ProductImages.FindAsync(imageId);
                if (productImage == null)
                    return new ApiErrorResult<int>("Khong tim thay image co id = " + imageId);
                _dbContext.ProductImages.Remove(productImage);
                await _dbContext.SaveChangesAsync();
                return new ApiSuccessResult<int>();
            }
            catch(Exception e)
            {
                return new ApiErrorResult<int>(e.Message);
            }
        }

        public async Task<ApiResult<int>> Update(ProductUpdateRequest request)
        {
            try
            {
                using (var transaction = await _dbContext.Database.BeginTransactionAsync())
                {
                    if (!await _dbContext.Products.AnyAsync(x => x.Id == request.Id))
                        return new ApiErrorResult<int>("Khong tim thay product co id = " + request.Id);
                    var productTranslations = await _dbContext.ProductTranslations
                        .FirstOrDefaultAsync(x => x.Id == request.Id && x.LanguageId == request.LanguageId);

                    productTranslations.Name = request.Name;
                    productTranslations.SeoAlias = request.SeoAlias;
                    productTranslations.SeoDescription = request.SeoDescription;
                    productTranslations.SeoPageTitle = request.SeoTitle;
                    productTranslations.Description = request.Description;
                    productTranslations.Details = request.Details;

                    if (request.ThumbnailImage != null)
                    {
                        var productImage = await _dbContext.ProductImages
                            .FirstOrDefaultAsync(x => x.IsDefault == true && x.ProductId == request.Id);
                        if (productImage != null)
                        {
                            productImage.FileSize = request.ThumbnailImage.Length;
                            productImage.ImagePath = await _storageService.SaveFileAsync(request.ThumbnailImage);
                            _dbContext.ProductImages.Update(productImage);
                        }
                    }
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                return new ApiSuccessResult<int>();
            }
            catch (Exception e)
            {
                return new ApiErrorResult<int>(e.Message);
            }
        }

        public async Task<ApiResult<int>> UpdateImage(int imageId, ProductImageUpdateRequest request)
        {
            try
            {
                if (!await _dbContext.ProductImages.AnyAsync(x => x.Id == imageId))
                    return new ApiErrorResult<int>("Khong tim thay image co id = "+ imageId);
                
                var productImage = await _dbContext.ProductImages.FindAsync(imageId);
                if (request.ImageFile != null)
                {
                    productImage.ImagePath = await _storageService.SaveFileAsync(request.ImageFile);
                    productImage.FileSize = request.ImageFile.Length;
                }
                _dbContext.ProductImages.Update(productImage);
                await _dbContext.SaveChangesAsync();
                return new ApiSuccessResult<int>();
            }
            catch(Exception e)
            {
                return new ApiErrorResult<int>(e.Message);
            }
        }

        public async Task<ApiResult<int>> UpdatePice(int productId, decimal newPrice)
        {
            try
            {
                var product = await _dbContext.Products.FindAsync(productId);
                if (product == null)
                    return new ApiErrorResult<int>("Khong tim thay product co id = " + productId);
                product.Price = newPrice;
                _dbContext.Update(product);
                await _dbContext.SaveChangesAsync();
                return new ApiSuccessResult<int>();
            }
            catch(Exception e)
            {
                return new ApiErrorResult<int>(e.Message);
            }

        }

        public async Task<ApiResult<bool>> UpdateStock(int productId, int addedQuantity)
        {
            try
            {
                var product = await _dbContext.Products.FindAsync(productId);
                if (product == null)
                    return new ApiErrorResult<bool>("Khong tim thay product co id = " + productId);
                product.Stock += addedQuantity;
                _dbContext.Products.Update(product);
                await _dbContext.SaveChangesAsync();
                return new ApiSuccessResult<bool>();
            }catch(Exception e)
            {
                return new ApiErrorResult<bool>(e.Message);
            }
        }
    }
}
