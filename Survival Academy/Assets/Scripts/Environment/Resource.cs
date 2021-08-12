using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public ItemData itemToGive;
    public int quantityPerHit = 1;
    public int capacity;
    public GameObject hitParticle;

    public void Gather(Vector3 hitpoint, Vector3 hitNormal)
    {
        for (int i = 0; i < quantityPerHit; i++)
        {
            if (capacity <= 0)
                break;
            capacity -= 1;
            Inventory.instance.AddItem(itemToGive);
        }

        Destroy(Instantiate(hitParticle, hitpoint, Quaternion.LookRotation(hitNormal, Vector3.up)), 1.0f);

        if (capacity <= 0)
            Destroy(gameObject);
    }
}
