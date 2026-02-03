using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using pishrooAsp.Data;
using pishrooAsp.Middlewares;
using pishrooAsp.Models.Newses;
using pishrooAsp.Services;
using System.Globalization;
using System.Text.Encodings.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// اضافه کردن DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication("CookieAuth")
	.AddCookie("CookieAuth", options =>
	{
		options.Cookie.Name = "AuthCookie";
		options.LoginPath = "/Auth/Login";
		options.LogoutPath = "/Auth/Logout";
		options.AccessDeniedPath = "/Auth/AccessDenied";
		options.ExpireTimeSpan = TimeSpan.FromDays(30);
		options.SlidingExpiration = true;
	});
builder.Services.AddScoped<VisitService>();
// Program.cs (یا Startup.cs)
builder.Services.AddScoped<IGroupSmsService, GroupSmsService>();
// Add Services
builder.Services.AddScoped<ITemplateService, TemplateService>();
// Add Persian Calendar (اگر نیاز دارید)

//builder.Services.AddPersianDateTimePicker();

builder.Services.AddControllersWithViews();
// ثبت سرویس بازدید
//builder.Services.AddScoped<IVisitService, VisitService>();


//builder.Services.AddControllers();
builder.Services.AddScoped<ISmsSender, KavenegarSmsSender>();

builder.Services.AddHttpContextAccessor();

//builder.Services.AddScoped<ICultureService, CultureService>();

// در Startup.cs یا Program.cs
builder.Services.AddControllersWithViews()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
		options.JsonSerializerOptions.WriteIndented = true;
	});

var app = builder.Build();

app.UseStatusCodePages(context =>
{
	if (context.HttpContext.Response.StatusCode == 404)
	{
		// گرفتن culture فعلی
		var culture = context.HttpContext.GetRouteValue("culture")?.ToString() ?? "fa";
		context.HttpContext.Response.Redirect($"/404");
		return Task.CompletedTask;
	}
	return Task.CompletedTask;
});

// روش پیشرفته‌تر برای مدیریت خطاهای 404
app.UseStatusCodePagesWithReExecute("/Error/NotFound404", "?statusCode={0}");

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
	app.UseExceptionHandler("/Home/Index");
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
//app.UseMiddleware<CultureValidationMiddleware>();

app.UseAuthorization();

// 🔼 Localization بعد از Build و قبل از Routing
using (var scope = app.Services.CreateScope())
{

	var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	var langs = db.Langs.ToList();
	DbSeeder.Seed(db);


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
	pattern: "{culture:regex(^(fa|en|ar|tr)$)}/{controller=Home}/{action=Index}/{id?}",
	defaults: new { culture = "fa" });


app.MapControllerRoute(
	name: "productRoute",
	pattern: "{culture:regex(^(fa|en|ar|tr)$)}/products/{slug}",
	defaults: new { controller = "Home", action = "ProductDetail" });

app.MapControllerRoute(
	name: "news-details",
	pattern: "{culture:regex(^(fa|en|ar|tr)$)}/news/{id}/{title}",
	defaults: new { controller = "News", action = "Details" }
);

app.MapControllerRoute(
	name: "productRoute",
	pattern: "{culture:regex(^(fa|en|ar|tr)$)}/products",
	defaults: new { controller = "Home", action = "ProgramList" });

app.MapControllerRoute(
	name: "newsRoutes",
	pattern: "{culture:regex(^(fa|en|ar|tr)$)}/Newses",
	defaults: new { controller = "Home", action = "NewsList" });

app.MapControllerRoute(
	name: "newsRoute",
	pattern: "{culture=fa}/newsList",
	defaults: new { controller = "News", action = "index" });

// در Program.cs
app.MapControllerRoute(
	name: "invoiceTracking",
	pattern: "{culture=fa}/invoice/track",
	defaults: new { controller = "PublicInvoice", action = "Index" }
);

// یا برای URL زیباتر:
app.MapControllerRoute(
	name: "publicInvoice",
	pattern: "{culture=fa}/track/{action=Index}",
	defaults: new { controller = "PublicInvoice" }
);

app.MapStaticAssets(); // حالا اینجا درست است



app.Run();
