using devblog.Interfaces;
using devblog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace devblog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        public SignInManager<User> _signInMgr { get; }
        public UserManager<User> _userMgr { get; }
        public IConfiguration _config;
        private readonly INotificationService _notifications;
        private readonly IUsernameService _username;
        private readonly IEmailService _email;

        public AccountsController(INotificationService notifications, SignInManager<User> signInMgr, UserManager<User> usermgr, IConfiguration config, IUsernameService username, IEmailService email)
        {
            _notifications = notifications;
            _userMgr = usermgr;
            _signInMgr = signInMgr;
            _config = config;
            _username = username;
            _email = email;
        }


        [Authorize]
        [HttpPut("subscribe")]
        public async Task<bool> ToggleSubscribe()
        {
            var username = User.FindFirstValue("userName");
            var user = await _userMgr.Users.Where(u => u.NormalizedUserName == username).FirstOrDefaultAsync();

            var subscribed = await _email.ToggleSubscribe(user);
            return subscribed;
        }

        [Authorize]
        [HttpGet("user")]
        public async Task<UserInfo> GetCurrentUser()
        {
            var username = User.FindFirstValue("userName");
            var user = await _userMgr.Users.Where(u => u.NormalizedUserName == username).FirstOrDefaultAsync();

            var userInfo = new UserInfo
            {
                UserName = user.UserName,
                Email = user.Email,
                Subscribed = user.Subscribed,
            };

            return userInfo;
        }

        /// <summary>
        /// Returns all user's usernames and emails
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<List<UserInfo>> GetUsers()
        {
            var users = await _userMgr.Users
                .Select(u => new UserInfo
                {
                    UserName = u.UserName,
                    Email = u.Email,
                    Subscribed = u.Subscribed,
                })
                .ToListAsync();

            return users;
        }

        /// <summary>
        /// Delete an account
        /// </summary>
        /// <param name="username"></param>
        [Authorize(Roles = "Visitor")]
        [HttpDelete]
        public async Task<IActionResult> DeleteAccount()
        {
            // get current user
            var username = User.FindFirstValue("username");
            User user = _userMgr.Users.Where(x => x.NormalizedUserName == username).FirstOrDefault();

            if (user != null)
            {
                // sign current user out and delete account & notifications
                await _notifications.DeleteAllForUser(username);
                await _signInMgr.SignOutAsync();
                await _userMgr.DeleteAsync(user);
                return Ok();
            }

            return BadRequest(new { error = "Error attempting to delete account." });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("adminDelete/{username}")]
        public async Task AdminDeleteAccount(string username)
        {
            User user = _userMgr.Users.Where(x => x.NormalizedUserName == username).FirstOrDefault();

            if (user != null)
            {
                await _notifications.DeleteAllForUser(username);
                await _userMgr.DeleteAsync(user);
            }
        }


        /// <summary>
        /// Signs in a user
        /// </summary>
        /// <param name="signIn"></param>
        [HttpPost("signin")]
        public async Task<IActionResult> SignIn(SignIn signIn)
        {
            var user = await _userMgr.FindByNameAsync(signIn.Username);

            if (user == null || !await _userMgr.CheckPasswordAsync(user, signIn.Password))
            {
                return BadRequest(new { description = "Invalid username or password", code = "" });
            }
            else
            {
                var res = await _signInMgr.PasswordSignInAsync(user, signIn.Password, true, false);
                var claims = await GenerateClaims(user);
                var token = GenerateToken(claims);

                if (res.Succeeded)
                {
                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo,
                        username = user.UserName.Normalize(),
                        authenticated = true,
                        admin = await _userMgr.IsInRoleAsync(user, "Admin"),
                    });
                }
                else
                    return BadRequest(new { description = "Unable to sign in. Please try again", code = "" });

            }
        }

        /// <summary>
        /// Signs out the currently sign in user
        /// </summary>
        /// <returns>Task<IActionResult></returns>
        //[Authorize]
        [HttpPost("signout")]
        public async Task<IActionResult> LogOut()
        {
            await _signInMgr.SignOutAsync();

            return Ok(new
            {
                token = "",
                expiration = "",
                username = "",
                authenticated = false,
                admin = false,
            });
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="user">New user to add</param>
        /// <returns>Task<IActionResult></returns>
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(User user)
        {
            var errors = new List<IdentityError>();

            // verify unique username
            var userName = await _username.Exists(user.UserName.Normalize());
            if (userName)
            {
                var error = new IdentityError();
                error.Description = $"Username '{user.UserName}' is already taken.";
                errors.Add(error);
            }

            // verify unique email
            var email = _userMgr.Users.Where(x => x.NormalizedEmail == user.Email.Normalize()).FirstOrDefault();

            user.Subscribed = true;
            var res = await _userMgr.CreateAsync(user, user.PasswordHash);
            if (res.Succeeded && errors.Count == 0)
            {
                await _email.Welcome(user.Email);
                var currentUser = await _userMgr.FindByNameAsync(user.UserName);

                await _userMgr.AddToRoleAsync(currentUser, "Visitor");
                await _signInMgr.PasswordSignInAsync(user.UserName, user.PasswordHash, true, false);
                await _username.Create(user.UserName.Normalize());

                var claims = await GenerateClaims(user);
                var token = GenerateToken(claims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    username = user.UserName,
                    authenticated = true,
                    admin = false,
                });
            }
            else
            {
                errors.AddRange(res.Errors);

                // Ensure to not return 'null' as this will cause front end parsing errors
                errors.ForEach(e =>
                {
                    if (e.Code is null)
                        e.Code = "";
                });

                return BadRequest(errors);
            }
        }


        private JwtSecurityToken GenerateToken(List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetValue<string>("Key")));

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

            var userRoles = await _userMgr.GetRolesAsync(user);
            foreach (var userRole in userRoles)
                claims.Add(new Claim("role", userRole));
            //claims.Add(new Claim(ClaimTypes.Role, userRole));

            return claims;
        }

        public class UserInfo
        {
            public string? UserName { get; set; }
            public string? Email { get; set; }
            public bool Subscribed { get; set; }
        }
    }
}