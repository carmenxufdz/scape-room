using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem; // <- NUEVO INPUT SYSTEM

public class InventoryMenuController : MonoBehaviour
{
    public GameObject scrollViewMenu;

    void Update()
    {
        // Detecta toque o clic con el nuevo sistema
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            TryCloseInventory(Touchscreen.current.primaryTouch.position.ReadValue());
        }
        else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryCloseInventory(Mouse.current.position.ReadValue());
        }
    }

    void TryCloseInventory(Vector2 screenPosition)
    {
        if (!scrollViewMenu.activeSelf) return;

        if (!IsPointerOverUIElement(screenPosition, scrollViewMenu))
        {
            scrollViewMenu.SetActive(false);
            Debug.Log("📦 Menú cerrado.");
        }
    }

    bool IsPointerOverUIElement(Vector2 position, GameObject panel)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = position;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            if (result.gameObject == panel || result.gameObject.transform.IsChildOf(panel.transform))
            {
                return true;
            }
        }

        return false;
    }
}
