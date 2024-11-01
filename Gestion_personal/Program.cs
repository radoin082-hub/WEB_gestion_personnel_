using Blazored.Modal;
using Gestion_personal.Components;
using Implementation.App.Employee;
using Infrastructures.Storages.EmployeStorages;
using MudBlazor.Services;
using Services;
using Services.Interfaces;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();
builder.Services.AddSingleton<IConfiguration>(provider =>
	new ConfigurationBuilder().AddJsonFile("appsettings.json").Build());

builder.Services.AddScoped<EmployeStorage>();


builder.Services.AddScoped<IEmployeService, EmployeService>();
builder.Services.AddBlazoredModal();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.Run();
