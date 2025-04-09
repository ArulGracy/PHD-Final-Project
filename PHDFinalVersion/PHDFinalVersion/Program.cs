using Microsoft.EntityFrameworkCore;
using PHDFinalVersion.AppDBContext;
using PHDFinalVersion.ChatHubb;
using PHDFinalVersion.Email_Verification;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using PHDFinalVersion.ChatHub;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#region Add Session Service
// Enable session for the application
builder.Services.AddSession();
#endregion

#region Add SignalR Service
// Add SignalR for real-time web functionality
builder.Services.AddSignalR();
#endregion

#region Add Controllers and Views
// Add MVC controllers and views
builder.Services.AddControllersWithViews();
#endregion

#region Add Transient Services
// Add transient services for dependency injection
builder.Services.AddTransient<SendAndReceiveMessage>();
#endregion

#region Add Database Context Service
// Add DbContext for the ApplicationDBContext
builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
#endregion

#region Add Singleton Service for Email Verification
// Add EmailVerification service as a singleton
builder.Services.AddSingleton<IEmailVerification, EmailVerifications>();
#endregion

#region Add Json Options for Controllers
// Configure JSON serialization options for controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Keep the original property names in JSON (no camel case)
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.

#region Exception Handling in Production
if (!app.Environment.IsDevelopment())
{
    // Set up exception handling for production environment
    app.UseExceptionHandler("/Home/Error");
    // Enable HSTS (HTTP Strict Transport Security) in production
    app.UseHsts();
}
#endregion

#region Middleware Configuration
// Use HTTPS Redirection to ensure secure connection
app.UseHttpsRedirection();
// Serve static files (e.g., CSS, JavaScript, images, etc.)
app.UseStaticFiles();

// Enable session state management
app.UseSession();

// Configure routing for incoming requests
app.UseRouting();

// Enable authorization (this can be customized based on your needs)
app.UseAuthorization();
#endregion

#region SignalR Hub Mapping
// Map SignalR Hub to the /chatHub endpoint for real-time communication
app.UseWebSockets();
app.MapHub<ChatHubCommunication>("/PHDFinalVersion/chatHub");
#endregion

#region MVC Route Mapping
// Set up the default route for controllers and views
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=LandingPage}/{id?}");
#endregion

// Run the application
app.Run();
