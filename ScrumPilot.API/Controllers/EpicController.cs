using Microsoft.AspNetCore.Mvc;
using ScrumPilot.Data.Repositories;
using ScrumPilot.Shared.Models;

namespace ScrumPilot.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EpicController : ControllerBase
    {
        private readonly IEpicRepository _epicRepository;

        public EpicController(IEpicRepository epicRepository)
        {
            _epicRepository = epicRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Epic>>> GetAll()
        {
            var epics = await _epicRepository.GetAllAsync();
            return Ok(epics);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Epic>> GetById(int id)
        {
            var epic = await _epicRepository.GetByIdAsync(id);
            if (epic is null) return NotFound();
            return Ok(epic);
        }

        [HttpPost]
        public async Task<ActionResult<Epic>> Create([FromBody] Epic epic)
        {
            var created = await _epicRepository.AddAsync(epic);
            return CreatedAtAction(nameof(GetById), new { id = created.EpicId }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Epic>> Update(int id, [FromBody] Epic epic)
        {
            if (id != epic.EpicId) return BadRequest();
            var updated = await _epicRepository.UpdateAsync(epic);
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _epicRepository.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
