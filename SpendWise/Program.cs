using Microsoft.EntityFrameworkCore;
using SpendWise.Business;
using SpendWise.Business.Interfaces;
using SpendWise.DataAccess;
using SpendWise.DataAccess.Entities;
using SpendWise.DataAccess.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("SpendWiseContext");
builder.Services.AddDbContext<SpendWiseContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// Add CORS services to allow any origin
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin",
        builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddTransient<ICategoryService, CategoryService>();
builder.Services.AddTransient<IRepository<Category>, CategoryRepository>();
builder.Services.AddTransient<IReceiptService, ReceiptService>();
builder.Services.AddTransient<IRepository<Product>, ProductRepository>();
builder.Services.AddTransient<IRepository<Cart>, BaseRepository<Cart>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAnyOrigin"); // Use the CORS policy to allow any origin

app.UseAuthorization();

app.MapControllers();

app.Run();