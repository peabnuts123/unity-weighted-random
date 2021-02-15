using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A visualisation of a series of values, drawn to the screen
/// as a histogram
/// </summary>
public class GraphTest : MonoBehaviour
{
    // Public references
    [SerializeField]
    [NotNull]
    private new Camera camera;
    [SerializeField]
    [NotNull]
    private Material material;

    // Private state
    private float[] weights = new float[0];
    private float currentMax = 1;
    private Vector2 oldScreenSize = new Vector2(Screen.width, Screen.height);
    private Rect currentGraphExtents;
    private List<Transform> barTransforms;

    public void Update()
    {
        // Check for screen resizes / update graph size
        if (this.oldScreenSize.x != Screen.width || this.oldScreenSize.y != Screen.height)
        {
            this.oldScreenSize = new Vector2(Screen.width, Screen.height);

            Debug.Log("Resizing graph as screen size has changed");
            this.SetupGraph();
            this.RedrawGraph();
        }
    }

    /// <summary>
    /// Set the weights of the graph and redraw it
    /// </summary>
    /// <param name="weights">Weights to set</param>
    public void SetWeights(float[] weights)
    {
        float currentNumWeights = this.weights.Length;
        this.weights = weights;
        float maxWeight = this.weights.Max();

        // Rescale this.currentMax to be within acceptable bounds
        while (maxWeight < this.currentMax * 0.45F || maxWeight > this.currentMax * 0.9F)
        {
            if (maxWeight < this.currentMax * 0.45F)
            {
                this.currentMax /= 2F;
            }
            else if (maxWeight > this.currentMax * 0.9F)
            {
                this.currentMax *= 2F;
            }
        }

        if (weights.Length != currentNumWeights)
        {
            // Need to restructure graph
            Debug.Log("Resizing graph as the number of weights has changed");
            this.SetupGraph();
        }

        this.RedrawGraph();
    }

    /// <summary>
    /// Resize all the bars with values from the current weights
    /// </summary>
    private void RedrawGraph()
    {
        if (this.currentGraphExtents == null)
        {
            throw new InvalidOperationException("I dunno what's happening");
        }

        if (this.weights.Length > 0)
        {
            int i = 0;
            foreach (Transform childTransform in this.barTransforms)
            {
                float weight = this.weights[i];
                float barY = this.currentGraphExtents.yMin;
                float barHeight = this.currentGraphExtents.height * (weight / this.currentMax);

                childTransform.localScale = new Vector3(childTransform.localScale.x, barHeight, 1);
                childTransform.position = new Vector2(childTransform.position.x, barY + (barHeight / 2F));

                i++;
            }
        }
    }

    /// <summary>
    /// Set up the graph to be drawn. Ensures there are the correct amount of
    /// buckets being drawn, creates the necessary GameObjects, etc.
    /// </summary>
    private void SetupGraph()
    {
        int numBoxes = this.weights.Length;
        Debug.Log($"Setting up {numBoxes} boxes");

        // Destroy any existing children (2 steps)
        // Step 1. Put children into an array
        IList<Transform> childrenToDelete = new List<Transform>(this.transform.childCount);
        foreach (Transform child in this.transform)
        {
            childrenToDelete.Add(child);
        }
        // Step 2. Delete everything in the array
        foreach (Transform child in childrenToDelete)
        {
            child.parent = null;
            Destroy(child.gameObject);
        }

        // Calculate camera size in world coordinates
        float vExtent = this.camera.orthographicSize;
        float hExtent = vExtent * Screen.width / Screen.height;

        // Calculate graph region in world coordinates
        float marginBottom = 0.15F;
        float marginH = 0.05F;
        float marginTop = 0.1F;
        float marginBetween = 0.00F;

        float bottom = -vExtent * (1 - (marginBottom * 2F));
        float left = -hExtent * (1 - (marginH * 2F));
        float right = hExtent * (1 - (marginH * 2F));
        float top = vExtent * (1 - (marginTop * 2F));
        this.currentGraphExtents = Rect.MinMaxRect(left, bottom, right, top);

        float width = right - left;
        float height = top - bottom;

        // Etc lol
        float marginWidth = marginBetween * hExtent * 2;
        float barWidth = (width - ((numBoxes - 1) * marginWidth)) / numBoxes;
        // @NOTE bars default to 0
        float barHeight = 0;
        this.barTransforms = new List<Transform>();
        for (int i = 0; i < numBoxes; i++)
        {
            float barX = i * (barWidth + marginWidth) + left;
            float barY = bottom;

            GameObject bar = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bar.transform.parent = this.transform;
            bar.GetComponent<MeshRenderer>().material = material;

            bar.transform.localScale = new Vector3(barWidth, barHeight, 1);
            bar.transform.position = new Vector2(barX + (barWidth / 2F), barY + (barHeight / 2F));
            this.barTransforms.Add(bar.transform);
        }
    }
}
