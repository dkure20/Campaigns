using Campaign.Model.Config;
using Campaign.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ICampaign, CampaignService>();
builder.Services.Configure<Connection>(builder.Configuration.GetSection("Connection"));
builder.Services.AddOptions();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();

app.MapControllers();

app.Run();
