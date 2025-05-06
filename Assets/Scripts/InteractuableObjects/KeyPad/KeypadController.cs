using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation.Samples;

public class KeypadController : MonoBehaviour, IInspectable
{
    public TextMeshProUGUI pantalla; // Asigna aqu� el Text de Unity donde se ve la entrada
    public string contrase�aCorrecta = "2709";
    private string entradaActual = "";

    public AudioClip keyPadSound;
    public AudioSource audioSource;

    private bool isOpen = false;

    public SafeBox safeBox;

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
        // y giro 180� en Y
        transform.localRotation = Quaternion.Euler(-90f, 180f, 0f);
    }

    // IInspectable: al salir de la inspecci�n
    public void OnExitInspect()
    {
        // restituyo todo
        transform.SetParent(originalParent);
        transform.localPosition = originalLocalPos;
        transform.localRotation = originalLocalRot;
        GetComponent<Collider>().enabled = true;
    }

    public void IngresarNumero(string numero)
    {
        if (isOpen) return;
        audioSource.PlayOneShot(keyPadSound);
        if (entradaActual.Length < contrase�aCorrecta.Length)
        {
            entradaActual += numero;
            pantalla.text = entradaActual;
        }
    }

    public void Borrar()
    {
        if (isOpen) return;
        // Elimina el �ltimo car�cter
        audioSource.PlayOneShot(keyPadSound);
        entradaActual = entradaActual.Substring(0, entradaActual.Length - 1);
        // Actualiza la pantalla
        pantalla.text = entradaActual;
    }


    public void VerificarContrase�a()
    {
        if (isOpen) return;
        audioSource.PlayOneShot(keyPadSound);
        if (entradaActual == contrase�aCorrecta)
        {
            isOpen = true;
            StartCoroutine(MostrarTextoGradualmente("OPEN", Color.green));
            safeBox.Open();
        }
        else
        {
            NotificationManager.Instance.ShowMessage("Contrase�a incorrecta");
            Invoke("Incorrect", 1f); // Borrar despu�s de 1 segundo
        }
    }

    private void Incorrect()
    {
        entradaActual = "";
        pantalla.text = "";
    }

    private IEnumerator MostrarTextoGradualmente(string mensaje, Color color)
    {
        pantalla.text = "";
        pantalla.color = color;

        for (int i = 0; i < mensaje.Length; i++)
        {
            pantalla.text += mensaje[i];
            yield return new WaitForSeconds(0.2f); // Puedes ajustar la velocidad aqu�
        }
    }
}
