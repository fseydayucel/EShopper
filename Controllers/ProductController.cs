using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinalProjectMvc.Data;
using FinalProjectMvc.Models;
using Microsoft.AspNetCore.Authorization;

namespace FinalProjectMvc.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Product
        public async Task<IActionResult> Index(string searchName, string sizeFilter, string colorFilter, string sortOrder)
        {
            // Load into memory first so we can use string.Split safely
            var products = await _context.Products
                .Include(p => p.Reviews)
                .ToListAsync();

            // Filter by name
            if (!string.IsNullOrEmpty(searchName))
            {
                products = products
                    .Where(p => p.Name != null && p.Name.Contains(searchName, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Filter by size
            if (!string.IsNullOrEmpty(sizeFilter))
            {
                products = products
                    .Where(p => p.AvailableSizes
                        .Contains(sizeFilter, StringComparer.OrdinalIgnoreCase))
                    .ToList();
            }

            // Filter by color
            if (!string.IsNullOrEmpty(colorFilter))
            {
                products = products
                    .Where(p => p.AvailableColors
                        .Contains(colorFilter, StringComparer.OrdinalIgnoreCase))
                    .ToList();
            }

            // Sorting
            products = sortOrder switch
            {
                "price_asc" => products.OrderBy(p => p.Price).ToList(),
                "price_desc" => products.OrderByDescending(p => p.Price).ToList(),
                "rating_asc" => products.OrderBy(p => p.Rating).ToList(),
                "rating_desc" => products.OrderByDescending(p => p.Rating).ToList(),
                _ => products.OrderBy(p => p.Name).ToList()
            };

            return View(products);
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            ViewBag.AllSizes = new[] { "XS", "S", "M", "L", "XL" };
            ViewBag.AllColors = new[] { "Black", "Blue", "Gray", "Green", "Red", "White", "Yellow" };
            var model = new Product
            {
                AvailableSizes = new List<string>(),
                AvailableColors = new List<string>()
            };
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, string[] SelectedSizes, string[] SelectedColors)
        {
            product.AvailableSizes = SelectedSizes?.ToList() ?? new List<string>();
            product.AvailableColors = SelectedColors?.ToList() ?? new List<string>();

            ModelState.Remove(nameof(Product.AvailableSizes));
            ModelState.Remove(nameof(Product.AvailableColors));

            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.AllSizes = new[] { "XS", "S", "M", "L", "XL" };
            ViewBag.AllColors = new[] { "Black", "Blue", "Gray", "Green", "Red", "White", "Yellow" };
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file, int productId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            Directory.CreateDirectory(uploadsFolder);

            var extension = Path.GetExtension(file.FileName);
            var fileName = $"img_{productId}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Overwrite if exists
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = $"/uploads/{fileName}";
            return Content(relativePath);
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.AllSizes = new[] { "XS", "S", "M", "L", "XL" };
            ViewBag.AllColors = new[] { "Black", "Blue", "Gray", "Green", "Red", "White", "Yellow" };
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, string[] SelectedSizes, string[] SelectedColors)
        {
            if (id != product.Id)
                return NotFound();

            product.AvailableSizes = SelectedSizes.ToList();
            product.AvailableColors = SelectedColors.ToList();
            ModelState.Remove(nameof(Product.AvailableSizes));
            ModelState.Remove(nameof(Product.AvailableColors));

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
