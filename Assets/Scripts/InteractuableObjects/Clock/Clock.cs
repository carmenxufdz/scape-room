using UnityEngine;
using System.Collections;

public class Clock : MonoBehaviour, IInspectable
{
    [Header("Manecillas")]
    public Transform minutero;
    public Transform horario;

    [Header("Ángulos objetivo (en grados)")]
    public float objetivoMinutero = 270f;
    public float objetivoHorario = 90f;

    [Header("Tolerancia (grados)")]
    public float tolerancia = 5f;

    [Header("Panel a abrir")]
    public GameObject panelSecreto;

    [Header("Animación de apertura")]
    public float gradosApertura = 90f;
    public float tiempoApertura = 1f;

    private bool puzzleResuelto = false;
    private bool isAnimating = false;

    // … tus campos de IInspectable …
    private Transform originalParent;
    private Vector3 originalLocalPos;
    private Quaternion originalLocalRot;

    public void OnInspect(Transform focusPoint)
    {
        // guardo transform padre/local
        originalParent = transform.parent;
        originalLocalPos = transform.localPosition;
        originalLocalRot = transform.localRotation;

        GetComponent<Collider>().enabled = false;
        transform.SetParent(focusPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
    }

    public void OnExitInspect()
    {
        transform.SetParent(originalParent);
        transform.localPosition = originalLocalPos;
        transform.localRotation = originalLocalRot;
        GetComponent<Collider>().enabled = true;
    }

    void Update()
    {
        // Siempre comprobamos, aunque puzzleResuelto == true
        float angMin = minutero.localEulerAngles.z;
        float angHor = horario.localEulerAngles.z;

        bool okMin = Mathf.Abs(Mathf.DeltaAngle(angMin, 
            objetivoMinutero)) < tolerancia;
        bool okHor = Mathf.Abs(Mathf.DeltaAngle(angHor, 
            objetivoHorario)) < tolerancia;
        bool enPosicion = okMin && okHor;

        if (enPosicion && !puzzleResuelto && !isAnimating)
        {
            // Manecillas acaban de entrar en la posición correcta
            StartCoroutine(AbrirPanel());
        }
        else if (!enPosicion && puzzleResuelto && !isAnimating)
        {
            // Manecillas han salido de la posición correcta => cerrar
            StartCoroutine(CerrarPanel());
        }
    }

    private IEnumerator AbrirPanel()
    {
        puzzleResuelto = true;
        isAnimating = true;

        if (panelSecreto == null)
        {
            Debug.LogWarning("Clock: panelSecreto no asignado");
            yield break;
        }

        Transform t = panelSecreto.transform;
        Quaternion rotInicio = t.localRotation;
        Quaternion rotFinal = rotInicio * Quaternion.Euler(0f, 
            gradosApertura, 0f);
        float elapsed = 0f;

        while (elapsed < tiempoApertura)
        {
            t.localRotation = Quaternion.Slerp(rotInicio, rotFinal, 
                elapsed / tiempoApertura);
            elapsed += Time.deltaTime;
            yield return null;
        }
        t.localRotation = rotFinal;
        isAnimating = false;
    }

    private IEnumerator CerrarPanel()
    {
        puzzleResuelto = false;
        isAnimating = true;

        Transform t = panelSecreto.transform;
        Quaternion rotInicio = t.localRotation;
        // Deshacer el giro de apertura:
        Quaternion rotFinal = rotInicio * Quaternion.Euler(0f, 
            -gradosApertura, 0f);
        float elapsed = 0f;

        while (elapsed < tiempoApertura)
        {
            t.localRotation = Quaternion.Slerp(rotInicio, rotFinal, 
                elapsed / tiempoApertura);
            elapsed += Time.deltaTime;
            yield return null;
        }
        t.localRotation = rotFinal;
        isAnimating = false;
    }

    public bool getPuzzleResuelto()
    {
        return puzzleResuelto;
    }
}
