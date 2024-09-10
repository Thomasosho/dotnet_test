using Microsoft.AspNetCore.Mvc;
using VendorBiddingApp.Models;
using VendorBiddingApp.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace VendorBiddingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetProjects()
        {
            var projects = _context.Projects
                .Select(p => new 
                { 
                    p.Id, 
                    p.Title, 
                    p.Description 
                })
                .ToList();
            return Ok(projects);
        }

        [HttpGet("{id}")]
        public IActionResult GetProject(Guid id)
        {
            var project = _context.Projects
                .Where(p => p.Id == id)
                .Select(p => new 
                { 
                    p.Id, 
                    p.Title, 
                    p.Description 
                })
                .FirstOrDefault();

            if (project == null)
            {
                return NotFound();
            }
            return Ok(project);
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

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            var response = new
            {
                projectId = project.Id,
                message = "Project Created Successfully"
            };

            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, response);
        }
    }
}
