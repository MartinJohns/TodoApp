using System.Collections.Generic;
using Microsoft.AspNet.Mvc;
using TodoApp.Service.Models;
using TodoApp.Service.Repositories;

namespace TodoApp.Service.Controllers
{
    [ApiExplorerSettings(GroupName = "Todo")]
    [Route("todo")]
    public class TodoController : Controller
    {
        private readonly ITodoRepository _todoRepository;

        public TodoController(ITodoRepository todoRepository)
        {
            _todoRepository = todoRepository;
        }

        [HttpGet, Route("")]
        [Produces(typeof(IEnumerable<Todo>))]
        public IActionResult GetAll()
        {
            var items = _todoRepository.GetAll();
            return Ok(items);
        }

        [HttpGet, Route("{key}")]
        [Produces(typeof(Todo))]
        public IActionResult Get(string key)
        {
            var item = _todoRepository.Get(key);
            if (item == null)
                return HttpNotFound();

            return Ok(item);
        }

        [HttpPost, Route("")]
        public IActionResult Add([FromBody]Todo item)
        {
            var result = _todoRepository.Add(item.Id, item);
            if (!result)
                return HttpBadRequest();

            return Ok();
        }

        [HttpDelete, Route("{key}")]
        public IActionResult Delete(string key)
        {
            var result = _todoRepository.Delete(key);
            if (!result)
                return HttpBadRequest();

            return Ok();
        }

        [HttpPut, Route("{key}")]
        public IActionResult Update(string key, [FromBody]Todo item)
        {
            var result = _todoRepository.Update(key, item);
            if (!result)
                return HttpBadRequest();

            return Ok();
        }
    }
}
