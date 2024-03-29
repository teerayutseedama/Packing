using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Packing;
using Packing.Function;
using Packing.Middlewares;
using Packing.Models;
using Packing.vmsPackingDB;
using System.Globalization;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


var constr = builder.Configuration.GetConnectionString("VMSConnectionString");
var constr2 = builder.Configuration.GetConnectionString("VMS2ConnectionString");

// Add services to the container.
//builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddControllersWithViews();
builder.Services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");
builder.Services.AddDbContext<vms_packingContext>(options => options.UseSqlServer(constr, option =>
{
    option.EnableRetryOnFailure();
}));
builder.Services.AddDbContext<VMS_CORE_2Context>(options => options.UseSqlServer(constr2, option =>
{
    option.EnableRetryOnFailure();
}));
builder.Services.AddSession();
//Configure multi langnage
builder.Services.AddLocalization();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
builder.Services.AddScoped<IConfigureInterface, ConfigureInterface>();
builder.Services.AddScoped<ISummaryInterface, SummaryInterface>();
builder.Services.AddScoped<IHistoryInterface, HistoryInterface>(); 
builder.Services.AddScoped<ILoaddingListInterface, LoaddingListInterface>();
builder.Services.AddScoped<IApprovalInterface, ApprovalInterface>(); 
    builder.Services.AddScoped<IMasterDataInterface, MasterDataInterface>();
builder.Services.AddScoped<ILiInterface, LiInterface>();
builder.Services.AddMvc()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(JsonStringLocalizerFactory));
    });

//builder.Services.Configure<RequestLocalizationOptions>(options =>
//{
//    var supportedCultures = new[]
//    {
//        new CultureInfo("en-US"),
//        new CultureInfo("th-TH"),
//    };

//    options.SupportedCultures = supportedCultures;
//    options.SupportedUICultures = supportedCultures;
//});
//*******************************************************
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

//Configure multi langnage
var supportedCultures = new[] { "th-TH","en-US" };
var localizationOptions = new RequestLocalizationOptions()
    .AddSupportedCultures("en-US")
    .AddSupportedUICultures(supportedCultures)
    .SetDefaultCulture("en-US");
app.UseRequestLocalization(localizationOptions);


//*******************************************************

app.UseAuthorization();
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=LI}/{action=Logout}/{id?}");

app.Run();
