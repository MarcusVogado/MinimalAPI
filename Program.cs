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
//Pesquisa
app.MapGet("/pizzas", async (PizzaDb db) => await db.Pizzas.ToListAsync());
app.MapGet("/pizza/{id}", async (PizzaDb db, int id) => await db.Pizzas.FindAsync(id));
//Criar 
app.MapPost("/pizza", async (PizzaDb db, Pizza pizza) =>
{
	await db.Pizzas.AddAsync(pizza);
	await db.SaveChangesAsync();
	return Results.Created($"/pizza/{pizza.Id}", pizza);
});
//Atualizar
app.MapPut("/pizza/{id}", async (PizzaDb db, Pizza updatepizza, int id) =>
{
	var pizza = await db.Pizzas.FindAsync(id);
	if (pizza is null) return Results.NotFound();
	pizza.Name = updatepizza.Name;
	pizza.Description = updatepizza.Description;
	await db.SaveChangesAsync();
	return Results.NoContent();
});
//Deletar
app.MapDelete("/pizza/{id}", async (PizzaDb db, int id) =>
{
	var pizza = await db.Pizzas.FindAsync(id);
	if (pizza is null)
	{
		return Results.NotFound();
	}
	db.Pizzas.Remove(pizza);
	await db.SaveChangesAsync();
	return Results.Ok();
});
app.UseSwagger();
app.UseSwaggerUI(c =>
{
	c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API V1");
});

app.Run();
