using System;

namespace Case1
{

    public class Program
    {

        public static void Main(string[] args)
        {
            Motor m1 = new Motor();
            // m1.Move(11,5);
            m1.Move(400, 100);
        }
    }

    class Motor
    {
        const double maxVelocity = 30;
        const double maxAcceleration = 30;
        int PositionX = 0, PositionY = 0;

        const double ACCELERATION = 0.02, INCREMENT = 0.01;

        public void Move(double x, double y)
        {
            Console.WriteLine("\nSTART MOVE");
            Console.Write($"{PositionX},{PositionY}");
            Console.WriteLine($" => {x},{y}");

            double distance = CalculateDistance(x, y);

            Console.WriteLine("distance: " + dSTR(distance));

            double traveledDistance = 0.0, velocity = 0.00;
            double acceleration = ACCELERATION;//30 m/s2

            long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            double time = 0,
            remaningTimeAfterMaxVelocity = 0,
            timeMaxVelocity = 0;

            bool reachDestination = false;
            bool stoppingMode = false;
            bool reachMaxVelocity = false;
            double accelerationTime = 0.0;
            double midDistance = distance/2;
            // int 
            while (!reachDestination)
            {
                long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                if (now - currentTime < 10)
                    continue;

                currentTime = now;
                time += INCREMENT;

                bool nowStoppingMode = traveledDistance > midDistance
                                            && !stoppingMode
                                            && reachMaxVelocity
                                            && CheckIfStoppingMode(timeMaxVelocity, distance, traveledDistance);

                if (nowStoppingMode && !stoppingMode)
                {
                    Console.WriteLine("\nTime to brake at " + dSTR(time) + " s");
                    Console.WriteLine("Remaming distance: "+dSTR(distance - traveledDistance));
                    Console.WriteLine("Stop at :"+dSTR(time + timeMaxVelocity)+" s");
                    stoppingMode = true;
                    acceleration = -acceleration;
                    accelerationTime = 0;
                }
                //Update Velocity
                if (velocity < maxVelocity || stoppingMode)
                {
                    velocity = velocity + (acceleration * accelerationTime);
                    accelerationTime += INCREMENT;
                }

                if (velocity < 0)
                {
                    velocity = 0;
                    Console.WriteLine("\n===== STOPPED ======= :(");
                    break;
                }
                if (!reachMaxVelocity && velocity > maxVelocity)
                {
                    timeMaxVelocity = time;
                    reachMaxVelocity = true;
                    velocity = maxVelocity;
                    remaningTimeAfterMaxVelocity = (distance - traveledDistance) / velocity;

                    Console.WriteLine("\nReach max velocity at " + dSTR(time) + "s");
                }

                traveledDistance += (velocity / 100);

                if (traveledDistance >= distance)
                    reachDestination = true;


                Point p = CalculateCurrentPosition(x, y, distance, traveledDistance);
                Console.Write($"\r traveled a = {dSTR(acceleration)}mm/s2 v = {dSTR(velocity)}mm/s distance = {dSTR(traveledDistance)}, remaining dist={dSTR(distance - traveledDistance)}, accelerationTime = {accelerationTime}, time {dSTR(time)}");
                // Console.Write($"\r traveled a = {dSTR(acceleration)}mm/s2 v = {dSTR(velocity)}mm/s s = {dSTR(traveledDistance)} mm, position: {p.xStr}, {p.yStr}, time {dSTR(time)}");
            }
            Console.WriteLine($"\nDistance: {dSTR(distance)}, Traveled: {dSTR(traveledDistance)}");
            Console.WriteLine($"END MOVE, time: { dSTR(time) } s \n");
        }

        private bool CheckIfStoppingMode(double timeMaxVelocity, double distance, double traveledDistance)
        {
            double runningDistance = 0;
            double remainingDistance = runningDistance = distance - traveledDistance;
            double velocity = maxVelocity;
            for (double accelerationTime = 0.0; accelerationTime <= timeMaxVelocity; accelerationTime += INCREMENT)
            {
                velocity = velocity - (ACCELERATION * accelerationTime);
                runningDistance -= velocity/100;
            }
            bool result = runningDistance <= 0;// >= remainingDistance;
            if (result) {
                // Console.Write("\n <!> Will brake: "+result+"max vel: "+dSTR(velocity)+" time:" + dSTR(timeMaxVelocity) + "s dist: "+remainingDistance);
            }
            // Console.WriteLine("\nCheckIfStoppingMode: "+result+ ",velocity:"+dSTR(velocity)+", remaining: "+dSTR(runningDistance) +" of "+dSTR(remainingDistance));
            return result;
        }

        private Point CalculateCurrentPosition(double x, double y, double distance, double traveled)
        {
            double ratio = distance / traveled;
            return new Point
            {
                x = x / ratio,
                y = y / ratio
            };
        }

        private double CalculateDistance(double x, double y)
        {

            double xDistance = Math.Abs(PositionX - x);
            double yDistance = Math.Abs(PositionY - y);

            return Math.Sqrt(xDistance * xDistance + yDistance * yDistance);
        }

        private string dSTR(double d)
        {
            return String.Format("{0:0.###}", d);
        }

    }

    public struct Point
    {
        public double x; public double y;
        public string xStr
        {
            get
            {
                return String.Format("{0:0.##}", x);
            }
        }
        public string yStr
        {
            get
            {
                return String.Format("{0:0.##}", y);
            }
        }
    }
}