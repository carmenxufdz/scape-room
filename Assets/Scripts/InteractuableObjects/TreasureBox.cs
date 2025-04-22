using UnityEngine;
using UnityEngine.XR.ARFoundation.Samples;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class TreasureBox : MonoBehaviour, Interactable
{
    public GameObject boxClosed;
    public GameObject boxOpen;
    public string requiredItemName = "ChestKey";

    private bool isOpen = false;

    public void Interact()
    {
        if (isOpen) this.GetComponent<Collider>().enabled = false;
        else if (PlayerManager.Instance.IsHoldingItem(requiredItemName))
        {
            // Abre la caja en primer plano
            if (boxClosed != null) boxClosed.SetActive(false);
            if (boxOpen != null) boxOpen.SetActive(true);
            isOpen = true;
            this.GetComponent<Collider>().enabled = false;
            NotificationManager.Instance.ShowMessage("La caja se abrió.");
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

    // Función para abrir la caja original (sincronización de estado)
    public void OpenBox()
    {
        if (boxClosed != null) boxClosed.SetActive(false);
        if (boxOpen != null) boxOpen.SetActive(true);

        isOpen = true;
    }
}
