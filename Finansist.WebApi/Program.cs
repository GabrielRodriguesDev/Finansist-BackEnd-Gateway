using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
// docker build -t gabrielrodriguesdev/finansist-backend-gateway:latest .
// docker push gabrielrodriguesdev/finansist-backend-gateway
var builder = WebApplication.CreateBuilder(args);

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region  Ocelot
var configurationFile = "ocelot.json";
if (environment == "Development") configurationFile = "ocelot-dev.json";
builder.Configuration.AddJsonFile(configurationFile, optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration)
    .AddPolly();

#endregion
#region CORS
builder.Services.AddCors(o => o.AddPolicy("CorsPolicy", b =>
            {
                b.AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowAnyOrigin()
                    .AllowCredentials()
                    .WithOrigins("https://localhost:7144")
                    .WithOrigins("https://localhost:4200")
                    .WithOrigins("http://localhost:4200");
            }));
#endregion


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseWebSockets();
app.MapControllers();
app.UseOcelot().Wait();
app.Run();
