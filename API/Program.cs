using API.Data;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<Context>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//be able to inject JWTService class inside our Controllers
builder.Services.AddScoped<JWTService>(); 

//Defining our IdentityCore Service
builder.Services.AddIdentityCore<User>(options =>
{
	//Password Configuration
	options.Password.RequiredLength = 6;
	options.Password.RequireDigit = false;
	options.Password.RequireLowercase = false;
	options.Password.RequireUppercase = false;
	options.Password.RequireNonAlphanumeric = false;

	options.SignIn.RequireConfirmedEmail = true;
})
	.AddRoles<IdentityRole>() //be able to add roles
	.AddRoleManager<RoleManager<IdentityRole>>() // bea able to make use of RoleManager
	.AddEntityFrameworkStores<Context>() //Providing our context
	.AddSignInManager<SignInManager<User>>()//Make use of signing manager
	.AddUserManager<UserManager<User>>() //make use of UserManager to create users
	.AddDefaultTokenProviders(); //be able to create tokens for email configuration

//Be able to authenticate users using JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
			ValidIssuer = builder.Configuration["JWT:Issuer"],
			ValidateIssuer = true,
			ValidateAudience = false
		};
	});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
