using AutoMapper;
using Contracts.Models;
using Entities;
using Entities.Models;
using Entities.Models.DataTransferObject;
using Microsoft.EntityFrameworkCore;

namespace Repository.Models;

public class ContactRepository : IContactRepository
{
    private readonly MasterDbContext _dbContext;
    private readonly IMapper _mapper;

    public ContactRepository(MasterDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<ContactDto>> GetAllContacts()
    {
        var listContacts = await _dbContext.Contacts.ToListAsync();

        var result = _mapper.Map<List<ContactDto>>(listContacts);
        return result;
    }

    public async Task<ContactDto> CreateContact(Contact request)
    {
        var contact = _mapper.Map<Contact>(request);
        contact.DateSent = DateTime.Now;

        await _dbContext.Contacts.AddAsync(contact);
        await _dbContext.SaveChangesAsync();

        var result = _mapper.Map<ContactDto>(contact);
        return result;
    }

    public async Task DeleteContact(string id)
    {
        var contact = await _dbContext.Contacts.FirstOrDefaultAsync(x => x.ContactId.ToString() == id);
        if (contact == null)
        {
            throw new Exception("Contact not found");
        }
        
        _dbContext.Contacts.Remove(contact);
        await _dbContext.SaveChangesAsync();
    }
}