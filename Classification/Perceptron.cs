using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classification
{
    class Perceptron
    {
        Input input;
        Dictionary<int, List<int>> weightVectors;
        bool isTrainingComplete = false;
        List<Label> inputTrainingMatrix; 
        List<Label> inputTestingMatrix;
        List<Label> inputValidationMatrix;

        bool isFaceClassification = false;

        int digitSize = 28;
        int faceRowSize = 70;
        int faceColSize = 60;
        int inputRowSize = 0;
        int inputColSize = 0;

        int digitLabels = 10;
        int faceLabels = 2;
        int inputLabels = 0;

        int inputVectorLength = 0;

        int numberofTestValues = 100;
        int numberOfTrainingValues = 451;

        string inputTrainingPath = null;
        string inputTrainingLabelPath = null;

        string inputTestingPath = null;
        string inputTestingLabelPath = null;

        public Perceptron(bool isFaceClassification, int noTrainingSamples, int noTestSamples)
        {
            this.isFaceClassification = isFaceClassification;
            this.numberOfTrainingValues = noTrainingSamples;
            this.numberofTestValues = noTestSamples;
            initialize();
        }

        public void initialize()
        {
            if (!isFaceClassification)
            {
                inputRowSize = digitSize;
                inputColSize = digitSize;
                inputLabels = digitLabels;
               
            }
            else //is Face Classification
            {
                inputRowSize = faceRowSize;
                inputColSize = faceColSize;
                inputLabels = faceLabels;

            }

            inputVectorLength = inputRowSize * inputColSize;
            input = new Input(isFaceClassification);
            
            
            inputTrainingPath = input.trainingDataPath;
            inputTrainingLabelPath = input.trainingLabelPath;
            inputTestingPath = input.validationDataPath;
            inputTestingLabelPath = input.validationLabelPath;
            
            weightVectors = new Dictionary<int, List<int>>();
            
            //Initializing weight vectors for each of the 10 labels for digits
            // or 2 labels for faces
            for (int i = 0; i < inputLabels; i++)
            {
                List<int> tempList = new List<int>();

                for (int j = 0; j < inputVectorLength+1; j++)
                {
                    tempList.Add(0);
                }

                //Setting bias = 1 for the first label
                if (i == 0)
                    //tempList[0] = 1;
                    tempList[inputVectorLength] = 1;//*****Moving bias from start to the end*****

                weightVectors.Add(i, tempList);
            }

            inputTrainingMatrix = input.LoadDataFromFile(inputTrainingPath, numberOfTrainingValues, true,inputTrainingLabelPath);
            inputTestingMatrix = input.LoadDataFromFile(inputTestingPath, numberofTestValues, false,inputTestingLabelPath);
        }

        public Dictionary<int, Dictionary<int, List<int>>> getFeatureVectorOfInput(List<Label> inputMatrix)
        {
           // List<Label> inputMatrix = input.LoadDataFromFile(input.trainigDataPath, 1000, true);
            Dictionary<int, Dictionary<int, List<int>>> inputVector = getVectorFromLabelMatrix(inputMatrix);
            Dictionary<int, Dictionary<int, List<int>>> featureVector = new Dictionary<int, Dictionary<int, List<int>>>();
 
            int tempKey;
            Dictionary<int, List<int>> temp;
            List<int> tempList;
            //Add bias as 1 at the end of each vector
            foreach (var v in inputVector)
            {
                tempKey = v.Key;
                temp = v.Value;
                foreach (var t in temp)
                {                    
                    tempList = t.Value;
                    tempList.Add(1); 
                }
                featureVector.Add(tempKey, temp);                 
            }

            //If the pixel is '#' or '+', insert 1 in the feature vector
            //Modifying the input vector and returning it directly
                       
            return featureVector;

        }

        public Dictionary<int,Dictionary<int,List<int>>> getVectorFromLabelMatrix(List<Label> matrix)
        {
            Dictionary<int, Dictionary<int, List<int>>> vector = new Dictionary<int, Dictionary<int, List<int>>>();
            Dictionary<int, List<int>> subvector;
            List<int> tempList;
            foreach (Label l in matrix)
            {
                int key = l.LabelId;
                int subkey = l.LabelValue;
                subvector = new Dictionary<int, List<int>>();
                tempList = new List<int>();

                //int i = 0;
                for (int row = 0; row < inputRowSize; row++)
                {
                    for (int col = 0; col < inputColSize; col++)
                    {
                        tempList.Add(l.Pixels[row][col]);
                        //i++;
                    }
                }

                subvector.Add(subkey, tempList);
                vector.Add(key, subvector);
            }

            return vector;
        }

        public int getDotProduct(List<int> weightVector, List<int> featureVector)
        {
            int result = 0;
            int size = weightVector.Count;
            if (featureVector.Count != size)
                return -99999;

            for (int i = 0; i < size; i++)
            {
                result += weightVector[i] * featureVector[i];                
            }
            return result;
        }

        List<int> getVectorDifference(List<int> a, List<int> b)
        {
            int size = a.Count;
            if (size != b.Count) 
                return null;
            else
            {
                for (int i=0; i< size; i++)
                {
                    a[i] = a[i] - b[i];
                }
            }

            return a;
        }

        List<int> getVectorSum(List<int> a, List<int> b)
        {
            int size = a.Count;
            if (size != b.Count)
                return null;
            else
            {
                for (int i = 0; i < size; i++)
                {
                    a[i] = a[i] + b[i];
                }
            }

            return a;
        }

        public void runTraining()
        {
            //Compute dot product of weight vector and feature vector of the input
            //for all labels
            Dictionary<int, Dictionary<int, List<int>>> inputFeatureVector;
            Dictionary<int,List<int>> tempDict;
            List<int> weightVector;
            List<int> sampleFeatureVector;
            List<int> dotProd;
            int correctLabel;
            int predictedLabel;
            //bool isAnyWeightVectorUpdated = false;
            //int numComparisons;
            int numUpdates;

            inputFeatureVector = getFeatureVectorOfInput(inputTrainingMatrix);

            while(true)
            {
              //  numComparisons = 0;
                numUpdates = 0;
                for (int i = 0; i < numberOfTrainingValues; i++)
                {
                    // For each training sample, compute the dot product
                    //with weight vector of each label 
                    tempDict = inputFeatureVector[i];
                    correctLabel = tempDict.Keys.First(); // contains only one key
                    sampleFeatureVector = tempDict.Values.First(); // contains only one value
                    
                    dotProd = new List<int>();
                    for (int j = 0; j < inputLabels; j++)
                    {
                        weightVector = weightVectors[j];
                        dotProd.Add(getDotProduct(weightVector, sampleFeatureVector));
                    }
                                        
                    predictedLabel = dotProd.IndexOf(dotProd.Max());

                    //Do nothing if accurate classification happens
                    //if (predictedLabel == correctLabel)
                    //{

                    //}
                    //Update the weights of correct and predicted labels otherwise
                    if (predictedLabel != correctLabel)
                    {
                        //isAnyWeightVectorUpdated = true;
                        numUpdates++;
                        weightVectors[predictedLabel] = getVectorDifference(weightVectors[predictedLabel], sampleFeatureVector);
                        weightVectors[correctLabel] =  getVectorSum(weightVectors[correctLabel], sampleFeatureVector);
                    }

                }//end of for loop over all the samples
                if (numUpdates == 0) break;
            }
            isTrainingComplete = true;

        }

        public void runTesting()
        {
            //Compute dot product of weight vector and feature vector of the input
            //for all labels
            Dictionary<int, Dictionary<int, List<int>>> inputFeatureVector;
            Dictionary<int, List<int>> tempDict;
            List<int> weightVector;
            List<int> sampleFeatureVector;
            List<int> dotProd;
            int correctLabel;
            int predictedLabel;
            int numCorrectResults = 0;
            double accuracyPercent = 0.0;
            inputFeatureVector = getFeatureVectorOfInput(inputTestingMatrix);

            for (int i = 0; i < numberofTestValues; i++)
            {
                // For each training sample, compute the dot product
                //with weight vector of each label 
                tempDict = inputFeatureVector[i];
                correctLabel = tempDict.Keys.First(); // contains only one key
                sampleFeatureVector = tempDict.Values.First(); // contains only one value

                dotProd = new List<int>();
                for (int j = 0; j < inputLabels; j++)
                {
                    weightVector = weightVectors[j];
                    dotProd.Add(getDotProduct(weightVector, sampleFeatureVector));
                }

                predictedLabel = dotProd.IndexOf(dotProd.Max());
                                
                if (predictedLabel == correctLabel)
                {
                    numCorrectResults++;
                }
                
            }//end of for loop over all the samples

            accuracyPercent = ((double)numCorrectResults) / ((double)numberofTestValues);
            accuracyPercent *= 100;

            System.Windows.Forms.MessageBox.Show("Accuracy percent " + accuracyPercent.ToString());
        }
    }
}
