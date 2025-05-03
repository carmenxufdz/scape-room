using UnityEngine;
using UnityEngine.XR.ARFoundation.Samples; // o donde tengas definida la interfaz Interactable

public class KeypadButton : MonoBehaviour, IInteractable
{
    [Tooltip("Puede ser '0'�'9', o 'CLEAR', o 'OK'")]
    public string valor;

    [Tooltip("Arrastra aqu� tu KeypadController")]
    public KeypadController controlador;

    private void Awake()
    {
        if (controlador == null)
            Debug.LogError($"[{name}] No has asignado el KeypadController.", this);
    }

    // Ser� llamado por tu sistema de interacci�n al �tocar� este collider
    public void Interact()
    {
        switch (valor.ToUpper())
        {
            case "CLEAR":
                controlador.Borrar();
                break;
            case "OK":
                controlador.VerificarContrase�a();
                break;
            default:
                controlador.IngresarNumero(valor);
                break;
        }
    }
}