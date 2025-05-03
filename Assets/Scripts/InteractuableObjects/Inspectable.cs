using UnityEngine;

public interface IInspectable
{
    /// Se llama cuando el objeto pasa al foco (ShowObject)
    void OnInspect(Transform focusPoint);
    /// Se llama cuando el objeto vuelve al mundo (BackToRoom)
    void OnExitInspect();
}
