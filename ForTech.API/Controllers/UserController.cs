using ForTech.API.Models;
using ForTech.API.Models.DTOs;
using ForTech.API.Models.RequestDTOs;
using ForTech.Core;
using ForTech.Data.RepositoryInterfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ForTech.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IReplyRepository _replyRepository;
        public UserController(UserManager<User> UserManager, IConfiguration Configuration, IReplyRepository replyRepository)
        {
            _userManager = UserManager;
            _configuration = Configuration;
            _replyRepository = replyRepository;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegisterRequestDTO userRegisterRequestDTO)
        {
            if (ModelState.IsValid)
            {
                var isADPEmail = userRegisterRequestDTO.Email[^8..];
                if(isADPEmail.Equals("@adp.com"))
                {
                    var isUserExists = await _userManager.FindByEmailAsync(userRegisterRequestDTO.Email);
                    if(isUserExists == null)
                    {
                        var user = new User()
                        {
                            Email = userRegisterRequestDTO.Email,
                            UserName = userRegisterRequestDTO.Email,
                            Name = userRegisterRequestDTO.FirstName + " " + userRegisterRequestDTO.LastName,
                            DateCreated = System.DateTime.Now,
                            Role = userRegisterRequestDTO.Role,
                            City = userRegisterRequestDTO.City,
                            Skills = userRegisterRequestDTO.Skills,
                            Phone = userRegisterRequestDTO.PhoneNumber,
                            Score = 0,
                            ProfileImageUrl = userRegisterRequestDTO.ProfileImageUrl,
                            Tier = ""
                        };
                        var isUserCreated = await _userManager.CreateAsync(user, userRegisterRequestDTO.Password);
                        if (isUserCreated.Succeeded)
                        {
                            var userDTO = new UserDTO()
                            {
                                Id = user.Id,
                                Email = user.Email,
                                UserName = user.Name,
                                DateCreated = user.DateCreated,
                                Role = user.Role,
                                City = user.City,
                                Score = user.Score,
                                ProfileImageUrl = user.ProfileImageUrl,
                                Tier = user.Tier,
                                Skills = user.Skills,
                                PhoneNumber = user.Phone,
                                Experience = user.Experience,
                                Token = GenerateToken(user)
                            };
                            return Ok(new AuthResponseModel()
                            {
                                ResponseCode = ResponseCode.Ok,
                                ResponseMessage = "User Registration Successfull",
                                DataSet = userDTO
                            });
                        }
                        return BadRequest(new AuthResponseModel()
                        {
                            ResponseCode = ResponseCode.Error,
                            ResponseMessage = string.Join("\n", isUserCreated.Errors.Select(x => x.Description).ToArray()),
                            DataSet = null
                        });
                    }
                    return BadRequest(new AuthResponseModel()
                    {
                        ResponseCode = ResponseCode.Error,
                        ResponseMessage = "Email Already Exists, Please Login",
                        DataSet = null
                    });
                }
                return BadRequest(new AuthResponseModel()
                {
                    ResponseCode = ResponseCode.Error,
                    ResponseMessage = "This is email is not a valid Email",
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

        [HttpPost("Login")]
        public async Task<IActionResult> LoginUser([FromBody] UserLoginRequestDTO userLoginRequest)
        {

            if (ModelState.IsValid)
            {
                var isUserExists = await _userManager.FindByEmailAsync(userLoginRequest.Email);
                if (isUserExists == null)
                {
                    return BadRequest(new AuthResponseModel()
                    {
                        ResponseCode = ResponseCode.Error,
                        ResponseMessage = "Invalid User, Please register",
                        DataSet = null
                    });
                }
                var user = await _userManager.CheckPasswordAsync(isUserExists, userLoginRequest.Password);
                //var user = await _signInManager.PasswordSignInAsync(userLoginRequest.Email, userLoginRequest.Password, false, false);
                if (!user)
                {
                    return BadRequest(new AuthResponseModel()
                    {
                        ResponseCode = ResponseCode.Error,
                        ResponseMessage = "Invalid Credentials",
                        DataSet = null
                    });
                }
                var userDTO = new UserDTO()
                {
                    Id = isUserExists.Id,
                    Email = isUserExists.Email,
                    UserName = isUserExists.Name,
                    DateCreated = isUserExists.DateCreated,
                    Role = isUserExists.Role,
                    City = isUserExists.City,
                    Score = isUserExists.Score,
                    ProfileImageUrl = isUserExists.ProfileImageUrl,
                    Tier = isUserExists.Tier,
                    Skills = isUserExists.Skills,
                    PhoneNumber = isUserExists.Phone,
                    Experience = isUserExists.Experience,
                    Token = GenerateToken(isUserExists),

                };
                return Ok(new AuthResponseModel()
                {
                    ResponseCode = ResponseCode.Ok,
                    ResponseMessage = $"Login Successfull, Hello {isUserExists.UserName} ",
                    DataSet = userDTO
                });
            }
            return BadRequest(new AuthResponseModel()
            {
                ResponseCode = ResponseCode.Error,
                ResponseMessage = "All Fields are Required !",
                DataSet = null
            });
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = _userManager.Users.Select(x => new UserDTO()
            {
                Id=x.Id,
                UserName = x.Name,
                Email = x.Email,
                DateCreated = x.DateCreated,
                City = x.City,
                Role = x.Role,
                Score = x.Score,
                Tier = x.Tier,
                ProfileImageUrl=x.ProfileImageUrl,
                Skills = x.Skills,
                PhoneNumber = x.Phone,
                Experience = x.Experience
            });
            return Ok(new AuthResponseModel()
            {
                ResponseCode = ResponseCode.Ok,
                ResponseMessage = "All Users",
                DataSet = users
            });
        }

        [HttpPut("UpdateUserScore")]
        public async Task<IActionResult> UpdateUserScore()
        {
            var users = await _userManager.Users.ToListAsync();
            foreach(var user in users)
            {
                var score = await _replyRepository.GetUpvotesForUserReplies(user.Id);
                user.Score = score;
                await _userManager.UpdateAsync(user);
            }
            return Ok(new AuthResponseModel()
            {
                ResponseCode= ResponseCode.Ok,
                ResponseMessage= "User Scores have been updated",
                DataSet = null
            });
        }

        [HttpGet("GetTopUsers")]
        public async Task<IActionResult> GetTopUsers()
        {
            var users = await _userManager.Users.OrderByDescending(x => x.Score).ToListAsync();
            return Ok(new AuthResponseModel()
            {
                ResponseCode = ResponseCode.Ok,
                ResponseMessage = "Top Users",
                DataSet = users
            });
        }

        [HttpGet("GetUser/{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user == null)
            {
                return BadRequest(new AuthResponseModel()
                {
                    ResponseCode= ResponseCode.Error,
                    ResponseMessage = "User Not Found",
                    DataSet= null
                });
            }
            return Ok(new AuthResponseModel()
            {
                ResponseCode = ResponseCode.Ok,
                ResponseMessage = $"Hello {user.UserName}",
                DataSet = new UserDTO()
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.Name,
                    DateCreated = user.DateCreated,
                    Role = user.Role,
                    City = user.City,
                    Score = user.Score,
                    ProfileImageUrl = user.ProfileImageUrl,
                    Tier = user.Tier,
                    Skills = user.Skills,
                    PhoneNumber = user.Phone,
                    Experience = user.Experience
                }
            });
        }

        private string GenerateToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("JwtConfig:SecretKey").Value);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.NameId, user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                }),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Expires = DateTime.Now.AddHours(2),
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }

        [HttpPost("ResetPasswordToken")]
        public async Task<IActionResult> ResetPasswordToken([FromBody] ResetPasswordTokenRequest model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                return Ok(new AuthResponseModel() { ResponseCode = ResponseCode.Ok, ResponseMessage = "Reset Password Token generated", DataSet = token });
            }
            return BadRequest(new AuthResponseModel() { ResponseCode = ResponseCode.Error, ResponseMessage = "Invalid User", DataSet = null });
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDTO resetPasswordRequest)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(resetPasswordRequest.Email);
                if (user == null)
                {
                    return BadRequest(new AuthResponseModel()
                    {
                        ResponseCode = ResponseCode.Error,
                        ResponseMessage = "User Not Found",
                        DataSet = null
                    });
                }
                if (string.Compare(resetPasswordRequest.NewPassword, resetPasswordRequest.ConfirmNewPassword) != 0)
                {
                    return BadRequest(new AuthResponseModel()
                    {
                        ResponseCode = ResponseCode.Error,
                        ResponseMessage = "Passwords doesn't match",
                        DataSet = null
                    });
                }
                if (string.IsNullOrEmpty(resetPasswordRequest.Token))
                {
                    return BadRequest(new AuthResponseModel() { ResponseCode = ResponseCode.Error, ResponseMessage = "Invalid Token", DataSet = null });
                }
                var passwordReset = await _userManager.ResetPasswordAsync(user, resetPasswordRequest.Token, resetPasswordRequest.NewPassword);
                if (!passwordReset.Succeeded)
                {
                    return BadRequest(new AuthResponseModel()
                    {
                        ResponseCode = ResponseCode.Error,
                        ResponseMessage = string.Join("\n", passwordReset.Errors.Select(x => x.Description).ToList()),
                        DataSet = null
                    });
                }
                return Ok(new AuthResponseModel() { ResponseCode = ResponseCode.Ok, ResponseMessage = "Password reset successfull", DataSet = null });
            }
            return BadRequest(new AuthResponseModel()
            {
                ResponseCode = ResponseCode.Error,
                ResponseMessage = "Unauthorized",
                DataSet = null
            });
        }

    }
}
