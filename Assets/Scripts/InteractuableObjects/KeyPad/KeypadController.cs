using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation.Samples;

public class KeypadController : MonoBehaviour
{
    public TextMeshProUGUI pantalla; // Asigna aqu� el Text de Unity donde se ve la entrada
    public string contrase�aCorrecta = "1234";
    private string entradaActual = "";

    public SafeBox safeBox;

    public void IngresarNumero(string numero)
    {
        if (entradaActual.Length < contrase�aCorrecta.Length)
        {
            entradaActual += numero;
            pantalla.text = entradaActual;
        }
    }

    public void Borrar()
    {
        // Elimina el �ltimo car�cter
        entradaActual = entradaActual.Substring(0, entradaActual.Length - 1);
        // Actualiza la pantalla
        pantalla.text = entradaActual;
    }


    public void VerificarContrase�a()
    {
        if (entradaActual == contrase�aCorrecta)
        {
            pantalla.text = "OPEN";
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
}
