using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class InteractionManager : MonoBehaviour
{
    private Camera arCamera;

    public Transform focusPoint; // Empty GameObject frente a la cámara (local z=0.5 aprox)
    public GameObject interactionPanel; // UI con botón de "atrás"
    public GameObject backButton;


    // Campos para el keypad
    private Transform keypadHome;         // el padre original del keypad
    private Vector3 keypadLocalPos;     // posición local original
    private Quaternion keypadLocalRot;     // rotación local original

    // Campos genéricos para cualquier objeto
    private Transform originalParent;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private GameObject currentObject;


    void Start()
    {
        arCamera = Camera.main;
        if (arCamera == null)
        {
            Debug.LogError("¡Cámara AR no encontrada! Asigna manualmente en el Inspector.");
        }
        interactionPanel.SetActive(false);
    }

    void Update()
    {
        // Si hay un objeto instanciado, permitir rotarlo con touch o mouse
        if (currentObject != null)
        {
            HandleObjectInteraction();

            if (currentObject.CompareTag("Inspectionable"))
            {
                HandleRotation();
            }
        }
        else
        {
            HandleObjectInteraction();
        }
    }

    void HandleRotation()
    {
        float rotationSpeed = 0.2f;

        // Touch (móvil)
        if (Touchscreen.current != null && Touchscreen.current.touches.Count > 0)
        {
            var touch = Touchscreen.current.touches[0];

            if (touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                Vector2 delta = touch.delta.ReadValue();
                currentObject.transform.Rotate(Vector3.up, -delta.x * rotationSpeed, Space.World);
                currentObject.transform.Rotate(Vector3.right, delta.y * rotationSpeed, Space.World);
            }
        }

        // Mouse (Editor o PC)
        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            currentObject.transform.Rotate(Vector3.up, -delta.x * rotationSpeed, Space.World);
            currentObject.transform.Rotate(Vector3.right, delta.y * rotationSpeed, Space.World);
        }
    }

    void HandleObjectInteraction()
    {
        // Si hay un currentInstance, solo permitir la interacción con objetos dentro de él
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleTouch(Mouse.current.position.ReadValue());
        }

        // Verifica si hay toques en la pantalla (en móviles)
        if (Touchscreen.current != null)
        {
            foreach (var touch in Touchscreen.current.touches)
            {
                if (touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
                {
                    HandleTouch(touch.position.ReadValue());
                }
            }
        }
    }

    void HandleTouch(Vector2 touchPosition)
    {
        Ray ray = arCamera.ScreenPointToRay(touchPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject touchedObject = hit.transform.gameObject;


            // Si estamos tocando el currentInstance, verificamos si es un hijo
            if (currentObject != null)
            {
                Interactable interactableChild = touchedObject.GetComponent<Interactable>();
                Debug.Log(interactableChild);
                if (interactableChild != null)
                {
                    interactableChild.Interact();
                    return;
                }

                // Si no implementa Interactable, sigue con fallback (como objetos coleccionables)
                if (touchedObject.CompareTag("Collectionable"))
                {
                    ItemObject itemObject = touchedObject.GetComponent<ItemObject>();
                    if (itemObject != null)
                    {
                        InventoryManager.Instance.Add(itemObject.item);
                        Destroy(touchedObject);
                        return;
                    }
                }

                return; // si tocamos algo dentro del currentInstance, salimos
            }
            else
            {
                // Si no hay currentInstance o tocamos fuera de él, seguimos con la lógica normal
                if (touchedObject.CompareTag("Collectionable"))
                {
                    ItemObject itemObject = touchedObject.GetComponent<ItemObject>();
                    if (itemObject != null)
                    {
                        InventoryManager.Instance.Add(itemObject.item);
                        Destroy(touchedObject);
                    }
                }
                else if (touchedObject.CompareTag("Interactuable") || touchedObject.CompareTag("Inspectionable"))
                {
                    ShowObject(touchedObject);
                }
                else if (touchedObject.CompareTag("Door"))
                {
                    touchedObject.GetComponent<Interactable>().Interact();
                }
                else if (touchedObject.CompareTag("Window"))
                {
                    NotificationManager.Instance.ShowMessage("Esta cerrado.");
                }
            }
        }
    }




    void ShowObject(GameObject obj)
    {
        if (currentObject != null) return;

        // guardamos padre/pos/rot mundiales por si sirve en otros objetos
        originalParent = obj.transform.parent;
        originalPosition = obj.transform.position;
        originalRotation = obj.transform.rotation;
        currentObject = obj;

        // **Si es el keypad**, además guardamos su padre y su localTransform
        var kc = obj.GetComponent<KeypadController>();
        if (kc != null)
        {
            keypadHome = obj.transform.parent;
            keypadLocalPos = obj.transform.localPosition;
            keypadLocalRot = obj.transform.localRotation;
        }

        // lo enviamos al foco
        obj.transform.SetParent(focusPoint);
        obj.transform.localPosition = Vector3.zero;

        // gira solo el keypad 180° en Y
        if (kc != null)
            obj.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        else
            obj.transform.localRotation = Quaternion.identity;

        interactionPanel.SetActive(true);
        backButton.SetActive(true);
    }


    public void BackToRoom()
    {
        if (currentObject == null) return;

        // **Si es el keypad**, restauramos su padre y su localTransform guardado
        if (currentObject.GetComponent<KeypadController>() != null && keypadHome != null)
        {
            currentObject.transform.SetParent(keypadHome);
            currentObject.transform.localPosition = keypadLocalPos;
            currentObject.transform.localRotation = keypadLocalRot;
        }
        else
        {
            // resto de objetos: restauración genérica
            currentObject.transform.SetParent(originalParent);
            currentObject.transform.position = originalPosition;
            currentObject.transform.rotation = originalRotation;
        }

        // reactiva el collider y limpia
        var col = currentObject.GetComponent<Collider>();
        if (col != null) col.enabled = true;
        currentObject = null;

        interactionPanel.SetActive(false);
        backButton.SetActive(false);
    }


}
