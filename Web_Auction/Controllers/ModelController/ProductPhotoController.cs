using System.Net.Http.Headers;
using Contracts.Models;
using Entities;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web_Auction.Controllers.ModelController;

[ApiController]
[Route("api/[controller]")]
public class ProductPhotoController : ControllerBase
{
    private readonly ILogger<ProductPhotoController> _logger;
    private readonly MasterDbContext _dbContext;
    private readonly IProductPhotoRepository _repository;

    public ProductPhotoController(ILogger<ProductPhotoController> logger,
        MasterDbContext dbContext, IProductPhotoRepository repository)
    {
        _logger = logger;
        _dbContext = dbContext;
        _repository = repository;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetPhotoByProductId(string id)
    {
        try
        {
            var productPhotos = await _repository.GetProductPhotos(id);
            _logger.LogDebug("Return all photos by productId : {Id}", id);
            return Ok(productPhotos);
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong inside GetProductPhoto action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }

    [HttpPost("upload"), DisableRequestSizeLimit]
    public async Task<IActionResult> UploadPhoto([FromQuery] string id)
    {
        try
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.ProductId.ToString() == id);
            if (product == null)
            {
                throw new Exception("Product not found.");
            }


            var formCollection = await Request.ReadFormAsync();
            var files = formCollection.Files;
            var folderName = Path.Combine("Resources", "Product");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            if (files.Any(f => f.Length <= 0))
                return BadRequest(new Response()
                {
                    Title = "Invalid Upload",
                    Message = "No files found"
                });
            foreach (var file in files)
            {
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName?.Trim('"');
                if (_dbContext.ProductPhotos.Where(p => p.Product.ProductId.ToString() == id).Select(x => x.FileName)
                    .Contains(fileName))
                {
                    throw new Exception("FileName already exists.");
                }

                var fullPath = Path.Combine(pathToSave, fileName);
                await using var stream = new FileStream(fullPath, FileMode.Create);
                await file.CopyToAsync(stream);

                var productPhoto = new ProductPhoto()
                {
                    FileName = fileName,
                    Product = product
                };

                await _dbContext.ProductPhotos.AddAsync(productPhoto);
                await _dbContext.SaveChangesAsync();
                _logger.LogDebug("Photo upload successfully");
            }

            return Ok(new Response()
            {
                Title = "Success",
                Message = "All files have been uploaded successfully"
            });
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong inside UploadPhoto action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }

    [HttpDelete("delete/{id}")]
    public async Task<ActionResult> DeletePhoto(string id)
    {
        try
        {
            await _repository.DeletePhotoAsync(id);
            _logger.LogDebug("Photo has id : {Id} deleted successfully", id);
            return Ok(new Response()
            {
                Title = "Successfully",
                Message = "Photo deleted"
            });
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong inside DeletePhoto action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }
}