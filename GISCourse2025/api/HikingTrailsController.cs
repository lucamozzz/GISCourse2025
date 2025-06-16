using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.Text.Json;
using GISCourse2025.Data;
using GISCourse2025.Models;
using NetTopologySuite.IO;

namespace GISCourse2025.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class HikingTrailsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HikingTrailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult GetTrail(int id)
        {
            var trail = _context.hikingTrails
                .Where(p => p.id == id)
                .Select(p => new
                {
                    p.id,
                    p.name,
                    p.sac_scale,
                    p.geometry
                })
                .FirstOrDefault();

            if (trail == null)
                return NotFound();

            var wktWriter = new WKTWriter();
            string wkt = wktWriter.Write(trail.geometry);

            return Ok(new
            {
                id = trail.id,
                name = trail.name,
                sac_scale = trail.sac_scale,
                wkt = wkt
            });
        }

        [HttpPost("add")]
        public virtual IActionResult Add([FromQuery] string name, [FromBody] LineStringDto geometryDto)
        {
            if (string.IsNullOrWhiteSpace(name) || geometryDto?.Coordinates == null || geometryDto.Coordinates.Length < 2)
                return BadRequest("Name and geometry must be provided with at least 2 points.");

            try
            {
                var coordinates = geometryDto.Coordinates
                    .Select(coord => new Coordinate(coord[0], coord[1]))
                    .ToArray();

                var geometry = new LineString(coordinates); // create NetTopologySuite geometry

                var entity = new hikingTrails
                {
                    id = new Random().Next(1000000000, 2000000000),
                    name = name,
                    sac_scale = geometryDto.sac_scale,
                    geometry = geometry
                };

                _context.Set<hikingTrails>().Add(entity);
                _context.SaveChanges();

                return Ok(entity.id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Server error: " + ex.Message);
            }
        }

        [HttpPut("update")]
        public IActionResult UpdateLine([FromBody] TrailUpdateDTO data)
        {
            var trail = _context.hikingTrails.FirstOrDefault(t => t.id == data.id);
            if (trail == null)
                return NotFound();

            trail.name = data.name;
            trail.sac_scale = data.sac_scale;

            var reader = new WKTReader();
            trail.geometry = reader.Read(data.wkt);

            _context.SaveChanges();
            return Ok();
        }

        [HttpDelete("delete/{id}")]
        public virtual IActionResult Delete(int id)
        {
            try
            {
                var entity = _context
                .Set<hikingTrails>()
                .Find(id);

                if (entity == null)
                    return NotFound($"Entity with id {id} not found.");
                else
                    _context
                    .Set<hikingTrails>()
                    .Remove(entity);

                _context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Server error: " + ex.Message);
            }
        }
    }
}