# Unity - Weighted Random utility

This is a small repo that exists purely to house a utility for generating weighted pseudo-random numbers for use with Unity.

This repo contains a small demo project that visualises the outputs of the Random utility. You can tweak some of the parameters while the visualisation is running to see how it affects the output.

For what it's worth it would be trivial to alter this for use with regular .NET C#, it just uses the native Unity implementation of `Random` and `Mathf` under the hood, which could be replaced with `System.Random` and `System.Math`.

## Including in your project

The class itself is found at `Assets/Scripts/Util/RandomUtil.cs` and is all you need, just dump it into your Assets folder somewhere and reference it from your code.

## Using

The `RandomUtil` class is static and simply exposes a few functions that you can call from anywhere in your code.

### Functions

 - `Polynomial(float min, float max)`
    - Get a random number between `min` (inclusive) and `max` (exclusive) weighted by a 2nd-order polynomial (i.e. `x^2`). This is to say that the most likely values generated will be close to `min` and `max` and the least likely values generated will be the midpoint between `min` and `max` (i.e. `(max + min) / 2`). See [this plot](https://www.wolframalpha.com/input/?i=plot+%28x%5E2%29+where+-10+%3C+x+%3C+10) for a visualisation.
 - `Guassian(float min, float max)`
    - Get a random number between `min` (inclusive) and `max` (exclusive) weighted by a Guassian (aka "Normal") distribution. Technically speaking, the distribution is centered on the midpoint between `min` and `max` (i.e. `(max + min) / 2`) and extends out 3 standard distributions. This is to say that the most likely values generated will be the midpoint between `min` and `max` (i.e. `(max + min) / 2`) and the least likely values generated will be close to `min` and `max` (similar but not identical to the inverse of the `Polynomial` distribution). See [this plot](https://www.wolframalpha.com/input/?i=plot+normal%280%2C1%29) for a visualisation.