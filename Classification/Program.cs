using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Classification
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool isFace = false;
            int numberofTestorValidationValues = 1000;// 301;// 1000;//301 for validation in face and 100 in test for face
            int numberOfTrainingValues = 5000;// 451;// 5000; //451 for face
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            //Testing the classes here
           
            
            Input currentInput = new Input(isFace, numberOfTrainingValues, numberofTestorValidationValues);
            currentInput.Initialize();

            NaiveBayes nb = new NaiveBayes(3, false, numberOfTrainingValues, numberofTestorValidationValues);
            nb.TrainAndTest();

            //////Perceptron Testing Beginning

            Perceptron myPerceptron = new Perceptron(isFace,numberOfTrainingValues,numberofTestorValidationValues);
            myPerceptron.runTraining();
            myPerceptron.runTesting();

            ////Perceptron Ending

            //KNN Testing Beginning

            ////So far maximum is at k=3 and it is 92
            KNN KClassifier = new KNN(3,isFace);
            
            Dictionary<int, int> output = KClassifier.Run(currentInput.trainingDataPath, currentInput.validationDataPath, numberOfTrainingValues, numberofTestorValidationValues,true);

            List<int> validationLabels = currentInput.LoadValidationLabelValues(numberofTestorValidationValues);

            int correctCount = 0;
            for (int i = 0; i < numberofTestorValidationValues; i++)
            {
                //List<int> outputtemp = output[i];
                string outputString = output[i].ToString();
                //Console.WriteLine("Actual : {0} and the predicted value is {1} :  ", validationLabels[i], output[i]);
                //outputtemp.ForEach(Console.WriteLine);

                if (validationLabels[i] == output[i])
                    correctCount += 1;
            }

            //Console.WriteLine("Press any key to exit");
            //Console.ReadKey();
            double percent = (((double)correctCount / (double)numberofTestorValidationValues) * 100);
            //Console.WriteLine("The accuracy is : ", percent);
            MessageBox.Show("The percentage value is " + percent.ToString());

            //KNN Ending

        }
    }
}
