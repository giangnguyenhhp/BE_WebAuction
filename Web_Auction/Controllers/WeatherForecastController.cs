using EmailService.SendMailServices;
using Entities.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Web_Auction.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IEmailSender _emailSender;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, SignInManager<AppUser> signInManager,
        IEmailSender emailSender)
    {
        _logger = logger;
        _signInManager = signInManager;
        _emailSender = emailSender;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public Task<IActionResult> Get()
    {
        // _logger.LogInformation("Inside GetWeatherForecast endpoint");
        // throw new Exception("Can not retrieve weatherForecast information");
        var response = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

        // _logger.LogInformation("The response for the get weather forecast is {SerializeObject}",
        //     JsonConvert.SerializeObject(response));
        // Serilog.Debugging.SelfLog.Enable(Console.WriteLine);

        return Task.FromResult<IActionResult>(Ok(response));
    }
    
    [HttpGet("sendSms")]
    public async Task<IActionResult> SendSmsAsync()
    {
        await _emailSender.SendSmsAsync("0909261293", "Test send Sms");
        return Ok();
    }

    [HttpPost("sendEmail")]
    public async Task<IActionResult> SendEmailAsync()
    {
        //Gửi Email bình thường
        var message = new Message(new[] { "testmailhp121@gmail.com" }, "Test Email", "This is a test email");
        await _emailSender.SendEmailAsync(message);
        
        //Gửi Email với file đính kèm
        var files = Request.Form.Files.Any() ? Request.Form.Files : new FormFileCollection();
        var messageWithAttach = new Message(new[] { "testmailhp121@gmail.com" }, "Test Email with Attachments",
            "This is a test email with Attachments", files);
        await _emailSender.SendEmailAsync(messageWithAttach);

        return Ok();
    }


}