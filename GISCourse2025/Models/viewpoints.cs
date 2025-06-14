using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace GISCourse2025.Models
{
    public class viewpoints
    {
        public long id { get; set; }
        public string? name { get; set; }
        public Geometry geometry { get; set; }
    }
}