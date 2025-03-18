using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

[Authorize]
[Route("api/files")]
[ApiController]
public class FileController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;

    public FileController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    [HttpGet("{filename}")]
    public IActionResult ShowFile(string filename)
    {
        var filePath = Path.Combine(_environment.WebRootPath, "Images", filename);
        
        if (!System.IO.File.Exists(filePath))
        {
            return NotFound("File Not Found");
        }

        var mimeType = GetMimeType(filePath);
        var fileExtension = Path.GetExtension(filename);
        var customFileName = $"Image_{DateTime.UtcNow:yyyyMMddHHmmss}{fileExtension}";

        var fileBytes = System.IO.File.ReadAllBytes(filePath);
        return File(fileBytes, mimeType, customFileName);
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile image)
    {
        if (image == null || image.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        var uniqueFileName = Path.GetRandomFileName() + Path.GetExtension(image.FileName);
        var filePath = Path.Combine(_environment.WebRootPath, "Images", uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(stream);
        }

        var relativePath = $"Images/{uniqueFileName}";
        return Ok(new { imageUrl = relativePath });
    }

    private string GetMimeType(string filePath)
    {
        var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(filePath, out string mimeType))
        {
            mimeType = "application/octet-stream";
        }
        return mimeType;
    }
}
