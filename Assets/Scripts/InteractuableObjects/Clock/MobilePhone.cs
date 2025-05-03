using UnityEngine;
using UnityEngine.XR.ARFoundation.Samples;
using System.Collections;
public class MobilePhone : MonoBehaviour, IInspectable
{

    [Header("Transform padre original (guardado)")]
    private Transform originalParent;
    private Vector3 originalLocalPos;
    private Quaternion originalLocalRot;

    // IInspectable: al inspeccionar
    public void OnInspect(Transform focusPoint)
    {
        // guardo
        originalParent = transform.parent;
        originalLocalPos = transform.localPosition;
        originalLocalRot = transform.localRotation;
        // desactivo collider y llevo al foco
        GetComponent<Collider>().enabled = false;
        transform.SetParent(focusPoint);
        transform.localPosition = Vector3.zero;
        // y giro 180° en Y
        transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
    }

    // IInspectable: al salir de la inspección
    public void OnExitInspect()
    {
        // restituyo todo
        transform.SetParent(originalParent);
        transform.localPosition = originalLocalPos;
        transform.localRotation = originalLocalRot;
        GetComponent<Collider>().enabled = true;
    }

}
