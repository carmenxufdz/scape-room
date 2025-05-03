using UnityEngine;
using UnityEngine.XR.ARFoundation.Samples;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CrumpledPaper : MonoBehaviour, IInspectable
{

    [Header("Transform padre original (guardado)")]
    private Transform originalParent;
    private Vector3 originalLocalPos;
    private Quaternion originalLocalRot;

    [Header("Estados del papel")]
    public GameObject crumpled; // arrugado
    public GameObject paper;    // abierto

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
        transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
    }

    // IInspectable: al salir de la inspección
    public void OnExitInspect()
    {
        // restituyo transform
        transform.SetParent(originalParent);
        transform.localPosition = originalLocalPos;
        transform.localRotation = originalLocalRot;

        // vuelve a estar arrugado
        if (crumpled != null) crumpled.SetActive(true);
        if (paper != null) paper.SetActive(false);

        // reactivo collider del arrugado (si es el contenedor)
        GetComponent<Collider>().enabled = true;
    }

}
