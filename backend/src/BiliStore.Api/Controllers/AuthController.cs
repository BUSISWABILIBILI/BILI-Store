using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BiliStore.Api.Options;
using BiliStore.Application.DTOs.Auth;
using BiliStore.Domain.Entities;
using BiliStore.Infrastructure.Data;
using BiliStore.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BiliStore.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    UserManager<ApplicationUser> userManager,
    BiliStoreDbContext dbContext,
    IConfiguration configuration) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(
        RegisterCustomerRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!string.Equals(request.Password, request.ConfirmPassword, StringComparison.Ordinal))
        {
            return BadRequest(new { errors = new[] { "Password and confirmation password do not match." } });
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var existingUser = await userManager.FindByEmailAsync(normalizedEmail);

        if (existingUser is not null)
        {
            return BadRequest(new { errors = new[] { "An account with this email already exists." } });
        }

        var user = new ApplicationUser
        {
            UserName = normalizedEmail,
            Email = normalizedEmail,
            PhoneNumber = request.Phone.Trim(),
            EmailConfirmed = true
        };

        var identityResult = await userManager.CreateAsync(user, request.Password);

        if (!identityResult.Succeeded)
        {
            return BadRequest(new { errors = identityResult.Errors.Select(error => error.Description).ToArray() });
        }

        var profile = new CustomerProfile
        {
            ApplicationUserId = user.Id,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            PreferredPhone = request.Phone.Trim()
        };

        dbContext.CustomerProfiles.Add(profile);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(CreateAuthResponse(user, profile));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginCustomerRequestDto request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email.Trim().ToLowerInvariant());

        if (user is null || !user.IsActive || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            return Unauthorized(new { errors = new[] { "Invalid email or password." } });
        }

        var profile = await dbContext.CustomerProfiles
            .SingleOrDefaultAsync(customer => customer.ApplicationUserId == user.Id, cancellationToken);

        if (profile is null)
        {
            return Unauthorized(new { errors = new[] { "This account does not have a customer profile." } });
        }

        return Ok(CreateAuthResponse(user, profile));
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult> ForgotPassword(ForgotPasswordRequestDto request)
    {
        _ = await userManager.FindByEmailAsync(request.Email.Trim().ToLowerInvariant());

        return Accepted(new
        {
            message = "If an account exists for that email, password reset instructions will be sent when email delivery is configured."
        });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<CustomerProfileDto>> GetCurrentCustomer(CancellationToken cancellationToken)
    {
        var result = await GetCurrentUserAndProfileAsync(cancellationToken);

        return result is null ? Unauthorized() : Ok(MapProfile(result.Value.User, result.Value.Profile));
    }

    [Authorize]
    [HttpPut("me")]
    public async Task<ActionResult<CustomerProfileDto>> UpdateCurrentCustomer(
        UpdateCustomerProfileRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await GetCurrentUserAndProfileAsync(cancellationToken);

        if (result is null)
        {
            return Unauthorized();
        }

        var (user, profile) = result.Value;
        profile.FirstName = request.FirstName.Trim();
        profile.LastName = request.LastName.Trim();
        profile.PreferredPhone = request.Phone.Trim();
        profile.UpdatedAt = DateTimeOffset.UtcNow;
        user.PhoneNumber = request.Phone.Trim();

        await userManager.UpdateAsync(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(MapProfile(user, profile));
    }

    private async Task<(ApplicationUser User, CustomerProfile Profile)?> GetCurrentUserAndProfileAsync(CancellationToken cancellationToken)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return null;
        }

        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null || !user.IsActive)
        {
            return null;
        }

        var profile = await dbContext.CustomerProfiles
            .SingleOrDefaultAsync(customer => customer.ApplicationUserId == user.Id, cancellationToken);

        return profile is null ? null : (user, profile);
    }

    private AuthResponseDto CreateAuthResponse(ApplicationUser user, CustomerProfile profile)
    {
        var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>()
            ?? throw new InvalidOperationException("JWT settings are not configured.");
        var expiresAt = DateTimeOffset.UtcNow.AddHours(8);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Name, $"{profile.FirstName} {profile.LastName}".Trim()),
            new Claim("customer_profile_id", profile.Id.ToString())
        };
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SigningKey));
        var token = new JwtSecurityToken(
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            claims: claims,
            expires: expiresAt.UtcDateTime,
            signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

        return new AuthResponseDto(
            new JwtSecurityTokenHandler().WriteToken(token),
            expiresAt,
            MapProfile(user, profile));
    }

    private static CustomerProfileDto MapProfile(ApplicationUser user, CustomerProfile profile)
    {
        return new CustomerProfileDto(
            profile.Id,
            profile.ApplicationUserId,
            profile.FirstName,
            profile.LastName,
            user.Email ?? string.Empty,
            profile.PreferredPhone);
    }
}
