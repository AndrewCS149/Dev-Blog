using DevBlog.Interfaces;
using DevBlog.Models;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
//using static DevBlog.Pages.Modals.Register;

namespace DevBlog.Controllers
{
    public class AccountController : ControllerBase
    {
        public SignInManager<UserModel> SignInMgr { get; }
        public UserManager<UserModel> UserMgr { get; }

        private readonly IEmailRepository _email;

        public AccountController(SignInManager<UserModel> sm, UserManager<UserModel> um, IEmailRepository email)
        {
            _email = email;
            UserMgr = um;
            SignInMgr = sm;
        }

        [HttpPost("/deleteAccount")]
        public async Task<IActionResult> DeleteAccount()
        {
            // get current user
            var username = User.Identity.Name.ToUpper();
            UserModel user = UserMgr.Users.Where(x => x.NormalizedUserName == username).FirstOrDefault();

            // sign current user out and delete account
            await SignInMgr.SignOutAsync();
            await UserMgr.DeleteAsync(user);

            return Redirect("/");
        }

        [HttpPost("/signin")]
        public async Task<IActionResult> SignIn([FromForm] string username, [FromForm] string password)
        {
            var res = await SignInMgr.PasswordSignInAsync(username, password, true, lockoutOnFailure: true);
            if (res.Succeeded)
                return Redirect("/");

            return Redirect("/signin/attempt");
        }

        [HttpPost("/signout")]
        public async Task<IActionResult> LogOut()
        {
            await SignInMgr.SignOutAsync();
            return Redirect("/");
        }

        //[HttpPost("/signup")]
        //public async Task<IActionResult> Register([FromForm] RegisterVM registerVM)
        //{
        //    var user = new UserModel()
        //    {
        //        Subscribed = true,
        //        UserName = registerVM.UserName,
        //        Email = registerVM.Email
        //    };

        //    var res = await UserMgr.CreateAsync(user, registerVM.Password);
        //    if (res.Succeeded)
        //    {
        //        await _email.Welcome(registerVM.Email);

        //        var curUser = await UserMgr.FindByNameAsync(user.UserName);
        //        var roleResult = UserMgr.AddToRoleAsync(curUser, "Visitor");
        //        roleResult.Wait();

        //        var signIn = await SignInMgr.PasswordSignInAsync(user.UserName, registerVM.Password, true, lockoutOnFailure: true);

        //        if (signIn.Succeeded)
        //            return Redirect("/");
        //    }

        //    return Redirect("/error");
        //}
    }
}