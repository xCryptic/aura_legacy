using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aura.World.World;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Aura.World.Util
{
    // Not much more than an idea atm
    public class Spawner
    {
        /// <summary>
        /// Add a spawner. All spawners should be added at server 
        /// startup, no need for locking.
        /// </summary>
        public static void Add(uint regionId, Spawner spawner)
        {

        }

        /// <summary>
        /// Get a spawner.
        /// </summary>
        public static Spawner Get(uint regionId)
        {
            return null;
        }

        /// <summary>
        /// Spawn 4 corner orbs in a specified place (usually a room).
        /// </summary>
        /// <param name="place">Place to spawn the orbs</param>
        public void CornerOrbs(string place)
        {

        }

        public void StarOrbs(string place)
        {

        }


        public void StarOrbs(MabiVertex center, float dir = 0f, float spacing = 10f)
        {
            var max = (Math.PI * 2);
            
            // Easy way to check?
            if (dir >= max)
                return;

            var origin = new DenseVector(new[] { (double)center.X, (double)center.Y });

            // vector.Norm(2d); // Euclidian norm
            double dx, dy;
            for (int i = 0; i < 5; i++)
            {
                dx = 0d; dy = 0d;

                if (dir >= max) dir -= (float)max;

                // 4 cases where triangle cannot be made
                if (dir == 0f) dy = spacing;
                else if (dir == (Math.PI / 2d)) dx = spacing;
                else if (dir == (Math.PI)) dy = (spacing * -1d);
                else if (dir == (Math.PI * 1.5d)) dx = (spacing * -1d);
                // 4 triangle cases
                else if (dir > (Math.PI * 1.5d)) // Quad 2, -x +y
                {
                    var a = dir - (Math.PI * 1.5d);
                    //(Math.PI + dir)
                }
                else if (dir > (Math.PI)) // Quad 3, -x -y
                {

                }
                else if(dir > (Math.PI / 2)) // Quad 4, +x -y
                {

                }
                else // Quad 1, +x +y
                {

                }

                dir += (float)(Math.PI * 0.4d);
            }
        }
    }
}