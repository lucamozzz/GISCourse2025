public class ParkUpdateDTO
{
    public long id { get; set; }
    public string name { get; set; }
    public string wkt { get; set; } // Polygon or MultiPolygon WKT
}