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


        [Route("allusers")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery]UserParams userParams)
        {
            var repo = new TableStorageRepository();

            var users = await repo.GetUsersWithoutParams();

            var CurrentUserId = (User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var userLogged = await repo.GetUser(CurrentUserId);

            if(userLogged.Role == "admin") {

                var usersAdmin = await repo.GetUsersWithoutParamsForAdmin();

                usersAdmin = usersAdmin.Where(x => x.RowKey != userParams.UserId).OrderBy(x => x.IsDeleted);
                var usersForAdmin = _mapper.Map<IEnumerable<UserDto>>(usersAdmin);

                return Ok(usersForAdmin);
            }

            // get all user for request minus the requested one
            if(!string.IsNullOrEmpty(userParams.UserId)) {
                users = users.Where(x => x.RowKey != userParams.UserId).OrderBy(x => x.Name);
            }

            users = users.OrderBy(x => x.Name);

            var usersToReturn = _mapper.Map<IEnumerable<UserDto>>(users);
            if (!usersToReturn.Any())
                return NotFound();
            return Ok(usersToReturn);
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
                if (!string.IsNullOrEmpty(userLogged.Manager_ID)) {
                    userParams.Manager = userLogged.Manager_ID;
                }
                
            }
            
            var users = await repo.GetUsers(userParams);
            var usersToReturn = _mapper.Map<IEnumerable<UserDto>>(users); 

            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

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

        [HttpPut("undoDelete")]
        public async Task<IActionResult> SoftDeleteAdmin(UserDto userDto)
        {   
            var repo = new TableStorageRepository();
            var cards = await repo.GetUser(userDto.ID);
            _mapper.Map(userDto, cards);
            await repo.PostEntityUser(cards);
            
            return Ok();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserSoft(string id)
        {
            // var repo = new TableStorageRepository();
            // var cards = await repo.GetUser(id);
            // await repo.DeleteUser(cards);
            // return Ok();

            var repo = new TableStorageRepository();
            var cards = await repo.GetUser(id);
            cards.IsDeleted = true;
            await repo.PostEntityUser(cards) ;
            return Ok();
        }



    }


}