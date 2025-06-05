using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation.Samples;


public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public List<Item> Items = new List<Item>();
    private ItemButton selectedItem = null;

    public Transform ItemContent;
    public GameObject InventoryItem;
    public GameObject InventoryPanel;
    private bool isInventoryOpen = false;

    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        InventoryPanel.SetActive(isInventoryOpen);
    }



    private void Awake()
    {
        Instance = this;
    }

    public void Add(Item item)
    {
        Items.Add(item);
    }

    public void Remove(Item item)
    {
        Items.Remove(item);
    }

    public void ListItems()
    {
        Debug.Log("Listado de ítems en el inventario:");

        foreach (Transform item in ItemContent)
        {
            Destroy(item.gameObject);
        }

        selectedItem = null; // Reiniciamos la selección visual

        foreach (var item in Items)
        {
            GameObject obj = Instantiate(InventoryItem, ItemContent);
            var itemUI = obj.GetComponent<ItemButton>();
            itemUI.SetData(item);

            // Comprobamos si el item en mano tiene el mismo uniqueId
            if (PlayerManager.Instance.GetHeldItem() == item)
            {
                itemUI.SetSelected(true);
                selectedItem = itemUI;
            }
            else
            {
                itemUI.SetSelected(false);
            }
        }
    }


    public void UpdateSelection(ItemButton newSelected)
    {
        if (selectedItem != null)
        {
            selectedItem.SetSelected(false);
        }

        selectedItem = newSelected;

        if (selectedItem != null)
        {
            selectedItem.SetSelected(true);
        }
    }

}

