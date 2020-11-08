using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Dev_Blog.Models;
using Dev_Blog.Models.Base;
using Dev_Blog.Models.Interfaces;
using Dev_Blog.Models.ViewModels;
using Dropbox.Api.CloudDocs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using PagedList;
using PagedList.Mvc;

namespace Dev_Blog.Pages.Status
{
    public class PostsModel : BasePage
    {
        private readonly IPost _post;
        private readonly IConfiguration _config;
        private readonly IComment _comment;

        [BindProperty]
        public int PageNumber { get; set; }

        [BindProperty]
        public int LastPage { get; set; }

        [BindProperty]
        public List<Post> Posts { get; set; }

        [BindProperty]
        public Post Post { get; set; }

        [BindProperty]
        public List<Comment> Comments { get; set; }

        [BindProperty]
        public Comment Comment { get; set; }

        [BindProperty]
        public string AdminUser { get; set; }

        public PostsModel(IPost post, IConfiguration config, IComment comment, SignInManager<User> signInManager, UserManager<User> userManager) : base(signInManager, userManager)
        {
            _comment = comment;
            _config = config;
            _post = post;
        }

        public async Task<IActionResult> OnGet()
        {
            // if user does not have page tracking cookie, create one
            int pageIdx;
            if (Request.Cookies["pageTracker"] == null)
                pageIdx = AddCookie(1);
            else
                pageIdx = int.Parse(Request.Cookies["pageTracker"]);

            PageNumber = pageIdx;
            AdminUser = _config["AdminUserName"];
            Posts = await _post.GetPage(pageIdx);

            return Page();
        }

        public async Task<IActionResult> OnGetPageLeft()
        {
            int pageIdx = int.Parse(Request.Cookies["pageTracker"]) - 1;
            AdminUser = _config["AdminUserName"];

            if (pageIdx >= 1)
            {
                AddCookie(pageIdx);
                Posts = await _post.GetPage(pageIdx);
            }
            else
                Posts = await _post.GetPage(++pageIdx);

            PageNumber = pageIdx;

            return Page();
        }

        public async Task<IActionResult> OnGetPageRight()
        {
            int pageIdx = int.Parse(Request.Cookies["pageTracker"]) + 1;
            AdminUser = _config["AdminUserName"];

            if (await _post.CanPageRight(pageIdx))
            {
                AddCookie(pageIdx);
                Posts = await _post.GetPage(pageIdx);
            }
            else
                Posts = await _post.GetPage(--pageIdx);

            LastPage = await _post.GetLastPage();
            PageNumber = pageIdx;

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteComment()
        {
            Comment comment = await _comment.GetComment(Comment.Id);
            await _comment.Delete(comment);
            return RedirectToPagePermanent("Posts");
        }

        public async Task<IActionResult> OnPostDeletePost()
        {
            // get post being deleted
            var post = await _post.GetPost(Post.Id);

            await _post.Delete(post);
            return RedirectToPagePermanent("Posts");
        }

        // cookie to keep track of current page
        public int AddCookie(int page)
        {
            string key = "pageTracker";
            string val = page.ToString();
            CookieOptions cookie = new CookieOptions();
            cookie.Expires = DateTime.Now.AddDays(30);
            Response.Cookies.Append(key, val, cookie);

            return int.Parse(val);
        }
    }
}