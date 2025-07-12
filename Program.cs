using Microsoft.EntityFrameworkCore;
using Zdrav_I_SIlen.Data;
using Zdrav_I_SIlen.Models;

var builder = WebApplication.CreateBuilder(args);

// Add local configuration file for developer-specific settings
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register Email Service
builder.Services.AddTransient<Zdrav_I_SIlen.Services.IEmailService, Zdrav_I_SIlen.Services.EmailService>();

// Configure SQL Server with connection pooling and retry logic
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions =>
        {
            sqlServerOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
            sqlServerOptions.CommandTimeout(300); // 5 minutes timeout
        });
    
    // Enable sensitive data logging in development
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    SeedData(context);
}

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
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Seed data method
static void SeedData(ApplicationDbContext context)
{
    try
    {
        // Ensure database is created
        context.Database.EnsureCreated();

        // Seed categories if they don't exist
        if (!context.Categories.Any())
        {
            var categories = new[]
            {
                new Category { Name = "Витамини", DisplayOrder = 1 },
                new Category { Name = "Протеини", DisplayOrder = 2 },
                new Category { Name = "Минерали", DisplayOrder = 3 },
                new Category { Name = "Билки", DisplayOrder = 4 },
                new Category { Name = "Спортни добавки", DisplayOrder = 5 }
            };

            context.Categories.AddRange(categories);
            context.SaveChanges();
        }

        // Seed products if they don't exist
        if (!context.Products.Any())
        {
            var categories = context.Categories.ToList();
            var products = new[]
            {
                new Product
                {
                    Name = "Витамин C 1000mg",
                    Description = "Висококачествен витамин C за укрепване на имунната система",
                    Size = "100 таблетки",
                    UnitPrice = 25.99m,
                    Quantity = 50,
                    CategoryId = categories.First(c => c.Name == "Витамини").Id,
                    ImagePath = "https://via.placeholder.com/300x300?text=Vitamin+C"
                },
                new Product
                {
                    Name = "Whey Protein",
                    Description = "Суроватъчен протеин за мускулен растеж",
                    Size = "2кг",
                    UnitPrice = 89.99m,
                    Quantity = 30,
                    CategoryId = categories.First(c => c.Name == "Протеини").Id,
                    ImagePath = "https://via.placeholder.com/300x300?text=Whey+Protein"
                },
                new Product
                {
                    Name = "Магнезий",
                    Description = "Магнезий за здрави кости и мускули",
                    Size = "60 капсули",
                    UnitPrice = 19.99m,
                    Quantity = 40,
                    CategoryId = categories.First(c => c.Name == "Минерали").Id,
                    ImagePath = "https://via.placeholder.com/300x300?text=Magnesium"
                }
            };

            context.Products.AddRange(products);
            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        // Log the error (in production, use proper logging)
        Console.WriteLine($"Error seeding data: {ex.Message}");
    }
}
