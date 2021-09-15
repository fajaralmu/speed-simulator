using System;

namespace SpeedTest
{

    public class Program
    {

        public static void Main(string[] args)
        {
            Motor m1 = new Motor();
            // m1.Move(11,5);
            // m1.Move(6);
            m1.Move(200, 100);

            // simulateAccel();
            // simulateDeccel();
        }

        private static void simulateDeccel()
        {
            double distance = 0;
            double velocity = 30;
            double time = 0;
            double sumVEL = 0;
            while (velocity >= 0) {
                velocity = velocity - (0.02 * time);
                distance += velocity/100;
                time += 0.01;
                sumVEL += velocity;
            }
            Console.WriteLine("\nDECCELERATION distance: "+distance);
            Console.WriteLine("sumVEL: "+sumVEL);
        }

        private static void simulateAccel()
        {
            
            double velocity = 0;
            double distance = 0;
            double time = 0;
            double sumVEL = 0;
            while (velocity <= 30.0)
            {
                velocity = velocity + (0.02 * time);
                distance += velocity/100;
                time += 0.01;
                sumVEL+=velocity;
            }
            Console.WriteLine("\nACCELERATION distance: "+distance);
            Console.WriteLine("sumVEL: "+sumVEL);
            // return distance;
        }
    }

   
}