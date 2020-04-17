using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PointOfInterestMarker : MonoBehaviour
{

    public Transform TargetMarker;

    private RectTransform _canvas;
    private UnityEngine.UI.Image _image;
    public Sprite ArrowSprite;
    public Sprite OnScreenSprite;

    public Transform PlayerPosition;
    public Text DistanceText;

    public Rect BoundaryRect;

    public float FadeTime = 0.5f;

    public enum SpriteTypes
    {
        OnScreen,
        OffscreenLeft,
        OffscreenRight,
        OffscreenUp,
        OffscreenDown
    }

    private SpriteTypes _prevSpriteType;

    public void SetSprite(SpriteTypes sprite)
    {
        if (sprite == _prevSpriteType) return;

        _prevSpriteType = sprite;

        if (sprite == SpriteTypes.OnScreen)
        {
            _image.transform.localRotation = Quaternion.identity;
            _image.sprite = OnScreenSprite;
            _image.GetComponent<RectTransform>().sizeDelta = _image.sprite.rect.size;
            
            return;
        }

        _image.sprite = ArrowSprite;
        _image.GetComponent<RectTransform>().sizeDelta = _image.sprite.rect.size;

        switch (sprite)
        {
            case SpriteTypes.OffscreenLeft:
            {
                _image.transform.localRotation = Quaternion.AngleAxis(180, Vector3.forward);
                break;
            }
            case SpriteTypes.OffscreenRight:
            {
                _image.transform.localRotation = Quaternion.identity;
                break;
            }
            case SpriteTypes.OffscreenUp:
            {
                _image.transform.localRotation = Quaternion.AngleAxis(90, Vector3.forward);
                break;
            }
            case SpriteTypes.OffscreenDown:
            {
                _image.transform.localRotation = Quaternion.AngleAxis(270, Vector3.forward);
                break;
            }
        }
    }

    public void OnEnable()
    {
        _canvas = transform.root.GetComponent<RectTransform>();
        _image = GetComponent<UnityEngine.UI.Image>();
        _distanceBuilder = new StringBuilder(7, 10);
        _alpha = 0;
        _image.color = new Color(1, 1, 1, 0);
        DistanceText.color = new Color(1, 1, 1, 0);
    }

    private StringBuilder _distanceBuilder;

    private void BuildDistanceStringNoAlloc(float distance)
    {
        _distanceBuilder.Length = 0;
        
        var wholeVal = (int) distance;
        var fracVal = distance - wholeVal;
        do
        {
            int digit = wholeVal%10;
            _distanceBuilder.Insert(0, (char)('0' + digit));
            wholeVal = (wholeVal - digit)/10;
        } while (wholeVal > 0);

        _distanceBuilder.Append('.');

        var fracDigit = (int) (fracVal*10);
        _distanceBuilder.Append((char)('0' + fracDigit));
        _distanceBuilder.Append('m');
    }

    private float GetCameraHorizontalFOV(Camera c)
    {
        var cameraHeightAt1 = Mathf.Tan(c.fieldOfView * Mathf.Deg2Rad * 0.5f);
        var hFOVrad = Mathf.Atan(cameraHeightAt1 * c.aspect) * 2;
        return hFOVrad * Mathf.Rad2Deg;
    }

    public void LateUpdate()
    {
        if (!TargetMarker) return;
        if (!PlayerPosition)
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj) PlayerPosition = playerObj.transform;
        }
        if (!PlayerPosition)
        {
            SetAlpha(0);
            return;
        }

        var cam = Camera.main;

        var vpPoint = cam.WorldToViewportPoint(TargetMarker.position);

        var flatCamDir = cam.transform.forward;
        flatCamDir.y = 0;
        flatCamDir.Normalize();

        var flatTargetDir = TargetMarker.transform.position - cam.transform.position;
        flatTargetDir.y = 0;
        flatTargetDir.Normalize();

        var bearing = Vector3.Angle(flatCamDir, flatTargetDir)*Mathf.Sign(Vector3.Cross(flatCamDir, flatTargetDir).y);

        // We want a forwardness curve which is:
        // 1 within the camera's horizontal FOV
        // 1-0 tapering off smoothly outside the FOV
        // 0 by the time you get to 90 degrees
        var cameraHorizontalFOV = GetCameraHorizontalFOV(cam);
        var forwardness = 1f - Mathf.Clamp01(((Mathf.Abs(bearing) - (cameraHorizontalFOV/2)) / (cameraHorizontalFOV/4)));
        vpPoint.y = Mathf.SmoothStep(0.5f, vpPoint.y, forwardness);

        Vector2 clampedVPPoint;
        if (vpPoint.z < 0f)
        {
            // objective is behind the camera
            SetSprite(bearing > 0 ? SpriteTypes.OffscreenRight : SpriteTypes.OffscreenLeft);
            clampedVPPoint = new Vector2(bearing > 0 ? BoundaryRect.xMax : BoundaryRect.xMin, Mathf.Clamp(vpPoint.y, BoundaryRect.yMin, BoundaryRect.yMax));
        }
        else
        {
            clampedVPPoint = new Vector2(Mathf.Clamp(vpPoint.x, BoundaryRect.xMin, BoundaryRect.xMax),
                Mathf.Clamp(vpPoint.y, BoundaryRect.yMin, BoundaryRect.yMax));

            if (clampedVPPoint.x > vpPoint.x)
            {
                SetSprite(SpriteTypes.OffscreenLeft);
            }
            else if (clampedVPPoint.x < vpPoint.x)
            {
                SetSprite(SpriteTypes.OffscreenRight);
            }
            else if (clampedVPPoint.y > vpPoint.y)
            {
                SetSprite(SpriteTypes.OffscreenDown);
            }
            else if (clampedVPPoint.y < vpPoint.y)
            {
                SetSprite(SpriteTypes.OffscreenUp);
            }
            else
            {
                SetSprite(SpriteTypes.OnScreen);
            }
        }
        _image.transform.localPosition = new Vector3((clampedVPPoint.x - 0.5f) * _canvas.sizeDelta.x, (clampedVPPoint.y - 0.5f) * _canvas.sizeDelta.y, 0);

        SetAlpha(_alpha);

        if (_image.sprite == OnScreenSprite)
        {
            var distanceToPlayer = Vector3.Distance(PlayerPosition.position, TargetMarker.position);
            
            DistanceText.enabled = true;

            BuildDistanceStringNoAlloc(distanceToPlayer);
            DistanceText.text = _distanceBuilder.ToString();
        }
        else
        {
            DistanceText.enabled = false;
        }
    }

    private void SetAlpha(float alpha)
    {
        _image.color = new Color(1, 1, 1, alpha);
        DistanceText.color = new Color(1, 1, 1, alpha);
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeAlpha(0, 1, FadeTime, false));
    }

    public void Deactivate()
    {
        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeAlpha(1, 0, FadeTime, true));
    }

    private float _alpha;
    private Coroutine _fadeCoroutine;

    private IEnumerator FadeAlpha(float from, float to, float overTime, bool deactivateWhenFinished)
    {
        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < startTime + overTime)
        {
            _alpha = Mathf.Lerp(from, to, Mathf.Clamp01((Time.realtimeSinceStartup - startTime)/overTime));
            yield return null;
        }
        _alpha = to;
        if (deactivateWhenFinished)
            gameObject.SetActive(false);
    }
}
