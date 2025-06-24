namespace MauiDemo2.Models;
public class Review
{
    public int Id { get; set; }
    public int UserId { get; set; }
    
    public int RouteId { get; set; }
    
    public string Username { get; set; }

    public string UserImageUrl {get;set;}
    
    public string ReviewText { get; set; }
    
    public float Grade { get; set; }
}
public class CreateReviewRequestDto
{
    public int UserId { get; set; }
    public int RouteId { get; set; }
    public string Text { get; set; }
    public float Grade { get; set; }
}