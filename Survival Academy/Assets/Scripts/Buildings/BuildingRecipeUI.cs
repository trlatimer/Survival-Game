using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingRecipeUI : MonoBehaviour
{
    public BuildingRecipe recipe;
    public Image backgroundImage;
    public Image icon;
    public TextMeshProUGUI buildingName;
    public Image[] resourceCosts;

    public Color canBuildColor;
    public Color cannotBuildColor;
    private bool canBuild;

    private void OnEnable()
    {
        UpdateCanCraft();
    }

    private void Start()
    {
        icon.sprite = recipe.icon;
        buildingName.text = recipe.displayName;

        for (int x = 0; x < resourceCosts.Length; x++)
        {
            if (x < recipe.cost.Length)
            {
                resourceCosts[x].gameObject.SetActive(true);

                resourceCosts[x].sprite = recipe.cost[x].item.icon;
                resourceCosts[x].transform.GetComponentInChildren<TextMeshProUGUI>().text = recipe.cost[x].quantity.ToString();
            }
            else
                resourceCosts[x].gameObject.SetActive(false);
        }
    }

    private void UpdateCanCraft()
    {
        canBuild = true;

        for (int x = 0; x < recipe.cost.Length; x++)
        {
            if (!Inventory.instance.HasItems(recipe.cost[x].item, recipe.cost[x].quantity))
            {
                canBuild = false;
                break;
            }
        }

        backgroundImage.color = canBuild ? canBuildColor : cannotBuildColor;
    }

    public void OnClickButton()
    {

    }
}
