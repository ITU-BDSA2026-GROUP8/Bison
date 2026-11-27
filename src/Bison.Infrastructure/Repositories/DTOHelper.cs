using Bison.Core.DTOs;
using Bison.Core.Models;


namespace Bison.Infrastructure.Repositories;

public class DTOHelper
{
    public PostDTO createPostDTO(string text, Author author, DateTime timeStamp, int id, string? imageUrl, string? imageLicense)
    {
        var DTO = new PostDTO();
        DTO.Author = author;
        DTO.Text = text;
        DTO.Timestamp = timeStamp;
        DTO.Id = id;
        DTO.ImageUrl = imageUrl;
        DTO.ImageLicense = imageLicense;
        return DTO;
    }

    public CommentDTO createCommentDTO(string text, int Id,string author,string timeStamp)
    {
        var CDTO = new CommentDTO();
        CDTO.Text = text;
        CDTO.Id = Id;
        CDTO.Author=author;
        CDTO.TimeStamp=timeStamp;
        return CDTO;
    }

    public TaxonDTO createTaxonDTO(string TaxonId, string DanishVernacularName)
    {
        var TDTO = new TaxonDTO();
        TDTO.TaxonId = TaxonId;
        TDTO.DanishVernacularName = DanishVernacularName;
        return TDTO;
    }
}

