using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EventEase_CLDV6211_ST10444488_.Data;
using Microsoft.Extensions.Azure;
using Microsoft.AspNetCore.Http.Features;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<EventEase_CLDV6211_ST10444488_Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EventEase_CLDV6211_ST10444488_Context1") ?? throw new InvalidOperationException("Connection string 'EventEase_CLDV6211_ST10444488_Context1' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddSingleton<BlobService>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    return new BlobService(configuration);
});
builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["AzureBlobStorage:ConnectionString1:blobServiceUri:blobServiceUri"]!).WithName("AzureBlobStorage:ConnectionString1:blobServiceUri");
    clientBuilder.AddQueueServiceClient(builder.Configuration["AzureBlobStorage:ConnectionString1:blobServiceUri:queueServiceUri"]!).WithName("AzureBlobStorage:ConnectionString1:blobServiceUri");
    clientBuilder.AddTableServiceClient(builder.Configuration["AzureBlobStorage:ConnectionString1:blobServiceUri:tableServiceUri"]!).WithName("AzureBlobStorage:ConnectionString1:blobServiceUri");
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10485760; 
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
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
