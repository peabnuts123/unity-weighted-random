using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Generate random numbers and graph them
/// </summary>
public class RandomTest : MonoBehaviour
{
    enum RandomType
    {
        Polynomial,
        Guassian,
    }

    // Public references
    /// <summary>
    /// Reference to Graph component
    /// </summary>
    [SerializeField]
    [NotNull]
    private GraphTest graph;

    [SerializeField]
    [Range(2, 100)]
    private int numGraphBuckets = 20;

    [SerializeField]
    private RandomType randomType;

    // Private state
    private float[] weights;
    private RandomType? lastFrameRandomType;
    private int? lastFrameNumGraphBuckets;

    void Start()
    {
        ResetGraph();
    }

    private void ResetGraph()
    {
        this.weights = new float[this.numGraphBuckets];
    }

    void Update()
    {
        // Reset graph if randomType changes
        if (this.lastFrameRandomType.HasValue && this.lastFrameRandomType.Value != this.randomType)
        {
            ResetGraph();
        }
        this.lastFrameRandomType = this.randomType;

        // Reset graph if numGraphBuckets changes
        if (this.lastFrameNumGraphBuckets.HasValue && this.lastFrameNumGraphBuckets != this.numGraphBuckets)
        {
            ResetGraph();
        }
        this.lastFrameNumGraphBuckets = this.numGraphBuckets;

        // Weighted random sample between min,max - based on selected type of random
        int sample;
        switch (this.randomType)
        {
            case RandomType.Guassian:
                sample = (int)Mathf.Floor(RandomUtil.Guassian(0, this.numGraphBuckets));
                break;
            case RandomType.Polynomial:
                sample = (int)Mathf.Floor(RandomUtil.Polynomial(0, this.numGraphBuckets));
                break;
            default:
                throw new NotImplementedException(@"Unimplemented random type: {this.randomType}");
        }

        // Increase count at sample index
        this.weights[sample]++;
        // Update graph with weights
        this.graph.SetWeights(this.weights);
    }
}