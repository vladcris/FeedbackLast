using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using FeedbackV1.Dtos;
using FeedbackV1.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FeedbackV1.Controllers
{     
    [Authorize] 
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IMapper _mapper;
        
        public UserController(IAuthRepository repo, IMapper mapper)
        {

            _repo = repo;
            _mapper = mapper;

        }


        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {   
            var repo = new TableStorageRepository();

            var CurrentUserId = (User.FindFirst(ClaimTypes.NameIdentifier).Value);

            userParams.UserId = CurrentUserId;

            var userLogged = await repo.GetUser(CurrentUserId);

            if(!string.IsNullOrEmpty(userLogged.Role) && userParams.Team) 
            {
                userParams.Role = userLogged.Role;
                userParams.Manager = userLogged.Manager_ID;
            }
            
            var users = await repo.GetUsers(userParams);
            var usersToReturn = _mapper.Map<IEnumerable<UserDto>>(users); 
            if (!usersToReturn.Any())
                return NotFound();
            return Ok(usersToReturn);
        }

        [HttpGet("{id:guid}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(string id)
        {
            var repo = new TableStorageRepository();
            var user = await repo.GetUser(id);
            var userToReturn = _mapper.Map<UserDto>(user); 
            if (userToReturn == null)
                return NotFound();
            return Ok(userToReturn);
        }

        [HttpGet("managers")]
        public async Task<IActionResult> GetManagers()
        {
            var repo = new TableStorageRepository();
            var managers = await repo.GetAllManagers();

            var managersToReturn = _mapper.Map<IEnumerable<UserDto>>(managers);
            
            if (!managersToReturn.Any())
                return NotFound();
            return Ok(managersToReturn);
            
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, UpdateUserDto  requestUpdate)
        {   
            var repo = new TableStorageRepository();
            var cards = await repo.GetUser(id);
            _mapper.Map(requestUpdate, cards);
            await repo.PostEntityUser(cards);
            
            return Ok();
        }

        [HttpDelete("{id}")]
        
        public async Task<IActionResult> DeleteUser(string id)
        {
            var repo = new TableStorageRepository();
            var cards = await repo.GetUser(id);
            await repo.DeleteUser(cards);
            return Ok();
        }

    }


}