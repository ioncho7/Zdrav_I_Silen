using Microsoft.EntityFrameworkCore;
using Zdrav_I_SIlen.Data;
using Zdrav_I_SIlen.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

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

static void SeedData(ApplicationDbContext context)
{
    // Check if data already exists
    if (context.Categories.Any() || context.Products.Any())
        return;

    // Add Categories
    var categories = new List<Category>
    {
        new Category { Id = Guid.NewGuid(), Name = "Витамини", DisplayOrder = 1 },
        new Category { Id = Guid.NewGuid(), Name = "Минерали", DisplayOrder = 2 },
        new Category { Id = Guid.NewGuid(), Name = "Протеини", DisplayOrder = 3 },
        new Category { Id = Guid.NewGuid(), Name = "Билки", DisplayOrder = 4 }
    };

    context.Categories.AddRange(categories);
    context.SaveChanges();

    // Add Products
    var products = new List<Product>
    {
        new Product 
        { 
            Id = Guid.NewGuid(),
            Name = "Витамин C 1000mg", 
            Description = "Мощен антиоксидант за силен имунитет",
            Size = "100 таблетки",
            UnitPrice = 25.99m,
            Quantity = 50,
            CategoryId = categories[0].Id,
            ImagePath = "/images/vitamin-c.jpg"
        },
        new Product 
        { 
            Id = Guid.NewGuid(),
            Name = "Мултивитамини Комплекс", 
            Description = "Пълен комплекс от основни витамини и минерали за ежедневна подкрепа на организма. Съдържа витамини A, B-комплекс, C, D3, E и K, заедно с важни минерали като цинк, желязо и магнезий. Подходящ за активни хора и всички, които искат да поддържат оптимално здраве.",
            Size = "90 таблетки",
            UnitPrice = 32.99m,
            Quantity = 40,
            CategoryId = categories[0].Id,
            ImagePath = "/images/vitamini.jpg"
        },
        new Product 
        { 
            Id = Guid.NewGuid(),
            Name = "Магнезий 400mg", 
            Description = "За здрави мускули и нервна система",
            Size = "60 капсули",
            UnitPrice = 19.99m,
            Quantity = 30,
            CategoryId = categories[1].Id,
            ImagePath = "/images/magnesium.jpg"
        },
        new Product 
        { 
            Id = Guid.NewGuid(),
            Name = "Whey Protein", 
            Description = "Висококачествен суроватъчен протеин",
            Size = "2kg",
            UnitPrice = 89.99m,
            Quantity = 15,
            CategoryId = categories[2].Id,
            ImagePath = "/images/whey-protein.jpg"
        }
    };

    context.Products.AddRange(products);
    context.SaveChanges();
}
