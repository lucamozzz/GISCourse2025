using GISCourse2025.Data;
using GISCourse2025.Models;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;

namespace GISCourse2025.Controllers
{
    public class Viewpoints : Controller
    {
        ApplicationDbContext context;
        public Viewpoints(ApplicationDbContext context)
        {
            this.context = context;
        }
        public IActionResult Index(string search)
        {
            var viewpointsQuery = context.viewpoints
                .Where(k => k.id > 0 && k.name != null);

            if (!string.IsNullOrEmpty(search))
            {
                viewpointsQuery = viewpointsQuery.Where(k => k.name.ToLower().Contains(search.ToLower()));
            }

            ViewBag.Search = search;

            var viewpoints1 = viewpointsQuery.ToList();
            return View("Viewpoints", viewpoints1);
        }
    }
}
