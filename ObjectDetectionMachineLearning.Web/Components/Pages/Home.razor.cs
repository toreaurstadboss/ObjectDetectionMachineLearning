﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using ObjectDetectionMachineLearning.Web.Models;
using System.Text;
using System.Text.Json;

namespace ObjectDetectionMachineLearning.Web.Components.Pages
{
    partial class Home
    {

        [Inject]
        private HttpClient Http { get; set; } = default!;

        [Inject]
        private IJSRuntime JsRunTime { get; set; } = default!;

        private string? UploadedImagePreview;

        private MLPrediction LatestPrediction = default!;

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

                var jsonBboxes = CreateBoundingBoxJson(prediction);
                await JsRunTime.InvokeVoidAsync("InitLoadBoundingBoxes", jsonBboxes);

                Console.WriteLine($"Prediction {prediction}");

                //StateHasChanged();
            }
        }

        private string CreateBoundingBoxJson(string? prediction)
        {
            if (string.IsNullOrEmpty(prediction))
                return "[]";
            try
            {
                var mlPrediction = JsonSerializer.Deserialize<MLPrediction>(prediction);
                LatestPrediction = mlPrediction;
                return ConvertMLPredictionToBoundingBoxJson(mlPrediction!);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing prediction: {ex.Message}");
                return "[]";
            }

        }

        public static string ConvertMLPredictionToBoundingBoxJson(MLPrediction prediction)
        {
            var boxes = prediction.predictedBoundingBoxes;
            var labels = prediction.predictedLabel ?? new List<string>();
            var scores = prediction.score ?? new List<float>();

            if (boxes == null || boxes.Count % 4 != 0)
                return "[]";

            var results = new List<object>();

            for (int i = 0; i < boxes.Count; i += 4)
            {
                float x1 = boxes[i];
                float y1 = boxes[i + 1];
                float x2 = boxes[i + 2];
                float y2 = boxes[i + 3];

                float width = x2 - x1;
                float height = y2 - y1;

                results.Add(new
                {
                    Name = i / 4 < labels.Count ? labels[i / 4] : "Unknown",
                    X = x1,
                    Y = y1,
                    Width = width,
                    Height = height,
                    Confidence = i / 4 < scores.Count ? scores[i / 4].ToString("0.0000") : "0.0000"
                });
            }

            return JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true });
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
