using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation.Samples;

public class KeypadController : MonoBehaviour
{
    public TextMeshProUGUI pantalla; // Asigna aquí el Text de Unity donde se ve la entrada
    public string contraseñaCorrecta = "1234";
    private string entradaActual = "";

    public SafeBox safeBox;

    public void IngresarNumero(string numero)
    {
        if (entradaActual.Length < contraseñaCorrecta.Length)
        {
            entradaActual += numero;
            pantalla.text = entradaActual;
        }
    }

    public void Borrar()
    {
        // Elimina el último carácter
        entradaActual = entradaActual.Substring(0, entradaActual.Length - 1);
        // Actualiza la pantalla
        pantalla.text = entradaActual;
    }


    public void VerificarContraseña()
    {
        if (entradaActual == contraseñaCorrecta)
        {
            pantalla.text = "OPEN";
            safeBox.Open();
        }
        else
        {
            NotificationManager.Instance.ShowMessage("Contraseña incorrecta");
            Invoke("Incorrect", 1f); // Borrar después de 1 segundo
        }
    }

    private void Incorrect()
    {
        entradaActual = "";
        pantalla.text = "";
    }
}
