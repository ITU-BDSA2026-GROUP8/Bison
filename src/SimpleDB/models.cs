namespace SimpleDB;

public class Observation
{
    public string Author { get; set; }
    public string Message { get; set; }
    public long Timestamp { get; set; }
    public int Id { get; set; }
    public string Location { get; set; }

    public Observation(string author, string message, long timestamp, int id, string location)
    {
        Author = author;
        Message = message;
        Timestamp = timestamp;
        Id = id;
        Location = location;
    }
}

public class Comment
{
    public string Message { get; set; }
    public int Id { get; set; }

    public Comment(string message, int id){
    Message = message;
    Id = id;
    }
}
