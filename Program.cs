using System;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Session;
using TriviaGameApp.Models;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtConfig:Secret"]);
builder.Services.AddMvc();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
}); 
builder.Services.AddSingleton<IMongoClient, MongoClient>(s =>
{
var mongoUrl = new MongoUrl(builder.Configuration["MongoDB:ConnectionString"]);
    return new MongoClient(mongoUrl);
});
builder.Services.AddSingleton<IMongoDatabase>(s =>
{
    var mongoClient = s.GetService<IMongoClient>();
    return mongoClient.GetDatabase(builder.Configuration["MongoDBSettings:DatabaseName"]);
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})


.AddJwtBearer(jwtBearerOptions =>
{
    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The following line enables application to use Https
    app.UseHsts();
}
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession(); 

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
