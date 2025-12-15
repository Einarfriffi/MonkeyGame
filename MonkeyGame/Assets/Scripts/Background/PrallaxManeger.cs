using UnityEngine;

public class PrallaxManeger : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform layerTransform;
        [Range(0f, 1f)] public float parallaxFactor;
    }

    public ParallaxLayer[] parallaxLayers;
    public Transform cameraTransform;

    private Vector3 lastCameraPosition;

    void Start()
    {
        if (!cameraTransform) cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
    }

    void LateUpdate()
    {
        Vector3 cameraDelta = cameraTransform.position - lastCameraPosition;

        foreach (var layer in parallaxLayers)
        {
            layer.layerTransform.position += new Vector3(cameraDelta.x * layer.parallaxFactor, 0f, 0f);
        }

        lastCameraPosition = cameraTransform.position;
    }
}
