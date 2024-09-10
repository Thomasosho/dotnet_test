using Microsoft.AspNetCore.Mvc;
using VendorBiddingApp.Models;
using VendorBiddingApp.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace VendorBiddingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly ProjectService _projectService;

        public ProjectsController(ProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            var projects = await _projectService.GetProjectsAsync();
            var projectDtos = projects.Select(p => new
            {
                p.Id,
                p.Title,
                p.Description
            }).ToList();

            return Ok(projectDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject(Guid id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            var projectDto = new
            {
                project.Id,
                project.Title,
                project.Description
            };

            return Ok(projectDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] ProjectDto projectDto)
        {
            if (projectDto == null)
            {
                return BadRequest("Project data is null.");
            }

            var project = new Project
            {
                Id = Guid.NewGuid(),
                Title = projectDto.Title,
                Description = projectDto.Description
            };

            await _projectService.CreateProjectAsync(project);

            var response = new
            {
                projectId = project.Id,
                message = "Project Created Successfully"
            };

            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, response);
        }
    }
}
