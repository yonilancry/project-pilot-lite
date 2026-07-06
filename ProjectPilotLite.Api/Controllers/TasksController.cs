using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectPilotLite.Api.Data;
using ProjectPilotLite.Api.Mapping;
using ProjectPilotLite.Core.Dtos;

namespace ProjectPilotLite.Api.Controllers;

[ApiController]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _db;

    public TasksController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("api/projects/{projectId:guid}/tasks")]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks(Guid projectId)
    {
        if (!await _db.Projects.AnyAsync(p => p.Id == projectId))
        {
            return NotFound();
        }

        var tasks = await _db.Tasks
            .Where(t => t.ProjectId == projectId)
            .OrderBy(t => t.Title)
            .Select(t => new TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Priority = t.Priority,
                Status = t.Status,
                ProjectId = t.ProjectId
            })
            .ToListAsync();

        return Ok(tasks);
    }

    [HttpPost("api/projects/{projectId:guid}/tasks")]
    public async Task<ActionResult<TaskDto>> CreateTask(Guid projectId, TaskCreateDto dto)
    {
        if (!await _db.Projects.AnyAsync(p => p.Id == projectId))
        {
            return NotFound();
        }

        var task = dto.ToEntity(projectId);
        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTasks), new { projectId }, task.ToDto());
    }

    [HttpPatch("api/tasks/{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, TaskStatusUpdateDto dto)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task is null)
        {
            return NotFound();
        }

        task.Status = dto.Status;
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
