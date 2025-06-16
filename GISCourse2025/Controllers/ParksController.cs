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
        public IActionResult Index()
        {
            List<parks> parks1 = context.parks
                .Where(k => k.id > 0 && k.name != null)
                // .OrderBy(k => k.name)
                // .Take(15)
                .ToList();
            return View("Parks", parks1);
        }
    }
}
