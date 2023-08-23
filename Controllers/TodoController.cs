using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MeuToDo.Data;
using MeuToDo.Models;
using MeuToDo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace MeuToDo.Controllers
{
    [ApiController]
    [Route(("v1"))]
    public class TodoController : ControllerBase
    {
        [HttpGet]
        [Route("todos")]
        public async Task<IActionResult> GetAsync(
            [FromServices] AppDbContext context)
        {
            var todo = await context
                .Todos
                .AsNoTracking()
                .ToListAsync();

            return Ok(todo);
        }

        [HttpGet]
        [Route("todos/{id}")]

        public async Task<IActionResult> GetByIdAsync(
            [FromServices] AppDbContext context,
            [FromRoute] int id)
        {
            var todo = await context
                .Todos
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            return todo == null
                ? NotFound()
                : Ok(todo);
        }

        [HttpPost("todos")]
        public async Task<IActionResult> PostAsync(
            [FromServices] AppDbContext context,
            [FromBody] CreateTodoViewModel model)
        {
            if(!ModelState.IsValid)
                return BadRequest();

            var todo = new Todo
            {
                Date = DateTime.Now,
                Done = false,
                Title = model.Title
            };

            try
            {
                await context.Todos.AddAsync(todo);
                await context.SaveChangesAsync();
                return Created($"v1/todos/{todo.Id}", todo);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpPut("todos/{Id}")]
        public async Task<IActionResult> PutAsync(
            [FromServices] AppDbContext context,
            [FromBody] CreateTodoViewModel model,
            [FromRoute] int id)
        {
            if(!ModelState.IsValid)
                return BadRequest();

            var todo = await context.Todos.FirstOrDefaultAsync(x => x.Id == id);

            if (todo == null)
            {
                return NotFound();
            }

            try
            {
                todo.Title = !string.IsNullOrEmpty(model.Title) ? model.Title : todo.Title;
                todo.Done = model.Done ?? todo.Done;
                todo.Date = DateTime.Now;

                context.Todos.Update(todo);
                await context.SaveChangesAsync();
                
                return Ok(todo);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }
        
        
        [HttpDelete("todos/{Id}")]
        public async Task<IActionResult> DeleteAsync(
            [FromServices] AppDbContext context,
            [FromRoute] int id)
        {
            var todo = await context.Todos.FirstOrDefaultAsync(x => x.Id == id);
            
            try
            {
                context.Todos.Remove(todo);
                await context.SaveChangesAsync();
                
                return Ok(todo);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }
    }
}