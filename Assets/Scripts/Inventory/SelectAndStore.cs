using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class SelectAndStore : MonoBehaviour
{
    private Camera arCamera;

    void Start()
    {
        arCamera = Camera.main;
        if (arCamera == null)
        {
            Debug.LogError("¡Cámara AR no encontrada! Asigna manualmente en el Inspector.");
        }
    }

    void Update()
    {
        // Verifica si el mouse está presente y si se presionó el botón izquierdo
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleTouch(Mouse.current.position.ReadValue());
        }

        // Verifica si hay toques en la pantalla (en móviles)
        if (Touchscreen.current != null)
        {
            // Iterar sobre los toques
            foreach (var touch in Touchscreen.current.touches)
            {
                if (touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began) // Detecta el toque al inicio
                {
                    HandleTouch(touch.position.ReadValue());
                }
            }
        }
    }

    void HandleTouch(Vector2 touchPosition)
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        Ray ray = arCamera.ScreenPointToRay(touchPosition);
        RaycastHit hit;

        Debug.Log("Click");

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.CompareTag("Collectionable"))
            {
                ItemObject itemObject = hit.transform.GetComponent<ItemObject>();
                if (itemObject != null)
                {
                    InventoryManager.Instance.Add(itemObject.item);
                    Destroy(hit.transform.gameObject); // esto sí se puede destruir
                }
            }
        }
    }
}
