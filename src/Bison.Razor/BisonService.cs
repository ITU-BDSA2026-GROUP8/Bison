using Bison.Razor.Models;
using Microsoft.EntityFrameworkCore;
using SimpleDB;


public interface IObservationService
{
    public  List<PostDTO> GetObservations(int page);

    public PostDTO? GetObservation(int id);

    public List<PostDTO> GetObservationsFromAuthor(string author, int page);

    public void StoreObservation(string author, string message, string location);
}
public class ObservationService : IObservationService
{
    private readonly IPostRepository _repo;

    public ObservationService(IPostRepository postRepository) {_repo = postRepository;}

    public  List<PostDTO> GetObservations(int page)
    {
        return _repo.GetObservations(page);
    }

    public PostDTO? GetObservation(int id)
    {
        return _repo.GetObservation(id);
    }

    public List<PostDTO> GetObservationsFromAuthor(string author, int page)
    {
        return _repo.GetObservationsFromAuthor(author,page);
    }

    public void StoreObservation(string author, string message, string location)
    {
        _repo.StoreObservation(author,message,location);
    }

    public List<Bison.Razor.Models.Comment> GetCommentsForObservation(int id){

        return _context.Comments
            .Where(comment => comment.Observation.Id == id)
            .ToList();
        }
}
public class TaxonService{
    private readonly CSVDataBase<SimpleDB.Taxon> _taxondb;
    public TaxonService(CSVDataBase<SimpleDB.Taxon> taxondb){_taxondb = taxondb;}

    public List<SimpleDB.Taxon> GetTaxons()
    {
        return _taxondb.Read().ToList();
    }
    
 
}
public class ProposalService
{
    private readonly CSVDataBase<SimpleDB.Proposal> _proposalDB;
    public ProposalService(CSVDataBase<SimpleDB.Proposal> proposalDB){_proposalDB = proposalDB;}

    public List<SimpleDB.Proposal> GetProposals()
    {
        return _proposalDB.Read().ToList();
    }
    
    public  List<SimpleDB.Proposal> GetProposalsForObservation(int id)
    {
        return _proposalDB.Read().Where(proposal => proposal.observationId==id).ToList();}
    
}