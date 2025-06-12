using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.ARFoundation.Samples;

public class InteractionManager : MonoBehaviour
{
    private Camera arCamera;

    public Transform focusPoint; 
    public GameObject interactionPanel; 
    public GameObject backButton;

    private GameObject currentObject;

    private float rotationSpeed = 0.2f;
    private bool isDragging = false;
    private Vector2 lastInputPosition;

    private float focusPaddingFactor = 1.5f; 
    private float minimumDistance = 0.2f; // distancia mínima absoluta


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
        if (Touchscreen.current != null && Touchscreen.current.touches.Count > 0)
        {
            var touch = Touchscreen.current.touches[0];
            var phase = touch.phase.ReadValue();
            var position = touch.position.ReadValue();

            if (phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                isDragging = true;
                lastInputPosition = position;
            }
            else if (phase == UnityEngine.InputSystem.TouchPhase.Moved && 
                isDragging)
            {
                Vector2 delta = position - lastInputPosition;

                // Rota el objeto en su espacio local para que el giro sea coherente
                currentObject.transform.Rotate(Vector3.up, -delta.x * 
                    rotationSpeed, Space.Self);
                currentObject.transform.Rotate(Vector3.right, delta.y * 
                    rotationSpeed, Space.Self);

                lastInputPosition = position;
            }
            else if (phase == UnityEngine.InputSystem.TouchPhase.Ended 
                || phase == UnityEngine.InputSystem.TouchPhase.Canceled)
            {
                isDragging = false;
            }
        }

        // Mouse (Editor o PC)
        if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                isDragging = true;
                lastInputPosition = Mouse.current.position.ReadValue();
            }
            else if (Mouse.current.leftButton.isPressed && isDragging)
            {
                Vector2 currentPos = Mouse.current.position.ReadValue();
                Vector2 delta = currentPos - lastInputPosition;

                currentObject.transform.Rotate(Vector3.up, -delta.x * rotationSpeed, Space.Self);
                currentObject.transform.Rotate(Vector3.right, delta.y * rotationSpeed, Space.Self);

                lastInputPosition = currentPos;
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                isDragging = false;
            }
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


                else if (touchedObject.CompareTag("Interactuable") || 
                    touchedObject.CompareTag("Inspectionable"))
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

        // Recalcular el punto
        PositionFocusPointForObject(obj); 

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

    /// <summary>
    /// Coloca el `focusPoint` a una distancia adecuada frente a la cámara para encuadrar completamente
    /// un objeto 3D en la vista, usando su tamaño real y el FOV de la cámara.
    /// Si el objeto es muy pequeño, se usa una distancia mínima configurable para evitar que quede demasiado cerca.
    /// </summary>
    void PositionFocusPointForObject(GameObject obj)
    {
        Renderer renderer = obj.GetComponentInChildren<Renderer>();
        if (renderer == null)
        {
            focusPoint.position = arCamera.transform.position + 
                arCamera.transform.forward * minimumDistance;
            return;
        }

        Bounds bounds = renderer.bounds;
        Vector3 size = bounds.size;
        float objectHeight = size.y;
        float objectWidth = size.x;
        float objectRadius = bounds.extents.magnitude;

        if (size.magnitude < 0.05f)
        {
            focusPoint.position = arCamera.transform.position + 
                arCamera.transform.forward * minimumDistance;
            return;
        }

        float aspect = (float)Screen.width / Screen.height;
        float verticalFovRad = arCamera.fieldOfView * Mathf.Deg2Rad;

        float horizontalFovRad = 2f * 
            Mathf.Atan(Mathf.Tan(verticalFovRad / 2f) * aspect);

        float distanceByHeight = objectHeight / 
            (2f * Mathf.Tan(verticalFovRad / 2f));

        float distanceByWidth = objectWidth / 
            (2f * Mathf.Tan(horizontalFovRad / 2f));

        float distanceByRadius = objectRadius / 
            Mathf.Sin(Mathf.Max(verticalFovRad, horizontalFovRad) / 2f);

        float finalDistance = Mathf.Max(distanceByHeight, distanceByWidth, 
            distanceByRadius) * focusPaddingFactor;

        focusPoint.position = arCamera.transform.position + 
            arCamera.transform.forward * finalDistance;

    }

}
