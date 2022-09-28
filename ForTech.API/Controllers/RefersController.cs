using ForTech.API.Models;
using ForTech.API.Models.DTOs;
using ForTech.API.Models.RequestDTOs;
using ForTech.Core;
using ForTech.Data.RepositoryInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ForTech.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefersController : ControllerBase
    {
        private readonly IReferRepository _referRepository;
        private readonly UserManager<User> _userManager;
        private readonly IChannelRepository _channelRepository;
        private readonly IForumRepository _forumRepository; 
        public RefersController(IReferRepository referRepository, UserManager<User> userManager, IChannelRepository channelRepository, IForumRepository forumRepository)
        {
            _referRepository = referRepository;
            _userManager = userManager;
            _channelRepository = channelRepository;
            _forumRepository = forumRepository;
        }

        [HttpPost("AddRefer")]
        public async Task<IActionResult> AddRefer([FromBody] ReferRequestDTO referRequestDTO)
        {
            var refer = new Refer()
            {
                Id = Guid.NewGuid(),
                ForumId = referRequestDTO.ForumId,
                SenderUserId = referRequestDTO.SenderUserId,
                ReceiverUserId = referRequestDTO.ReceiverUserId,
                ForumUserId = referRequestDTO.ForumUserId,
                ForumUserName = referRequestDTO.ForumOwnerName,
                ChannelId = referRequestDTO.ChannelId,
                ChannelName = referRequestDTO.ChannelName,
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
                var refers = receivedRefers.Select(x => new ReferDTO()
                {
                    Id = x.Id,
                    ReceiverUserId = x.ReceiverUserId,
                    SenderUserId = x.SenderUserId,
                    ReceiverUserName = _userManager.FindByIdAsync(x.ReceiverUserId).Result.Name,
                    SenderUserName = _userManager.FindByIdAsync(x.SenderUserId).Result.Name,
                    ForumId = x.ForumId,
                    ForumUserName = x.ForumUserName,
                    ForumUserId = x.ForumUserId,
                    ChannelId = x.ChannelId,
                    ChannelName = x.ChannelName,
                    DateCreated = x.DateCreated,
                    IsReferOpened = x.IsReferOpened,
                    ProfileImageUrl = _userManager.FindByIdAsync(x.ForumUserId).Result.ProfileImageUrl
                    
                });
                return Ok(new AuthResponseModel()
                {
                    ResponseCode = ResponseCode.Ok,
                    ResponseMessage = $"List of refers received for {isUserExists.Name}",
                    DataSet = refers
                });
            }
            return BadRequest(new AuthResponseModel()
            {
                ResponseCode = ResponseCode.Error,
                ResponseMessage = "Invalid User",
                DataSet = null
            });
        }

        [HttpGet("GetNotificationsCount/{userId}")]
        public async Task<IActionResult> Get(string userId)
        {
            var isUserExists = await _userManager.FindByIdAsync(userId);
            if (isUserExists != null)
            {
                var refers = await _referRepository.GetReceivedRefers(userId);
                return Ok(new AuthResponseModel()
                {
                    ResponseCode = ResponseCode.Ok,
                    ResponseMessage = "Notifcations count",
                    DataSet = refers.Count
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
                var refers = sentRefers.Select(x => new ReferDTO()
                {
                    Id = x.Id,
                    ReceiverUserId = x.ReceiverUserId,
                    SenderUserId = x.SenderUserId,
                    ReceiverUserName = _userManager.FindByIdAsync(x.ReceiverUserId).Result.UserName,
                    SenderUserName = _userManager.FindByIdAsync(x.SenderUserId).Result.UserName,
                    ForumId = x.ForumId,
                    ForumUserName = x.ForumUserName,
                    ForumUserId = x.ForumUserId,
                    ChannelId = x.ChannelId,
                    ChannelName = x.ChannelName,
                    DateCreated = x.DateCreated,
                    IsReferOpened = x.IsReferOpened,
                    ProfileImageUrl = _userManager.FindByIdAsync(x.ForumUserId).Result.ProfileImageUrl

                });
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

        [HttpDelete("RemoveRefer/{id}")]
        public async Task<IActionResult> RemoveRefer(Guid id)
        {
            var remove = await _referRepository.RemoveRefer(id);
            if (remove)
            {
                return Ok(new AuthResponseModel()
                {
                    ResponseCode = ResponseCode.Ok,
                    ResponseMessage = "Refer removed successfully",
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
