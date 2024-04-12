using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using API._Repositories;
using API._Services.Interfaces.Auth;
using API.Configurations;
using API.Helpers.Base;
using API.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ViewModels.Auth;
using ViewModels.UserManager;

namespace API._Services.Services.Auth;

public class S_Token(IRepositoryAccessor repoStore, IOptions<AppSetting> jwtTokenSettings) : BaseServices(repoStore), I_Token
{
    private readonly AppSetting _jwtTokenSettings = jwtTokenSettings.Value;

    public async Task<TokenRequest> GenerateTokenAsync(UserVM userVM)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();

        var secretKeyBytes = Encoding.UTF8.GetBytes(_jwtTokenSettings.SecretKey);

        var tokenDescription = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[] {
                    new(ClaimTypes.Name, userVM.FullName),
                    new(JwtRegisteredClaimNames.Email, userVM.Email),
                    new(JwtRegisteredClaimNames.Sub, userVM.Email),
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new("UserName", userVM.UserName),
                    new("Id", userVM.Id.ToString()),
                    //roles
                }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha512Signature)
        };

        var token = jwtTokenHandler.CreateToken(tokenDescription);
        string? accessToken = jwtTokenHandler.WriteToken(token);
        string? refreshToken = GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            JwtId = token.Id,
            UserId = userVM.Id,
            Token = refreshToken,
            IsUsed = false,
            IsRevoked = false,
            IssuedAt = DateTime.UtcNow,
            ExpiredAt = DateTime.UtcNow.AddHours(1)
        };


        await _repoStore.RefreshTokens.AddAsync(refreshTokenEntity);

        await _repoStore.SaveChangesAsync();

        return new TokenRequest
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    private static string GenerateRefreshToken()
    {
        var random = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(random);

        return Convert.ToBase64String(random);
    }

    public async Task<OperationResult<TokenRequest>> RefreshTokenAsync(TokenRequest request)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var secretKeyBytes = Encoding.UTF8.GetBytes(_jwtTokenSettings.SecretKey);
        var tokenValidateParam = new TokenValidationParameters
        {
            //tự cấp token
            ValidateIssuer = false,
            ValidateAudience = false,

            //ký vào token
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),

            ClockSkew = TimeSpan.Zero,

            ValidateLifetime = false //ko kiểm tra token hết hạn
        };

        //check 1: AccessToken valid format
        var tokenInVerification = jwtTokenHandler.ValidateToken(request.AccessToken, tokenValidateParam, out var validatedToken);

        //check 2: Check alg
        if (validatedToken is JwtSecurityToken jwtSecurityToken)
        {
            var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase);
            if (!result)//false
            {
                return OperationResult<TokenRequest>.BadRequest("Invalid token");
            }
        }

        //check 3: Check accessToken expire?
        string? expClaim = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)?.Value;
        if (expClaim is null)
            return OperationResult<TokenRequest>.BadRequest("Access token is null");
        var utcExpireDate = long.Parse(expClaim);

        var expireDate = ConvertUnixTimeToDateTime(utcExpireDate);
        if (expireDate > DateTime.UtcNow)
            return OperationResult<TokenRequest>.BadRequest("Access token has not yet expired");

        //check 4: Check refreshtoken exist in DB
        var storedToken = _repoStore.RefreshTokens.FirstOrDefault(x => x.Token == request.RefreshToken);
        if (storedToken is null)
            return OperationResult<TokenRequest>.NotFound("Refresh token does not exist");


        //check 5: check refreshToken is used/revoked?
        if (storedToken.IsUsed)
            return OperationResult<TokenRequest>.BadRequest("Refresh token has been used");

        if (storedToken.IsRevoked)
            return OperationResult<TokenRequest>.BadRequest("Refresh token has been revoked");

        //check 6: AccessToken id == JwtId in RefreshToken
        string? jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;
        if (storedToken.JwtId != jti)
            return OperationResult<TokenRequest>.NotFound("Token doesn't match");

        //Update token is used
        storedToken.IsRevoked = true;
        storedToken.IsUsed = true;
        _repoStore.RefreshTokens.Update(storedToken);
        await _repoStore.SaveChangesAsync();

        //create new token
        var user = await _repoStore.Users.FirstOrDefaultAsync(nd => nd.Id == storedToken.UserId);
        var token = await GenerateTokenAsync(new UserVM
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            UserName = user.UserName ?? string.Empty
        });
        return OperationResult<TokenRequest>.Success(token);
    }

    private static DateTime ConvertUnixTimeToDateTime(long utcExpireDate)
    {
        var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTimeInterval.AddSeconds(utcExpireDate).ToUniversalTime();

        return dateTimeInterval;
    }
}
