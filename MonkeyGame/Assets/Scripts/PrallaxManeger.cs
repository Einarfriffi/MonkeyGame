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
        lastCameraPosition = cameraTransform.position;
    }

    void FixedUpdate()
    {
        Vector3 cameraDelta = cameraTransform.position - lastCameraPosition;
        foreach (ParallaxLayer layer in parallaxLayers)
        {
            float moveX = cameraDelta.x * layer.parallaxFactor;
            layer.layerTransform.position += new Vector3(moveX, 0, 0);
        }

        lastCameraPosition = cameraTransform.position;
    }
}
