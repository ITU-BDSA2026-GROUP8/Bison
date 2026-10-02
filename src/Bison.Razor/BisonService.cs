using SimpleDB;

public record ObservationViewModel(string Author, string Message, string Timestamp);

public interface IObservationService
{
    public List<ObservationViewModel> GetObservations();
    public List<ObservationViewModel> GetObservationsFromAuthor(string author);
    public void StoreObservation(string author, string message, string location);
}

public class ObservationService : IObservationService
{
    private readonly CSVDataBase<Observation> _observationDb;

    public ObservationService(CSVDataBase<Observation> observationDb)
    {
        _observationDb = observationDb;
    }

    public List<ObservationViewModel> GetObservations()
    {
        return _observationDb.Read()
            .Select(observation => new ObservationViewModel(
                observation.Author,
                observation.Message,
                UnixTimeStampToDateTimeString(observation.Timestamp)))
            .ToList();
    }

    public List<ObservationViewModel> GetObservationsFromAuthor(string author)
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
