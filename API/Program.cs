using API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddCors((opt) => opt.AddPolicy(
    "DatingApp", 
    (policy) => policy.WithOrigins("http://localhost:4200", "https://localhost:4200").AllowAnyMethod().AllowAnyHeader()
));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors("DatingApp");

app.MapControllers();

app.Run();
