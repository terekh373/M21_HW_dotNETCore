using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop_app.Models;
using Shop_app.Services;

namespace Shop_app.Controllers.API
{
    //http://localhost:5247/api/product
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class APIProductController : Controller
    {
        private readonly IServiceProducts _serviceProducts;
        public APIProductController(IServiceProducts serviceProducts)
        {
            _serviceProducts = serviceProducts;
        }
        [Authorize(Roles = "admin,moderator,user")]
        [HttpGet]
        public async Task<IActionResult> GetJsonAsync()
        {
            var products = await _serviceProducts.ReadAsync();
            if(products != null)
            {
                var result = products.Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.Description,
                    Image = p.ImageFile != null ? $"data:{p.ImageType};base64,{Convert.ToBase64String(p.ImageFile)}" : null
                });
                return Ok(result);
            }
            else
            {
                return NotFound(new { status = "404" });
            }
        }
        //http://localhost:5247/api/product/4
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator,user")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _serviceProducts.GetByIdAsync(id);
            if(product != null)
            {
                return Ok(product);
            }
            return NotFound(new { status = "404" });
        }
        //Method: POST 
        //http://localhost:5247/api/product
        //Request:
        //Body
        //{
        //  Id: 0,
        //  Name: "A new Product",
        //  Price: 25,50,
        //  Description: "Some description"
        //}
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            if(product != null)
            {
                var result = await _serviceProducts.CreateAsync(product);
                return Ok(product);
            }
            return BadRequest(new { state = "Product is null ..." });
        }
        //Method: PUT 
        //http://localhost:5247/api/product/4
        //Request:
        //Body
        //{
        //  Id: 4,
        //  Name: "The product for update",
        //  Price: 25,50,
        //  Description: "Some description"
        //}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
        {
            if (product != null)
            {
                var result = await _serviceProducts.UpdateAsync(id, product);
                return Ok(product);
            }
            return BadRequest(new { state = "Product is null ..." });
        }
        //Method: DELETE 
        //http://localhost:5247/api/product/4
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _serviceProducts.DeleteAsync(id);
            if(result)
            {
                return Ok(new { status = "Ok!" });
            }
            return BadRequest(new { status = "Bad!" });
        }
    }
}
