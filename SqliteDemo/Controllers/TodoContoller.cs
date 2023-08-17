using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SqliteDemo.Models;


namespace SqliteDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoContoller : ControllerBase
    {
        private readonly TodoContext _context;
        public TodoContoller(TodoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            return await _context.TodoItems.ToListAsync();
        
        }

        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostToDoItem(TodoItem item)
        {
            _context.TodoItems.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTodoItems),new {id=item.Id},item);
        }
    }
}
