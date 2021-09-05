using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    [HideInInspector]
    public ItemData[] items;
    [HideInInspector]
    public Resource[] resources;
    [HideInInspector]
    public BuildingData[] buildings;
    [HideInInspector]
    public NPCData[] npcs;

    public static ObjectManager instance;

    private void Awake()
    {
        instance = this;

        // load in all assets we need
        items = Resources.LoadAll<ItemData>("Items");
        buildings = Resources.LoadAll<BuildingData>("Buildings");
        npcs = Resources.LoadAll<NPCData>("NPCs");
    }

    private void Start()
    {
        // Get all resources
        resources = FindObjectsOfType<Resource>();
    }

    public ItemData GetItemByID(string id)
    {
        for (int x = 0; x < items.Length; x++)
        {
            if (items[x].id == id)
                return items[x];
        }

        Debug.LogError("No item has been found");
        return null;
    }

    public BuildingData GetBuildingByID(string id)
    {
        for (int x = 0; x < buildings.Length; x++)
        {
            if (buildings[x].id == id)
                return buildings[x];
        }

        Debug.LogError("No item has been found");
        return null;
    }
}
