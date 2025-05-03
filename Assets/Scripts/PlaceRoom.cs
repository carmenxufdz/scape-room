using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class PlaceRoomOnPlane : MonoBehaviour
{
    [Header("Prefab de la habitación")]
    [SerializeField]
    private GameObject roomPrefab;

    [Header("Altura de la cámara (m)")]
    [Tooltip("Altura mínima a la que se permite colocar la habitación")]
    [SerializeField]
    private float minCameraHeight = 0.9f;
    [Tooltip("Altura máxima a la que se permite colocar la habitación")]
    [SerializeField]
    private float maxCameraHeight = 2.0f;

    private ARRaycastManager raycastManager;
    private Camera arCamera;
    private bool isPlaced = false;

    // Reutilizamos esta lista para no alocar cada frame
    private static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        arCamera = Camera.main;
        if (arCamera == null)
            Debug.LogError("PlaceRoomOnPlane: no se encontró Camera.main", this);
    }

    void Update()
    {
        if (isPlaced)
            return;

        // 1) Filtramos por altura de cámara (opcional)
        float camY = arCamera.transform.position.y;
        if (camY < minCameraHeight || camY > maxCameraHeight)
            return;

        // 2) Raycast vertical hacia abajo contra los planes detectados
        Ray downRay = new Ray(arCamera.transform.position, Vector3.down);
        if (!raycastManager.Raycast(downRay, s_Hits, TrackableType.PlaneWithinPolygon))
            return;

        // 3) Escogemos el plano más bajo (último de s_Hits, que vienen ordenados por distancia al origen del rayo)
        var floorHit = s_Hits[s_Hits.Count - 1];
        Pose hitPose = floorHit.pose;

        // 4) Calculamos la rotación para que la habitación “mire” hacia la cámara en Y
        Quaternion faceCamY = Quaternion.LookRotation(
            Vector3.ProjectOnPlane(arCamera.transform.forward, Vector3.up),
            Vector3.up
        );

        // 5) Instanciamos la habitación justo en hitPose.position
        Instantiate(roomPrefab, hitPose.position, faceCamY);
        isPlaced = true;
    }

}
