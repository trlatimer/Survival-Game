using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingRecipeUI : MonoBehaviour
{
    public CraftingRecipe recipe;
    public Image backgroundImage;
    public Image icon;
    public TextMeshProUGUI itemName;
    public Image[] resourceCosts;

    public Color canCraftColor;
    public Color cannotCraftColor;

    private bool canCraft;

    private void OnEnable()
    {
        UpdateCanCraft();
    }

    private void Start()
    {
        icon.sprite = recipe.itemToCraft.icon;
        itemName.text = recipe.itemToCraft.displayName;

        for (int i = 0; i < resourceCosts.Length; i++)
        {
            if (i < recipe.costs.Length)
            {

            }
            else
            {
                resourceCosts[i].gameObject.SetActive(false);
            }
        }
    }

    public void UpdateCanCraft()
    {
        canCraft = true;

        for(int i = 0; i < recipe.costs.Length; i++)
        {
            if (!Inventory.instance.HasItems(recipe.costs[i].item, recipe.costs[i].quantity))
            {
                canCraft = false;
                break;
            }
        }

        backgroundImage.color = canCraft ? canCraftColor : cannotCraftColor;
    }
}
