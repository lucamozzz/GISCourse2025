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
    public class Viewpoints : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public Viewpoints(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult GetViewpoint(long id)
        {
            var viewpoint = _context.viewpoints
                .Where(p => p.id == id)
                .Select(p => new
                {
                    p.id,
                    p.name,
                    p.geometry
                })
                .FirstOrDefault();

            if (viewpoint == null)
                return NotFound();

            var wktWriter = new WKTWriter();
            string wkt = wktWriter.Write(viewpoint.geometry);

            return Ok(new
            {
                id = viewpoint.id,
                name = viewpoint.name,
                wkt = wkt
            });
        }

        public class ViewpointWKTDto
        {
            public string wkt { get; set; }
        }

        [HttpPost("add")]
        public IActionResult Add([FromQuery] string name, [FromBody] ViewpointWKTDto dto)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(dto?.wkt))
                return BadRequest("Missing name or geometry.");

            var reader = new WKTReader();
            var geometry = reader.Read(dto.wkt) as Point;

            if (geometry == null)
                return BadRequest("Invalid geometry.");

            var entity = new viewpoints
            {
                id = new Random().Next(1000000000, 2000000000),
                name = name,
                geometry = geometry
            };

            _context.viewpoints.Add(entity);
            _context.SaveChanges();

            return Ok(entity.id);
        }

        [HttpPut("update")]
        public IActionResult UpdatePoint([FromBody] ViewpointUpdateDTO data)
        {
            var point = _context.viewpoints.FirstOrDefault(v => v.id == data.id);
            if (point == null)
                return NotFound();

            point.name = data.name;

            var reader = new WKTReader();
            point.geometry = reader.Read(data.wkt);

            _context.SaveChanges();
            return Ok();
        }

        [HttpDelete("delete/{id}")]
        public virtual IActionResult Delete(long id)
        {
            try
            {
                var entity = _context
                    .Set<viewpoints>()
                    .Find(id);

                if (entity == null)
                    return NotFound($"Entity with id {id} not found.");
                else
                    _context
                        .Set<viewpoints>()
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