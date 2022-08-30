namespace DevBlog.Interfaces
{
    public interface IImageRepository
    {
        /// <summary>
        /// Uploads an image to Dropbox account
        /// </summary>
        /// <param name="fs">Image file stream</param>
        /// <param name="name">Name of image</param>
        /// <returns>Dropbox url to image</returns>
        Task<string> AddImgToDropBox(Stream fs, string name);
    }
}