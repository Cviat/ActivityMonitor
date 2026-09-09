using Server.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ClientStore>();
builder.Services.AddControllers();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();
app.MapControllers();

app.Run();
