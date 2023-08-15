using AspNetDB;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// получаем строку подключени€ из файла конфигурации
string? connection = builder.Configuration.GetConnectionString("DefaultConnection");
// добавл€ем контекст ApplicationContext в качестве сервиса в приложение
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/users", async (ApplicationContext db) => await db.Users.ToListAsync());

// определим конечные точки
app.MapGet("/api/users/{id:int}", async (int id, ApplicationContext db) =>
{
    // получаем пользовател€ по id
    User? user = await db.Users.FirstOrDefaultAsync(x => x.Id == id);

    // елси ёзер не найден, то отпр. стат. код и сообщ. об ошибке
    if (user == null)
    {
        return Results.NotFound(new { message = "ѕользователь не найден!" });
    }

    // если пользователь найден, то отпр его в джейсоне
    return Results.Json(user);
});

app.MapDelete("/api/user/{id:int}", async (int id, ApplicationContext db) =>
{
    User? user = await db.Users.FirstOrDefaultAsync(x => x.Id == id);
    if (user == null)
    {
        return Results.NotFound(new { message = "ѕользователь не найден"});
    }

    db.Users.Remove(user);
    await db.SaveChangesAsync();
    return Results.Json(user);
});

app.MapPost("/api/users", async (User user, ApplicationContext db) =>
{
    // добавл€ем пользовател€ в массив
    await db.Users.AddAsync(user);
    await db.SaveChangesAsync();
    return user;
});

app.MapPut("/api/users", async (User userData, ApplicationContext db) =>
{
    // получаем пользовател€ по id
    var user = await db.Users.FirstOrDefaultAsync( x => x.Id == userData.Id);
    if (user == null)
    {
        return Results.NotFound(new { message = "ѕользователь не найден!" });
    }

    user.Age = userData.Age;
    user.Name = userData.Name;
    await db.SaveChangesAsync();
    return Results.Json(user);
});


app.Run();
