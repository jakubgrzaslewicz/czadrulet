using System;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using CzadRuletAPI.Entities;
using CzadRuletAPI.Helpers;
using CzadRuletAPI.Models;
using CzadRuletAPI.Services.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CzadRuletAPI.Controllers
{
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

        /// <summary>
        /// Authenticated user with provided username or email and password. Returns account details and token
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /authenticate
        ///     {
        ///        "Username": "cyber",
        ///        "Password": "punk"
        ///     }
        ///
        /// </remarks>
        /// <param name="model">AuthenticateModel that contains Username and Password or Email and Password</param>
        /// <returns>Instance of AuthenticatedModel that contains account details and token.</returns>
        [AllowAnonymous]
        [HttpPost("authenticate")]
        [ProducesResponseType(typeof(AuthenticatedModel), StatusCodes.Status200OK)]
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
            return Ok(new AuthenticatedModel(user.Id, user.Username, user.Email, tokenString));
        }

        /// <summary>
        /// Returns UserModel of currently authenticated account
        /// </summary>
        /// <returns>UserModel of currently authenticated account</returns>
        [Authorize]
        [HttpGet]
        [Description("Returns account details for currently authenticated user")]
        [ProducesResponseType(typeof(UserModel), StatusCodes.Status200OK)]
        public IActionResult GetOwn()
        {
            var currentId = HttpContext.User.Identity.Name;
            var user = _userService.GetById(int.Parse(currentId));
            var model = _mapper.Map<UserModel>(user);
            return Ok(model);
        }

        /// <summary>
        /// Creates new account with provided details
        /// </summary>
        /// <param name="model">RegisterModel describing new account</param>
        /// <returns>UserModel that represents newly created user account</returns>
        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(typeof(UserModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public IActionResult Register([FromBody] RegisterModel model)
        {
            var user = _mapper.Map<User>(model);

            try
            {
                _userService.Create(user, model.Password);
                var userModel = _mapper.Map<UserModel>(user);
                return Created(nameof(GetOwn), userModel);
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(new {message = ex.Message});
            }
        }

        /// <summary>
        /// Updates account details of currently authenticated user 
        /// </summary>
        /// <param name="model"> UpdateModel that will be used to change account fields</param>
        /// <returns>Code 200 if updated correctly</returns>
        /// <response code="200">Returns if updated correctly</response>
        [Authorize]
        [HttpPut]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Update([FromBody] UpdateModel model)
        {
            // map model to entity and set id
            var user = _mapper.Map<User>(model);
            user.Id = int.Parse(HttpContext.User.Identity.Name);
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