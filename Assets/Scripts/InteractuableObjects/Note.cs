using UnityEngine;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class Note : MonoBehaviour, IInspectable
    {
        // … tus campos de IInspectable …
        private Transform originalParent;
        private Vector3 originalLocalPos;
        private Quaternion originalLocalRot;

        public void OnInspect(Transform focusPoint)
        {
            // guardo transform padre/local
            originalParent = transform.parent;
            originalLocalPos = transform.localPosition;
            originalLocalRot = transform.localRotation;

            GetComponent<Collider>().enabled = false;
            transform.SetParent(focusPoint);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        }

        public void OnExitInspect()
        {
            transform.SetParent(originalParent);
            transform.localPosition = originalLocalPos;
            transform.localRotation = originalLocalRot;
            GetComponent<Collider>().enabled = true;
        }
    }
}
