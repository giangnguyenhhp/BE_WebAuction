using Contracts.Models;
using Entities;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;

namespace Web_Auction.Controllers.ModelController;
[ApiController]
[Route("/api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly IContactRepository _repository;
    private readonly ILogger<ContactController> _logger;

    public ContactController(IContactRepository repository, ILogger<ContactController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllContacts()
    {
        try
        {
            var listContacts = await _repository.GetAllContacts();
            _logger.LogDebug("Returned all contacts from database");
            return Ok(listContacts);
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong inside GetAllContacts action : {Message}",e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateContact(Contact request)
    {
        try
        {
            var contact = await _repository.CreateContact(request);
            _logger.LogDebug("Contact created");
            return Ok(contact);
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong inside CreateContact action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteContact(string id)
    {
        try
        {
            await _repository.DeleteContact(id);
            _logger.LogDebug("Contact deleted");
            return Ok(new Response()
            {
                Title = "Success",
                Message = "Contact deleted"
            });
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong inside DeleteContact action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }
}