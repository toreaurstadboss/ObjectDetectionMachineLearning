using Microsoft.AspNetCore.Components.Forms;

namespace ObjectDetectionMachineLearning.Web.Components.Pages
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

            if (file != null && (file.ContentType == "image/jpeg" || file.ContentType == "image/png"))
            {
                await SaveUploadedImage(file);

                // Optional: Set preview if you still want to show it in the UI
                using var ms = new MemoryStream();
                using var previewStream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
                await previewStream.CopyToAsync(ms);
                var bytes = ms.ToArray();
                UploadedImagePreview = $"data:{file.ContentType};base64,{Convert.ToBase64String(bytes)}";
            }
        }

        private static async Task SaveUploadedImage(IBrowserFile file)
        {
            var uploadsFolder = Path.Combine(Environment.CurrentDirectory, "UploadedImages");
            Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

            var fileName = $"{Guid.NewGuid()}_{file.Name}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024))
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await stream.CopyToAsync(fileStream);
            }
        }
    }
}
