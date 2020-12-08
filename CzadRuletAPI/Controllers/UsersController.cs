using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CzadRuletAPI.Entities;
using CzadRuletAPI.Helpers;
using CzadRuletAPI.Models;
using CzadRuletAPI.Services.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CzadRuletAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public UsersController(
            IUserService userService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] AuthenticateModel model)
        {
            User user = _userService.Authenticate(model.Username, model.Password);

            if (user == null)
            {
                //check if can auth with email
                user = _userService.Authenticate(model.Username, model.Password);
            }

            if (user == null)
            {
                return BadRequest(new {message = "Username or password is incorrect"});
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // return basic user info and authentication token
            return Ok(new
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Token = tokenString
            });
        }

        [HttpGet]
        [Description("Returns account details for currently authenticated user")]
        public IActionResult GetOwn()
        {
            var currentId = HttpContext.User.Identity.Name;
            var user = _userService.GetById(int.Parse(currentId));
            var model = _mapper.Map<UserModel>(user);
            return Ok(model);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Register([FromBody] RegisterModel model)
        {
            // map model to entity
            var user = _mapper.Map<User>(model);

            try
            {
                // create user
                _userService.Create(user, model.Password);
                var userModel = _mapper.Map<UserModel>(user);
                return Created(nameof(GetOwn),userModel);
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(new {message = ex.Message});
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult Update(int id, [FromBody] UpdateModel model)
        {
            if (HttpContext.User.Identity.Name != id.ToString())
            {
                return Forbid("You can't change user details for someone else account.");
            }

            // map model to entity and set id
            var user = _mapper.Map<User>(model);
            user.Id = id;
            try
            {
                // update user 
                _userService.Update(user, model.Password);
                return Ok();
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(new {message = ex.Message});
            }
        }
    }
}