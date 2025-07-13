using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Text;
using System.Text.Json;

namespace ObjectDetectionMachineLearning.Web.Components.Pages
{
    partial class Home
    {

        [Inject]
        private HttpClient Http { get; set; } = default!;

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
                string savedUploadedImageFullPath = await SaveUploadedImage(file);

                // Optional: Set preview if you still want to show it in the UI
                using var ms = new MemoryStream();
                using var previewStream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
                await previewStream.CopyToAsync(ms);
                var bytes = ms.ToArray();
                UploadedImagePreview = $"data:{file.ContentType};base64,{Convert.ToBase64String(bytes)}";

                string? prediction = await CallPredictApiAsync(savedUploadedImageFullPath);
                Console.WriteLine($"Prediction {prediction}");
            }
        }

        private static async Task<string> SaveUploadedImage(IBrowserFile file)
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

            return filePath;
        }

        private async Task<string?> CallPredictApiAsync(string imagePath)
        {
            try
            {
                var payload = new { imagePath = imagePath };
                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                var response = await Http.PostAsync("https://localhost:65194/predict", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Prediction result: " + result);
                    return result;
                }
                else
                {
                    Console.WriteLine($"API call failed: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error calling API: " + ex.Message);
            }

            return null;
        }

    }
}
