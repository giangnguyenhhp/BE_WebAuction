using Entities.Models;
using Entities.Models.DataTransferObject;

namespace Contracts.Models;

public interface IContactRepository
{
    Task<List<ContactDto>> GetAllContacts();
    Task<ContactDto> CreateContact(Contact request);
    Task DeleteContact(string id);
}