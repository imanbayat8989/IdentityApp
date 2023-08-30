using API.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Services
{
	public class JWTService
	{
		private readonly IConfiguration _config;
		private readonly SymmetricSecurityKey _JwtKey;
		public JWTService(IConfiguration configuration)
		{
			_config = configuration;
			//jwt is used for both encripting and decripting the jwt token
			_JwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));
		}
		public string CreateJWT(User user)
		{
			var userClaims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id),
				new Claim(ClaimTypes.Email, user.Email),
				new Claim(ClaimTypes.GivenName, user.FirstName),
				new Claim(ClaimTypes.Surname, user.LastName)
			};

			var creadentials = new SigningCredentials(_JwtKey, SecurityAlgorithms.HmacSha256);
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(userClaims),
				Expires = DateTime.UtcNow.AddDays(int.Parse(_config["JWT:ExpiresInDays"])),
				SigningCredentials = creadentials,
				Issuer = _config["JWT:Issuer"]
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			var jwt = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(jwt);
		}
	}
}
