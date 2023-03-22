using BlazorWebApp.Areas.Identity;
using BlazorWebApp.Data;
using MatBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using PlaylistManagementSystem;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//  :given
//  supplied database connection due to the fact that we create this
//      web app to use Individual accounts
//  Cored retreieves the connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


//  :added
//  code retrieves the playlist management connection string
var connectionStringPlaylist = builder.Configuration.GetConnectionString("PlaylistManagementDB");

//  :given
//  register the supplied connections tring with the IServiceCollection (.Services)
//  Register the connection string for individual accounts
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

//  added:
//  Code the logic to add our class library services to IServiceCollection
//  One could do the registration code here in Program.cs
//  HOWEVER, every time a service class is added, you would be changing this file
//  The implementation of the DBContent and AddTransient(...) code in this example
//        will be done in an extension metod to IServiceCollection
//  The extension method will be code inside the PlaylistManagementSystem class library
//  The extension method will have a paramater: options.UseSqlServer()
builder.Services.AddBackendDependencies(options => 
                    options.UseSqlServer(connectionStringPlaylist));


builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddMatBlazor();
//  placeholder for db context
builder.Services.AddDbContext<ApplicationDbContext>(a =>
    {
        // will add out connection string to chinook
        a.UseSqlServer();
    }
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
