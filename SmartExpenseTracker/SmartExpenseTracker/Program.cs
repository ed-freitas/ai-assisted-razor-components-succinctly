using SmartExpenseTracker.Client.Pages;
using SmartExpenseTracker.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Register expense service
builder.Services.AddSingleton<SmartExpenseTracker.Services.IExpenseService, SmartExpenseTracker.Services.ExpenseService>();
builder.Services.AddSingleton<SmartExpenseTracker.Services.AIService>();
builder.Services.AddSingleton<SmartExpenseTracker.Services.IAIService>(sp => sp.GetRequiredService<SmartExpenseTracker.Services.AIService>());
builder.Services.AddSingleton<SmartExpenseTracker.Services.IAICategorySuggestionService>(sp => sp.GetRequiredService<SmartExpenseTracker.Services.AIService>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(SmartExpenseTracker.Client._Imports).Assembly);

app.Run();
