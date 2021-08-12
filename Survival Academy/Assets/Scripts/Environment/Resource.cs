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
            capacity -= 1;
            if (capacity <= 0)
            {
                break;
            }
            Inventory.instance.AddItem(itemToGive);
        }

        Instantiate(hitParticle, hitpoint, Quaternion.LookRotation(hitNormal, Vector3.up));

        if (capacity <= 0)
            Destroy(gameObject);
    }
}
