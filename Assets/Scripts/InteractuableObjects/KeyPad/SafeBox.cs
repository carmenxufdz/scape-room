using UnityEngine;
using System.Collections;

public class SafeBox : MonoBehaviour
{
    [Header("Puerta / Tapa")]
    [Tooltip("Transform de la parte m�vil que gira 180� en Z")]
    public Transform puerta;

    [Header("Animaci�n")]
    [Tooltip("Duraci�n en segundos del giro")]
    public float tiempoApertura = 1f;

    [Header("Estado")]
    [Tooltip("Indica si la caja ya est� abierta")]
    public bool isOpen = false;

    void Awake()
    {
        if (puerta == null)
            Debug.LogError("CajaFuerte: falta asignar el Transform �puerta�", this);
    }

    /// <summary>
    /// Llama a este m�todo para abrir la caja (por ejemplo, desde el KeypadController)
    /// </summary>
    public void Open()
    {
        if (isOpen) return;
        isOpen = true;
        StartCoroutine(AbrirPuerta());
    }

    private IEnumerator AbrirPuerta()
    {
        Quaternion rotInicio = puerta.localRotation;
        Quaternion rotFin = rotInicio * Quaternion.Euler(0f, 0f, 100f);
        float elapsed = 0f;

        while (elapsed < tiempoApertura)
        {
            puerta.localRotation = Quaternion.Slerp(rotInicio, rotFin, elapsed / tiempoApertura);
            elapsed += Time.deltaTime;
            yield return null;
        }
        // Asegura la rotaci�n exacta
        puerta.localRotation = rotFin;
    }
}

