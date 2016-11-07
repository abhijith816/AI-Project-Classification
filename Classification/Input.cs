using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classification
{
    class Input
    {
        public int LabelHeight = 0;
        public int LabelWidth = 0;
        bool isFace = false;
        public string inputFilePath;

        private string trainigDigitDataPath = @"C:\Users\Abhijith\Google Drive\Fall 2015\AI\Project\digitdata\trainingimages";
        private string trainingDigitLabelPath = @"C:\Users\Abhijith\Google Drive\Fall 2015\AI\Project\digitdata\traininglabels";

        private string testDigitDataPath = @"C:\Users\Abhijith\Google Drive\Fall 2015\AI\Project\digitdata\testimages";
        private string testDigitLabelPath = @"C:\Users\Abhijith\Google Drive\Fall 2015\AI\Project\digitdata\testlabels";

        private string validationDigitDataPath = @"C:\Users\Abhijith\Google Drive\Fall 2015\AI\Project\digitdata\validationimages";
        private string validationDigitLabelPath = @"C:\Users\Abhijith\Google Drive\Fall 2015\AI\Project\digitdata\validationlabels";


        private string trainigFaceDataPath = @"C:\Users\Abhijith\Google Drive\Fall 2015\AI\Project\facedata\facedatatrain";
        private string trainingFaceLabelPath = @"C:\Users\Abhijith\Google Drive\Fall 2015\AI\Project\facedata\facedatatrainlabels";

        private string testFaceDataPath = @"C:\Users\Abhijith\Google Drive\Fall 2015\AI\Project\facedata\facedatatest";
        private string testFaceLabelPath = @"C:\Users\Abhijith\Google Drive\Fall 2015\AI\Project\facedata\facedatatestlabels";

        private string validationFaceDataPath = @"C:\Users\Abhijith\Google Drive\Fall 2015\AI\Project\facedata\facedatavalidation";
        private string validationFaceLabelPath = @"C:\Users\Abhijith\Google Drive\Fall 2015\AI\Project\facedata\facedatavalidationlabels";


        public string trainingDataPath;
        public string trainingLabelPath;
        public string testDataPath;
        public string testLabelPath;
        public string validationDataPath;
        public string validationLabelPath;

        public int numberOfTrainingSamples = 0;
        public int numberOfTestorValidationSamples = 0;
        public Input()
        {

        }

        public Input(int height, int width, string fileName = null)
        {
            LabelHeight = height;
            LabelWidth = width;
            inputFilePath = fileName;
            Initialize();
        }

        public Input(bool isFaceFlag= false, int noTrainingSamples =0, int noTestOrValidationSamples = 0 )
        {
            isFace = isFaceFlag;
            numberOfTestorValidationSamples = noTestOrValidationSamples;
            numberOfTrainingSamples = noTrainingSamples;
            Initialize();
        }

        public void Initialize()
        {
            if(!isFace)
            {
                LabelHeight = 28;
                LabelWidth = 28;

                trainingDataPath = trainigDigitDataPath;
                trainingLabelPath = trainingDigitLabelPath;
                testDataPath = testDigitDataPath;
                testLabelPath = testDigitLabelPath;
                validationDataPath = validationDigitDataPath;
                validationLabelPath = validationDigitLabelPath;
            }
            else
            {
                LabelHeight = 70;
                LabelWidth = 60;

                trainingDataPath = trainigFaceDataPath;
                trainingLabelPath = trainingFaceLabelPath;
                testDataPath = testFaceDataPath;
                testLabelPath = testFaceLabelPath;
                validationDataPath = validationFaceDataPath;
                validationLabelPath = validationFaceLabelPath;
            }

        }

        public List<string> ReadLines(string inputFile = null)
        {
            if (inputFile != null)
                inputFilePath = inputFile;
            string[] textLines = System.IO.File.ReadAllLines(inputFilePath);
            return textLines.ToList();

        }

        public int ConvertToInt(Char ch)
        {
            if (ch == ' ')
                return 0;
            else if (ch == '+')
                return 1;
            else if (ch == '#')
                return 2;
            else return 0;
        }

        public char ConvertToChar(int data)
        {
            if (data == 0)
                return ' ';
            else if (data == 1)
                return '+';
            else if (data == 2)
                return '#';
            else
                return ' ';
        }

        public List<int> ConvertLineToInt(string currentLine)
        {
            List<int> output = new List<int>();
            foreach(char ch in currentLine)            
                output.Add(ConvertToInt(ch));
            
            return output;
        }

        /// <summary>
        /// Read the pixel data of first n labels in the data file. This is common for training,test and validation.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="n"></param>
        public List<Label> LoadDataFromFile(string inputFilePathName, int n = 0, bool isTraining = false, string inputLabelPath = null)
        {
            List<Label> labels = new List<Label>();
            List<List<int>> currentLabelPixels = new List<List<int>>();
            List<string> trainingLabelLines = new List<string>();
            Label currentLabel = new Label();

            //Get all the lines in the file. Probably can be optimised later based on the width and height.
            List<string> allLines = ReadLines(inputFilePathName);

            if (inputLabelPath != null)
                trainingLabelLines = ReadLines(inputLabelPath);

            //List<List<int>> labels = new List<List<int>>();
            int range = n * LabelHeight;
            if (range > allLines.Count() || range == 0)
                range = allLines.Count;
            
            for(int i=0; i < range; i++)
            {
                if(allLines[i] == "")
                    break;

                currentLabelPixels.Add(ConvertLineToInt(allLines[i]));
                if(((i+1)%LabelHeight) == 0)
                {
                    currentLabel.LabelId = i / LabelHeight;
                    currentLabel.Pixels = currentLabelPixels;

                    if (inputLabelPath != null)
                        currentLabel.LabelValue = Int32.Parse(trainingLabelLines[currentLabel.LabelId]);

                    labels.Add(currentLabel);

                    currentLabelPixels = new List<List<int>>();
                    currentLabel = new Label();                    
                }
                
                
            }

            return labels;

        }

        public List<int> LoadTestLabelValues(int n = 0)
        {
            List<int> testLabels = new List<int>();
            List<string> testLabelLines = ReadLines(testLabelPath);

            if (n == 0)
                n = testLabelLines.Count;

            for (int i = 0; i < n; i++)
                testLabels.Add(Int32.Parse(testLabelLines[i]));

            return testLabels;

        }

        public List<int> LoadValidationLabelValues(int n = 0)
        {
            List<int> validationLabels = new List<int>();
            List<string> validationLabelLines = ReadLines(validationLabelPath);

            if (n == 0)
                n = validationLabelLines.Count;

            for (int i = 0; i < n; i++)
                validationLabels.Add(Int32.Parse(validationLabelLines[i]));

            return validationLabels;

        }


    }
}
