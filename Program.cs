using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BeFit.Data;
using BeFit.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => 
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// Ensure admin role exists and seed exercise types
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    
    var adminRoleExists = await roleManager.RoleExistsAsync("Administrator");
    if (!adminRoleExists)
    {
        await roleManager.CreateAsync(new IdentityRole("Administrator"));
    }

    // Create admin user if it doesn't exist
    var adminEmail = "admin@befit.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };
        var result = await userManager.CreateAsync(adminUser, "Admin123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Administrator");
        }
    }
    else
    {
        // Ensure existing admin user has the role
        if (!await userManager.IsInRoleAsync(adminUser, "Administrator"))
        {
            await userManager.AddToRoleAsync(adminUser, "Administrator");
        }
    }

    // Seed exercise types if database is empty
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (!dbContext.ExerciseTypes.Any())
    {
        var exerciseTypes = new List<ExerciseType>
        {
            new ExerciseType { Name = "Wyciskanie sztangi na ławce płaskiej", Description = "Ćwiczenie na klatkę piersiową" },
            new ExerciseType { Name = "Przysiady ze sztangą", Description = "Ćwiczenie na nogi - mięśnie czworogłowe i pośladkowe" },
            new ExerciseType { Name = "Martwy ciąg", Description = "Ćwiczenie na całe ciało, głównie plecy i nogi" },
            new ExerciseType { Name = "Wyciskanie żołnierskie", Description = "Ćwiczenie na barki i triceps" },
            new ExerciseType { Name = "Podciąganie na drążku", Description = "Ćwiczenie na plecy i biceps" },
            new ExerciseType { Name = "Uginanie ramion ze sztangą", Description = "Ćwiczenie na biceps" },
            new ExerciseType { Name = "Wyciskanie francuskie", Description = "Ćwiczenie na triceps" },
            new ExerciseType { Name = "Wiosłowanie sztangą", Description = "Ćwiczenie na plecy" },
            new ExerciseType { Name = "Rozpiętki z hantlami", Description = "Ćwiczenie na klatkę piersiową" },
            new ExerciseType { Name = "Wznosy bokiem z hantlami", Description = "Ćwiczenie na barki" },
            new ExerciseType { Name = "Prostowanie nóg na maszynie", Description = "Ćwiczenie na mięśnie czworogłowe" },
            new ExerciseType { Name = "Uginanie nóg na maszynie", Description = "Ćwiczenie na mięśnie dwugłowe" },
            new ExerciseType { Name = "Wspięcia na palce", Description = "Ćwiczenie na łydki" },
            new ExerciseType { Name = "Brzuszki", Description = "Ćwiczenie na mięśnie brzucha" },
            new ExerciseType { Name = "Deska (plank)", Description = "Ćwiczenie izometryczne na core" },
            new ExerciseType { Name = "Pompki", Description = "Ćwiczenie na klatkę piersiową i triceps" },
            new ExerciseType { Name = "Dipy na poręczach", Description = "Ćwiczenie na triceps i klatkę piersiową" },
            new ExerciseType { Name = "Shrugi ze sztangą", Description = "Ćwiczenie na kaptury" },
            new ExerciseType { Name = "Wyciskanie hantli na skosie", Description = "Ćwiczenie na górną część klatki piersiowej" },
            new ExerciseType { Name = "Przyciąganie linki wyciągu", Description = "Ćwiczenie na plecy i biceps" }
        };

        dbContext.ExerciseTypes.AddRange(exerciseTypes);
        await dbContext.SaveChangesAsync();
    }
}

await app.RunAsync();
