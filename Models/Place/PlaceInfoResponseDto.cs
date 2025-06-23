public class PlaceInfoResponseDto
{
    public LocationInfoResponseDto LocationInfo { get; set; }
    public int? OrderOfVisit { get; set; }
    public bool IsCompleted { get; set; }
}