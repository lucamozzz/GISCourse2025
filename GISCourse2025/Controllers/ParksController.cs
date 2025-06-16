using GISCourse2025.Data;
using GISCourse2025.Models;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;

namespace GISCourse2025.Controllers
{
    public class ParksController : Controller
    {
        ApplicationDbContext context;
        public ParksController(ApplicationDbContext context)
        {
            this.context = context;
        }
        public IActionResult Index(string search)
        {
            var parksQuery = context.parks
                .Where(k => k.id > 0 && k.name != null);

            if (!string.IsNullOrEmpty(search))
            {
                parksQuery = parksQuery.Where(k => k.name.ToLower().Contains(search.ToLower()));
            }

            ViewBag.Search = search;

            var parks1 = parksQuery.ToList();
            return View("Parks", parks1);
        }
    }
}
