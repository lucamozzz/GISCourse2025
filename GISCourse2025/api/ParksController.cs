using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.Text.Json;
using GISCourse2025.Data;
using GISCourse2025.Models;
using NetTopologySuite.IO;
using NetTopologySuite;

namespace GISCourse2025.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ParksController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult GetPark(long id)
        {
            var park = _context.parks
                .Where(p => p.id == id)
                .Select(p => new
                {
                    p.id,
                    p.name,
                    p.geometry
                })
                .FirstOrDefault();

            if (park == null)
                return NotFound();

            var wktWriter = new WKTWriter();
            string wkt = wktWriter.Write(park.geometry); // This can be Polygon or MultiPolygon

            return Ok(new
            {
                id = park.id,
                name = park.name,
                wkt = wkt
            });
        }

        [HttpPost("add")]
        public IActionResult Add([FromQuery] string name, [FromBody] MultiPolygonDto geometryDto)
        {
            if (string.IsNullOrWhiteSpace(name) || geometryDto?.Polygons == null || geometryDto.Polygons.Count == 0)
                return BadRequest("Name and geometry must be provided with at least one polygon.");

            try
            {
                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 3857);

                var polygons = geometryDto.Polygons.Select(ringList =>
                {
                    var shellCoords = ringList[0].Select(c => new Coordinate(c[0], c[1])).ToArray();
                    var shell = geometryFactory.CreateLinearRing(shellCoords);

                    var holes = ringList.Skip(1).Select(hole =>
                    {
                        var holeCoords = hole.Select(c => new Coordinate(c[0], c[1])).ToArray();
                        return geometryFactory.CreateLinearRing(holeCoords);
                    }).ToArray();

                    return geometryFactory.CreatePolygon(shell, holes);
                }).ToArray();

                var multiPolygon = geometryFactory.CreateMultiPolygon(polygons);

                var entity = new parks
                {
                    id = Random.Shared.Next(1_000_000_000, 2_000_000_000),
                    name = name,
                    geometry = multiPolygon
                };

                _context.parks.Add(entity);
                _context.SaveChanges();

                return Ok(entity.id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex); // Log completo
                return StatusCode(500, "Server error: " + ex.Message);
            }
        }

        [HttpPut("update")]
        public IActionResult UpdatePark([FromBody] ParkUpdateDTO data)
        {
            var park = _context.parks.FirstOrDefault(p => p.id == data.id);
            if (park == null)
                return NotFound();

            park.name = data.name;

            try
            {
                var reader = new WKTReader();
                var geometry = reader.Read(data.wkt);

                if (geometry == null || !(geometry is MultiPolygon || geometry is Polygon))
                    return BadRequest("Invalid geometry type. Expected Polygon or MultiPolygon.");

                // If a single Polygon is received, wrap it in a MultiPolygon for consistency
                if (geometry is Polygon singlePolygon)
                {
                    var factory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                    geometry = factory.CreateMultiPolygon(new[] { singlePolygon });
                }

                park.geometry = geometry;
                _context.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Server error: " + ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeletePark(long id)
        {
            try
            {
                var entity = _context
                    .Set<parks>()
                    .Find(id);

                if (entity == null)
                    return NotFound($"Park with id {id} not found.");

                _context
                    .Set<parks>()
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