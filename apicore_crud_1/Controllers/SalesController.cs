using System.Net.Http.Headers;
using apicore_crud_1.DTO;
using apicore_crud_1.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace apicore_crud_1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("R61")]
    public class SalesController : ControllerBase
    {
        private readonly ModelContext db;
        IWebHostEnvironment webHost;
        public SalesController(ModelContext db, IWebHostEnvironment webHost)
        {
            this.db = db;
            this.webHost = webHost;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var orders = await db.Orders
                                   .Include(a => a.Details)
                                   .ThenInclude(d => d.Product)
                                   .ToListAsync();

            var orderDtos = orders.Select(p => new Order
            {
                Id = p.Id,
                CustomerName = p.CustomerName,
                OrderDate = p.OrderDate,
                Picture = p.Picture,
                IsDelivered = p.IsDelivered,
                Details = p.Details.Select(a => new Detail
                {
                    Id = a.Id,                 
                    Price = a.Price,
                    OrderId=a.OrderId,
                    ProductId = a.ProductId
                }).ToList()
            }).ToList();

            return Ok(orderDtos);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrderById(int id)
        {
            var p = await db.Orders.Include(p => p.Details).ThenInclude(a => a.Product).FirstOrDefaultAsync(p => p.Id == id);
            if (p == null) return NotFound();
            var data = new Order
            {
                Id = p.Id,
                CustomerName = p.CustomerName,
                OrderDate = p.OrderDate,
                Picture = p.Picture,
                IsDelivered = p.IsDelivered,
                Details = p.Details.Select(a => new Detail
                {
                    Id = a.Id,
                    OrderId = a.OrderId,
                    Price = a.Price,
                    ProductId = a.ProductId
                }).ToList()
            };

            return Ok(data);
        }

        [HttpPost("UploadAjax")]
        public IActionResult UploadAjax()
        {
            try { 
            if (ModelState.IsValid)
            {
                var imageHeader = Request.Headers["Image-Data"];
                var p = HttpContext.Request.Form["orderdata"];
                var entity = JsonConvert.DeserializeObject<orderDTO>(p);
                if (!string.IsNullOrEmpty(imageHeader))
                {
                    var imageData = Convert.FromBase64String(imageHeader);
                    var rootfolder = webHost.WebRootPath;
                    var filePath = Path.Combine(rootfolder, "Pictures", entity.Order.CustomerName + ".png");// "uploadedImage.png"
                    System.IO.File.WriteAllBytes(filePath, imageData);
                    var insertModel = new Order
                    {
                        CustomerName =entity.Order.CustomerName,
                        Picture = "~/Pictures/" + entity.Order.CustomerName + ".png",
                         
                         Details = entity.Order.Details,
                        IsDelivered = entity.Order.IsDelivered,
                        OrderDate = entity.Order.OrderDate
                    };
                    db.Orders.Add(insertModel);
                    int x = db.SaveChanges();
                    if (x > 0)
                    {
                        return Ok(new { Success = true, Message = "Data Successfully Added" });
                    }
                    else
                    {
                        return Problem("Data Save failed");
                    }
                }
                else
                {
                    return Problem( "Please Provide valid Image" );
                }
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return Problem(message);
            }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Post()
        {           
            var requestedFile = HttpContext.Request.Form.Files[0];            
            var p = HttpContext.Request.Form["orderdata"];
            var entity = JsonConvert.DeserializeObject<Order>(p); 
            try
            {

                if (requestedFile != null)
                {
                    string ext = Path.GetExtension(requestedFile.FileName);
                    string fileName = entity.CustomerName + ext;
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Pictures", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        requestedFile.CopyTo(stream);
                    }
                    var order = new Order
                    {
                        CustomerName = entity.CustomerName,
                        OrderDate = entity.OrderDate,
                        IsDelivered = entity.IsDelivered,
                        Details = entity.Details,
                        Picture = "/Pictures/" + fileName

                    };
                    db.Orders.Add(order);
                    db.SaveChanges();

                }

                return Ok();
                
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [Route("Upload")]
        [HttpPost, DisableRequestSizeLimit]
        public IActionResult UploadPic()
        {
            try
            {
                var file = Request.Form.Files[0];
                var MemberId = Request.Form["order"];
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), "Pictures");
                if (file.Length > 0)
                {
                    //logo
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var ext = Path.GetExtension(fileName).ToLower();
                    var fullPath = Path.Combine(pathToSave, MemberId + ext);

                    if (ext == ".jpg" || ext == ".png" || ext == ".jpeg")
                    {

                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                        return Ok();
                    }
                    else
                    {
                        return BadRequest();
                    }
                }

                else
                {
                    return Problem("File Missing");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

    }
}
