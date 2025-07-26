using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Backend.Helpers;

public static class JwtHelper
{
	public static string GenerateToken(string username, string role, IConfiguration config)
	{
		var claims = new[]
		{
			new Claim(ClaimTypes.Name, username),
			new Claim("role", role)
		};

		var key = new SymmetricSecurityKey(
			Encoding.UTF8.GetBytes(config["Jwt:Key"]!)
		);

		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var token = new JwtSecurityToken(
			issuer: config["Jwt:Issuer"],
			audience: config["Jwt:Audience"],
			claims: claims,
			expires: DateTime.UtcNow.AddDays(1),
			signingCredentials: creds
		);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}
}