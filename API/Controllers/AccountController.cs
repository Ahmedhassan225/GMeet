using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,ITokenService tokenService, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto){

            if(await UserExists(registerDto.UserName)) return BadRequest("UserName is tacken");
            
            var user = _mapper.Map<AppUser>(registerDto);
            using var hmac = new HMACSHA512();

            user.UserName = registerDto.UserName.ToLower();
            // user.PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(registerDto.PassWord));
            // user.PasswordSalt = hmac.Key;

            var result = await _userManager.CreateAsync(user, registerDto.PassWord);

            if(!result.Succeeded) return BadRequest(result.Errors);

            var roleResult = await _userManager.AddToRoleAsync(user, "Member");

            if(!roleResult.Succeeded) return BadRequest(result.Errors);

            return new UserDto{
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto){

            var user = await _userManager.Users
                    .Include(p => p.Photos)
                    .SingleOrDefaultAsync(x => x.UserName == loginDto.UserName.ToLower());

            if(user == null) return Unauthorized("Invalid userName");

            // using var hmac = new HMACSHA512(user.PasswordSalt);

            // var computerhash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(loginDto.PassWord));

            // for(int i =0 ;i<computerhash.Length; i++){
            //     if(computerhash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
            // }
            var result = await _signInManager
                    .CheckPasswordSignInAsync(user, loginDto.PassWord, false);

            if(!result.Succeeded)return Unauthorized();

            return new UserDto
            {
                Username = user.UserName,
                Token =  await _tokenService.CreateToken(user),
                photoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        private async Task<bool> UserExists(string userName){
            return await _userManager.Users.AnyAsync(x => x.UserName == userName.ToLower());
        }
        
        
    }
}