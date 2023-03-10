using Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        public SignInManager<User> SignInMgr { get; }
        public UserManager<User> UserMgr { get; }

        public IConfiguration _config;

        //private readonly IEmailRepository _email;

        public AccountsController(SignInManager<User> signInMgr, UserManager<User> usermgr, IConfiguration config)
        {
            UserMgr = usermgr;
            SignInMgr = signInMgr;
            _config = config;
        }

        //[HttpPost("/deleteAccount")]
        //public async Task<IActionResult> DeleteAccount()
        //{
        //    // get current user
        //    var userName = User.Identity.Name.ToUpper();
        //    User user = UserMgr.Users.Where(x => x.NormalizedUserName == userName).FirstOrDefault();

        //    // sign current user out and delete account
        //    await SignInMgr.SignOutAsync();
        //    await UserMgr.DeleteAsync(user);

        //    return Redirect("/");
        //}

        //[HttpGet("{user}")]
        //public async Task<string> GetUserName(User user)
        //{
        //    var res = await UserMgr.GetUserNameAsync(user);
        //    return res;
        //}

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn(SignIn signIn)
        {
            var user = await UserMgr.FindByNameAsync(signIn.Username);

            if (user == null || !await UserMgr.CheckPasswordAsync(user, signIn.Password))
            {
                return BadRequest(new { error = "Invalid userName or password" });
            }
            else
            {
                //var res = await SignInMgr.PasswordSignInAsync(signIn.Password, signIn.Username, true, false);
                await SignInMgr.PasswordSignInAsync(user, signIn.Password, true, false);

                var claims = await GenerateClaims(user);
                var token = GenerateToken(claims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                });
            }
        }

        /// <summary>
        /// Signs out the currently sign in user
        /// </summary>
        /// <returns>Task<IActionResult></returns>
        [Authorize]
        [HttpPost("signout")]
        public async Task<IActionResult> LogOut()
        {
            await SignInMgr.SignOutAsync();
            return Ok();
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="user">New user to add</param>
        /// <returns>Task<IActionResult></returns>
        [HttpPost]
        public async Task<IActionResult> SignUp(User user)
        {
            var res = await UserMgr.CreateAsync(user, user.PasswordHash);
            if (res.Succeeded)
            {
                //await _email.Welcome(registerVM.Email);
                var currentUser = await UserMgr.FindByNameAsync(user.UserName);
                await UserMgr.AddToRoleAsync(currentUser, "Visitor");
                await SignInMgr.PasswordSignInAsync(user.UserName, user.PasswordHash, true, false);

                var claims = await GenerateClaims(user);
                var token = GenerateToken(claims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            else
            {
                return BadRequest(new { error = "There was an error creating your account" });
            }
        }


        private JwtSecurityToken GenerateToken(List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetValue<string>("JWT:Key")));

            var token = new JwtSecurityToken(
                issuer: "https://localhost:44482/",
                audience: "https://localhost:44482/api/",
                expires: DateTime.Now.AddHours(3),
                claims: claims,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }

        private async Task<List<Claim>> GenerateClaims(User user)
        {
            var claims = new List<Claim>
            {
                new Claim("userName", user.UserName),
                new Claim("email",  user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var userRoles = await UserMgr.GetRolesAsync(user);
            foreach (var userRole in userRoles)
                claims.Add(new Claim("role", userRole));
            //claims.Add(new Claim(ClaimTypes.Role, userRole));

            return claims;
        }
    }
}