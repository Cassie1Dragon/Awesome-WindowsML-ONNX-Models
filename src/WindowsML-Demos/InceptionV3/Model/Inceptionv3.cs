using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.AI.MachineLearning;
namespace InceptionV3
{
    
    public sealed class Inceptionv3Input
    {
        public TensorFloat16Bit image; // shape(-1,3,299,299)
    }
    
    public sealed class Inceptionv3Output
    {
        public TensorString classLabel; // shape(-1,1)
        public IList<Dictionary<string,float>> classLabelProbs;
    }
    
    public sealed class Inceptionv3Model
    {
        private LearningModel model;
        private LearningModelSession session;
        private LearningModelBinding binding;
        public static async Task<Inceptionv3Model> CreateFromStreamAsync(IRandomAccessStreamReference stream)
        {
            Inceptionv3Model learningModel = new Inceptionv3Model();
            learningModel.model = await LearningModel.LoadFromStreamAsync(stream);
            learningModel.session = new LearningModelSession(learningModel.model);
            learningModel.binding = new LearningModelBinding(learningModel.session);
            return learningModel;
        }
        public async Task<Inceptionv3Output> EvaluateAsync(Inceptionv3Input input)
        {
            binding.Bind("image", input.image);
            var result = await session.EvaluateAsync(binding, "0");
            var output = new Inceptionv3Output();
            output.classLabel = result.Outputs["classLabel"] as TensorString;
            output.classLabelProbs = result.Outputs["classLabelProbs"] as IList<Dictionary<string,float>>;
            return output;
        }
    }
}
