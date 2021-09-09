using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SaveManager : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(LoadGame());
    }

    IEnumerator LoadGame()
    {
        yield return new WaitForEndOfFrame();

        if (PlayerPrefs.HasKey("Save"))
            Load();
    }

    private void Update()
    {
        if (Keyboard.current.nKey.wasPressedThisFrame)
            Save();

        if (Keyboard.current.mKey.wasPressedThisFrame)
            Load();
    }

    private void Save()
    {
        SaveData data = new SaveData();

        // Player location
        data.playerPos = new SVec3(PlayerController.instance.transform.position);
        data.playerRot = new SVec3(PlayerController.instance.transform.eulerAngles);
        data.playerLook = new SVec3(PlayerController.instance.cameraContainer.localEulerAngles);

        // Player needs
        data.health = PlayerNeeds.instance.health.curValue;
        data.hunger = PlayerNeeds.instance.hunger.curValue;
        data.thirst = PlayerNeeds.instance.thirst.curValue;
        data.sleep = PlayerNeeds.instance.sleep.curValue;

        // Inventory
        data.inventory = new SInventorySlot[Inventory.instance.slots.Length];
        for (int x = 0; x < Inventory.instance.slots.Length; x++)
        {
            data.inventory[x] = new SInventorySlot();
            data.inventory[x].occupied = Inventory.instance.slots[x].item != null;

            if (!data.inventory[x].occupied) continue;

            data.inventory[x].itemId = Inventory.instance.slots[x].item.id;
            data.inventory[x].quantity = Inventory.instance.slots[x].quantity;
            data.inventory[x].equipped = Inventory.instance.uiSlots[x].equipped;
        }

        // Dropped Items
        ItemObject[] droppedItems = FindObjectsOfType<ItemObject>();
        data.droppedItems = new SDroppedItem[droppedItems.Length];
        for (int x = 0; x < droppedItems.Length; x++)
        {
            data.droppedItems[x] = new SDroppedItem();
            data.droppedItems[x].itemId = droppedItems[x].item.id;
            data.droppedItems[x].position = new SVec3(droppedItems[x].transform.position);
            data.droppedItems[x].rotation = new SVec3(droppedItems[x].transform.eulerAngles);
        }

        // Buildings
        Building[] buildingObjects = FindObjectsOfType<Building>();
        data.buildings = new SBuilding[buildingObjects.Length];

        for (int x = 0; x < buildingObjects.Length; x++)
        {
            data.buildings[x] = new SBuilding();
            data.buildings[x].buildingId = buildingObjects[x].data.id;
            data.buildings[x].position = new SVec3(buildingObjects[x].transform.position);
            data.buildings[x].rotation = new SVec3(buildingObjects[x].transform.eulerAngles);
            data.buildings[x].customProperties = buildingObjects[x].GetCustomProperties();
        }

        // Resources
        data.resources = new SResource[ObjectManager.instance.resources.Length];
        for (int x = 0; x < ObjectManager.instance.resources.Length; x++)
        {
            data.resources[x] = new SResource();
            data.resources[x].index = x;
            data.resources[x].destroyed = ObjectManager.instance.resources[x] == null;

            if (!data.resources[x].destroyed)
                data.resources[x].capacity = ObjectManager.instance.resources[x].capacity;
        }

        // NPCs
        NPC[] npcs = FindObjectsOfType<NPC>();
        data.npcs = new SNPC[npcs.Length];
        for (int x = 0; x < npcs.Length; x++)
        {
            data.npcs[x] = new SNPC();
            data.npcs[x].prefabId = npcs[x].data.id;
            data.npcs[x].position = new SVec3(npcs[x].transform.position);
            data.npcs[x].rotation = new SVec3(npcs[x].transform.eulerAngles);
            data.npcs[x].aiState = (int)npcs[x].aiType;
            data.npcs[x].hasAgentDestination = !npcs[x].agent.isStopped;
            data.npcs[x].agentDestination = new SVec3(npcs[x].agent.destination);
        }

        // Time of Day
        data.timeOfDay = DayNightCycle.instance.time;

        string rawData = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("Save", rawData);

    }

    private void Load()
    {
      
    }
}
