using videochamada.frontend.Helper;
using VideoChatApp.FrontEnd.Services;
using VideoChatApp.FrontEnd.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".Usuario.Session";
    options.IdleTimeout = TimeSpan.FromSeconds(6000*60*1); //Uma hora de sessão.
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<IServiceCliente, ServiceCliente>();

builder.Services.AddRazorPages().AddMvcOptions(options =>
    {
        options.MaxModelValidationErrors = 50;
        options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(_ => "campo não informado.");
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
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<VideoChamadaHub>("/videochamadaHub");
});

app.Run();