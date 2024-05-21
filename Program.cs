using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Http.Features;
using KingUploader;
using KingUploader.Core.Application.Interfaces.Context;
using KingUploader.Core.Persistence;
using Microsoft.EntityFrameworkCore;
using KingUploader.Core.Application.Interfaces.Facades;
using KingUploader.Core.Application.Services.Files.FacadePattern;
using KingUploader.Core.Application.Services.MultiFiles.FacadePattern;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IDataBaseContext, DataBaseContext>();
builder.Services.AddScoped<IFilesFacade, FilesFacade>();
builder.Services.AddScoped<IMultiFilesFacade, FacadeMultiFiles>();

var ConnStr = builder.Configuration.GetConnectionString("ConnStr");
builder.Services.AddEntityFrameworkSqlServer().AddDbContext<DataBaseContext>(x => x.UseSqlServer(ConnStr));



builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = long.MaxValue;
});

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
