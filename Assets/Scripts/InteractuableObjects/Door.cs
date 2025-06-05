using UnityEngine;
using UnityEngine.XR.ARFoundation.Samples;
using System.Collections;
public class Door : MonoBehaviour, IInteractable
{
    public string requiredItemName = "DoorKey";
    public AudioClip doorOpen;
    public AudioSource audioSource;

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
        audioSource = GetComponent<AudioSource>();
        if (puerta == null)
            Debug.LogError("CajaFuerte: falta asignar el Transform ‘puerta’", this);
    }

    public void Interact()
    {
        if (isOpen) this.GetComponent<Collider>().enabled = false;
        else if (PlayerManager.Instance.IsHoldingItem(requiredItemName))
        {
            Open();
            PlayerManager.Instance.ClearHeldItem();
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

        audioSource.PlayOneShot(doorOpen);

        // Espera un pequeño tiempo para que el sonido se sienta "primero"
        yield return new WaitForSeconds(1.0f); // Ajusta el tiempo según tu necesidad

        while (elapsed < tiempoApertura)
        {
            puerta.localRotation = Quaternion.Slerp(rotInicio, rotFin, elapsed / tiempoApertura);
            elapsed += Time.deltaTime;
            yield return null;
        }
        // Asegura la rotación exacta
        puerta.localRotation = rotFin;

        yield return new WaitForSeconds(0.5f);

        NotificationManager.Instance.ShowMessage("La puerta se abrió.");

        NotificationManager.Instance.finalMessage("¡ENHORABUENA!");

        Timer.Instance.GameOver();
    }
}
