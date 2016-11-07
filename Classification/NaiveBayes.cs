using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Classification
{
	class NaiveBayes
	{
        int resultsNum = 0;
        int laplaceConstant = 1;
        int gridHeight;
        int gridWidth;
        double[] resultCount;
        double[] answerMatrix;
		private PixelProb[][] probMatrix;
        Input currentInput;
        /*
        resultsNum -> No. of possible results.(10 for numbers(1...10) and 2 for faces(T/F))
        gridSize - > Size of the image grid. (28 for numbers)
        resultCount - > resultCount[i] has number of instances in training data whose solution is 'i'
        answerMatrix - > Used for calculating answer during actual data.(Needs to be reinitialized after every answer)
        */
		public NaiveBayes(int gridHeight, int gridWidth, int resultsNum, int laplaceConstant)
        {
		    this.gridHeight = gridHeight;
            this.gridWidth = gridWidth;
            this.resultsNum = resultsNum;
            this.laplaceConstant = laplaceConstant;
            this.probMatrix = new PixelProb[gridHeight][];
            resultCount = new double[resultsNum];
            answerMatrix = new double[resultsNum];
            initialize(resultsNum, laplaceConstant);	
		}
        
        public NaiveBayes(int laplaceConstant, bool isFace, int numberOfTrainingSamples, int numberOfTestSamples)
        {
            currentInput = new Input(isFace,numberOfTrainingSamples,numberOfTestSamples);
            this.laplaceConstant = laplaceConstant;
            this.gridHeight = currentInput.LabelHeight;
            this.gridWidth = currentInput.LabelWidth;
            if (isFace)
                resultsNum = 2;
            else
                resultsNum = 10;
            this.probMatrix = new PixelProb[gridHeight][];
            resultCount = new double[resultsNum];
            answerMatrix = new double[resultsNum];
            initialize(resultsNum, laplaceConstant);	
        }

        public NaiveBayes() 
        {
                        
        }
                
        private void initialize(int size, int laplaceConstant)
        {
            for(int i = 0; i <  gridHeight; i++) 
            {
                probMatrix[i] = new PixelProb[gridWidth];
                for(int j = 0; j < gridHeight; j++)
                {
                        probMatrix[i][j] = new PixelProb(size);
                }
            }
            initializeAnswerMatrix();
        }
                
                
        /* 
        Populate entire probMatrix with count first. 
        Generate all probabilities using the above counts using laplace smoothing.
        */
        public void train_populate(List<Label> labels)
        {
            int answer = 0;
            List<List<int>> pixels = null;
            double[] insideArr;
            double[] pixelonInsideArr;
            foreach(Label label in labels)
            {
                answer = label.LabelValue;
                resultCount[answer]++;
                pixels = label.Pixels;
                for(int i = 0; i < gridHeight; i++)
                {
                    for(int j = 0; j < gridHeight; j++)
                    {
                        if(pixels[i][j] != 0)
                        {
                            insideArr = probMatrix[i][j].prob;
                            insideArr[answer]++;

                            //probMatrix[i][j].prob = insideArr;
                        }
                    }
                }
            }
          //  generateProbabilities();
        }

        public void TrainAndTest()
        {
            int numCorrectResults = 0;
            double accuracyPercent;
            train_populate(currentInput.LoadDataFromFile(currentInput.trainingDataPath, currentInput.numberOfTrainingSamples, true, currentInput.trainingLabelPath));
            List<Label> testLabels = new List<Label>();
            testLabels = currentInput.LoadDataFromFile(currentInput.validationDataPath, currentInput.numberOfTestorValidationSamples, false, currentInput.validationLabelPath);
            foreach (Label l in testLabels)
            {
                int output = getNumberFromImage(l);

                if (output == l.LabelValue)
                {
                    numCorrectResults++;
                }
            }

            
                
            //end of for loop over all the samples

            accuracyPercent = ((double)numCorrectResults) / ((double)currentInput.numberOfTestorValidationSamples);
            accuracyPercent *= 100;

            System.Windows.Forms.MessageBox.Show("Accuracy percent " + accuracyPercent.ToString());

        }
                
        private void generateProbabilities()
        {
            double[] insideArr;
            for(int i = 0; i < gridHeight; i++) 
            {
                for(int j = 0; j < gridHeight; j++)
                {
                    insideArr = probMatrix[i][j].prob;
                    for(int k = 0; k < resultsNum; k++)
                    {
                        insideArr[k] = (insideArr[k] + laplaceConstant)/(resultCount[k] + (laplaceConstant*resultsNum));
                    }
                }
            }
        }
                
        /*
        Populate answerMatrix using the probabilities calculated during training.
        Next, return maxPos from answerMatrix
        */
        public int getNumberFromImage(Label image)
        {
            initializeAnswerMatrix();
            double[] insideArr;
            for(int i = 0;i < gridHeight; i++)
            {
                for(int j = 0; j < gridHeight;j++)
                {
                    if (image.Pixels[i][j] != 0) {
                        insideArr = probMatrix[i][j].prob;
                        for (int k = 0; k < resultsNum; k++)
                        {
                            answerMatrix[k] +=Math.Log10((insideArr[k]) / (resultCount[k] + 2 * laplaceConstant)); 
                        }
                    }
                    else
                    {
                        insideArr = probMatrix[i][j].prob;
                        for (int k = 0; k < resultsNum; k++)
                        {
                            answerMatrix[k] += Math.Log10((resultCount[k] - insideArr[k]) / (resultCount[k] + 2 * laplaceConstant));
                        }
                    }
                    
                }
            }

            for (int k = 0; k < resultsNum; k++ )
            {
                answerMatrix[k] += Math.Log10(resultCount[k] / currentInput.numberOfTrainingSamples);
            }

            return getMaxPositionFromAnswerMatrix();
        }
                
        private int getMaxPositionFromAnswerMatrix()
        {
            double max = answerMatrix[0] ;
            int maxPos = 0;
            for(int i = 0; i < resultsNum; i++)
            {
                if(answerMatrix[i] > max)
                {
                        max = answerMatrix[i];
                        maxPos = i;
                }
            }
            return maxPos;
        }
                
        private void initializeAnswerMatrix()
        {
            for(int i = 0; i < resultsNum; i++)
            {
                    answerMatrix[i] = 0.0;
            }
        }

        
    }
}