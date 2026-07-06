using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectPilotLite.Api.Data;
using ProjectPilotLite.Core.Dtos;
using ProjectPilotLite.Core.Enums;

namespace ProjectPilotLite.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _db;

    public DashboardController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<DashboardDto>> GetDashboard()
    {
        var dashboard = new DashboardDto
        {
            TotalProjects = await _db.Projects.CountAsync(),
            ProjectsInProgress = await _db.Projects.CountAsync(p => p.Status == ProjectStatus.InProgress),
            ProjectsBlocked = await _db.Projects.CountAsync(p => p.Status == ProjectStatus.Blocked),
            TotalTasks = await _db.Tasks.CountAsync(),
            CompletedTasks = await _db.Tasks.CountAsync(t => t.Status == ProjectTaskStatus.Done),
            SubmittedDeliverables = await _db.Deliverables.CountAsync(d => d.Status == DeliverableStatus.Submitted)
        };

        return Ok(dashboard);
    }
}
