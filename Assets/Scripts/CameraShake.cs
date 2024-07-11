using UnityEngine;

public class CameraShake : MonoBehaviour
{

    [SerializeField] Transform camTransform;

    // How long the object should shake for.
    [SerializeField] float shakeDuration = 0f;

    // Amplitude of the shake. A larger value shakes the camera harder.
    [SerializeField] float shakeAmount = 0.7f;
    [SerializeField] float decreaseFactor = 1.0f;

    private bool shaketrue = false;

    Vector3 originalPos;
    float originalShakeDuration; //<--add this

    void Awake()
    {
        if (camTransform == null)
        {
            camTransform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    void OnEnable()
    {
        originalPos = camTransform.localPosition;
        originalShakeDuration = shakeDuration; //<--add this
    }

    void Update()
    {
        if (shaketrue)
        {
            if (shakeDuration > 0)
            {
                camTransform.localPosition = Vector3.Lerp(camTransform.localPosition, originalPos + Random.insideUnitSphere * shakeAmount, Time.deltaTime * 3);

                shakeDuration -= Time.deltaTime * decreaseFactor;
            }
            else
            {
                shakeDuration = originalShakeDuration; //<--add this
                camTransform.localPosition = originalPos;
                shaketrue = false;
            }
        }
    }

    public void ShakeCamera()
    {
        shaketrue = true;
    }
}
