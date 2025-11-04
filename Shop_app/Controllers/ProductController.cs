using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop_app.Models;
using Shop_app.Services;
using System.Threading.Tasks;

namespace Shop_app.Controllers
{
    public class ProductController : Controller
    {
        private readonly IServiceProducts _serviceProducts;
        public ProductController(IServiceProducts serviceProducts)
        {
            _serviceProducts = serviceProducts;
        }

        [HttpGet]
        public async Task<ViewResult> Index()
        {
            var products = await _serviceProducts.ReadAsync();
            return View(products);
        }

        //Will add CRUD operations
        [Authorize(Roles = "moderator,admin")]
        [HttpGet] //https://localhost:port/product/create
        public ViewResult Create() => View();

        [Authorize(Roles = "moderator,admin")]
        [HttpPost]
        [ValidateAntiForgeryToken] // Validate the anti-forgery token for security
        // POST: http://localhost:[port]/products/create
        // Handle product creation form submission
        //Price is must have , => example 12,50
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Description,ImageData")] Product product)
        {
            if (ModelState.IsValid) // Check if the form data is valid
            {
                //Product photo
                using (var ms = new MemoryStream())
                {
                    await product.ImageData.CopyToAsync(ms);
                    product.ImageFile = ms.ToArray();
                }
                //image/png
                product.ImageType = product.ImageData.ContentType; //format
                await _serviceProducts.CreateAsync(product); // Create the product asynchronously
                return RedirectToAction(nameof(Index)); // Redirect to the product list
            }
            return NotFound(); // If validation fails, return to the form with the entered data
        }

        [Authorize(Roles = "moderator,admin")]
        [HttpGet]
        public async Task<ViewResult> Update(int id)
        {
            var product = await _serviceProducts.GetByIdAsync(id);
            return View(product);
        }

        [Authorize(Roles = "moderator,admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, [Bind("Id,Name,Price,Description,ImageData")] Product product)
        {
            //Додати перевірку для Price
            //Щоб працювало з комою та крапкою та без коми і крапки

            Console.WriteLine(product);
            if (ModelState.IsValid)
            {
                //Product photo
                using (var ms = new MemoryStream())
                {
                    await product.ImageData.CopyToAsync(ms);
                    product.ImageFile = ms.ToArray();
                }
                //image/png
                product.ImageType = product.ImageData.ContentType; //format
                await _serviceProducts.UpdateAsync(id, product);
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }

        [Authorize(Roles = "moderator,admin")]
        [HttpGet]
        // GET: http://localhost:[port]/products/delete
        // Display the product delete confirmation form
        public async Task<ViewResult> Delete()
        {
            return View();
        }

        [Authorize(Roles = "moderator,admin")]
        [HttpPost]
        // POST: http://localhost:[port]/products/delete/{id}
        // Handle product deletion
        public async Task<IActionResult> Delete(int id)
        {
            bool result = await _serviceProducts.DeleteAsync(id); // Delete the product asynchronously
            if(result)
            {
                return RedirectToAction(nameof(Index)); // Redirect to the product list
            }
            return NotFound();
        }
        // GET: http://localhost:[port]/products/details/{id}
        // Displays details of a single product by ID
        public async Task<ViewResult> Details(int id)
        {
            var product = await _serviceProducts.GetByIdAsync(id); // Retrieve product by ID asynchronously
            return View(product); // Return the product details to the view
        }

    }
}
