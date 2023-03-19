using Entities.Identity.AuthenticateRequestModels;
using Entities.Identity.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace Contracts.Identity;

public interface IAuthenticateRepository
{
    Task<IActionResult> LoginAsync(LoginRequest request);
    Task<IActionResult> EmailConfirmation(string email, string token);
    Task<IActionResult> RegisterForClient(CreateUserRequest request);
    Task<IActionResult> ForgotPassword(ForgotPasswordRequest request);
    Task<IActionResult> ResetPassword(ResetPasswordRequest request);
}