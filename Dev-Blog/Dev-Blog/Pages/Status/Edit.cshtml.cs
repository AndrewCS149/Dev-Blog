using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dev_Blog.Models;
using Dev_Blog.Models.Base;
using Dev_Blog.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dev_Blog.Pages.Status
{
    [Authorize(Policy = "Admin")]
    public class EditModel : BasePage
    {
        private readonly IPost _post;

        [BindProperty]
        public Post Post { get; set; }

        [BindProperty]
        public string Description { get; set; }

        public EditModel(IPost post)
        {
            _post = post;
        }

        public async Task<IActionResult> OnGet(int id)
        {
            Post = await _post.GetPost(id);
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            await _post.Edit(Post, Description);
            return RedirectToPage("Posts");
        }
    }
}