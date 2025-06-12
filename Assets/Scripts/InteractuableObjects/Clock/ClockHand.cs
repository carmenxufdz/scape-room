using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation.Samples;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class ClockHand : MonoBehaviour, IInteractable
    {
        private Camera arCamera;
        private bool isDragging = false;

        // Para el cálculo de rotación
        private Plane dragPlane;
        private float initialTouchAngle;
        private float initialHandAngle;
        private Transform parentTransform;

        void Start()
        {
            arCamera = Camera.main;
            if (arCamera == null)
                Debug.LogError("ClockHand: no se encontró Camera.main", this);

            parentTransform = transform.parent;
        }

        public void Interact()
        {
            // 1) Al “agarrar” guardamos plano, ángulos iniciales:
            isDragging = true;
            dragPlane = new Plane(parentTransform.forward, parentTransform.position);

            Vector2 screenPos = GetPointerPosition();
            Ray ray = arCamera.ScreenPointToRay(screenPos);

            if (dragPlane.Raycast(ray, out float enter))
            {
                Vector3 worldHit = ray.GetPoint(enter);
                // Pasamos a coords locales del padre
                Vector3 localHit = parentTransform.InverseTransformPoint(worldHit);

                // Ángulo del dedo respecto al centro
                float touchAngle = Mathf.Atan2(localHit.y, localHit.x) * Mathf.Rad2Deg;
                // Ajusta si tu manecilla 0° apunta hacia arriba
                touchAngle -= 90f;

                initialTouchAngle = touchAngle;
                initialHandAngle = transform.localEulerAngles.z;
            }
        }

        void Update()
        {
            if (!isDragging) return;

            // Lectura del drag
            Vector2 screenPos = GetPointerPosition();
            Ray ray = arCamera.ScreenPointToRay(screenPos);

            if (dragPlane.Raycast(ray, out float enter))
            {
                Vector3 worldHit = ray.GetPoint(enter);
                Vector3 localHit = parentTransform.
                    InverseTransformPoint(worldHit);

                float touchAngle = Mathf.Atan2(localHit.y, localHit.x) * 
                    Mathf.Rad2Deg;

                touchAngle -= 90f;

                // Delta entre donde estabas y donde estas
                float delta = Mathf.DeltaAngle(initialTouchAngle, touchAngle);
                float newAngle = initialHandAngle + delta;

                transform.localRotation = Quaternion.Euler(0f, 0f, newAngle);
            }

            // Detectamos fin de drag
            if (Touchscreen.current != null && 
                Touchscreen.current.touches.Count > 0)
            {
                var t = Touchscreen.current.touches[0];
                if (t.phase.ReadValue() == UnityEngine.InputSystem.
                    TouchPhase.Ended)
                    isDragging = false;
            }
            else if (Mouse.current != null && Mouse.current.
                leftButton.wasReleasedThisFrame)
            {
                isDragging = false;
            }
        }

        // Helper para obtener la posición actual del dedo o ratón
        private Vector2 GetPointerPosition()
        {
            if (Touchscreen.current != null && Touchscreen.current.touches.Count > 0)
                return Touchscreen.current.touches[0].position.ReadValue();
            if (Mouse.current != null)
                return Mouse.current.position.ReadValue();
            return Vector2.zero;
        }
    }
}
