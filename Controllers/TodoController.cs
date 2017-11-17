using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using System.Linq;

namespace TodoApi.Controllers
{
    [Route("api/Todo")]
    public class TodoController : Controller
    {
        /**
        O construtor usa a Injeção de Dependência para injetar o contexto de banco de dados
        (TodoContext) no controlador. O contexto de banco de dados é usado em cada um dos métodos CRUD
         no controlador.
        O construtor adiciona um item no banco de dados em memória, caso ele não exista.
         */
        private readonly TodoContext _context;

        public TodoController(TodoContext context)
        {
            _context = context;
            if (_context.TodoItems.Count() == 0)
            {
                _context.TodoItems.Add(new TodoItem {Name = "Filipe"});
                _context.SaveChanges();
            }
        }

        //GET /api/todo
        [HttpGet]
        public IEnumerable<TodoItem> GetAll()
        {
            return _context.TodoItems.ToList();
        }

        //GET /api/todo/{id}
        //Name = "GetTodo" cria uma rota nomeada 
        [HttpGet("{id}", Name = "GetTodo")]
        public IActionResult GetById(long id)
        {
            var item = _context.TodoItems.FirstOrDefault(t => t.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        //O atributo [FromBody] informa ao MVC para obter o valor
        //do item de tarefas pendentes do corpo da solicitação HTTP
        [HttpPost]
        public IActionResult Create([FromBody] TodoItem item)
        {
            if (item == null)
            {
                return BadRequest();
            }
            _context.TodoItems.Add(item);
            _context.SaveChanges();

            /*O método CreatedAtRoute retorna uma resposta 201,
            que é a resposta padrão para um método HTTP POST que
            cria um novo recurso no servidor. CreatedAtRoute também
            adiciona um cabeçalho Local à resposta
             */
            return CreatedAtRoute("GetToto", new {id = item.Id}, item);
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] TodoItem item)
        {
            if (item == null || item.Id != id)
            {
                return BadRequest();
            }

            var todo = _context.TodoItems.FirstOrDefault(t => t.Id == id);
            if (todo == null)
            {
                return NotFound();
            }

            todo.IsComplete = item.IsComplete;
            todo.Name = item.Name;

            _context.TodoItems.Update(todo);
            _context.SaveChanges();
            return new NoContentResult();
        }

        public IActionResult Delete(long id)
        {
            var todo = _context.TodoItems.FirstOrDefault(t => t.Id == id);
            if (todo == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todo);
            _context.SaveChanges();
            return new NoContentResult();
        }
    }
}