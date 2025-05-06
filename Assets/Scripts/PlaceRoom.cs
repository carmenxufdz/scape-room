using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;


namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Listens for touch events and performs an AR raycast from the screen touch point.
    /// AR raycasts will only hit detected trackables like feature points and planes.
    ///
    /// If a raycast hits a trackable, the <see cref="placedPrefab"/> is instantiated
    /// and moved to the hit position.
    /// </summary>
    [RequireComponent(typeof(ARRaycastManager))]
    public class PlaceOnPlane : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Instantiates this prefab on a plane at the touch location.")]
        GameObject m_PlacedPrefab;

        bool isPlaced = false;
        ARPointCloudManager m_PointCloud;
        ARPlaneManager m_PlaneManager;
        Camera m_Camera;

        /// <summary>
        /// The prefab to instantiate on touch.
        /// </summary>
        public GameObject placedPrefab
        {
            get { return m_PlacedPrefab; }
            set { m_PlacedPrefab = value; }
        }

        /// <summary>
        /// The object instantiated as a result of a successful raycast intersection with a plane.
        /// </summary>
        public GameObject spawnedObject { get; private set; }

        void Awake()
        {
            m_PointCloud = this.GetComponent<ARPointCloudManager>();
            m_PlaneManager = GetComponent<ARPlaneManager>();

            m_Camera = Camera.main;

        }


        void Update()
        {

            if (!isPlaced)
            {
                // OJO, no funciona "m_PointCloud.trackables.count" para contar el n�mero de puntos, hay que leer las posiciones
                List<ARPoint> updatedPoints = new List<ARPoint>();
                float altura = 0;
                foreach (var pointCloud in m_PointCloud.trackables)
                {
                    foreach (var pos in pointCloud.positions)
                    {
                        ARPoint newPoint = new ARPoint(pos);
                        updatedPoints.Add(newPoint);
                        if (pos.y < altura) altura = pos.y;
                    }
                }
                Transform oTransform = m_Camera.transform;
                float personHeight = -oTransform.position.y + altura;
                Debug.Log("Puntos detectados: " + updatedPoints.Count);


                if (updatedPoints.Count > 5 && personHeight < -0.5f && personHeight > -2.0f)
                {
                    isPlaced = true;
                    Quaternion kk = Quaternion.identity;
                    Vector3 kkeu = kk.eulerAngles;
                    Vector3 pos = new Vector3(oTransform.position.x, oTransform.position.y + personHeight, oTransform.position.z);

                    spawnedObject = Instantiate(m_PlacedPrefab, pos, kk);
                    m_PointCloud.SetTrackablesActive(false);
                    m_PlaneManager.SetTrackablesActive(false);
                    m_PlaneManager.enabled = false;
                }
            }
        }

        public bool isPlacedTrue()
        {
            return isPlaced;
        }


        public class ARPoint
        {
            public float x;
            public float y;
            public float z;
            public ARPoint(Vector3 pos)
            {
                x = pos.x;
                y = pos.y;
                z = pos.z;
            }
        }
    }
}
