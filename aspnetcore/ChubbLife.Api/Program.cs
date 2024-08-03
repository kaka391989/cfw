using CFW.Core;
using CFW.EFCore;
using Microsoft.EntityFrameworkCore;
using CFW.OData.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services
    .AddCoreServices()
    .AddControllers()
    .AddGenericOData();

var AllowSpecificOrigins = "AllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowSpecificOrigins,
        builder =>
        {
            builder.WithOrigins("http://localhost",
                "http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowedToAllowWildcardSubdomains();
        });
});

var app = builder.Build();

app.UseCors(AllowSpecificOrigins);

app.UseGenericOData();

app.Run();

