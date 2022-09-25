using ForTech.API.Models;
using ForTech.API.Models.DTOs;
using ForTech.API.Models.RequestDTOs;
using ForTech.Core;
using ForTech.Data.RepositoryInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForTech.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForumController : ControllerBase
    {
        private readonly IForumRepository _forumRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly UserManager<User> _userManager;
        public ForumController(IForumRepository forumRepository, IChannelRepository channelRepository, UserManager<User> userManager)
        {
            _forumRepository = forumRepository;
            _channelRepository = channelRepository;
            _userManager = userManager;
        }

        [HttpPost("CreateForum")]
        public async Task<IActionResult> CreateForum([FromBody] ForumRequestDTO forumRequestDTO)
        {
            if (ModelState.IsValid)
            {
                var forum = new Forum()
                {
                    Id = new System.Guid(),
                    UserId = forumRequestDTO.UserId,
                    ChannelId = forumRequestDTO.ChannelId,
                    Description = forumRequestDTO.Description,
                    DateModified = System.DateTime.Now,
                    DataCreated = System.DateTime.Now,
                    ForumUpvotes = 0,
                    ForumReplies = 0
                };
                var isForumSuccess = await _forumRepository.CreateForum(forum);
                if(isForumSuccess != null)
                {
                    return Ok(new AuthResponseModel()
                    {
                        ResponseCode = ResponseCode.Ok,
                        ResponseMessage = "Successfully added forum",
                        DataSet = isForumSuccess
                    });
                }
                return BadRequest(new AuthResponseModel()
                {
                    ResponseCode = ResponseCode.Error,
                    ResponseMessage = "Failed to upload forum",
                    DataSet = null
                });
            }
            return BadRequest(new AuthResponseModel()
            {
                ResponseCode = ResponseCode.Error,
                ResponseMessage = "All Fields are required",
                DataSet = null
            });
        }

        [HttpGet("GetAllForums/{userId}")]
        public async Task<IActionResult> GetAllForums(string userId)
        {
            var allForums = await _forumRepository.GetAllForums();
            var forums = allForums.Select(x => new ForumDTO()
            {
                Id = x.Id,
                ChannelId = x.ChannelId,
                UserId = x.UserId,
                UserName = _userManager.FindByIdAsync(x.UserId).Result.Name,
                ChannelName = _channelRepository.GetChannel(x.ChannelId).Result.ChannelName,
                Description = x.Description,
                DateCreated = x.DataCreated,
                ForumUpvotes = x.ForumUpvotes,
                ForumReplies = x.ForumReplies,
                IsLiked = _forumRepository.IsForumLiked(userId, x.Id).Result
            });
            return Ok(new AuthResponseModel()
            {
                ResponseCode = ResponseCode.Ok,
                ResponseMessage = "All Forums",
                DataSet = forums
            });
        }

        [HttpGet("GetForumsByChannel/{ChannelId}/{userId}/{filter}")]
        public async Task<IActionResult> GetForumsByChannel(Guid ChannelId, string userId, string filter)
        {
            var isChannelExists = await _channelRepository.GetChannel(ChannelId);
            if(isChannelExists != null)
            {
                if(filter == "All")
                {
                    var forums = await _forumRepository.GetAllForumsByChannelId(ChannelId);
                    var forumDTO = forums.Select(x => new ForumDTO()
                    {
                        Id = x.Id,
                        ChannelId = x.ChannelId,
                        UserId = x.UserId,
                        UserName = _userManager.FindByIdAsync(x.UserId).Result.Name,
                        ChannelName = _channelRepository.GetChannel(x.ChannelId).Result.ChannelName,
                        Description = x.Description,
                        DateCreated = x.DataCreated,
                        ForumUpvotes = x.ForumUpvotes,
                        ForumReplies = x.ForumReplies,
                        IsLiked = _forumRepository.IsForumLiked(userId, x.Id).Result
                    });
                    return Ok(new AuthResponseModel()
                    {
                        ResponseCode = ResponseCode.Ok,
                        ResponseMessage = $"All {isChannelExists.ChannelName} Forums",
                        DataSet = forumDTO
                    });
                }else if(filter == "Top")
                {
                    var forums = await _forumRepository.GetTopForumsByChannelId(ChannelId);
                    var forumDTO = forums.Select(x => new ForumDTO()
                    {
                        Id = x.Id,
                        ChannelId = x.ChannelId,
                        UserId = x.UserId,
                        UserName = _userManager.FindByIdAsync(x.UserId).Result.Name,
                        ChannelName = _channelRepository.GetChannel(x.ChannelId).Result.ChannelName,
                        Description = x.Description,
                        DateCreated = x.DataCreated,
                        ForumUpvotes = x.ForumUpvotes,
                        ForumReplies = x.ForumReplies,
                        IsLiked = _forumRepository.IsForumLiked(userId, x.Id).Result
                    });
                    return Ok(new AuthResponseModel()
                    {
                        ResponseCode = ResponseCode.Ok,
                        ResponseMessage = $"Top {isChannelExists.ChannelName} Forums",
                        DataSet = forumDTO
                    });
                }else if(filter == "Recent")
                {
                    var forums = await _forumRepository.GetAllForumsByChannelId(ChannelId);
                    forums.Sort((x, y) => DateTime.Compare(x.DataCreated, y.DataCreated));
                    forums.Reverse();
                    var forumDTO = forums.Select(x => new ForumDTO()
                    {
                        Id = x.Id,
                        ChannelId = x.ChannelId,
                        UserId = x.UserId,
                        UserName = _userManager.FindByIdAsync(x.UserId).Result.Name,
                        ChannelName = _channelRepository.GetChannel(x.ChannelId).Result.ChannelName,
                        Description = x.Description,
                        DateCreated = x.DataCreated,
                        ForumUpvotes = x.ForumUpvotes,
                        ForumReplies = x.ForumReplies,
                        IsLiked = _forumRepository.IsForumLiked(userId, x.Id).Result
                    });
                    return Ok(new AuthResponseModel()
                    {
                        ResponseCode = ResponseCode.Ok,
                        ResponseMessage = $"Most Recent {isChannelExists.ChannelName} Forums",
                        DataSet = forumDTO
                    });
                }
                
            }
            return BadRequest(new AuthResponseModel()
            {
                ResponseCode = ResponseCode.Error,
                ResponseMessage = "Invalid Channel",
                DataSet = null
            });
        }

        [HttpGet("GetForumsInChannelBySearch/{channelId}/{userId}/{search}")]
        public async Task<IActionResult> SearchForumsInChannel(string search, Guid channelId, string userId)
        {
            var isChannelExists = await _channelRepository.GetChannel(channelId);
            if(isChannelExists != null)
            {
                if (!string.IsNullOrEmpty(search))
                {
                    List<Forum> searchForums = new List<Forum>();
                    //Forum[] searchForums = new Forum[] { };
                    var forums = await _forumRepository.GetAllForumsByChannelId(channelId);
                    foreach (var forum in forums)
                    {
                        if (forum.Description.ToLower().Contains(search.ToLower()))
                        {
                            searchForums.Add(forum);
                        }
                    }
                    var forumDTO = searchForums.Select(x => new ForumDTO()
                    {
                        Id = x.Id,
                        ChannelId = x.ChannelId,
                        UserId = x.UserId,
                        UserName = _userManager.FindByIdAsync(x.UserId).Result.Name,
                        ChannelName = _channelRepository.GetChannel(x.ChannelId).Result.ChannelName,
                        Description = x.Description,
                        DateCreated = x.DataCreated,
                        ForumUpvotes = x.ForumUpvotes,
                        ForumReplies = x.ForumReplies,
                        IsLiked = _forumRepository.IsForumLiked(userId, x.Id).Result
                    });
                    return Ok(new AuthResponseModel()
                    {
                        ResponseCode = ResponseCode.Ok,
                        ResponseMessage = $"Search for {search} in {isChannelExists.ChannelName} Forums",
                        DataSet = forumDTO
                    });
                }
            }
       
            return BadRequest(new AuthResponseModel()
            {
                ResponseCode = ResponseCode.Error,
                ResponseMessage = "Invalid search",
                DataSet = null
            });
        }

        [HttpGet("GetForumsByUser/{userId}")]
        public async Task<IActionResult> GetForumsByUser(string userId)
        {
            var isUserExists = await _userManager.FindByIdAsync(userId);
            if(isUserExists != null)
            {
                var userForums = await _forumRepository.GetAllForumsByUserId(userId);
                var forums = userForums.Select(x => new ForumDTO()
                {
                    Id = x.Id,
                    ChannelId = x.ChannelId,
                    UserId = x.UserId,
                    UserName = _userManager.FindByIdAsync(x.UserId).Result.Name,
                    ChannelName = _channelRepository.GetChannel(x.ChannelId).Result.ChannelName,
                    Description = x.Description,
                    DateCreated = x.DataCreated,
                    ForumUpvotes = x.ForumUpvotes,
                    ForumReplies = x.ForumReplies,
                    IsLiked = _forumRepository.IsForumLiked(userId, x.Id).Result
                });
                return Ok(new AuthResponseModel()
                {
                    ResponseCode = ResponseCode.Ok,
                    ResponseMessage = $"{isUserExists.Name} Forums",
                    DataSet = forums
                });
            }
            return BadRequest(new AuthResponseModel()
            {
                ResponseCode = ResponseCode.Error,
                ResponseMessage = "Invalid User",
                DataSet = null
            });
        }

        [HttpGet("GetForumById/{id}/{userId}")]
        public async Task<IActionResult> GetForumById(Guid id, string userId)
        {
            var forum = await _forumRepository.GetForum(id);
            if(forum != null)
            {
                var forumDTO = new ForumDTO()
                {
                    Id = forum.Id,
                    ChannelId = forum.ChannelId,
                    UserId = forum.UserId,
                    UserName = _userManager.FindByIdAsync(forum.UserId).Result.Name,
                    ChannelName = _channelRepository.GetChannel(forum.ChannelId).Result.ChannelName,
                    Description = forum.Description,
                    DateCreated = forum.DataCreated,
                    ForumUpvotes = forum.ForumUpvotes,
                    ForumReplies = forum.ForumReplies,
                    IsLiked = _forumRepository.IsForumLiked(userId, id).Result
                };
                return Ok(new AuthResponseModel()
                {
                    ResponseCode = ResponseCode.Ok,
                    ResponseMessage = $"",
                    DataSet = forumDTO
                });
            }
            return BadRequest(new AuthResponseModel()
            {
                ResponseCode = ResponseCode.Error,
                ResponseMessage = "Invalid Forum",
                DataSet = null
            });
        }

        [HttpPut("UpdateForumUpvotes/{id}/{liked}/{userId}")]
        public async Task<IActionResult> UpdateForumUpvotes(Guid id, bool liked, string userId)
        {
            var isForumLiked = await _forumRepository.IsForumLiked(userId, id);
            if (liked)
            {
                if (!isForumLiked)
                {
                    var forumUpvotes = await _forumRepository.UpdateUpvotes(id, liked);
                    var upVote = new ForumUpvote()
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        UserName = _userManager.FindByIdAsync(userId).Result.Name,
                        ForumId = id
                    };
                    var addForumUpvotes = await _forumRepository.AddUpvote(upVote);
                    if (forumUpvotes != null && addForumUpvotes != null)
                    {
                        return Ok(new AuthResponseModel()
                        {
                            ResponseCode = ResponseCode.Ok,
                            ResponseMessage = "Success",
                            DataSet = forumUpvotes
                        });
                    }
                    return BadRequest(new AuthResponseModel()
                    {
                        ResponseCode = ResponseCode.Error,
                        ResponseMessage = "Invalid Forum",
                        DataSet = null
                    });
                }
                return Ok(new AuthResponseModel()
                {
                    ResponseCode = ResponseCode.Ok,
                    ResponseMessage = "Forum is already Liked",
                    DataSet = null
                });
                
            }
            var upVotes = await _forumRepository.GetForumUpvotes(id);
            if(upVotes > 0 && isForumLiked)
            {
                var forumUpvotes = await _forumRepository.UpdateUpvotes(id, liked);
                var removeUpvote = await _forumRepository.DeleteUpvote(userId, id);
                if(forumUpvotes != null && removeUpvote)
                {
                    return Ok(new AuthResponseModel()
                    {
                        ResponseCode = ResponseCode.Ok,
                        ResponseMessage = "Success",
                        DataSet = forumUpvotes
                    });
                }
                return BadRequest(new AuthResponseModel()
                {
                    ResponseCode = ResponseCode.Error,
                    ResponseMessage = "Invalid Forum",
                    DataSet = null
                });
            }
            return BadRequest(new AuthResponseModel()
            {
                ResponseCode = ResponseCode.Error,
                ResponseMessage = "Cannot decrement upvotes",
                DataSet = null
            });
        }

        [HttpPut("UpdateForum")]
        public async Task<IActionResult> UpdateForum([FromBody] ForumRequestDTO forumRequestDTO)
        {
            var forum = await _forumRepository.GetForum(forumRequestDTO.Id);
            if(forum != null)
            {
                var updateForum = new Forum()
                {
                    Id = forumRequestDTO.Id,
                    ChannelId = forumRequestDTO.ChannelId,
                    UserId = forumRequestDTO.UserId,
                    Description = forumRequestDTO.Description,
                    DateModified = DateTime.Now,
                    DataCreated = forum.DataCreated,
                    ForumUpvotes = forum.ForumUpvotes,
                    ForumReplies = forum.ForumReplies
                };
                var isForumUpdated = await _forumRepository.UpdateForum(updateForum);
                return Ok(new AuthResponseModel()
                {
                    ResponseCode = ResponseCode.Ok,
                    ResponseMessage = "Forum Updated",
                    DataSet = isForumUpdated
                });
            }
            return BadRequest(new AuthResponseModel()
            {
                ResponseCode = ResponseCode.Error,
                ResponseMessage = "Forum Not Found",
                DataSet = null
            });
        }

        [HttpDelete("DeleteForum/{id}")]
        public async Task<IActionResult> DeleteForum(Guid id)
        {
            var isForumExists = await _forumRepository.GetForum(id);
            if(isForumExists != null)
            {
                var deleteForum = await _forumRepository.DeleteForum(id);
                if (deleteForum)
                {
                    return Ok(new AuthResponseModel()
                    {
                        ResponseCode = ResponseCode.Ok,
                        ResponseMessage = "Forum deleted",
                        DataSet = null
                    });
                }
            }
            return BadRequest(new AuthResponseModel()
            {
                ResponseCode = ResponseCode.Error,
                ResponseMessage = "Forum Not Found",
                DataSet = null
            });
        }

    }
}
