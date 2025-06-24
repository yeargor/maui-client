namespace MauiDemo2.Models;

public class Coordinate
{
    public int Id { get; set; }
    
    public float Latitude { get; set; }
    
    public float Longitude { get; set; }
    
    public ICollection<Point> Points { get; set; }
    
    public Coordinate(float latitude, float longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }
    
    public Coordinate() { }
}