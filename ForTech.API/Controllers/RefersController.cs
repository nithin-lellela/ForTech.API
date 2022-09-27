using ForTech.API.Models;
using ForTech.Core;
using ForTech.Data.RepositoryInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ForTech.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefersController : ControllerBase
    {
        private readonly IReferRepository _referRepository;
        private readonly UserManager<User> _userManager;
        public RefersController(IReferRepository referRepository, UserManager<User> userManager)
        {
            _referRepository = referRepository;
            _userManager = userManager;
        }

        [HttpPost("AddRefer/{forumId}/{senderUserId}/{receiverUserId}")]
        public async Task<IActionResult> AddRefer(Guid forumId, string senderUserId, string receiverUserId)
        {
            var refer = new Refer()
            {
                Id = Guid.NewGuid(),
                ForumId = forumId,
                SenderUserId = senderUserId,
                ReceiverUserId = receiverUserId,
                DateCreated = DateTime.Now,
                IsReferOpened = false,
            };
            var addRefer = await _referRepository.AddRefer(refer);
            if(addRefer != null)
            {
                return Ok(new AuthResponseModel()
                {
                    ResponseCode = ResponseCode.Ok,
                    ResponseMessage = "Refer added successfully",
                    DataSet = addRefer
                });
            }
            return BadRequest(new AuthResponseModel()
            {
                ResponseCode = ResponseCode.Error,
                ResponseMessage = "Invalid Details",
                DataSet = null
            });
        }

        [HttpGet("GetReceivedRefers/{userId}")]
        public async Task<IActionResult> GetReceivedRefers(string userId)
        {
            var isUserExists = await _userManager.FindByIdAsync(userId);
            if(isUserExists != null)
            {
                var receivedRefers = await _referRepository.GetReceivedRefers(userId);
                return Ok(new AuthResponseModel()
                {
                    ResponseCode = ResponseCode.Ok,
                    ResponseMessage = $"List of refers received for {isUserExists.Name}",
                    DataSet = receivedRefers
                });
            }
            return BadRequest(new AuthResponseModel()
            {
                ResponseCode = ResponseCode.Error,
                ResponseMessage = "Invalid User",
                DataSet = null
            });
        }

        [HttpGet("GetSentRefers/{userId}")]
        public async Task<IActionResult> GetSentRefers(string userId)
        {
            var isUserExists = await _userManager.FindByIdAsync(userId);
            if (isUserExists != null)
            {
                var sentRefers = await _referRepository.GetSentRefers(userId);
                return Ok(new AuthResponseModel()
                {
                    ResponseCode = ResponseCode.Ok,
                    ResponseMessage = $"List of refers sent by {isUserExists.Name}",
                    DataSet = sentRefers
                });
            }
            return BadRequest(new AuthResponseModel()
            {
                ResponseCode = ResponseCode.Error,
                ResponseMessage = "Invalid User",
                DataSet = null
            });
        }

        [HttpPut("UpdateRefer/{referId}")]
        public async Task<IActionResult> UpdateRefer(Guid referId)
        {
            var updateRefer = await _referRepository.UpdateIsOpenedRefer(referId);
            if (updateRefer)
            {
                return Ok(new AuthResponseModel()
                {
                    ResponseCode = ResponseCode.Ok,
                    ResponseMessage = "Refer updated successfully",
                    DataSet = null
                });
            }
            return BadRequest(new AuthResponseModel()
            {
                ResponseCode = ResponseCode.Error,
                ResponseMessage = "Invalid Refer",
                DataSet = null
            });
        }
    }
}
