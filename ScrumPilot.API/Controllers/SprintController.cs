using Microsoft.AspNetCore.Mvc;
using ScrumPilot.API.Services;
using ScrumPilot.Data.Repositories;
using ScrumPilot.Shared.Models;

namespace ScrumPilot.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SprintController : ControllerBase
    {
        private readonly ISprintService _sprintService;
        private readonly ISprintRepository _sprintRepository;

        public SprintController(ISprintService sprintService, ISprintRepository sprintRepository)
        {
            _sprintService = sprintService;
            _sprintRepository = sprintRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sprint>>> GetAllSprints()
        {
            var sprints = await _sprintService.GetAllSprintsAsync();
            return Ok(sprints);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Sprint>> GetById(int id)
        {
            var sprint = await _sprintRepository.GetByIdAsync(id);
            if (sprint is null) return NotFound();
            return Ok(sprint);
        }

        [HttpPost]
        public async Task<ActionResult<Sprint>> Create([FromBody] Sprint sprint)
        {
            var created = await _sprintRepository.AddAsync(sprint);
            return CreatedAtAction(nameof(GetById), new { id = created.SprintId }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Sprint>> Update(int id, [FromBody] Sprint sprint)
        {
            if (id != sprint.SprintId) return BadRequest();
            var updated = await _sprintRepository.UpdateAsync(sprint);
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _sprintRepository.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
