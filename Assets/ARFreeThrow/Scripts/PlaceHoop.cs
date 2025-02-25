using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class PlaceHoop : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Prefabin dokunduğu konumu algılayıp çalışmasını sağlar.")]
    GameObject m_HoopPrefab;

    /// <summary>
    /// Dokunulduğunda çıkacak prefab
    /// </summary>
    public GameObject placedHoop
    {
        get { return m_HoopPrefab; }
        set { m_HoopPrefab = value; }
    }

    /// <summary>
    /// Zemin ile uygun kesişimde spawn olmasını sağlar
    /// </summary>
    public GameObject spawnedHoop { get; private set; }

    [SerializeField]
    [Tooltip("Direkt AR Kameranın önünde oluşmasını sağlar.")]
    GameObject m_BallPrefab;

    /// <summary>
    /// Prefabin(pota) dokunmayla yerleştirilmesini sağlar.
    /// </summary>
    public GameObject placedBall
    {
        get { return m_BallPrefab; }
        set { m_BallPrefab = value; }
    }

    /// <summary>
    /// Zemin ile uygun kesişimde spawn olmasını sağlar
    /// </summary>
    public GameObject spawnedBall { get; private set; }

    /// <summary>
    /// Zeminin üzerindeyken çağırılmasını sağlar
    /// </summary>
    public static event Action onPlacedObject;

    private bool isPlaced = false;

    ARRaycastManager m_RaycastManager;

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
    }

    void Update()
    {
    	if(isPlaced)
    		return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if (m_RaycastManager.Raycast(touch.position, s_Hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = s_Hits[0].pose;

                    spawnedHoop = Instantiate(m_HoopPrefab, hitPose.position, Quaternion.AngleAxis(180, Vector3.up));
                    spawnedHoop.transform.parent = transform.parent;

                    isPlaced = true;

                    spawnedBall = Instantiate(m_BallPrefab);
                    spawnedBall.transform.parent = m_RaycastManager.transform.Find("AR Camera").gameObject.transform;

                    if (onPlacedObject != null)
                    {
                        onPlacedObject();
                    }
                }
            }
        }
    }
}
