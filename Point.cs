using System;
namespace SpeedTest
{
    public class Point
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