using Microsoft.AspNetCore.Mvc;
using TaskManagerServer.Model;
using TaskManagerServer.Services;

namespace TaskManagerServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly TaskService _taskService;

        public TaskController(TaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<ActionResult<List<MyTask>>> GetTasks()
        {
            try
            {
                var tasks = await _taskService.GetTasksAsync();
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving tasks.", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MyTask>> GetTask(string id)
        {
            try
            {
                var tasks = await _taskService.GetTasksAsync();
                var task = tasks.FirstOrDefault(t => t.Id == id);

                if (task == null)
                {
                    return NotFound(new { message = $"Task with ID {id} not found." });
                }

                return Ok(task);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the task.", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<MyTask>> AddTask([FromBody] MyTask task)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdTask = await _taskService.AddTaskAsync(task);
                return CreatedAtAction(nameof(GetTask), new { id = createdTask.Id }, createdTask);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding the task.", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<MyTask>> UpdateTask(string id, [FromBody] MyTask task)
        {
            try
            {

                var updatedTask = await _taskService.UpdateTaskAsync(id, task);

                if (updatedTask == null)
                {
                    return NotFound(new { message = $"Task with ID {id} not found." });
                }

                return Ok(updatedTask);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the task.", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTask(string id)
        {
            try
            {
                var result = await _taskService.DeleteTaskAsync(id);

                if (!result)
                {
                    return NotFound(new { message = $"Task with ID {id} not found." });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the task.", error = ex.Message });
            }
        }
    }
}
