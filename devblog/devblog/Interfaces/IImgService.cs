﻿using devblog.Models;

namespace devblog.Interfaces
{
    public interface IImgService
    {
        /// <summary>
        /// Gets the first or default img from a specific post
        /// </summary>
        /// <returns>Img</returns>
        Task<Img> Get(int postId);

        /// <summary>
        /// Uploads an image to Dropbox account
        /// </summary>
        /// <param name="files">Files to upload</param>
        /// <param name="postId">Post Id</param>
        Task<HttpResponseMessage> Create(IFormFile[] files, int postId);

        /// <summary>
        /// Delete an img from dropbox account
        /// </summary>
        /// <param name="imgs">List of imgs to delete</param>
        Task DeleteImgFromDropBox(List<Img> imgs);
    }
}
