using SimpleDB;
public class ObservationService
{
    private readonly CSVDataBase<Observation> _observationDb;

    public ObservationService(CSVDataBase<Observation> observationDb)
    {
        _observationDb = observationDb;
    }

    public List<Observation> GetObservations()
    {
        return _observationDb.Read().ToList();
    }

    public List<Observation> GetObservationsFromAuthor(string author)
    {
        return GetObservations().Where(observation => observation.Author == author).ToList();
    }

    public void StoreObservation(string author, string message, string location)
    {
        var observation = new Observation(
            author,
            message,
            DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            0,
            location);
        _observationDb.Store(observation);
    }

    private static string UnixTimeStampToDateTimeString(long unixTimeStamp)
    {
        return DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp).ToString("MM/dd/yy H:mm:ss");
    }
}
public class CommentService
{
    private readonly CSVDataBase<Comment> _commentDb;
    public CommentService(CSVDataBase<Comment> commentDb)
    {
        _commentDb = commentDb;
    }

    public List<Comment> GetObservations()
    {
        return _commentDb.Read().ToList();
    }

    public List<Comment> GetObservationsFromAuthor(int Id)
    {
        return GetObservations().Where(comment => comment.Id == Id).ToList();
    }
}