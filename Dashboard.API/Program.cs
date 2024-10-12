using Dashboard.BLL.Services.AccountService;
using Dashboard.BLL.Services.MailService;
using Dashboard.DAL;
using Dashboard.DAL.Data;
using Dashboard.DAL.Data.Initializer;
using Dashboard.DAL.Models.Identity;
using Dashboard.DAL.Repositories.UserRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add database context
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer("name=Default");
});

// Add identity
builder.Services.AddIdentity<User, Role>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = Settings.PasswordLength;
    options.Password.RequiredUniqueChars = 1;
})
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<AppDbContext>();

// Add services
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IMailService, MailService>();

// Add repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "data")),
    RequestPath = "/files"
});

app.MapControllers();

// ����� ��� ��������� �����
async Task SeedRoles(RoleManager<IdentityRole> roleManager)
{
    if (!await roleManager.RoleExistsAsync("seeder"))
    {
        await roleManager.CreateAsync(new IdentityRole("seeder"));
    }

    if (!await roleManager.RoleExistsAsync("settings"))
    {
        await roleManager.CreateAsync(new IdentityRole("settings"));
    }
}

// ������ SeedRoles ��� ��������� ����� ��� ����� �������
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await SeedRoles(roleManager); // ��������� SeedRoles
}

app.SeedData();

app.Run();
