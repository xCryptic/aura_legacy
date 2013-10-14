using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aura.Shared.Util;

namespace Aura.World.Util
{
    public static class Positions
    {
        public static List<Tuple<ulong, ulong>> Grid(ulong sx, ulong sy, ulong width, ulong height, ulong dx, ulong dy)
        {
            List<Tuple<ulong, ulong>> list = new List<Tuple<ulong, ulong>>();
            for (ulong i = 0; i < width; i++)
                for (ulong j = 0; j < height; j++)
                    list.Add(new Tuple<ulong, ulong>(sx + (dx * i), sy + (dy * j)));
            return list;
        }

        public static List<Tuple<double, double>> SquareFromCenter(double cx, double cy, double radius, double dir)
        {
            return null;
        }

        public static List<Tuple<double, double>> Circle(double cx, double cy, double radius, double dir, uint count)
        {
            List<Tuple<double, double>> list = new List<Tuple<double, double>>();
            double diff = ((Math.PI * 2d) / (double)count); // radians

            for (uint i = 0; i < count; i++, dir += diff)
            {
                if (dir >= (Math.PI * 2d)) dir -= (Math.PI * 2d);
                list.Add(Positions.GetSegmentEnd(cx, cy, radius, dir));
            }

            return list;
        }

        /// <summary>
        /// Get the second point of a line segment given the first point, a length (radius),
        /// and a direction in radians.
        /// </summary>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="radius"></param>
        /// <param name="dir">Radians</param>
        /// <returns></returns>
        private static Tuple<double, double> GetSegmentEnd(double cx, double cy, double radius, double dir)
        {
            if (dir > (Math.PI * 2d))
                throw new Exception("Invalid direction, greater than 2*pi");

            double deg = RadToDeg(dir);

            double dx = 0d, dy = 0d;

            if (deg == 0d) { dy = radius; }
            else if (deg == 90d) { dx = radius; }
            else if (deg == 180d) { dy = (radius * -1d); }
            else if (deg == 270d) { dx = (radius * -1d); }
            else if (deg < 90d) // 1st
            {
                double rdeg = deg;

                double ax = rdeg;       // opp of x
                double ay = 90d - rdeg; // opp of y

                // Law of sines
                dx = Math.Sin(ax) * radius;
                dy = Math.Sin(ay) * radius;
            }
            else if (deg < 180d) // 4th
            {
                double rdeg = deg - 90d;

                double ax = 90d - rdeg; // opp of x
                double ay = rdeg;       // opp of y

                // Law of sines
                dx = Math.Sin(ax) * radius;
                dy = Math.Sin(ay) * radius;

                dy *= -1d;
            }
            else if (deg < 270d) // 3rd
            {
                double rdeg = deg - 180d;

                double ax = rdeg;       // opp of x
                double ay = 90d - rdeg; // opp of y

                // Law of sines
                dx = Math.Sin(ax) * radius;
                dy = Math.Sin(ay) * radius;

                dx *= -1d; dy *= -1d;
            }
            else if (deg < 360d) // 2nd
            {
                double rdeg = deg - 270d;

                double ax = 90d - rdeg; // opp of x
                double ay = rdeg;       // opp of y

                // Law of sines
                dx = Math.Sin(ax) * radius;
                dy = Math.Sin(ay) * radius;

                dx *= -1d;
            }

            return new Tuple<double, double>(cx + dx, cy + dy);
        }

        private static bool DegIsQuad(double deg, uint quad)
        {
            var a = ((double)(quad - 1) * 90d);
            return (deg > a && deg < (a + 90d));
        }

        private static double RadToDeg(double rad)
        {
            return (rad * (180d / Math.PI));
        }

        private static double DegToRad(double deg)
        {
            return (deg * (Math.PI / 180d));
        }

        //public static List<Tuple<double, double>> Arc(double cx, double cy, double radius, double sdir, double edir, uint count)
        //{
        //
        //}
    }
}
