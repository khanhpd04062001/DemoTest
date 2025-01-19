using Microsoft.EntityFrameworkCore;
using WebApplication1;
using WebApplication1.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<SPContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DB"));
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", build => build.AllowAnyMethod()
        .AllowAnyHeader().AllowCredentials().SetIsOriginAllowed(hostName => true).Build());
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SPContext>();

    
    context.Database.EnsureCreated();

    
    await DataSeeder.SeedDataAsync(context);
}

app.UseAuthorization();
app.UseCors(build =>
{
    build.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
});
app.MapControllers();

app.Run();
