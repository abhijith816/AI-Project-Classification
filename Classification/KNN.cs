using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classification
{
    class KNN
    {
        //By default the K value of the Classifier is 1. 
        
        int k = 1;
        int labelRows = 28;
        int labelColumns = 28;
        bool isDigitData = true;
        
        public List<Label> trainingData = new List<Label>();
        public List<Label> testData = new List<Label>();
        public List<Label> validationData = new List<Label>();

        Input currentInput;

        public KNN()
        {
            currentInput = new Input(labelRows, labelColumns);
        }
        
        public KNN(int inputK, bool isFace = false )
        {
            currentInput = new Input(isFace);
            //currentInput.Initialize();
            labelRows = currentInput.LabelHeight;
            labelColumns = currentInput.LabelWidth;
            
            k = inputK;
            
        }
        //Distance is one of the feature here. We can use the Eucledian distance or manhattan distance or something else. This will decide the accuracy of the classifier
        //along with the value of K. So vary these two and check the accuracy.
        
        //This particular method calculates the difference of sum of all the pixel values. Using this method as a base method for now. 
        private double CalculateDistance(List<List<int>> trainingInput, List<List<int>> testInput)
        {
            double diff = Math.Abs(trainingInput.Sum(a => a.Sum()) - testInput.Sum(a => a.Sum())) ;
            return diff;
        }

        private double CalculateBoundaryDistance(List<List<int>> trainingInput, List<List<int>> testInput)
        {
            double sum = 0;

            for (int i = 0; i < labelRows; i++)
            {
                for (int j = 0; j < labelColumns; j++)
                {
                    sum += Math.Pow((trainingInput[i][j] - testInput[i][j]), 2);
                }

            }
            double diff = Math.Sqrt(sum);
            return diff;
        }

        private double CalculateEucledianDistance(List<List<int>> trainingInput, List<List<int>> testInput)
        {
            double sum = 0;
            for(int i =0; i< labelRows; i++)
            {
                for(int j =0 ; j< labelColumns; j++)
                {
                    sum += Math.Pow((trainingInput[i][j] - testInput[i][j]),2);
                }
                
            }
            double diff =  Math.Sqrt(sum);
            return diff;
        }

        //public List<int> Classify(List<Label> training, List<List<int>> test)
        //{
        //    List<int> kNearestLabels = training.OrderBy(a => CalculateEucledianDistance(a.Pixels, test)).Take(k).Select(b => b.LabelValue).ToList();
        //    //List<int> kNearestLabels = training.OrderBy(a => CalculateDistance(a.Pixels, test)).Take(k).Select(b => b.LabelValue).ToList();

        //    return kNearestLabels;
        //}
        
        public Dictionary<int, int> Run(string traininigPath = null, string testOrValidationFilePath = null,int numberOfTrainingSamples = 100,int numberOfTestSamples = 1, bool isValidation = false)
        {
            Dictionary<int, int> outputOne = new Dictionary<int, int>();
            //Initialize the Training data here.
            trainingData = currentInput.LoadDataFromFile(traininigPath, numberOfTrainingSamples, true,currentInput.trainingLabelPath);
            
            //Initialize the test data. Change the number of input labels gradually.
            testData = currentInput.LoadDataFromFile(testOrValidationFilePath, numberOfTestSamples,false,currentInput.validationLabelPath);

            Dictionary<int, List<int>> outputPairs = new Dictionary<int, List<int>>();

            foreach(Label l in testData)
            {
                //List<int> outputK = Classify(trainingData, l.Pixels);
                int outputK = Classify(trainingData, l.Pixels);
                //outputPairs.Add(l.LabelId, outputK);

                //var most = outputK.GroupBy(i=>i).
                //            OrderByDescending(grp=>grp.Count())
                //           .Select(grp=>grp.Key).First();

                //outputOne.Add(l.LabelId, Convert.ToInt32(most));
                outputOne.Add(l.LabelId, outputK);

            }

            //Chose the one that repeats most number of times among these K Nearest Neighbors.


            //return outputPairs;
            return outputOne;

        }

        public int Classify(List<Label> training, List<List<int>> test)
        {
            List<Label> kNearestLabels = training.OrderBy(a => CalculateEucledianDistance(a.Pixels, test)).Take(k).Select(b => b).ToList();

            //These Labels are the Closest. Now we have to calcualte the weight of these labels and sum if there are multiple occurences for the same label value.
            Dictionary<int, double> KClosestDictionary = new Dictionary<int, double>();
            double maxWeight = 0;
            int maxLabelValue = -1;
            foreach (Label l in kNearestLabels)
            {
                double weight = 1 / CalculateEucledianDistance(l.Pixels, test);
                if (KClosestDictionary.ContainsKey(l.LabelValue))
                {
                    KClosestDictionary[l.LabelValue] += weight;
                    if (maxWeight < KClosestDictionary[l.LabelValue])
                    {
                        maxWeight = KClosestDictionary[l.LabelValue];
                        maxLabelValue = l.LabelValue;
                    }
                }
                else
                {
                    KClosestDictionary.Add(l.LabelValue, weight);
                    if (maxWeight < KClosestDictionary[l.LabelValue])
                    {
                        maxWeight = KClosestDictionary[l.LabelValue];
                        maxLabelValue = l.LabelValue;
                    }
                }
            }

            return maxLabelValue;
        }

    }
}
