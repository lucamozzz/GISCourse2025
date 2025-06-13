using GISCourse2025.Data;
using GISCourse2025.Models;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;

namespace GISCourse2025.Controllers
{
    public class HikingTrailsController : Controller
    {
        ApplicationDbContext context;
        public HikingTrailsController(ApplicationDbContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            List<hikingTrails> hikingTrails1 = context.hikingTrails
                .Where(k => k.id > 0)
                .OrderBy(k => k.name)
                .Take(15)
                .ToList();
            return View("HikingTrails", hikingTrails1);
        }
    }
}
