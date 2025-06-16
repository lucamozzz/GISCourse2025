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
        public IActionResult Index(string search, string sacScale, double? minLength, double? maxLength)
        {
            var hikingTrailsQuery = context.hikingTrails
                .Where(k => k.id > 0 && k.name != null);

            if (!string.IsNullOrEmpty(search))
            {
                hikingTrailsQuery = hikingTrailsQuery.Where(k => k.name.Contains(search));
            }

            if (!string.IsNullOrEmpty(sacScale))
            {
                hikingTrailsQuery = hikingTrailsQuery.Where(k => k.sac_scale == sacScale);
            }

            if (minLength.HasValue)
            {
                hikingTrailsQuery = hikingTrailsQuery.Where(k => k.geometry.Length >= minLength.Value);
            }

            if (maxLength.HasValue)
            {
                hikingTrailsQuery = hikingTrailsQuery.Where(k => k.geometry.Length <= maxLength.Value);
            }

            ViewBag.Search = search;
            ViewBag.SacScale = sacScale;
            ViewBag.MinLength = minLength;
            ViewBag.MaxLength = maxLength;

            var hikingTrailsList = hikingTrailsQuery.ToList();
            return View("HikingTrails", hikingTrailsList);
        }
    }
}
