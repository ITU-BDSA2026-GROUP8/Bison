using SimpleDB;
public class ObservationService
{
    private readonly CSVDataBase<Observation> _observationDb;

    public ObservationService(CSVDataBase<Observation> observationDb) {_observationDb = observationDb;}

    public List<Observation> GetObservations(){return _observationDb.Read().ToList();}

    public Observation? GetObservation(int id){return GetObservations().FirstOrDefault(observation => observation.Id == id);}

    public List<Observation> GetObservationsFromAuthor(string author){return GetObservations().Where(observation => observation.Author == author).ToList();}

    public void StoreObservation(string author, string message, string location)
    {
        var id = GetObservations().Select(observation => observation.Id).DefaultIfEmpty(0).Max() + 1;
        var observation = new Observation(
            author,
            message,
            DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            id,
            location);
        _observationDb.Store(observation);
    }

    private static string UnixTimeStampToDateTimeString(long unixTimeStamp){return DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp).ToString("MM/dd/yy H:mm:ss");}
}
public class CommentService
{
    private readonly CSVDataBase<Comment> _commentDb;
    public CommentService(CSVDataBase<Comment> commentDb){_commentDb = commentDb;}

    public List<Comment> GetCommentsForObservation(int id){return _commentDb.Read().Where(comment => comment.Id == id).ToList();}
}