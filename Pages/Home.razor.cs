using Microsoft.AspNetCore.Components.Forms;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;

namespace ObjectDetectionMachineLearning.Pages
{
    partial class Home
    {

        private string? UploadedImagePreview;

        /// <summary>
        /// Uploads an image and sets the imagePreview property to display it
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task OnInputFile(InputFileChangeEventArgs e)
        {
            var file = e.File;

            if (file != null && (file!.ContentType == "image/jpeg" || file!.ContentType == "image/png"))
            {
                using (var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024))
                {
                    using var ms = new MemoryStream();
                    await stream.CopyToAsync(ms);
                    var bytes = ms.ToArray(); 
                    UploadedImagePreview = $"data:{file.ContentType};base64,{Convert.ToBase64String(bytes)}";
                }
            }            
        }

    }
}
