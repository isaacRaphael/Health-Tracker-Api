using HealthTracker.Dataservice.Configurations;
using HealthTracker.Dataservice.Data;
using HealthTracker.Entities;
using HealthTracker.Entities.DTOs.Incoming;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthTracker.Api.Controllers.v1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : BaseController
    {
        public UsersController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
        
        //Get
        [HttpGet]
        [HttpHead]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _unitOfWork.UserRepo.GetAll();
            return Ok(users);
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [HttpPost]
        public async Task<IActionResult> AddUser(UserDto userdto)
        {
            var _user = new User()
            {
                LastName = userdto.LastName,
                FirstName = userdto.FirstName,
                Email = userdto.Email,
                DateOfBirth = Convert.ToDateTime(userdto.DateOfBirth),
                Phone = userdto.Phone,
                Country = userdto.Country,
                Status = 1
            };
            await _unitOfWork.UserRepo.Add(_user);
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetUser), new { Id = _user.Id }, _user);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user =await  _unitOfWork.UserRepo.GetById(id);
            return Ok(user);
        }
    }
}
