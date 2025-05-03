using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.ARFoundation.Samples;

public class InteractionManager : MonoBehaviour
{
    private Camera arCamera;

    public Transform focusPoint; // Empty GameObject frente a la cámara (local z=0.5 aprox)
    public GameObject interactionPanel; // UI con botón de "atrás"
    public GameObject backButton;

    private GameObject currentObject;

    [SerializeField, Tooltip("Factor de margen visual al enfocar el objeto (1.1 = 10% más lejos)")]
    private float focusPaddingFactor = 1.1f; // Puedes ajustar esto desde el Inspector



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

            if (currentObject.CompareTag("Inspectionable") &&
                currentObject.GetComponent<Clock>() != null)
            {
                currentObject.GetComponent<ClockHand>().Interact();

            }
            else if (currentObject.CompareTag("Inspectionable"))
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
                IInteractable interactableChild = touchedObject.GetComponent<IInteractable>();
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
                    touchedObject.GetComponent<IInteractable>().Interact();
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

        currentObject = obj;

        PositionFocusPointForObject(obj); // 👈 Recalculamos el punto dinámicamente

        // Intentamos obtener el handler del propio objeto
        var inspectable = obj.GetComponent<IInspectable>();
        if (inspectable != null)
        {
            inspectable.OnInspect(focusPoint);
        }

        interactionPanel.SetActive(true);
        backButton.SetActive(true);
    }

    void BackToRoom()
    {
        if (currentObject == null) return;

        var inspectable = currentObject.GetComponent<IInspectable>();
        if (inspectable != null)
        {
            inspectable.OnExitInspect();
        }

        currentObject = null;
        interactionPanel.SetActive(false);
        backButton.SetActive(false);
    }

    void PositionFocusPointForObject(GameObject obj)
    {
        Renderer renderer = obj.GetComponentInChildren<Renderer>();
        if (renderer == null)
        {
            Debug.LogWarning("No se encontró un Renderer para calcular el tamaño");
            focusPoint.position = arCamera.transform.position + arCamera.transform.forward * 0.5f;
            return;
        }

        Bounds bounds = renderer.bounds;
        float objectHeight = bounds.size.y;
        float objectRadius = bounds.extents.magnitude;

        float fovRadians = arCamera.fieldOfView * Mathf.Deg2Rad;
        float distanceByHeight = objectHeight / (2.0f * Mathf.Tan(fovRadians / 2.0f));
        float distanceByRadius = objectRadius / Mathf.Sin(fovRadians / 2.0f);

        float finalDistance = Mathf.Max(distanceByHeight, distanceByRadius) * focusPaddingFactor;

        // Punto deseado en el espacio frente a la cámara
        Vector3 desiredCenter = arCamera.transform.position + arCamera.transform.forward * finalDistance;

        focusPoint.position = arCamera.transform.position + arCamera.transform.forward * finalDistance;

        Debug.Log(focusPoint.position);
    }



}
