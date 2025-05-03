using UnityEngine;
using UnityEngine.XR.ARFoundation.Samples;
using System.Collections;
public class MobilePhoneInteract : MonoBehaviour, IInteractable
{

    public void Interact()
    {
        NotificationManager.Instance.ShowMessage("Son las 12:30");
    }

}
