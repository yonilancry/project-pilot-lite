using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectPilotLite.Api.Data;
using ProjectPilotLite.Api.Mapping;
using ProjectPilotLite.Core.Dtos;

namespace ProjectPilotLite.Api.Controllers;

[ApiController]
public class DeliverablesController : ControllerBase
{
    private readonly AppDbContext _db;

    public DeliverablesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("api/projects/{projectId:guid}/deliverables")]
    public async Task<ActionResult<IEnumerable<DeliverableDto>>> GetDeliverables(Guid projectId)
    {
        if (!await _db.Projects.AnyAsync(p => p.Id == projectId))
        {
            return NotFound();
        }

        var deliverables = await _db.Deliverables
            .Where(d => d.ProjectId == projectId)
            .OrderBy(d => d.Name)
            .Select(d => new DeliverableDto
            {
                Id = d.Id,
                Name = d.Name,
                Type = d.Type,
                PathOrUrl = d.PathOrUrl,
                Status = d.Status,
                Comment = d.Comment,
                ProjectId = d.ProjectId
            })
            .ToListAsync();

        return Ok(deliverables);
    }

    [HttpPost("api/projects/{projectId:guid}/deliverables")]
    public async Task<ActionResult<DeliverableDto>> CreateDeliverable(Guid projectId, DeliverableCreateDto dto)
    {
        if (!await _db.Projects.AnyAsync(p => p.Id == projectId))
        {
            return NotFound();
        }

        var deliverable = dto.ToEntity(projectId);
        _db.Deliverables.Add(deliverable);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDeliverables), new { projectId }, deliverable.ToDto());
    }

    [HttpPatch("api/deliverables/{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, DeliverableStatusUpdateDto dto)
    {
        var deliverable = await _db.Deliverables.FindAsync(id);
        if (deliverable is null)
        {
            return NotFound();
        }

        deliverable.Status = dto.Status;
        if (dto.Comment is not null)
        {
            deliverable.Comment = dto.Comment;
        }
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
