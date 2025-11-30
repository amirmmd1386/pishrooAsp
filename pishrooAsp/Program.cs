using pishrooAsp.Data;
using pishrooAsp.Models.Newses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using pishrooAsp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// اضافه کردن DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<VisitService>();

builder.Services.AddControllersWithViews();
// ثبت سرویس بازدید
//builder.Services.AddScoped<IVisitService, VisitService>();

//builder.Services.AddControllers();




var app = builder.Build();



// غیرفعال کردن فشرده‌سازی
app.Use(async (context, next) =>
{
	context.Response.Headers.Remove("Content-Encoding");
	await next();
});

app.Use(async (context, next) => {
	if (context.Request.Path.StartsWithSegments("/admin"))
	{
		context.Response.Headers.Remove("Content-Encoding");
		context.Response.Headers.Remove("Vary");
	}
	await next();
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();

// 🔼 ترتیب صحیح Middleware
app.UseStaticFiles(); // برای wwwroot معمولی

app.UseMiddleware<VisitLoggingMiddleware>();


app.UseStaticFiles(new StaticFileOptions
{
	FileProvider = new PhysicalFileProvider(
		Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "admin")),
	RequestPath = "/admin",
	OnPrepareResponse = ctx => {
		ctx.Context.Response.Headers.Remove("Content-Encoding");
		ctx.Context.Response.Headers.Append("Cache-Control", "no-cache");
	}
});


//app.UseMiddleware<VisitLoggingMiddleware>();



app.UseRouting();
app.UseAuthorization();

// 🔼 Localization بعد از Build و قبل از Routing
using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	var langs = db.Langs.ToList();

	var supportedCultures = langs.Select(l => new CultureInfo(l.Code)).ToList();

	var localizationOptions = new RequestLocalizationOptions
	{
		DefaultRequestCulture = new RequestCulture(supportedCultures.FirstOrDefault()?.Name ?? "fa"),
		SupportedCultures = supportedCultures,
		SupportedUICultures = supportedCultures
	};

	localizationOptions.RequestCultureProviders.Insert(0, new RouteDataRequestCultureProvider
	{
		RouteDataStringKey = "culture",
		UIRouteDataStringKey = "culture",
		Options = localizationOptions
	});

	app.UseRequestLocalization(localizationOptions);
}

// 🔼 Map endpoints بعد از Authorization
app.MapControllerRoute(
	name: "default",
	pattern: "{culture=fa}/{controller=Home}/{action=Index}/{id?}");


app.MapControllerRoute(
	name: "productRoute",
	pattern: "{culture=fa}/products/{slug}",
	defaults: new { controller = "Home", action = "ProductDetail" });

app.MapControllerRoute(
	name: "news-details",
	pattern: "{culture}/news/{id}/{title}",
	defaults: new { controller = "News", action = "Details" }
);

app.MapControllerRoute(
	name: "productRoute",
	pattern: "{culture=fa}/products",
	defaults: new { controller = "Home", action = "ProgramList" });

app.MapStaticAssets(); // حالا اینجا درست است



app.Run();