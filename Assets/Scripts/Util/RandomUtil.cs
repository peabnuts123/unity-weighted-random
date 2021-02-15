using System;
using Random = UnityEngine.Random;
using Mathf = UnityEngine.Mathf;

/// <summary>
/// A class for generating various weighted randoms
/// </summary>
public static class RandomUtil
{
    private static float PolynomialFunc(float x) => x * x;

    /// <summary>
    /// Produce a random number between min (inclusive), max (exclusive), weighted according
    /// to an polynomial curve distribution.
    ///
    /// The polynomial is order 2 i.e. x^2
    /// </summary>
    /// <param name="min">Minimum value to return (inclusive)</param>
    /// <param name="max">Maximum value to return (exclusive)</param>
    /// <returns>Number between `min` (inclusive) and `max` (exclusive)</returns>
    public static float Polynomial(float min = 0, float max = 1)
    {
        // Validation
        if (max < min)
        {
            throw new InvalidOperationException("Polynomial region max cannot be smaller than min");
        }
        else if (max == min)
        {
            throw new InvalidOperationException("Polynomial region min and max cannot be equal");
        }

        // Sample between -10 and 10, for fun
        float result = SampleRegion(PolynomialFunc, -10, 10, 0, 100);

        // Rescale result value to be in requested interval
        return Rescale(result, -10, 10, min, max);
    }

    private static float GuassianFunc(float x) => Mathf.Exp((-(x * x)) / 2F) / Mathf.Sqrt(2F * Mathf.PI);
    /// <summary>
    /// Produce a random number between min (inclusive), max (exclusive), weighted according
    /// to a guassian (aka "Normal") distribution.
    ///
    /// Technically speaking, the extent of the distribution is 3 standard deviations.
    /// </summary>
    /// <param name="min">Minimum value to return (inclusive)</param>
    /// <param name="max">Maximum value to return (exclusive)</param>
    /// <returns>Number between `min` (inclusive) and `max` (exclusive)</returns>
    public static float Guassian(float min = 0, float max = 1)
    {
        // Validation
        if (max < min)
        {
            throw new InvalidOperationException("Polynomial region max cannot be smaller than min");
        }
        else if (max == min)
        {
            throw new InvalidOperationException("Polynomial region min and max cannot be equal");
        }

        // Standard normal distribution
        // Sample 3 standard deviations
        // Magic number 0.3989423 = GuassianFunc(0), hard-coded for performance reasons
        float result = SampleRegion(GuassianFunc, -3, 3, 0, 0.3989423F);

        // Rescale result value to be in requested interval
        return Rescale(result, -3, 3, min, max);
    }

    /// <summary>
    /// Randomly choose points within a range (defined by `xMin`, `xMax`, `yMin`, `yMax`) until the point is
    /// underneath the curve defined by function `fn`. This is a simple and expensive way of sampling
    /// any weighted random function.
    /// However, over time, the expected amount of loops in this call will amortise to:
    ///     region_area / fn_area
    ///
    /// Where `region_area` represents the area of the region (i.e. `(xMax-xMin) * (yMax-yMin)`) and
    /// `fn_area` represents the total area underneath the function `fn`. In all real world scenarios
    /// this will likely never be more than, say, 10 or so.
    /// </summary>
    /// <param name="fn">Function defining the weight for random sampling</param>
    /// <param name="xMin">Minimum x value of the region to sample from</param>
    /// <param name="xMax">Maximum x value of the region to sample from</param>
    /// <param name="yMin">Minimum y value of the region to sample from</param>
    /// <param name="yMax">Maximum y value of the region to sample from</param>
    /// <returns>A random number between `xMin` (inclusive) and `xMax` (exclusive), weighted by `fn`</returns>
    private static float SampleRegion(Func<float, float> fn, float xMin, float xMax, float yMin, float yMax)
    {
        int i = 0;
        float x, y = 1, result = 0;
        do
        {
            x = Random.Range(xMin, xMax);

            // Ensure we never return `xMax` as our result
            //  as we promised that `xMax` was exclusive
            if (x == xMax) continue;

            y = Random.Range(yMin, yMax);
            result = fn(x);
            i++;
        } while (y > result);

        return x;
    }

    /// <summary>
    /// Rescale parameter `v` which is in range (minA, maxA) to be in range (minB, maxB)
    /// </summary>
    /// <param name="v">Value to rescale</param>
    /// <param name="minA">Min value of original interval</param>
    /// <param name="maxA">Max value of original interval</param>
    /// <param name="minB">Min value of new interval</param>
    /// <param name="maxB">Max value of new interval</param>
    /// <returns></returns>
    private static float Rescale(float v, float minA, float maxA, float minB, float maxB)
    {
        return Mathf.Lerp(minB, maxB, Mathf.InverseLerp(minA, maxA, v));
    }
}