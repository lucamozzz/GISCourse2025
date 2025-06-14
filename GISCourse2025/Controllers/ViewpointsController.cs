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
        public IActionResult Index()
        {
            List<viewpoints> viewpoints1 = context.viewpoints
                .Where(k => k.id > 0)
                .OrderBy(k => k.name)
                .Take(15)
                .ToList();
            return View("Viewpoints", viewpoints1);
        }
    }
}
