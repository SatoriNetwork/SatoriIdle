using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Hardware : MonoBehaviour
{
    public List<Neuron> NeuronList = new List<Neuron>();
    public List<Neuron> PlacedNeuronList = new List<Neuron>();
    public RectTransform rect;
    public GameObject connector;
    public float neighborhoodThreshhold = 10000;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        PlaceNeuronsInHardware();
    }
    public void PlaceNeuronsInHardware()
    {
        foreach (Neuron neuron in NeuronList)
        {
            RectTransform neuronRect = neuron.GetComponent<RectTransform>();
            bool validPlace = false;
            int attemptCount = 0;
            int maxAttempts = 1000; 

            while (!validPlace && attemptCount < maxAttempts)
            {
                attemptCount++;
                // Generate random position within parent bounds
                float yOffset = Random.Range(0, (rect.rect.height / 2 - neuronRect.rect.height));
                yOffset *= Random.Range(0, 2) == 0 ? 1 : -1;
                int y = (int)yOffset;

                float xOffset = Random.Range(0, (rect.rect.width / 2 - neuronRect.rect.width));
                xOffset *= Random.Range(0, 2) == 0 ? 1 : -1;
                int x = (int)xOffset;

                validPlace = true;

                // Check against all placed neurons
                foreach (Neuron placedNeuron in PlacedNeuronList)
                {
                    float placedX = placedNeuron.transform.localPosition.x;
                    float placedY = placedNeuron.transform.localPosition.y;

                    // Calculate distance between current and placed neuron
                    float dx = Mathf.Abs(placedX - x);
                    float dy = Mathf.Abs(placedY - y);

                    // Check overlap on both axes using full dimensions
                    if (dx < neuronRect.rect.width && dy < neuronRect.rect.height)
                    {
                        validPlace = false;
                        break;
                    }
                }

                if (validPlace || PlacedNeuronList.Count == 0)
                {
                    neuron.transform.localPosition = new Vector3(x, y, 0);
                    PlacedNeuronList.Add(neuron);
                    validPlace = true;
                    break;
                }
            }

            if (attemptCount >= maxAttempts)
            {
                Debug.LogWarning("Failed to place neuron after " + maxAttempts + " attempts");
            }
        }
        ConnectNeuronsUsingRNG();
    }

    public static float DistanceBetweenNeurons(Vector2 neuron1Center, Vector2 neuron1Size, Vector2 neuron2Center, Vector2 neuron2Size)
    {
        // Calculate the edges of neuron1
        float neuron1Left = neuron1Center.x - neuron1Size.x / 2;
        float neuron1Right = neuron1Center.x + neuron1Size.x / 2;
        float neuron1Top = neuron1Center.y + neuron1Size.y / 2;
        float neuron1Bottom = neuron1Center.y - neuron1Size.y / 2;

        // Calculate the edges of neuron2
        float neuron2Left = neuron2Center.x - neuron2Size.x / 2;
        float neuron2Right = neuron2Center.x + neuron2Size.x / 2;
        float neuron2Top = neuron2Center.y + neuron2Size.y / 2;
        float neuron2Bottom = neuron2Center.y - neuron2Size.y / 2;

        // Calculate the horizontal distance considering the left and right edges
        float horizontalDistance = 0f;
        if (neuron1Right < neuron2Left)
        {
            horizontalDistance = neuron2Left - neuron1Right;
        }
        else if (neuron2Right < neuron1Left)
        {
            horizontalDistance = neuron1Left - neuron2Right;
        }

        // Calculate the vertical distance considering the top and bottom edges
        float verticalDistance = 0f;
        if (neuron1Top < neuron2Bottom)
        {
            verticalDistance = neuron2Bottom - neuron1Top;
        }
        else if (neuron2Top < neuron1Bottom)
        {
            verticalDistance = neuron1Bottom - neuron2Top;
        }

        // If horizontal and vertical distances are both zero, the neurons overlap
        if (horizontalDistance == 0f && verticalDistance == 0f)
        {
            return 0f;
        }

        // The total distance is the Euclidean distance between the closest edges
        return Mathf.Sqrt(horizontalDistance * horizontalDistance + verticalDistance * verticalDistance);
    }

    bool IsRNGEdge(Neuron neuronA, Neuron neuronB)
    {
        RectTransform RectA = neuronA.GetComponent<RectTransform>();
        RectTransform RectB = neuronB.GetComponent<RectTransform>();
        // Check if any other neuron is in the neighborhood of the line segment connecting neuronA and neuronB
        for (int k = 0; k < PlacedNeuronList.Count; k++)
        {
            if (PlacedNeuronList[k] == neuronA || PlacedNeuronList[k] == neuronB) continue; // Skip the neurons being checked

            Neuron otherNeuron = PlacedNeuronList[k];
            RectTransform RectO = otherNeuron.GetComponent<RectTransform>();
            if (DistanceBetweenNeurons(neuronA.transform.position, new Vector2(RectA.rect.width, RectA.rect.height), otherNeuron.transform.position, new Vector2(RectO.rect.width, RectO.rect.height)) < Mathf.Max(DistanceBetweenNeurons(neuronA.transform.position, new Vector2(RectA.rect.width, RectA.rect.height), neuronB.transform.position, new Vector2(RectB.rect.width, RectB.rect.height)), DistanceBetweenNeurons(otherNeuron.transform.position, new Vector2(RectO.rect.width, RectO.rect.height), neuronB.transform.position, new Vector2(RectB.rect.width, RectB.rect.height)))
                && (DistanceBetweenNeurons(neuronB.transform.position, new Vector2(RectB.rect.width, RectB.rect.height), otherNeuron.transform.position, new Vector2(RectO.rect.width, RectO.rect.height)) < Mathf.Max(DistanceBetweenNeurons(neuronA.transform.position, new Vector2(RectA.rect.width, RectA.rect.height), neuronB.transform.position, new Vector2(RectB.rect.width, RectB.rect.height)), DistanceBetweenNeurons(otherNeuron.transform.position, new Vector2(RectO.rect.width, RectO.rect.height), neuronA.transform.position, new Vector2(RectA.rect.width, RectA.rect.height)))))
            {
                return false;
            }
        }

        return true; // No neuron is in the neighborhood, valid edge
    }

    void ConnectNeuronsUsingRNG()
    {
        for (int i = 0; i < PlacedNeuronList.Count; i++)
        {
            for (int j = i + 1; j < PlacedNeuronList.Count; j++)
            {
                Neuron neuronA = PlacedNeuronList[i];
                Neuron neuronB = PlacedNeuronList[j];
                RectTransform RectA = neuronA.GetComponent<RectTransform>();
                RectTransform RectB = neuronB.GetComponent<RectTransform>();
                if (DistanceBetweenNeurons(neuronA.transform.position, new Vector2(RectA.rect.width, RectA.rect.height), neuronB.transform.position, new Vector2(RectB.rect.width, RectB.rect.height)) <= neighborhoodThreshhold)
                {
                    // Check if any neuron is in the neighborhood of the line segment connecting them
                    if (IsRNGEdge(neuronA, neuronB))
                    {
                        // Connect 
                        ConnectNeurons(neuronA, neuronB);
                    }
                }

            }
        }
    }
    void ConnectNeurons(Neuron neuronA, Neuron neuronB)
    {
        List<Vector2> Apoints = GetPerimeterPointsExCorner(neuronA);
        List<Vector2> Bpoints = GetPerimeterPointsExCorner(neuronB);
        float Dist = float.MaxValue;
        Vector2 bestAPoint = Vector2.zero;
        Vector2 bestBPoint = Vector2.zero;
        foreach (Vector2 point in Apoints)
        {
            foreach (Vector2 point2 in Bpoints)
            {
                if (Vector2.Distance(point, point2) < Dist)
                {
                    Dist = Vector2.Distance(point, point2);
                    bestAPoint = point;
                    bestBPoint = point2;
                }
            }
        }
        Vector2 midpoint = Vector2.Lerp(bestAPoint, bestBPoint, 0.5f);
        Vector2 direction = bestBPoint - bestAPoint;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        GameObject go = Instantiate(connector, new Vector3(midpoint.x, midpoint.y, 0), Quaternion.Euler(0, 0, angle),transform);
        go.transform.localScale = new Vector3(0.1f*(Dist/2)/5, 0.1f, 0.1f);
    }

    List<Vector2> GetPerimeterPointsExCorner(Neuron neuron, float segmentLength = 1f, float cornerOffset = 5f)
    {
        List<Vector2> points = new List<Vector2>();
        RectTransform rect = neuron.GetComponent<RectTransform>();
        float startpointYB = (neuron.transform.position.y - rect.rect.height / 2) + 1f;
        float startpointYT = (neuron.transform.position.y + rect.rect.height / 2) - 1f;
        float startpointXL = (neuron.transform.position.x - rect.rect.width / 2) + 1f;
        float startpointXR = (neuron.transform.position.x + rect.rect.width / 2) - 1f;

        // Left wall (excluding corners)
        for (float i = cornerOffset; i < rect.rect.height - cornerOffset; i += segmentLength)
        {
            points.Add(new Vector2(startpointXL, startpointYB + i));
        }

        // Right wall (excluding corners)
        for (float i = cornerOffset; i < rect.rect.height - cornerOffset; i += segmentLength)
        {
            points.Add(new Vector2(startpointXR, startpointYB + i));
        }

        // Top wall (excluding corners)
        for (float i = cornerOffset; i < rect.rect.width - cornerOffset; i += segmentLength)
        {
            points.Add(new Vector2(startpointXL + i, startpointYT));
        }

        // Bottom wall (excluding corners)
        for (float i = cornerOffset; i < rect.rect.width - cornerOffset; i += segmentLength)
        {
            points.Add(new Vector2(startpointXL + i, startpointYB));
        }

        return points;
    }
}
