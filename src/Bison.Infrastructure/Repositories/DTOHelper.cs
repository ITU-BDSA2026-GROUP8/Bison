using Bison.Core.DTOs;
using Bison.Core.Models;

namespace Bison.Infrastructure.Repositories;

public class DTOHelper
{
    public PostDTO createPostDTO(string text, Author author, DateTime timeStamp, int id)
    {
        var DTO = new PostDTO();
        DTO.Author = author;
        DTO.Text = text;
        DTO.Timestamp = timeStamp;
        DTO.Id = id;
        return DTO;
    }

    public CommentDTO createCommentDTO(string text, int Id)
    {
        var CDTO = new CommentDTO();
        CDTO.Text = text;
        CDTO.Id = Id;
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

