namespace ObjectDetectionMachineLearning.Web.Models
{

    public class MLPrediction
    {
        public List<string> predictedLabel { get; set; }
        public List<float> score { get; set; }
        public List<float> predictedBoundingBoxes { get; set; }
    }

}
