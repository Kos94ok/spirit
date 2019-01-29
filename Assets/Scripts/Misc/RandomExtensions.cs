using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Misc
{
    /// <summary>
    /// Some extension methods for <see cref="System.Random"/> for creating a few more kinds of random stuff.
    /// </summary>
    public static class RandomExtensions
    {
        /// <summary>
        ///   Generates normally distributed numbers. Each operation makes two Gaussians for the price of one, and apparently they can be cached or something for better performance, but who cares.
        /// </summary>
        /// <param name="r"></param>
        /// <param name = "mu">Mean of the distribution</param>
        /// <param name = "sigma">Standard deviation</param>
        /// <returns></returns>
        public static float Gaussian(this Random r, float mu = 0, float sigma = 1)
        {
            var u1 = Random.Range(0, 1);
            var u2 = Random.Range(0, 1);

            var randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) *
                                Mathf.Sin(2.0f * Mathf.PI * u2);

            var randNormal = mu + sigma * randStdNormal;

            return randNormal;
        }
    }
}