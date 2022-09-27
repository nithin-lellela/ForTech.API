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
    public class ReplyController : ControllerBase
    {
        private readonly IReplyRepository _replyRepository;
        private readonly UserManager<User> _userManager;
        private readonly IForumRepository _forumRepository;
        public ReplyController(IReplyRepository replyRepository, UserManager<User> userManager, IForumRepository forumRepository)
        {
            _replyRepository = replyRepository;
            _userManager = userManager; 
            _forumRepository = forumRepository;
        }

        [HttpPost("AddReply")]
        public async Task<IActionResult> AddReply([FromBody] ReplyRequestDTO replyRequestDTO)
        {
            if (ModelState.IsValid)
            {
                var forumReply = new ForumReply()
                {
                    Id = Guid.NewGuid(),
                    ForumId = replyRequestDTO.ForumId,
                    UserId = replyRequestDTO.UserId,
                    UserName = _userManager.FindByIdAsync(replyRequestDTO.UserId).Result.Name,
                    Description = replyRequestDTO.Description,
                    DateCreated = DateTime.Now,
                    ForumReplyUpvotes = 0
                };
                var addForumReply = await _replyRepository.AddForumReply(forumReply);
                if (addForumReply != null)
                {
                    var updateForumReplies = await _forumRepository.UpdateReplies(addForumReply.ForumId, true);
                    var forumReplyDTO = new ReplyDTO()
                    {
                        Id= addForumReply.Id,
                        ForumId = addForumReply.ForumId,
                        UserId= addForumReply.UserId,
                        UserName = addForumReply.UserName,
                        Description = addForumReply.Description,
                        DateCreated = addForumReply.DateCreated,
                        ForumReplyUpvotes = addForumReply.ForumReplyUpvotes,
                        IsLiked = _replyRepository.IsReplyLiked(addForumReply.Id, addForumReply.UserId).Result,
                        ProfileImageUrl = _userManager.FindByIdAsync(addForumReply.UserId).Result.ProfileImageUrl,
                    };
                    return Ok(new AuthResponseModel()
                    {
                        ResponseCode = ResponseCode.Ok,
                        ResponseMessage = "Reply added successfully",
                        DataSet = forumReplyDTO
                    });
                }
                return BadRequest(new AuthResponseModel()
                {
                    ResponseCode = ResponseCode.Error,
                    ResponseMessage = "Failed to Add Reply",
                    DataSet = null
                });
            }
            return BadRequest(new AuthResponseModel()
            {
                ResponseCode = ResponseCode.Error,
                ResponseMessage =  "All fields are required",
                DataSet = null
            });
        }

        [HttpGet("GetRepliesByForum/{forumId}/{userId}")]
        public async Task<IActionResult> GetReplies(Guid forumId, string userId)
        {
            var forum = await _forumRepository.GetForum(forumId);
            if(forum != null)
            {
                var forumReplies = await _replyRepository.GetAllRepliesByForum(forumId);
                var forumRepliesDTO = forumReplies.Select(x => new ReplyDTO()
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    UserName = x.UserName,
                    ForumId = x.ForumId,
                    Description = x.Description,
                    DateCreated = x.DateCreated,
                    ForumReplyUpvotes = x.ForumReplyUpvotes,
                    IsLiked = _replyRepository.IsReplyLiked(x.Id, userId).Result,
                    ProfileImageUrl = _userManager.FindByIdAsync(x.UserId).Result.ProfileImageUrl,
                });
                return Ok(new AuthResponseModel()
                {
                    ResponseCode = ResponseCode.Ok,
                    ResponseMessage = $"Replies for ForumId: {forumId}",
                    DataSet = forumRepliesDTO
                });
            }
            return BadRequest(new AuthResponseModel()
            {
                ResponseCode = ResponseCode.Error,
                ResponseMessage = "Invalid Forum",
                DataSet = null
            });
        }

        [HttpDelete("DeleteReply/{id}")]
        public async Task<IActionResult> DeleteReply(Guid id)
        {
            var isReplyExists = await _replyRepository.GetForumReply(id);
            if(isReplyExists != null)
            {
                var reply = await _replyRepository.DeleteForum(id);
                if (reply)
                {
                    var updateForumReplies = await _forumRepository.UpdateReplies(isReplyExists.ForumId, false);
                    return Ok(new AuthResponseModel()
                    {
                        ResponseCode = ResponseCode.Ok,
                        ResponseMessage = $"Reply deleted successfully",
                        DataSet = null
                    });
                }
            }
            return BadRequest(new AuthResponseModel()
            {
                ResponseCode = ResponseCode.Error,
                ResponseMessage = "Invalid Forum Reply",
                DataSet = null
            });
        }

        [HttpPut("UpdateReplyUpvotes/{replyId}/{isVoted}/{userId}")]
        public async Task<IActionResult> UpdateReplyUpvotes(Guid replyId, bool isVoted, string userId)
        {
            var isReplyLiked = await _replyRepository.IsReplyLiked(replyId, userId);
            if (isVoted)
            {
                if (!isReplyLiked)
                {
                    var updateReplyUpvotes = await _replyRepository.UpdateForumReplyUpvotes(replyId, isVoted);
                    var replyUpvote = new ReplyUpvote()
                    {
                        Id = Guid.NewGuid(),
                        ForumReplyId = replyId,
                        UserId = userId,
                        UserName = _userManager.FindByIdAsync(userId).Result.Name
                    };
                    var addReplyUpvote = await _replyRepository.AddReplyUpvote(replyUpvote);
                    if(updateReplyUpvotes!= null && addReplyUpvote != null)
                    {
                        return Ok(new AuthResponseModel()
                        {
                            ResponseCode = ResponseCode.Ok,
                            ResponseMessage = "Success",
                            DataSet = updateReplyUpvotes
                        });
                    }
                    return BadRequest(new AuthResponseModel()
                    {
                        ResponseCode = ResponseCode.Error,
                        ResponseMessage = "Invalid Reply",
                        DataSet = null
                    });
                }
                return BadRequest(new AuthResponseModel()
                {
                    ResponseCode= ResponseCode.Error,
                    ResponseMessage = "Reply already voted",
                    DataSet = null
                });
            }
            var upvotes = await _replyRepository.GetReplyUpvotes(replyId);
            if(upvotes > 0 && isReplyLiked)
            {
                var replyUpvote = await _replyRepository.UpdateForumReplyUpvotes(replyId, isVoted);
                var deleteUpvote = await _replyRepository.DeleteReplyUpvote(replyId, userId);
                if(replyUpvote != null && deleteUpvote)
                {
                    return Ok(new AuthResponseModel()
                    {
                        ResponseCode = ResponseCode.Ok,
                        ResponseMessage = "Success",
                        DataSet = replyUpvote
                    });
                }
                return BadRequest(new AuthResponseModel()
                {
                    ResponseCode = ResponseCode.Error,
                    ResponseMessage = "Invalid Reply",
                    DataSet = null
                });
            }
            return BadRequest(new AuthResponseModel()
            {
                ResponseCode = ResponseCode.Error,
                ResponseMessage = "Cannot UnVote reply",
                DataSet = null
            });

        }

    }
}
