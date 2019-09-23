using AutoMapper;
using FeedbackV1.Dtos;
using FeedbackV1.Models;
using FeedbackV1.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FeedbackV1.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MyFeedbacksController : ControllerBase
    {
        private readonly IMapper _mapper;

        public MyFeedbacksController(IMapper mapper)
        {
            _mapper = mapper;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetMyFeedbacks([FromQuery]UserParams userParams, string id)
        {
            var repo = new TableStorageRepository();
            var cards = await repo.GetMyFeedbacks(userParams, id);
            var feedbackToReturn = _mapper.Map<IEnumerable<FeedbackListDto>>(cards);

            //// adaugat in plus
            var users = await repo.GetUsersWithoutParams(); 
            foreach (var feedback in feedbackToReturn)
            {
                foreach (var user in users)
                {
                    
                if (user.Id == feedback.ID)
                {
                    feedback.Sender = user.Name;
                }

                if (user.Id == feedback.ID_receiver)
                {
                    feedback.Receiver = user.Name;
                }

                if (user.Id == feedback.ID_manager)
                {
                    feedback.Manager = user.Name;
                }

                }
            }
            //// end
            

            Response.AddPagination(cards.CurrentPage, cards.PageSize, cards.TotalCount, cards.TotalPages);

            return Ok(feedbackToReturn);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> RequestFeedback(RequestFeedbackDto requestFeedbackDto, string id)
        {
           
            var repo = new TableStorageRepository();
            var requestToCreate = _mapper.Map<Feedbacks>(requestFeedbackDto);
            var createdRequest = await repo.RequestFeedback(requestToCreate, id);
             var receiverId = createdRequest.ID;
            ////sendgrid
            var CurrentUserId = (User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userLogged = await repo.GetUser(CurrentUserId);
            var userToSend = await repo.GetUser(receiverId);
            Execute(userLogged,userToSend).Wait();

            //// end

            return Ok(createdRequest);

        }

        static async Task Execute(User sender, User receiver)
        {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(sender.Email, sender.Name);
            var subject = "Sending with Twilio SendGrid is Fun";
            var to = new EmailAddress(receiver.Email, receiver.Name);
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg); 
        }
    }
}
