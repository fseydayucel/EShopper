using FinalProjectMvc.Data;
using FinalProjectMvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProjectMvc.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var outOfStock = await _context.Products
                .FromSqlRaw("SELECT * FROM Products WHERE StockQuantity = 0")
                .ToListAsync();

            var noReviews = await _context.Products
                .FromSqlRaw(@"
                    SELECT *
                    FROM Products
                    WHERE Id NOT IN (
                        SELECT DISTINCT ProductId FROM Reviews
                    )")
                .ToListAsync();

            var mostReviewed = await _context.Set<MostReviewedResult>()
                .FromSqlRaw(@"
                    SELECT p.Id, p.Name, COUNT(r.Id) AS ReviewCount
                    FROM Products p
                    JOIN Reviews r ON p.Id = r.ProductId
                    GROUP BY p.Id, p.Name
                    ORDER BY ReviewCount DESC
                    OFFSET 0 ROWS FETCH NEXT 5 ROWS ONLY")
                .ToListAsync();

            var topRated = await _context.Products
                .FromSqlRaw("SELECT * FROM Products WHERE Rating > 4.5 ORDER BY Rating DESC")
                .ToListAsync();

            var model = new ReportViewModel
            {
                OutOfStockProducts = outOfStock,
                ProductsWithNoReviews = noReviews,
                MostReviewedProducts = mostReviewed,
                TopRatedProducts = topRated
            };

            return View(model);
        }
    }
}
