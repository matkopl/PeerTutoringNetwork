using Microsoft.EntityFrameworkCore;
using BL.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(); 
builder.Services.AddDbContext<PeerTutoringNetworkContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("PeerTutoringNetworkConnStr"));
});
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddHttpClient();

var app = builder.Build();

app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=MentorDashboard}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
