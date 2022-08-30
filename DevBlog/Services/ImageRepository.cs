using DevBlog.Interfaces;
using Dropbox.Api;
using Dropbox.Api.Files;

namespace DevBlog.Services
{
    public class ImageRepository : IImageRepository
    {
        private readonly IConfiguration _config;

        public ImageRepository(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Uploads an image to Dropbox account
        /// </summary>
        /// <param name="fs">Image file stream</param>
        /// <param name="name">Name of image</param>
        /// <returns>Dropbox url to image</returns>
        public async Task<string> AddImgToDropBox(Stream fs, string name)
        {
            // create unique file name
            string ext = Path.GetExtension(name);
            string fileName = $"{DateTime.Now.Ticks}{name}{ext}";


            var destinationPath = _config["DropboxDestinationPath"];

            string url = "";
            string dest = destinationPath + fileName;

            using (var dbx = new DropboxClient(_config["DropboxToken"]))
            {
                // upload file to dbx
                var updated = await dbx.Files.UploadAsync(
                    dest,
                    WriteMode.Overwrite.Instance,
                    body: fs
                );
                fs.Close();

                // create shareable link
                var link = dbx.Sharing.CreateSharedLinkWithSettingsAsync(dest);
                link.Wait();

                url = link.Result.Url;

                // remove id and replace with raw=1
                url = url.Substring(0, url.Length - 4) + "raw=1";
            }

            return url;
        }
    }
}