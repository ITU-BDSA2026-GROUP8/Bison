namespace SimpleDB;

public class Observation
{
    public string Author { get; set; }
    public string Message { get; set; }
    public long Timestamp { get; set; }
    public int Id { get; set; }
}

public class Comment
{
    public string Message { get; set; }
    public int ObservationId { get; set; }
}
