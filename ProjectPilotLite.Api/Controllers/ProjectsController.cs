using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectPilotLite.Api.Data;
using ProjectPilotLite.Api.Mapping;
using ProjectPilotLite.Core.Dtos;

namespace ProjectPilotLite.Api.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProjectsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectSummaryDto>>> GetProjects()
    {
        var projects = await _db.Projects
            .OrderBy(p => p.Name)
            .Select(p => new ProjectSummaryDto
            {
                Id = p.Id,
                Name = p.Name,
                Owner = p.Owner,
                Status = p.Status,
                StartDate = p.StartDate,
                Deadline = p.Deadline,
                TaskCount = p.Tasks.Count,
                DeliverableCount = p.Deliverables.Count
            })
            .ToListAsync();

        return Ok(projects);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProjectDetailDto>> GetProject(Guid id)
    {
        var project = await _db.Projects
            .Include(p => p.Tasks)
            .Include(p => p.Deliverables)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project is null)
        {
            return NotFound();
        }

        return Ok(project.ToDetailDto());
    }

    [HttpPost]
    public async Task<ActionResult<ProjectDetailDto>> CreateProject(ProjectCreateDto dto)
    {
        var project = dto.ToEntity();
        _db.Projects.Add(project);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project.ToDetailDto());
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, ProjectStatusUpdateDto dto)
    {
        var project = await _db.Projects.FindAsync(id);
        if (project is null)
        {
            return NotFound();
        }

        project.Status = dto.Status;
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
