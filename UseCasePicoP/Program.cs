using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using UseCasePicoP.Services;
using UseCasePicoP.Services.Interface;
using UseCasePicoP.MongoDB;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<MongoDBAccess>();
builder.Services.AddScoped<IItemService, ItemService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();