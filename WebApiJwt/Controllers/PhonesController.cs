using Microsoft.AspNetCore.Mvc;
using MediatR;
using UseCases.API.Dto;
using UseCases.API.Exceptions;
using UseCases.API.Phones.Commands;
using UseCases.API.Phones.Queries;
using Microsoft.AspNetCore.Authorization;
using UseCases.API.Authentication;
//using WebApiJwt.Auth;

namespace WebApiJwt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhonesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PhonesController(IMediator mediator) => _mediator = mediator;
        [HttpGet]
        public async Task<IEnumerable<PhoneDto>> GetPhones() => await _mediator.Send(new GetPhones.Query());
        [HttpGet("{id}")]
        public async Task<PhoneDto?> GetPhone(int id) => await _mediator.Send(new GetPhoneById.Query() { Id = id });
        [Authorize(Roles = UserRoles.User)]
        [HttpPost]
        public async Task<ActionResult> CreatePhone(PhoneDto phoneDto)
        {
            if (phoneDto == null)
            {
                throw new EntityNotFoundException("PhoneDto not found");
            }
            int createdPhoneId = await _mediator.Send(new AddPhone.Command
            {
                Name = phoneDto.Name,
                PhoneNumber = phoneDto.PhoneNumber < 10 ? 10 : phoneDto.PhoneNumber
            });
            return CreatedAtAction(nameof(GetPhone), new { id = createdPhoneId }, null);
        }
        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePhone(int id, PhoneDto phoneDto)
        {
            if (id != phoneDto.Id)
            {
                return BadRequest();
            }
            return Ok(await _mediator.Send(new EditPhone.Command()
            {
                Id = phoneDto.Id,
                Name = phoneDto.Name,
                PhoneNumber = phoneDto.PhoneNumber < 10 ? 10 : phoneDto.PhoneNumber
            }));
        }
        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePhone(int id)
        {
            await _mediator.Send(new DeletePhone.Command { Id = id });
            return NoContent();
        }
    }
}
