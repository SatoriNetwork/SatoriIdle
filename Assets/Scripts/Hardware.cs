using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Hardware : MonoBehaviour
{
    public List<Neuron> NeuronList = new List<Neuron>(); //neurons
    public List<Neuron> PlacedNeuronList = new List<Neuron>();
    public RectTransform rect;
    public List<GameObject> connectorPrefabs = new List<GameObject>();
    public List<GameObject> connections = new List<GameObject>();
    public GameObject connectorsHolder;
    public float neighborhoodThreshhold = 10000;
    public int connectionAmount = 5;
    public Image[] diskImages;
    public Image[] RAMImages;
    

    [SerializeField] GameObject NeuronPrefab;
    [SerializeField] GameObject NeuronOBJ, MemoryOBJ, RamOBJ, DiskOBJ, StakeOBJ, BackButton;
    [SerializeField] TextMeshProUGUI NeuronCostText, NeuronMaxText, MemoryCostText, MemoryMaxText, RamCostText, RamMaxText, DiskCostText, DiskMaxText, StakeCostText, StakeMaxText;
    public int MaxMemory = 16, MaxRam = 4, MaxDisk = 8, stakedNeurons = 0;
    public int MemorySlots = 1; //max amount of neurons
    public int RAM = 0; //progress speed
    public float RAMDecreaseAmount = 1f; //progress speed
    public int disk = 1; //offline idle time
    public float maxCritChance = 75; //Max Crit Chance allowed 
    public BGN RebirthMultiplier = new BGN(1);
    //public int GPUMultiplier = 1; //GPU Multiplier value
    BGN GPUMultiplier = new BGN(1);
    public BGN NeuronCost = new BGN(1); //max amount of neurons
    public BGN MemoryCost = new BGN(2); //max amount of neurons
    public BGN RamCost = new BGN(8); //max amount of neurons
    public BGN DiskCost = new BGN(1); //max amount of neurons
    public BGN StakeCost = new BGN(10); //max amount of neurons
	public BGN InitNeuronCost = new BGN(1); //max amount of neurons
	public BGN InitMemoryCost = new BGN(2); //max amount of neurons
	public BGN InitRamCost = new BGN(8); //max amount of neurons
	public BGN InitDiskCost = new BGN(5); //max amount of neurons
	public BGN InitStakeCost = new BGN(10); //max amount of neurons

    public BGN upgradeCostMultiplier = new BGN(1);

    public const string NEURON_COUNT = "neuronCount";
    public const string STAKED_NEURON_COUNT = "StakedNeuronCount";
    public const string MEMORY_SLOTS = "MemorySlots";
    public const string MEMORY_COST = "MemoryCost";
    public const string RAM_AMOUNT = "Ram";
    public const string RAM_COST = "RamCost";
    public const string DISK_AMOUNT = "Disk";
    public const string DISK_COST = "DiskCost";
    public const string INIT_NEURON_COST = "InitNeuronCost";
    public const string INIT_MEMORY_COST = "InitMemoryCost";
    public const string INIT_RAM_COST = "InitRamCost";
    public const string INIT_DISK_COST = "InitDiskCost";
    public const string INIT_Stake_COST = "InitStakeCost";
    int position;
	[SerializeField] Button AddNeuronBtn, UpgradeMemoryBtn, UpgradeRAMBtn, UpgradeDiskBtn, StakeBtn;

    bool backgroundRunning = true;
    float progressTimer = 5;
    float progressTimerMax = 5;
    private float updatePerSecTimer = 2.5f;
    [SerializeField] Sprite goldenBtn;
    [SerializeField] Sprite normalBtn;

    public static event EventHandler OnButtonPressed;

	public void disablNeurons() {
        foreach (Neuron n in NeuronList) {
            n.enableVisuals = false;
        }
        backgroundRunning = true;
        progressTimer = progressTimerMax - UnityEngine.Random.Range(0.0f, progressTimerMax);
		GameManager.instance.OnShelf = false;
        GameManager.instance.totalSPPerSec += ((new BGN(2)) * GPUMultiplier * RebirthMultiplier * GameManager.instance.SatoriConnectionMultiplier * stakedNeurons) / new BGN((int)progressTimerMax);

	}

    public void enableNeurons() {
		foreach (Neuron n in NeuronList) {
			n.enableVisuals = true;
		}
        backgroundRunning = false;
        GameManager.instance.OnShelf = true;
        GameManager.instance.totalSPPerSec -= ((new BGN(2)) * GPUMultiplier * RebirthMultiplier * GameManager.instance.SatoriConnectionMultiplier * stakedNeurons)/new BGN((int)progressTimerMax);
	    updateShopVisuals();
    }

	public void Load(int position, double seconds) {
        MemorySlots = PlayerPrefs.GetInt(MEMORY_SLOTS + position, 1);
        MemoryCost = CalculateCost(MemorySlots, InitMemoryCost);

        RAM = PlayerPrefs.GetInt(RAM_AMOUNT + position, 0);
		foreach (Neuron neuron in NeuronList) {
			neuron.progressTimerMax -= RAMDecreaseAmount * RAM;
			if (neuron.working == false || neuron.progressTimer > neuron.progressTimerMax) {
				neuron.progressTimer = neuron.progressTimerMax;
			}
		}
        progressTimerMax = 5 - (RAMDecreaseAmount * RAM);
        progressTimer = progressTimerMax;
        RamCost = CalculateCost(RAM, InitRamCost, 2f);



        disk = PlayerPrefs.GetInt(DISK_AMOUNT + position, 1);
        DiskCost = CalculateCost(disk, InitDiskCost);

        seconds = Math.Min(disk * 60 * 60, seconds);

        double timesNeuronEarned = seconds / (5 - RAM * RAMDecreaseAmount);

		int neuronCount = PlayerPrefs.GetInt(NEURON_COUNT + position, 0);
        for (int i = 0; i < neuronCount; i++) {
			Neuron newNeuron = Instantiate(NeuronPrefab, transform).GetComponent<Neuron>();
			newNeuron.progressTimerMax = newNeuron.progressTimerMax - RAM * RAMDecreaseAmount;
			newNeuron.progressTimer = newNeuron.progressTimerMax;
			newNeuron.GPUMultiplier = GPUMultiplier * RebirthMultiplier;
            newNeuron.enableVisuals = false;
			NeuronList.Add(newNeuron);

			NeuronCost = CalculateCost(NeuronList.Count, InitNeuronCost);
		}
		updateCritChance();
		PlacedNeuronList.Clear();
		foreach (GameObject connection in connections) {
			Destroy(connection.gameObject);

		}
		connections.Clear();
		PlaceNeuronsInHardware();
		stakedNeurons = PlayerPrefs.GetInt(STAKED_NEURON_COUNT + position, 0);
        for (int i = 0; i < stakedNeurons; i++) {
			foreach (Neuron neuron in NeuronList) {
				if (!neuron.stake) {
					neuron.CreateStake();
					//stakedNeurons++;
					break;
				}
			}
			StakeCost = CalculateCost(stakedNeurons, InitStakeCost);
		}
        timesNeuronEarned *= stakedNeurons;
		GameManager.instance.totalSPPerSec += ((new BGN(2)) * GPUMultiplier * RebirthMultiplier * GameManager.instance.SatoriConnectionMultiplier * stakedNeurons) / new BGN((int)progressTimerMax);
        GameManager.instance.offlineEarning += ((new BGN(2)) * GPUMultiplier * RebirthMultiplier * GameManager.instance.SatoriConnectionMultiplier * timesNeuronEarned);
		if (NeuronList.Count != 0) GameManager.instance.addPoints( (new BGN(2)) * GPUMultiplier * RebirthMultiplier * GameManager.instance.SatoriConnectionMultiplier * timesNeuronEarned);
	}

    public void SetGPUMultiplier(BGN multiplier, int position) {
        this.position = position;
        GPUMultiplier = multiplier;
		InitNeuronCost *= upgradeCostMultiplier;
		InitMemoryCost *= upgradeCostMultiplier;
		InitRamCost *= upgradeCostMultiplier;
		InitDiskCost *= upgradeCostMultiplier;
		InitStakeCost *= upgradeCostMultiplier;
        NeuronCost = InitNeuronCost;
        MemoryCost = InitMemoryCost;
        RamCost = InitRamCost; 
        DiskCost = InitDiskCost;
        StakeCost = InitStakeCost;

    }

    private void Start()
    {
        rect = GetComponent<RectTransform>();

        updateDiskImages();
        updateRAMImages();
    }

	private void Update() {
		
        if (backgroundRunning) {
            progressTimer -= Time.deltaTime;
            if (progressTimer <= 0) {

                if (NeuronList.Count != 0) GameManager.instance.addPoints((new BGN(2)) * GPUMultiplier * RebirthMultiplier * GameManager.instance.SatoriConnectionMultiplier * stakedNeurons);
                progressTimer = progressTimerMax;
            }
        } else {

			UpgradeMemoryBtn.interactable = (MaxMemory > MemorySlots && GameManager.instance.SatoriPoints >= MemoryCost) ? true : false;
			UpgradeRAMBtn.interactable = (RAM < MaxRam && GameManager.instance.SatoriPoints >= RamCost) ? true : false;
			UpgradeDiskBtn.interactable = (disk < MaxDisk && GameManager.instance.SatoriPoints >= DiskCost) ? true : false;
			StakeBtn.interactable = (stakedNeurons < NeuronList.Count && GameManager.instance.SatoriPoints >= StakeCost) ? true : false;
			AddNeuronBtn.interactable = (NeuronList.Count < MemorySlots && GameManager.instance.SatoriPoints >= NeuronCost) ? true : false;

        }

    }


	public void updateShopVisuals() {
		if (progressTimerMax == 1f) {
			GameManager.instance.updateSPPerSec(((new BGN(2)) * GPUMultiplier * RebirthMultiplier * GameManager.instance.SatoriConnectionMultiplier * stakedNeurons));
		} else {
			GameManager.instance.updateSPPerSec(((new BGN(2)) * GPUMultiplier * RebirthMultiplier * GameManager.instance.SatoriConnectionMultiplier * stakedNeurons) / new BGN((int)progressTimerMax));
		}

        NeuronMaxText.text = NeuronList.Count + "/" + MemorySlots;
        MemoryMaxText.text = MemorySlots + "/" + MaxMemory;
        RamMaxText.text = RAM + "/" + MaxRam;
        DiskMaxText.text = disk + "/" + MaxDisk;
        StakeMaxText.text = stakedNeurons + "/" + NeuronList.Count;

        updatePerSecTimer = 2.5f;
        if (NeuronList.Count >= MemorySlots && MaxMemory <= MemorySlots)
        {
            NeuronCostText.text = "";
            NeuronMaxText.text = "";
            SpriteState spriteState = AddNeuronBtn.spriteState;
            spriteState.disabledSprite = goldenBtn;
            AddNeuronBtn.spriteState = spriteState; }
        else if (NeuronList.Count < MemorySlots)
        {
            NeuronCostText.text = NeuronCost.ToString();
            SpriteState spriteState = AddNeuronBtn.spriteState;
            spriteState.disabledSprite = normalBtn;
            AddNeuronBtn.spriteState = spriteState;
        } 
        else {
            NeuronCostText.text = "MAXED";
        }

        if (MaxMemory > MemorySlots)
        {
            MemoryCostText.text = MemoryCost.ToString();
        } else {
            MemoryCostText.text = "";
            MemoryMaxText.text = "";    
            SpriteState spriteState = UpgradeMemoryBtn.spriteState;
            spriteState.disabledSprite = goldenBtn;
            UpgradeMemoryBtn.spriteState = spriteState;

        }

        if (RAM < MaxRam)
        {
            RamCostText.text = RamCost.ToString();   
        } else {
            RamCostText.text = "";
            RamMaxText.text = "";
            SpriteState spriteState = UpgradeRAMBtn.spriteState;
            spriteState.disabledSprite = goldenBtn;
            UpgradeRAMBtn.spriteState = spriteState;
        }

        if (disk < MaxDisk)
        {
            DiskCostText.text = DiskCost.ToString();
        } else{
            DiskCostText.text = "";
            DiskMaxText.text = "";
            SpriteState spriteState = UpgradeDiskBtn.spriteState;
            spriteState.disabledSprite = goldenBtn;
            UpgradeDiskBtn.spriteState = spriteState;
        }

        if (stakedNeurons >= NeuronList.Count && NeuronList.Count >= MaxMemory)
        {
            StakeCostText.text = "";
            StakeMaxText.text = "";
            SpriteState spriteState = StakeBtn.spriteState;
            spriteState.disabledSprite = goldenBtn;
            StakeBtn.spriteState = spriteState;
        } else if (stakedNeurons < NeuronList.Count) {
            StakeCostText.text = StakeCost.ToString();
            SpriteState spriteState = StakeBtn.spriteState;
            spriteState.disabledSprite = normalBtn;
            StakeBtn.spriteState = spriteState;
        } else {
            StakeCostText.text = "MAXED";
        }





    }
    void updateDiskImages()
    {
        for (int i = 0; i < disk; i++)
        {
            if (i < diskImages.Length) diskImages[i].color = Color.white;
        }
    }

    void updateRAMImages()
    {
        for (int i = 0; i < RAM; i++)
        {
            if (i < RAMImages.Length) RAMImages[i].color = Color.white;
        }
    }

    public BGN CalculateCost(int upgradeLevel, BGN initialCost, float rate = 1.25f) {// make this more complicated later
        BGN newCost = BGN.PowMultiply(initialCost, rate, upgradeLevel);
        return newCost;
    }

    public void UpgradeMemory()
    {
        if (GameManager.instance.SatoriPoints >= MemoryCost) {
			OnButtonPressed?.Invoke(this, EventArgs.Empty);
			if (MemorySlots < MaxMemory) {
                MemorySlots += 1; 
                GameManager.instance.SatoriPoints -= MemoryCost;
				MemoryCost = CalculateCost(MemorySlots, InitMemoryCost);
                MemoryCost.Save(MEMORY_COST + position + "n");
                PlayerPrefs.SetInt(MEMORY_SLOTS + position, MemorySlots);
                PlayerPrefs.Save();
                updateShopVisuals();
			} else {
                Debug.Log("Fully Upgraded");
            }

        }

    }
    public void UpgradeRAM()
    {

        if (RAM < MaxRam && GameManager.instance.SatoriPoints >= RamCost) {
			OnButtonPressed?.Invoke(this, EventArgs.Empty);
			RAM += 1;
            foreach (Neuron neuron in NeuronList)
            {
                neuron.progressTimerMax-= RAMDecreaseAmount;
                if (neuron.working == false || neuron.progressTimer > neuron.progressTimerMax)
                {
                    neuron.progressTimer = neuron.progressTimerMax;
                }
            }
            progressTimerMax -= RAMDecreaseAmount;
            progressTimer = progressTimerMax;
			GameManager.instance.SatoriPoints -= RamCost;
			RamCost = CalculateCost((int)RAM, InitRamCost, 2f);

            PlayerPrefs.SetInt(RAM_AMOUNT + position, RAM);
            RamCost.Save(RAM_COST + position + "n");

			updateShopVisuals();
			updateRAMImages();
        }
        else
        {
            Debug.Log("Fully Upgraded");
        }
        
        
    }
    public void UpgradeDisk()
    {
        if (disk < MaxDisk && GameManager.instance.SatoriPoints >= DiskCost) {
			OnButtonPressed?.Invoke(this, EventArgs.Empty);
			disk += 1;
			GameManager.instance.SatoriPoints -= DiskCost;
			DiskCost = CalculateCost(disk, InitDiskCost);
            PlayerPrefs.SetInt(DISK_AMOUNT + position, disk);
            DiskCost.Save(DISK_COST + position + "n");
            updateDiskImages();
			updateShopVisuals();
		}
    }
    public void addNeuron()
    {
        if (NeuronList.Count < MemorySlots && GameManager.instance.SatoriPoints >= NeuronCost)
        {
			OnButtonPressed?.Invoke(this, EventArgs.Empty);
			Neuron newNeuron = Instantiate(NeuronPrefab, transform).GetComponent<Neuron>();
            newNeuron.progressTimerMax = newNeuron.progressTimerMax - RAM*RAMDecreaseAmount;
            newNeuron.progressTimer = newNeuron.progressTimerMax;
            newNeuron.GPUMultiplier = GPUMultiplier * RebirthMultiplier;
            NeuronList.Add(newNeuron);
            updateCritChance();
            //PlacedNeuronList.Clear(); remove to have it random placement 
            foreach (GameObject connection in connections)
            {
                Destroy(connection.gameObject);

            }
            connections.Clear();
            PlaceNeuronsInHardware();
			GameManager.instance.SatoriPoints -= NeuronCost;
			NeuronCost = CalculateCost(NeuronList.Count, InitNeuronCost);
            PlayerPrefs.SetInt(NEURON_COUNT + position, NeuronList.Count);
            PlayerPrefs.Save();
			updateShopVisuals();
		}
        else
        {
            Debug.Log("Not Enough Memory!");
        }
    }
    public void updateCritChance()
    {
        foreach (Neuron neuron in NeuronList)
        {
            if (NeuronList.Count > 1) neuron.critChance = (maxCritChance/16) * (NeuronList.Count-1);

        }
    }
    public void StakeNeuron()
    {
        if (stakedNeurons < NeuronList.Count && GameManager.instance.SatoriPoints >= StakeCost) {
			OnButtonPressed?.Invoke(this, EventArgs.Empty);
			foreach (Neuron neuron in NeuronList) {
                if (!neuron.stake) {
                    neuron.CreateStake();
                    stakedNeurons++;
                    PlayerPrefs.SetInt(STAKED_NEURON_COUNT + position, stakedNeurons);
                    PlayerPrefs.Save();
                    break;
                }
			}
			GameManager.instance.SatoriPoints -= StakeCost;
			StakeCost = CalculateCost(stakedNeurons, InitStakeCost);
			updateShopVisuals();
		}
    }

    public void PlaceNeuronsInHardware()
    {
        foreach (Neuron neuron in NeuronList)
        {
            if (PlacedNeuronList.Contains(neuron)) continue; // Skip if already placed remove to have it random
            RectTransform neuronRect = neuron.GetComponent<RectTransform>();
            bool validPlace = false;
            int attemptCount = 0;
            int maxAttempts = 1000; 

            while (!validPlace && attemptCount < maxAttempts)
            {
                attemptCount++;
                // Generate UnityEngine.Random position within parent bounds
                float yOffset = UnityEngine.Random.Range(0, (rect.rect.height / 2 - neuronRect.rect.height));
                yOffset *= UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1;
                int y = (int)yOffset;

                float xOffset = UnityEngine.Random.Range(0, (rect.rect.width / 2 - neuronRect.rect.width));
                xOffset *= UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1;
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

                GameObject connector = Instantiate(connectorPrefabs[UnityEngine.Random.Range(0, connectorPrefabs.Count)],midpoint,Quaternion.Euler(0, 0, angle),connectorsHolder.transform);
                connections.Add(connector);
                connector.transform.localScale = new Vector3(0.15f * (pair.dist / 2) / 5,0.5f,0.5f);

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

    public void ResetHW() {
        PlayerPrefs.SetInt(NEURON_COUNT + 0, 1);
        PlayerPrefs.SetInt(MEMORY_SLOTS + 0, 1);
        PlayerPrefs.SetInt(RAM_AMOUNT + 0, 0);
        PlayerPrefs.SetInt(DISK_AMOUNT + 0, 1);
        PlayerPrefs.SetInt(STAKED_NEURON_COUNT + 0, 0);


    }

    public void firstTime()
    {

        GameManager.instance.TutorialManager.tutorialSteps[1].targetElement = PlacedNeuronList[0].GetComponent<Neuron>().GetComponent<RectTransform>();
        GameManager.instance.TutorialManager.tutorialSteps[1].targetButton = PlacedNeuronList[0].GetComponent<Neuron>().GetComponentInParent<Button>();
        GameManager.instance.TutorialManager.tutorialSteps[1].message = "Neurons are the source of Satori. Press them to get more Satori points";

        GameManager.instance.TutorialManager.tutorialSteps[3].targetElement = MemoryOBJ.GetComponent<RectTransform>();
        GameManager.instance.TutorialManager.tutorialSteps[3].targetButton = MemoryOBJ.GetComponent<Button>();
        GameManager.instance.TutorialManager.tutorialSteps[3].message = "Memory allows you to buy more Neurons. More Neurons will increase an individual Neuron's Crit Chance.";


        GameManager.instance.TutorialManager.tutorialSteps[4].targetElement = NeuronOBJ.GetComponent<RectTransform>();
        GameManager.instance.TutorialManager.tutorialSteps[4].targetButton = NeuronOBJ.GetComponent<Button>();
        GameManager.instance.TutorialManager.tutorialSteps[4].message = "Here is where you buy more Neurons. The more Neurons you have, the more Satori Points you will earn.";


        GameManager.instance.TutorialManager.tutorialSteps[5].targetElement = RamOBJ.GetComponent<RectTransform>();
        GameManager.instance.TutorialManager.tutorialSteps[5].message = "RAM increases the speed of your Neurons. The more RAM you have, the faster your Neurons will generate Satori Points.";

        GameManager.instance.TutorialManager.tutorialSteps[6].targetElement = DiskOBJ.GetComponent<RectTransform>();
        GameManager.instance.TutorialManager.tutorialSteps[6].message = "Disk allows your Neurons to generate Satori Points while you are offline. The more Disk you have, the more Satori Points you will earn";
        
        
        GameManager.instance.TutorialManager.tutorialSteps[7].targetElement = StakeOBJ.GetComponent<RectTransform>();
        GameManager.instance.TutorialManager.tutorialSteps[7].message = "Stake allows you to automate the earning of Satori. Automating your Neurons will significantly speed up your production of Satori.";

        GameManager.instance.TutorialManager.tutorialSteps[8].targetElement = BackButton.GetComponent<RectTransform>();
        GameManager.instance.TutorialManager.tutorialSteps[8].targetButton = BackButton.GetComponent<Button>();
        GameManager.instance.TutorialManager.tutorialSteps[8].message = ""; 
    }

}
