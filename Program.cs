/*var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

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
*//*


using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Newtonsoft.Json.Serialization;

namespace awsfx // Replace with your actual namespace
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddMemoryCache();
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                // Use the default property (Pascal) casing
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
            });

            // Build the application
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseCors("MyPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireCors("MyPolicy");
            });

            app.Run();
        }
    }
}


*/






/*
using Microsoft.AspNetCore.ResponseCompression;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "MyPolicy";
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    // Use the default property (Pascal) casing
    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
});
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                        builder => {
                            builder.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                        });
});

builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();

builder.Services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Optimal);
builder.Services.AddResponseCompression();

var app = builder.Build();

//Register Syncfusion license
string licenseKey = string.Empty;
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(licenseKey);

app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthorization();

app.UseResponseCompression();
app.MapControllers();

app.Run();*/

/*
using Microsoft.AspNetCore.ResponseCompression;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "MyPolicy";
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    // Use the default property (Pascal) casing
    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
});
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                        builder => {
                            builder.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                        });
});

builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();

builder.Services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Optimal);
builder.Services.AddResponseCompression();

var app = builder.Build();

//Register Syncfusion license
string licenseKey = string.Empty;
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(licenseKey);

app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthorization();

app.UseResponseCompression();
app.MapControllers();

app.Run();*/


/*
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.ResponseCompression;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "MyPolicy";

builder.Services.AddControllers().AddJsonOptions(options =>
{
    // Use Pascal casing for JSON property names
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();

builder.Services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Optimal);
builder.Services.AddResponseCompression();

var app = builder.Build();

// Register Syncfusion license
string licenseKey = string.Empty;
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(licenseKey);

app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthorization();

app.UseResponseCompression();
app.MapControllers();

app.Run();*/

/*
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "MyPolicy";

builder.Services.AddControllers()
    .ConfigureJsonOptions(options =>
    {
        // Use Pascal casing for JSON property names
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();

builder.Services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Optimal);
builder.Services.AddResponseCompression();

var app = builder.Build();

// Register Syncfusion license
string licenseKey = string.Empty;
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(licenseKey);

app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthorization();

app.UseResponseCompression();
app.MapControllers();

app.Run();
*/

using Microsoft.AspNetCore.ResponseCompression;

public class Program
{

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddMemoryCache();
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            // Use the default property (Pascal) casing
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

        builder.Services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<GzipCompressionProvider>();
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
        });

        // Build the application
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();
        app.UseCors("MyPolicy");

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers().RequireCors("MyPolicy");
        });

        app.Run();
    }
}