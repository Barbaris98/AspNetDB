using AspNetDB;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// получаем строку подключения из файла конфигурации
string? connection = builder.Configuration.GetConnectionString("DefaultConnection");
// добавляем контекст ApplicationContext в качестве сервиса в приложение
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/users", async (ApplicationContext db) => await db.Users.ToListAsync());

app.MapGet("/api/users/{id:int}", async (int id, ApplicationContext db) =>
{
    // получаем пользователя по id
    User? user = await db.Users.FirstOrDefaultAsync(x => x.Id == id);

    // елси Юзер не найден, то отпр. стат. код и сообщ. об ошибке
    if (user == null)
    {
        return Results.NotFound(new { message = "Пользователь не найден!" });
    }

    // если пользователь найден, то отпр его в джейсоне
    return Results.Json(user);
});

app.MapDelete("/api/user/{id:int}", async (int id, ApplicationContext db) =>
{
    User? user = await db.Users.FirstOrDefaultAsync(x => x.Id == id);
    if (user == null)
    {
        return Results.NotFound(new { message = "Пользователь не найден"});
    }

    db.Users.Remove(user);
    await db.SaveChangesAsync();
    return Results.Json(user);
});

app.MapPost("/api/users", async (User user, ApplicationContext db) =>
{
    // добавляем пользователя в массив
    await db.Users.AddAsync(user);
    await db.SaveChangesAsync();
    return user;
});

app.MapPut("/api/users", async (User userData, ApplicationContext db) =>
{
    // получаем пользователя по id
    var user = await db.Users.FirstOrDefaultAsync( x => x.Id == userData.Id);
    if (user == null)
    {
        return Results.NotFound(new { message = "Пользователь не найден!" });
    }

    user.Age = userData.Age;
    user.Name = userData.Name;
    await db.SaveChangesAsync();
    return Results.Json(user);

});


app.Run();
