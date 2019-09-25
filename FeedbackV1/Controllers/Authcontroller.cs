using AutoMapper;
using FeedbackV1.Dtos;
using FeedbackV1.Models;
using FeedbackV1.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace FeedbackV1.Controllers
{     
    [Authorize] 
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public AuthController(IAuthRepository repo, IConfiguration config, IMapper mapper)
        {

            _config = config;
            _repo = repo;
             _mapper = mapper;
            
        }

        [AllowAnonymous]
        [HttpPost("register")]

        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            //validate
             var repo = new TableStorageRepository();
            userForRegisterDto.Email = userForRegisterDto.Email.ToLower();

              if(await _repo.UserExists(userForRegisterDto.Email))
                 return BadRequest("Email already exists!");
            
            var userToCreate = _mapper.Map<User>(userForRegisterDto);
            var pass = userForRegisterDto.Password.ToString();


            ////sendgrid
            var CurrentUserId = (User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userLogged = await repo.GetUser(CurrentUserId);
            Execute(userLogged,userToCreate,pass).Wait();

            //// end

            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

            var userToReturn = _mapper.Map<UserDto>(createdUser);


            return CreatedAtRoute("GetUser", new {controller = "User", id = createdUser.Id}, userToReturn);
        }

            static async Task Execute(User sender, User receiver, string password)
             {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(sender.Email, sender.Name);
            var subject = "Your account was created";
            var to = new EmailAddress(receiver.Email, receiver.Name);
            var plainTextContent = "Username : receiver.Name, Password:";
            var htmlContent = "<strong>Username :  "+ receiver.Email  +"!</strong> <br> <strong> Password :" + password +"</strong> ";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
           
            }



        [AllowAnonymous]
        [HttpPost("login")]

        public async Task<IActionResult> Login(UserForLoginDto userForRegisterDto)
        {
            var userFromRepo = await _repo.Login(userForRegisterDto.Email, userForRegisterDto.Password);

            if(userFromRepo == null || userFromRepo.IsDeleted == true)
            return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id),
                new Claim(ClaimTypes.Name, userFromRepo.Name)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8
            .GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds

            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            // var user = new UserDto {
            //         ID = userFromRepo.Id,
            //         DEP_ID = userFromRepo.Dep_Id,
            //         Name = userFromRepo.Name,
            //         Email = userFromRepo.Email,
            //         Manager_ID = userFromRepo.Manager_ID,
            //         Role = userFromRepo.Id

            //     };

            return Ok(new {
                token = tokenHandler.WriteToken(token),
                    Id = userFromRepo.Id,
                    Dep_Id = userFromRepo.Dep_Id,
                    Name = userFromRepo.Name,
                    Email = userFromRepo.Email,
                    Role = userFromRepo.Role,
                    Manager_Id = userFromRepo.Manager_ID
                

            });

        }
        
    }
}

