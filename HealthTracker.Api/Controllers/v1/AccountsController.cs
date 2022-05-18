using HealthTracker.Api.Config.Models;
using HealthTracker.Authentication.Models.DTOs.Incoming;
using HealthTracker.Authentication.Models.DTOs.Outgoing;
using HealthTracker.Dataservice.Configurations;
using HealthTracker.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HealthTracker.Api.Controllers.v1
{
    
    public class AccountsController : BaseController
    {
        
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig;

        public AccountsController(IUnitOfWork unitOfWork,
            UserManager<IdentityUser> userManager,
            IOptionsMonitor<JwtConfig> optionsMonitor

            ) : base(unitOfWork)
        {
            _userManager = userManager;
            _jwtConfig = optionsMonitor.CurrentValue;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(UserRegRequestDto userRegRequestDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(new UserRegResponseDto
                {
                    Success = false,
                    Errors = new List<string> { "invalid payload" }
                });
            }

            var userExists = await _userManager.FindByEmailAsync(userRegRequestDto.Email);
            if(userExists is not null)
            {
                return BadRequest(new UserRegResponseDto
                {
                    Success = false,
                    Errors = new List<string> { "Email already in use" }
                });
            }

            var newUser = new IdentityUser()
            {
                Email = userRegRequestDto.Email,
                UserName = userRegRequestDto.Email,
                EmailConfirmed = true // todo build email functionality to send to the user to confirm email
            };

            var isCreated = await _userManager.CreateAsync(newUser, userRegRequestDto.Password);
            if(!isCreated.Succeeded)
                return BadRequest(new UserRegResponseDto
                {
                    Success = isCreated.Succeeded,
                    Errors = isCreated.Errors.Select(x => x.Description).ToList()
                });
            var _user = new User()
            {
                IdentityId = new Guid(newUser.Id),
                LastName = userRegRequestDto.LastName,
                FirstName = userRegRequestDto.FirstName,
                Email = userRegRequestDto.Email,
                DateOfBirth = DateTime.UtcNow,//Convert.ToDateTime(userdto.DateOfBirth),
                Phone = "",
                Country = "",
                Status = 1
            };
            await _unitOfWork.UserRepo.Add(_user);
            await _unitOfWork.CompleteAsync();

            var token = GenerateJwtToken(newUser);

            return Ok(new UserRegResponseDto
            {
                Success = true,
                Token = token
            }
            );
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(UserLoginRequestDto loginDto)
        {
            if(!ModelState.IsValid)
                return BadRequest(new UserRegResponseDto
                {
                    Success = false,
                    Errors = new List<string> { "invalid payload" }
                });

            var userExists = await _userManager.FindByEmailAsync(loginDto.Email);
            if (userExists is null)
                return BadRequest(new UserLoginResponseDto { Success = false, Errors = new List<string> { "Invalid authentication request" } });

            var isCorrect = await _userManager.CheckPasswordAsync(userExists, loginDto.Password);
            if(!isCorrect)
                return BadRequest(new UserLoginResponseDto { Success = false, Errors = new List<string> { "Invalid authentication request" } });

            var jwtToken = GenerateJwtToken(userExists);
            return Ok(new UserLoginResponseDto
            {
                Success = true,
                Token = jwtToken
            });
        }
        private string GenerateJwtToken(IdentityUser user)
        {
            // Handler is going to be responsible for creeating the Token
            var jwtHandler = new JwtSecurityTokenHandler();
            var Key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim("id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email), // a unique Id
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) //Used by the refresh token
                }),
                Expires = DateTime.UtcNow.AddHours(3),
                SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(Key), SecurityAlgorithms.HmacSha256Signature)
            };

            //Generate Secuirity Object oken
            var token = jwtHandler.CreateToken(tokenDescriptor);

            //turns security obj token to string
            var jwtToken = jwtHandler.WriteToken(token);

            return jwtToken;
        }
    }
}
