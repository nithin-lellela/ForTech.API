using ForTech.API.Models;
using ForTech.API.Models.RequestDTOs;
using ForTech.Core;
using ForTech.Data.RepositoryInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForTech.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChannelsController : ControllerBase
    {
        private readonly IChannelRepository _channelRepo;
        private readonly UserManager<User> _userManager;
        private readonly IForumRepository _forumRepository;
        public ChannelsController(IChannelRepository channelRepository, UserManager<User> UserManager, IForumRepository forumRepository)
        {
            _channelRepo = channelRepository;   
            _userManager = UserManager;
            _forumRepository = forumRepository;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateChannel([FromBody] ChannelRequestDTO channelRequestDTO)
        {
            if (ModelState.IsValid)
            {
                var isChannelExists = await _channelRepo.IsChannelExists(channelRequestDTO.Name);
                if (!isChannelExists)
                {
                    var channel = new Channel()
                    {
                        Id = new System.Guid(),
                        ChannelName = channelRequestDTO.Name,
                        NoOfInteractions = 0
                    };
                    var isSuccess = await _channelRepo.CreateNewChannel(channel);
                    return Ok(new AuthResponseModel()
                    {
                        ResponseCode = ResponseCode.Ok,
                        ResponseMessage = "Channel Creation Successfull",
                        DataSet = isSuccess
                    });
                }
                return BadRequest(new AuthResponseModel()
                {
                    ResponseCode = ResponseCode.Error,
                    ResponseMessage = "Channel Already Exists",
                    DataSet = null
                });
            }
            return BadRequest(new AuthResponseModel()
            {
                ResponseCode = ResponseCode.Error,
                ResponseMessage = "All Fields are mandatory",
                DataSet = null
            });
        }

        [HttpGet("GetAllChannels")]
        public async Task<IActionResult> GetAll()
        {
            var channels = await _channelRepo.GetAllChannels();
            if(channels.Count != 0)
            {
                return Ok(new AuthResponseModel()
                {
                    ResponseCode = ResponseCode.Ok,
                    ResponseMessage = "All Channels",
                    DataSet = channels
                });
            }
            return Ok(new AuthResponseModel()
            {
                ResponseCode = ResponseCode.Ok,
                ResponseMessage = "No Channel Exists",
                DataSet = null
            });
        }

        [HttpGet("GetChannel/{id}")]
        public async Task<IActionResult> GetChannel(Guid Id)
        {
            var isExists = await _channelRepo.GetChannel(Id);
            if(isExists != null)
            {
                return Ok(new AuthResponseModel()
                {
                    ResponseCode = ResponseCode.Ok,
                    ResponseMessage = $"{isExists.ChannelName} Channel exists",
                    DataSet = isExists
                });
            }
            return BadRequest(new AuthResponseModel()
            {
                ResponseCode = ResponseCode.Error,
                ResponseMessage = "Channel not found",
                DataSet = null
            });
        }

        [HttpPost("AddToFavourites")]
        public async Task<IActionResult> AddUserFavouriteChannel([FromBody] UserFavChannelReqDTO userFavChannelReq)
        {
            if (ModelState.IsValid)
            {
                var userFavouriteChannel = new UserFavouriteChannels()
                {
                    Id = new Guid(),
                    UserId = userFavChannelReq.UserId,
                    UserName = userFavChannelReq.UserName,
                    ChannelId = userFavChannelReq.ChannelId,
                    ChannelName = userFavChannelReq.ChannelName,
                };
                var favChannel = await _channelRepo.AddUserFavouriteChannel(userFavouriteChannel);
                if (favChannel != null)
                {
                    return Ok(new AuthResponseModel()
                    {
                        ResponseCode = ResponseCode.Ok,
                        ResponseMessage = $"{favChannel.ChannelName} Added to your Favourites",
                        DataSet = favChannel
                    });
                }
                return BadRequest(new AuthResponseModel()
                {
                    ResponseCode = ResponseCode.Error,
                    ResponseMessage = "Internal Error",
                    DataSet = null
                });
            }
            return BadRequest(new AuthResponseModel()
            {
                ResponseCode = ResponseCode.Error,
                ResponseMessage = "Invalid",
                DataSet = null
            });
        }

        [HttpGet("GetUserFavourites/{id}")]
        public async Task<IActionResult> GetUserFavourites(string id)
        {
            var isUserExists = await _userManager.FindByIdAsync(id);
            if(isUserExists != null)
            {
                var userFavourites = await _channelRepo.GetAllUserFavouriteChannels(id);
                return Ok(new AuthResponseModel()
                {
                    ResponseCode = ResponseCode.Ok,
                    ResponseMessage = $"Total count of fav channels: {userFavourites.Count}",
                    DataSet = userFavourites
                });
            }
            return BadRequest(new AuthResponseModel()
            {
                ResponseCode = ResponseCode.Error,
                ResponseMessage = "Invalid User",
                DataSet = null
            });
        }

        [HttpPut("UpdateChannelInteractions")]
        public async Task<IActionResult> UpdateChannel()
        {
            var channels = await _channelRepo.GetAllChannels();
            foreach(var channel in channels)
            {
                var channelInteractions = await _forumRepository.GetChannelForumsInteractions(channel.Id);
                var updateChannel = new Channel()
                {
                    Id = channel.Id,
                    ChannelName = channel.ChannelName,
                    NoOfInteractions = channelInteractions
                };
                await _channelRepo.UpdateChannel(updateChannel);
            }
            return Ok(new AuthResponseModel()
            {
                ResponseCode= ResponseCode.Ok,
                ResponseMessage = "Updated channel interactions",
                DataSet = null
            });
            
        }

        [HttpGet("TopChannels")]
        public async Task<IActionResult> GetTopChannels()
        {
            var channels = await _channelRepo.GetTopChannels();
            channels.Reverse();
            List<Channel> topChannels = new List<Channel>();
            int count = 0;
            foreach(var channel in channels)
            {
                if(count == 5)
                {
                    break;
                }
                topChannels.Add(channel);
                count++;
            }
            return Ok(new AuthResponseModel()
            {
                ResponseCode = ResponseCode.Ok,
                ResponseMessage = "Top 4 Channels",
                DataSet = topChannels
            });

        }

        [HttpDelete("DeleteUserFavourite/{userId}/{channelId}")]
        public async Task<IActionResult> DeleteUserFavChannel(string userId, Guid channelId)
        {
            var isUserExists = await _userManager.FindByIdAsync(userId);
            var isChannelExists = await _channelRepo.GetChannel(channelId);
            if(isUserExists!= null && isChannelExists != null)
            {
                var deleteFavouriteChannel = await _channelRepo.RemoveUserFavouriteChannel(userId, channelId);
                if (deleteFavouriteChannel)
                {
                    return Ok(new AuthResponseModel()
                    {
                        ResponseCode = ResponseCode.Ok,
                        ResponseMessage = "Channel removed from favourites",
                        DataSet = null
                    });
                }
                return BadRequest(new AuthResponseModel()
                {
                    ResponseCode = ResponseCode.Error,
                    ResponseMessage = "Uanble to remove from favourites, try again later",
                    DataSet = null
                });
            }
            return BadRequest(new AuthResponseModel()
            {
                ResponseCode = ResponseCode.Error,
                ResponseMessage = "Invalid User or Channel",
                DataSet = null
            });
        }

    }
}
