using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    public ItemSlotUI[] uiSlots;
    public ItemSlot[] slots;

    public GameObject inventoryWindow;
    public Transform dropPosition;

    [Header("Selected Item")]
    private ItemSlot selectedItem;
    private int selectedItemIndex;
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI selectedItemStatNames;
    public TextMeshProUGUI selectedItemStatValues;
    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unEquipButton;
    public GameObject dropButton;

    private int curEquipIndex;

    // Components
    private PlayerController controller;
    private PlayerNeeds needs;

    [Header("Events")]
    public UnityEvent onOpenInventory;
    public UnityEvent onCloseInventory;

    // singleton
    public static Inventory instance;

    private void Awake()
    {
        instance = this;
        controller = GetComponent<PlayerController>();
        needs = GetComponent<PlayerNeeds>();
    }

    private void Start()
    {
        inventoryWindow.SetActive(false);
        slots = new ItemSlot[uiSlots.Length];

        // Initialize slots
        for (int x = 0; x < slots.Length; x++)
        {
            slots[x] = new ItemSlot();
            uiSlots[x].index = x;
            uiSlots[x].Clear();
        }

        ClearSelectedItemWindow();
    }

    public void OnInventoryButton(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            Toggle();
        }
    }

    public void Toggle()
    {
        if (inventoryWindow.activeInHierarchy)
        {
            inventoryWindow.SetActive(false);
            onCloseInventory.Invoke();
            controller.ToggleCursor(false);
        }
        else
        {
            inventoryWindow.SetActive(true);
            onOpenInventory.Invoke();
            ClearSelectedItemWindow();
            controller.ToggleCursor(true);
        }
    }

    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }

    public void AddItem (ItemData item)
    {
        if (item.canStack)
        {
            ItemSlot slotToStackTo = GetItemStack(item);

            if (slotToStackTo != null)
            {
                slotToStackTo.quantity++;
                UpdateUI();
                return;
            }
        }

        ItemSlot emptySlot = GetEmptySlot();
        if (emptySlot != null)
        {
            emptySlot.item = item;
            emptySlot.quantity = 1;
            UpdateUI();
            return;
        }

        ThrowItem(item);
    }

    public void SelectItem(int index)
    {
        if (slots[index] == null) return;

        selectedItem = slots[index];
        selectedItemIndex = index;

        // Set Name and Description
        selectedItemName.text = selectedItem.item.displayName;
        selectedItemDescription.text = selectedItem.item.description;

        // set stat values and names
        selectedItemStatNames.text = string.Empty;
        selectedItemStatValues.text = string.Empty;
        for (int x = 0; x < selectedItem.item.consumables.Length; x++)
        {
            selectedItemStatNames.text += selectedItem.item.consumables[x].type.ToString() + "\n";
            selectedItemStatValues.text += selectedItem.item.consumables[x].value.ToString() + "\n";
        }

        // Activate appropriate buttons
        useButton.SetActive(selectedItem.item.type == ItemType.Consumable);
        equipButton.SetActive(selectedItem.item.type == ItemType.Equipable && !uiSlots[index].equipped);
        unEquipButton.SetActive(selectedItem.item.type == ItemType.Equipable && uiSlots[index].equipped);
        dropButton.SetActive(true);
    }

    public void OnUseButton()
    {
        if (selectedItem.item.type == ItemType.Consumable)
        {
            for (int x = 0; x < selectedItem.item.consumables.Length; x++)
            {
                switch (selectedItem.item.consumables[x].type)
                {
                    case ConsumableType.Health: 
                        needs.Heal(selectedItem.item.consumables[x].value);
                        break;
                    case ConsumableType.Hunger:
                        needs.Eat(selectedItem.item.consumables[x].value);
                        break;
                    case ConsumableType.Thirst:
                        needs.Drink(selectedItem.item.consumables[x].value);
                        break;
                    case ConsumableType.Sleep:
                        needs.Sleep(selectedItem.item.consumables[x].value);
                        break;
                }
            }
        }

        RemoveSelectedItem();
    }

    public void OnEquipButton()
    {
        if (uiSlots[curEquipIndex].equipped)
            UnEquip(curEquipIndex);
        uiSlots[selectedItemIndex].equipped = true;
        curEquipIndex = selectedItemIndex;
        EquipManager.instance.EquipNew(selectedItem.item);
        UpdateUI();

        SelectItem(selectedItemIndex);
    }

    public void OnUnEquipButton()
    {
        UnEquip(selectedItemIndex);
    }

    public void OnDropButton()
    {
        ThrowItem(selectedItem.item);
        RemoveSelectedItem();
    }

    public void RemoveItem (ItemData item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == item)
            {
                slots[i].quantity--;
                if (slots[i].quantity == 0)
                {
                    if (uiSlots[i].equipped == true)
                        UnEquip(i);

                    slots[i].item = null;
                    ClearSelectedItemWindow();
                }

                UpdateUI();
                return;
            }
        }
    }

    public bool HasItems(ItemData item, int quantity)
    {
        int amount = 0;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == item)
                amount += slots[i].quantity;
            if (amount >= quantity)
                return true;
        }

        return false;
    }

    private void UnEquip(int index)
    {
        uiSlots[index].equipped = false;
        EquipManager.instance.UnEquip();
        UpdateUI();

        if (selectedItemIndex == index)
        {
            SelectItem(index);
        }
    }

    private void ThrowItem (ItemData item)
    {
        Instantiate(item.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360.0f));
    }

    private void UpdateUI()
    {
        for (int x = 0; x < slots.Length; x++)
        {
            if (slots[x].item != null)
                uiSlots[x].Set(slots[x]);
            else
                uiSlots[x].Clear();
        } 
    }

    private ItemSlot GetItemStack (ItemData item)
    {
        for (int x = 0; x < slots.Length; x++)
        {
            if (slots[x].item == item && slots[x].quantity < item.maxStackAmount)
                return slots[x];
        }

        return null;
    }

    ItemSlot GetEmptySlot()
    {
        for (int x = 0; x < slots.Length; x++)
        {
            if (slots[x].item == null)
                return slots[x];
        }

        return null;
    }

    private void ClearSelectedItemWindow()
    {
        // clear the text elements
        selectedItem = null;
        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;
        selectedItemStatNames.text = string.Empty;
        selectedItemStatValues.text = string.Empty;

        // disable buttons
        useButton.SetActive(false);
        equipButton.SetActive(false);
        unEquipButton.SetActive(false);
        dropButton.SetActive(false);
    }

    private void RemoveSelectedItem()
    {
        selectedItem.quantity--;

        if (selectedItem.quantity == 0)
        {
            if (uiSlots[selectedItemIndex].equipped)
                UnEquip(selectedItemIndex);

            selectedItem.item = null;
            ClearSelectedItemWindow();
        }

        UpdateUI();
    }

}

public class ItemSlot
{
    public ItemData item;
    public int quantity;
}
