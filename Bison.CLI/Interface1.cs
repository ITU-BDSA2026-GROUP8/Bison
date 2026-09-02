namespace Bison.CLI;
using System.Globalization;

public interface Interface1
{
    static void PrintObservations(IEnumerable<T> cheeps)
    {
        foreach (var cheep in cheeps)
        {
            var time = DateTimeOffset.FromUnixTimeSeconds(long.Parse(cheep.TimeStamp));
            Console.WriteLine($"{cheep.Author} @ {time.ToString("MM/dd/yy HH:mm:ss", CultureInfo.InvariantCulture)}: {cheep.Message}");
        }
    }

}
