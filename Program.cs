using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using PizzaStore.Models;
using PizzaStore.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<PizzaDb>(options => options.UseInMemoryDatabase("items"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "Todo API", Description = "Keep track of your tasks", Version = "v1" });
});

var app = builder.Build();

app.MapGet("/pizzas", async (PizzaDb db) => await db.Pizzas.ToListAsync());
app.MapPost("/pizza", async (PizzaDb db, Pizza pizza) =>
{
	await db.Pizzas.AddAsync(pizza);
	await db.SaveChangesAsync();
	return Results.Created($"/pizza/{pizza.Id}", pizza);
});
app.UseSwagger();
app.UseSwaggerUI(c =>
{
	c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API V1");
});

app.Run();
