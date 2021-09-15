using System;
namespace  SpeedTest
{
     public class Motor
    {
        const double maxVelocity = 30;
        const double maxAcceleration = 30;
        int PositionX = 0, PositionY = 0;

        const double ACCELERATION = 0.015, INCREMENT = 0.01;

        public void Move(double xy) {
            Move(xy, xy);
        }
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

            double distanceMaxVelocity = PredictDistanceMaxVelocity();
            timeMaxVelocity = PredictTimeMaxVelocity();
            bool maxVelocityMoreThanHalfWay = distanceMaxVelocity > halfDistance;
            Console.WriteLine(
                "distanceMaxVelocity Approx: "+dSTR(distanceMaxVelocity)+ 
                ", > halfWay: "+maxVelocityMoreThanHalfWay+
                ",timeMaxVelocity: "+dSTR(timeMaxVelocity));
            // timeMaxVelocity = 0;
            while (true)
            {
                long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                if (now - currentTime < 10)
                    continue;
                currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                double nextTraveledDistance = traveledDistance + (velocity / 100);

                bool nowStoppingMode = !stoppingMode
                                            && CheckIfStoppingMode(distance, nextTraveledDistance,velocity);
                
                if (nowStoppingMode && !stoppingMode)
                {
                    Console.WriteLine(
                        "\nTime to brake at " + dSTR(time) + 
                         "\ndist:"+dSTR(traveledDistance) +
                         "\nremaining:"+dSTR(distance - traveledDistance)+
                         "\nApproximate Stop at :"+dSTR(time + timeMaxVelocity)+" s");
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
                    Console.WriteLine("\nReach max velocity at " + dSTR(time) + "s, distance: "+dSTR(traveledDistance));
                }
                if (traveledDistance >= distance || velocity < 0) {
                    break;
                }

                traveledDistance += (velocity / 100);
                time += INCREMENT;

                Point p = CalculateCurrentPosition(x, y, distance, traveledDistance);
                Console.Write($"\r traveled v = {dSTR(velocity)}mm/s distance = {dSTR(traveledDistance)}, accelerationTime = {dSTR(accelerationTime)}, time {dSTR(time)}");
                // Console.Write($"\r traveled a = {dSTR(acceleration)}mm/s2 v = {dSTR(velocity)}mm/s s = {dSTR(traveledDistance)} mm, position: {p.xStr}, {p.yStr}, time {dSTR(time)}");
            }
            Console.WriteLine($"\nDistance: {dSTR(distance)}, Traveled: {dSTR(traveledDistance)}");
            Console.WriteLine($"END MOVE, time: { dSTR(time) } s, last velocity: {velocity} m/s  \n");
        }

        private double PredictDistanceMaxVelocity()
        {
            double velocity = 0;
            double distance = 0;
            double time = 0;
            while (velocity <= maxVelocity)
            {
                velocity = velocity + (ACCELERATION * time);
                distance += velocity/100;
                time += INCREMENT;
            }
            return distance;
        }

        private double PredictTimeMaxVelocity()
        {
            
            double velocity = 0;
            double time = 0;
            while (velocity <= maxVelocity)
            {
                velocity = velocity + (ACCELERATION * time);
                time += INCREMENT;
            }
            return time;
        }

        private bool CheckIfStoppingMode(double distance, double traveledDistance, double velocity)
        {
            double runningDistance = 0;
            double remainingDistance = runningDistance = distance - traveledDistance;
            double time = 0.0;
            double sumDistance = 0;
            while (velocity >= 0)
            {
                velocity = velocity - (ACCELERATION * time);
                runningDistance -= velocity/100;
                sumDistance+=Math.Abs(velocity/100);
                time+=INCREMENT;
            }
            bool result = runningDistance <= 0;// >= remainingDistance;
            if (result) {
                Console.Write("\n <!> Will brake: "+result+", vel: "+dSTR(velocity)+" SUM DISTANCE:" + dSTR(sumDistance) + "s dist: "+remainingDistance);
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