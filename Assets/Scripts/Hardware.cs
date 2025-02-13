using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hardware : MonoBehaviour
{
    public List<Neuron> NeuronList = new List<Neuron>(); //neurons
    public List<Neuron> PlacedNeuronList = new List<Neuron>();
    public RectTransform rect;
    public List<GameObject> connectorPrefabs = new List<GameObject>();
    public List<GameObject> connections = new List<GameObject>();
    public float neighborhoodThreshhold = 10000;
    public int connectionAmount = 5;

    [SerializeField] GameObject NeuronPrefab;

    [SerializeField] TextMeshProUGUI NeuronCostText, NeuronMaxText, MemoryCostText, MemoryMaxText, RamCostText, RamMaxText, DiskCostText, DiskMaxText, StakeCostText, StakeMaxText;
    public int MaxMemory = 16, MaxRam = 4, MaxDisk = 8, stakedNeurons = 0;
    public int MemorySlots = 1; //max amount of neurons
    public float RAM = 0f; //progress speed
    public float RAMDecreaseAmount = 1f; //progress speed
    public int disk = 1; //offline idle time
    public BGN NeuronCost = new BGN(1); //max amount of neurons
    public BGN MemoryCost = new BGN(1); //max amount of neurons
    public BGN RamCost = new BGN(1); //max amount of neurons
    public BGN DiskCost = new BGN(1); //max amount of neurons
    public BGN StakeCost = new BGN(1); //max amount of neurons

    [SerializeField] Button AddNeuronBtn, UpgradeMemoryBtn, UpgradeRAMBtn, UpgradeDiskBtn, StakeBtn;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        PlaceNeuronsInHardware();
    }

	private void Update() {
		updateShopVisuals();

	}
	public void updateShopVisuals() {
        NeuronCostText.text = NeuronCost.ToString();
        MemoryCostText.text = MemoryCost.ToString();
        RamCostText.text = RamCost.ToString();
        DiskCostText.text = DiskCost.ToString();
        StakeCostText.text = StakeCost.ToString();

        NeuronMaxText.text = NeuronList.Count + "/" + MemorySlots;
        MemoryMaxText.text = MemorySlots + "/" + MaxMemory;
        RamMaxText.text = RAM + "/" + MaxRam;
        DiskMaxText.text = disk + "/" + MaxDisk;
        StakeMaxText.text = stakedNeurons + "/" + NeuronList.Count;

        UpgradeMemoryBtn.interactable = (GameManager.instance.SatoriPoints >= MemoryCost) ? true : false;
        UpgradeRAMBtn.interactable = (RAM < MaxRam && GameManager.instance.SatoriPoints >= RamCost) ? true : false;
        UpgradeDiskBtn.interactable = (disk < MaxDisk && GameManager.instance.SatoriPoints >= DiskCost) ? true : false;
        StakeBtn.interactable = (stakedNeurons < NeuronList.Count && GameManager.instance.SatoriPoints >= StakeCost) ? true : false;
        AddNeuronBtn.interactable = (NeuronList.Count < MemorySlots && GameManager.instance.SatoriPoints >= NeuronCost) ? true : false;


    }

    public BGN CalculateCost(BGN OldCost) {// make this more complicated later
        BGN newCost = new BGN(2);
        newCost *= OldCost;
        return newCost;
    }

    public void UpgradeMemory()
    {
        if (GameManager.instance.SatoriPoints >= MemoryCost) {
            if (MemorySlots < MaxMemory) {
                MemorySlots += 1; 
                GameManager.instance.SatoriPoints -= MemoryCost;
				MemoryCost = CalculateCost(MemoryCost);
			} else {
                Debug.Log("Fully Upgraded");
            }

        }

    }
    public void UpgradeRAM()
    {

        if (RAM < MaxRam && GameManager.instance.SatoriPoints >= RamCost)
        {
            RAM += 1;
            foreach (Neuron neuron in NeuronList)
            {
                neuron.progressTimerMax-= RAMDecreaseAmount;
                if (neuron.working == false)
                {
                    neuron.progressTimer = neuron.progressTimerMax;
                }
            }
            Debug.Log("Upgraded RAM");

			GameManager.instance.SatoriPoints -= RamCost;
			RamCost = CalculateCost(RamCost);
		}
        else
        {
            Debug.Log("Fully Upgraded");
        }
        
        
    }
    public void UpgradeDisk()
    {
        if (disk < MaxDisk && GameManager.instance.SatoriPoints >= DiskCost) {
            disk += 1;
			GameManager.instance.SatoriPoints -= DiskCost;
			DiskCost = CalculateCost(DiskCost);
		}
    }
    public void addNeuron()
    {
        if (NeuronList.Count < MemorySlots && GameManager.instance.SatoriPoints >= NeuronCost)
        {
            Neuron newNeuron = Instantiate(NeuronPrefab, transform).GetComponent<Neuron>();
            newNeuron.progressTimerMax = newNeuron.progressTimerMax - RAM*RAMDecreaseAmount;
            NeuronList.Add(newNeuron);
            PlacedNeuronList.Clear();
            foreach (GameObject connection in connections)
            {
                Destroy(connection.gameObject);

            }
            connections.Clear();
            PlaceNeuronsInHardware();
			GameManager.instance.SatoriPoints -= NeuronCost;
			NeuronCost = CalculateCost(NeuronCost);
		}
        else
        {
            Debug.Log("Not Enough Memory!");
        }
    }
    public void StakeNeuron()
    {
        if (stakedNeurons < NeuronList.Count && GameManager.instance.SatoriPoints >= StakeCost) {

            foreach (Neuron neuron in NeuronList) {
                if (!neuron.stake) {
                    neuron.CreateStake();
                    stakedNeurons++;
                    break;
                }
			}
			GameManager.instance.SatoriPoints -= StakeCost;
			StakeCost = CalculateCost(StakeCost);
		}
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
        List<Vector2> aPoints = GetPerimeterPointsExCorner(neuronA);
        List<Vector2> bPoints = GetPerimeterPointsExCorner(neuronB);

        // Generate all possible pairs with their distances
        List<(Vector2 a, Vector2 b, float dist)> allPairs = new List<(Vector2, Vector2, float)>();

        foreach (Vector2 aPoint in aPoints)
        {
            foreach (Vector2 bPoint in bPoints)
            {
                float distance = Vector2.Distance(aPoint, bPoint);
                allPairs.Add((aPoint, bPoint, distance));
            }
        }

        // Sort pairs by distance
        allPairs.Sort((x, y) => x.dist.CompareTo(y.dist));

        HashSet<Vector2> usedAPoints = new HashSet<Vector2>();
        HashSet<Vector2> usedBPoints = new HashSet<Vector2>();
        int connectionsMade = 0;

        foreach (var pair in allPairs)
        {
            if (connectionsMade >= connectionAmount) break;

            if (!usedAPoints.Contains(pair.a) && !usedBPoints.Contains(pair.b))
            {
                // Create connection
                Vector2 midpoint = Vector2.Lerp(pair.a, pair.b, 0.5f);
                Vector2 direction = pair.b - pair.a;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                GameObject connector = Instantiate(connectorPrefabs[Random.Range(0, connectorPrefabs.Count)],midpoint,Quaternion.Euler(0, 0, angle),transform);
                connections.Add(connector);
                connector.transform.localScale = new Vector3(0.1f * (pair.dist / 2) / 5,0.2f,0.5f);

                usedAPoints.Add(pair.a);
                usedBPoints.Add(pair.b);
                connectionsMade++;
            }
        }
    }

    List<Vector2> GetPerimeterPointsExCorner(Neuron neuron, float segmentLength = 10f, float cornerOffset = 25f)
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
