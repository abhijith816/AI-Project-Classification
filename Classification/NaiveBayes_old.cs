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
        int[] resultCount;
        double[] answerMatrix;
        private PixelProb[][] probMatrix;

        /*
        resultsNum -> No. of possible results.(10 for numbers(1...10) and 2 for faces(T/F))
        gridSize - > Size of the image grid. (28 for numbers)
        resultCount - > resultCount[i] has number of instances in training data whose solution is 'i'
        answerMatrix - > Used for calculating answer during actual data.(Needs to be reinitialized after every answer)
        */
        public NaiveBayes(int gridHeight, int gridWidth, int resultsNum, int laplaceConstant)
        {
		    this.gridHeight = gridHeight;
            this.resultsNum = resultsNum;
            this.laplaceConstant = laplaceConstant;
            this.probMatrix = new PixelProb[gridHeight][gridWidth];
            resultCount = new int[resultsNum];
            answerMatrix = new double[resultsNum];
            initialize(resultsNum, laplaceConstant);	
		}

        public NaiveBayes()
        {

        }

        private void initialize(int size, int laplaceConstant)
        {
            for (int i = 0; i < gridHeight; i++)
            {
                for (int j = 0; j < gridHeight; j++)
                {
                    probMatrix[i][j] = new ProbMatrix(size);
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
            foreach (Label label in labels)
            {
                answer = label.LabelValue;
                resultCount[answer]++;
                pixels = label.Pixels;
                for (int i = 0; i < gridHeight; i++)
                {
                    for (int j = 0; j < gridHeight; j++)
                    {
                        if (pixels[i][j] != 0)
                        {
                            insideArr = probMatrix[i][j].prob;
                            insideArr[answer]++;
                        }
                    }
                }
            }
            generateProbabilities();
        }

        private void generateProbabilities()
        {
            double[] insideArr;
            for (int i = 0; i < gridHeight; i++)
            {
                for (int j = 0; j < gridHeight; j++)
                {
                    insideArr = probMatrix[i][j].prob;
                    for (int k = 0; k < resultsNum; k++)
                    {
                        insideArr[k] = (insideArr[k] + laplaceConstant) / (resultCount[k] + (laplaceConstant * resultsNum));
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
            for (int i = 0; i < gridHeight; i++)
            {
                for (int j = 0; j < gridHeight; j++)
                {
                    insideArr = probMatrix[i][j].prob;
                    for (int k = 0; k < resultsNum; k++)
                    {
                        answerMatrix[k] = answerMatrix[k] * insideArr[k];
                    }
                }
            }
            return getMaxPositionFromAnswerMatrix();
        }

        private int getMaxPositionFromAnswerMatrix()
        {
            double max = 0.0;
            int maxPos = 0;
            for (int i = 0; i < resultsNum; i++)
            {
                if (answerMatrix[i] > max)
                {
                    max = answerMatrix[i];
                    maxPos = i;
                }
            }
            return maxPos;
        }

        private void initializeAnswerMatrix()
        {
            for (int i = 0; i < resultsNum; i++)
            {
                answerMatrix[i] = 1.0;
            }
        }
    }
}