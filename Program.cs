using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SistemaLeilao_web.Data;
using SistemaLeilao_web.Settings;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

builder.Services.AddControllersWithViews();

var app = builder.Build();

var apiSettings = app.Services.GetRequiredService<IOptions<ApiSettings>>().Value;
using (var httpClient = new HttpClient { BaseAddress = new Uri(apiSettings.BaseUrl) })
{
    try
    {
        var response = await httpClient.GetAsync("api/health/public");
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Não foi possível conectar à API. Status Code: {response.StatusCode}");
        }
        else
        {
            Console.WriteLine("✅ Conexão com a API estabelecida com sucesso.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Erro ao tentar conectar à API: {ex.Message}");

        app.MapGet("/", async context =>
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync($@"
            {{
                ""error"": ""Não foi possível conectar à API."",
                ""details"": ""{ex.Message}""
            }}");
        });

        app.Run();
        return;
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Usuario}/{action=Login}/{id?}");


app.Run();
