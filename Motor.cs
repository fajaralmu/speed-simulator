using System;
namespace  SpeedTest
{
     public class Motor
    {
        const double maxVelocity = 30;
        const double maxAcceleration = 30;
        int PositionX = 0, PositionY = 0;

        const double ACCELERATION = 0.02, INCREMENT = 0.01;

        public void Move(double x, double y)
        {
            double distance = CalculateDistance(x, y);

            Console.WriteLine("\nSTART MOVE\n" + $"{PositionX},{PositionY} => {x},{y}");
            Console.WriteLine("distance: " + dSTR(distance));

            long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            double traveledDistance = 0.0, velocity = 0.00;
            double time = 0, timeMaxVelocity = 0;
            double accelerationTime = 0.0, halfDistance = distance/2;

            bool stoppingMode = false, reachMaxVelocity = false;
            // int 
            while (true)
            {
                long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                if (now - currentTime < 10)
                    continue;

                double nextTraveledDistance = traveledDistance + (velocity / 100);
                currentTime = now;
                time += INCREMENT;

                bool nowStoppingMode = nextTraveledDistance > halfDistance
                                            && !stoppingMode
                                            && reachMaxVelocity
                                            && CheckIfStoppingMode(timeMaxVelocity, distance, nextTraveledDistance);

                if (nowStoppingMode && !stoppingMode)
                {
                    Console.WriteLine("\nTime to brake at " + dSTR(time) + " s\nApproximate Stop at :"+dSTR(time + timeMaxVelocity)+" s");
                    stoppingMode = true;
                    accelerationTime = 0;
                }
                
                //Update Velocity
                if (velocity < maxVelocity || stoppingMode)
                {
                    double acceleration = stoppingMode ? -ACCELERATION : ACCELERATION;
                    velocity = velocity + (acceleration * accelerationTime);
                    accelerationTime += INCREMENT;
                }
                if (!reachMaxVelocity && velocity > maxVelocity)
                {
                    timeMaxVelocity = time;
                    reachMaxVelocity = true;
                    velocity = maxVelocity;
                    Console.WriteLine("\nReach max velocity at " + dSTR(time) + "s");
                }
                if (traveledDistance >= distance || velocity < 0) {
                    break;
                }

                traveledDistance += (velocity / 100);

                Point p = CalculateCurrentPosition(x, y, distance, traveledDistance);
                Console.Write($"\r traveled v = {dSTR(velocity)}mm/s distance = {dSTR(traveledDistance)}, accelerationTime = {dSTR(accelerationTime)}, time {dSTR(time)}");
                // Console.Write($"\r traveled a = {dSTR(acceleration)}mm/s2 v = {dSTR(velocity)}mm/s s = {dSTR(traveledDistance)} mm, position: {p.xStr}, {p.yStr}, time {dSTR(time)}");
            }
            Console.WriteLine($"\nDistance: {dSTR(distance)}, Traveled: {dSTR(traveledDistance)}");
            Console.WriteLine($"END MOVE, time: { dSTR(time) } s, last velocity: {velocity} m/s  \n");
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
                Console.Write("\n <!> Will brake: "+result+"max vel: "+dSTR(velocity)+" time:" + dSTR(timeMaxVelocity) + "s dist: "+remainingDistance);
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

    
}