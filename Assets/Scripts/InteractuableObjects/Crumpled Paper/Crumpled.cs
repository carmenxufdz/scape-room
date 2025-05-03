using UnityEngine;
using UnityEngine.XR.ARFoundation.Samples;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Crumpled : MonoBehaviour, IInteractable
{
    public GameObject crumpled;
    public GameObject paper;

    public void Interact()
    {
        // Abre la caja en primer plano
        if (crumpled != null) crumpled.SetActive(false);
        if (paper != null) paper.SetActive(true);
    }


}
