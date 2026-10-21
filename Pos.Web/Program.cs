var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var apiBaseUIrl = builder.Configuration["Api:BaseUrl"] ?? throw new Exception("Api:BaseUrl no existe!");
builder.Services.AddHttpClient("PosApi", c => 
{
    c.BaseAddress = new Uri(apiBaseUIrl);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Pos}/{action=Index}/{id?}");

app.Run();
