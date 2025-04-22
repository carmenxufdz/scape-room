using UnityEngine;
using UnityEngine.XR.ARFoundation.Samples;
using System.Collections;
public class Door : MonoBehaviour, Interactable
{
    public string requiredItemName = "DoorKey";

    [Header("Puerta / Tapa")]
    [Tooltip("Transform de la parte móvil que gira 180° en Z")]
    public Transform puerta;

    [Header("Animación")]
    [Tooltip("Duración en segundos del giro")]
    public float tiempoApertura = 1f;

    [Header("Estado")]
    [Tooltip("Indica si la caja ya está abierta")]
    public bool isOpen = false;

    void Awake()
    {
        if (puerta == null)
            Debug.LogError("CajaFuerte: falta asignar el Transform ‘puerta’", this);
    }

    public void Interact()
    {
        if (isOpen) this.GetComponent<Collider>().enabled = false;
        else if (PlayerManager.Instance.IsHoldingItem(requiredItemName))
        {
            Open();
            NotificationManager.Instance.ShowMessage("La puerta se abrió.");
        }
        else if (PlayerManager.Instance.IsHoldingItem())
        {
            NotificationManager.Instance.ShowMessage("Este objeto no hace nada.");
        }
        else
        {
            NotificationManager.Instance.ShowMessage("Tal vez con una llave puedas abrirla...");
        }
    }

    public void Open()
    {
        if (isOpen) return;
        isOpen = true;
        StartCoroutine(AbrirPuerta());
    }

    private IEnumerator AbrirPuerta()
    {
        Quaternion rotInicio = puerta.localRotation;
        Quaternion rotFin = rotInicio * Quaternion.Euler(0f, 100f, 0f);
        float elapsed = 0f;

        while (elapsed < tiempoApertura)
        {
            puerta.localRotation = Quaternion.Slerp(rotInicio, rotFin, elapsed / tiempoApertura);
            elapsed += Time.deltaTime;
            yield return null;
        }
        // Asegura la rotación exacta
        puerta.localRotation = rotFin;
    }
}
