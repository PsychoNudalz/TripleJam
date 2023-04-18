using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UI = UnityEngine.UI;
using Klak.TestTools;
using BodyPix;
using Unity.VisualScripting;
using UnityEngine.Windows.WebCam;

[Serializable]
struct HeadImage
{
    [SerializeField]
    private Texture face;

    [SerializeField]
    private Vector2 ear_l;

    [SerializeField]
    private Vector2 ear_r;

    [SerializeField]
    private Vector2 eye_l;

    [SerializeField]
    private Vector2 eye_r;

    [SerializeField]
    private Vector2 nose;

    [SerializeField]
    private Vector2 center;

    [SerializeField]
    private float rotation;

    [SerializeField]
    private float scale;

    [SerializeField]
    private float score;

    [SerializeField]
    private float ratio;


    public Texture Face
    {
        get => face;
        set => face = value;
    }

    public Vector2 EarL
    {
        get => ear_l;
        set => ear_l = value;
    }

    public Vector2 EarR
    {
        get => ear_r;
        set => ear_r = value;
    }

    public Vector2 EyeL
    {
        get => eye_l;
        set => eye_l = value;
    }

    public Vector2 EyeR
    {
        get => eye_r;
        set => eye_r = value;
    }

    public Vector2 Nose
    {
        get => nose;
        set => nose = value;
    }

    public Vector2 Center
    {
        get => center;
        set => center = value;
    }

    public float Rotation
    {
        get => rotation;
        set => rotation = value;
    }

    public float Scale
    {
        get => scale;
        set => scale = value;
    }

    public float Score
    {
        get => score;
        set => score = value;
    }


    public float Ratio
    {
        get => ratio;
        set => ratio = value;
    }

    public HeadImage(Texture face, Vector2 earL, Vector2 earR, Vector2 eyeL, Vector2 eyeR, Vector2 nose, Vector2 center,
        float rotation, float scale, float score, float ratio)
    {
        this.face = face;
        ear_l = earL;
        ear_r = earR;
        eye_l = eyeL;
        eye_r = eyeR;
        this.nose = nose;
        this.center = center;
        this.rotation = rotation;
        this.scale = scale;
        this.score = score;
        this.ratio = ratio;
    }
}

public sealed class HeadDetector : MonoBehaviour
{
    public Texture _source = null;

    [SerializeField]
    private HeadImage headImage;

    [SerializeField]
    ResourceSet _resources = null;

    [SerializeField]
    Vector2Int _resolution = new Vector2Int(512, 384);

    [SerializeField]
    private Vector2 inWorldResolution = new Vector2(1, 1);

    // [SerializeField] UI.RawImage _previewUI = null;
    [Header("Prefabs")]
    [SerializeField]
    GameObject _markerPrefab = null;

    [SerializeField]
    private Renderer textureDisplay;

    const float ScoreThreshold = 0.3f;

    BodyDetector _detector;

    (Transform xform, UI.Text label)[]
        _markers = new (Transform, UI.Text) [Body.KeypointCount];

    private bool setMarkerFlag = false;

    [Header("Camera Capture")]
    [SerializeField]
    private bool useLive = false;

    PhotoCapture photoCaptureObject = null;

    private WebCamTexture liveWebCam;
    private Texture2D processPhoto;
    private bool captureDoneFlag = false;
    private bool takePictureLock = false;
    bool webCamOn = false;

    Texture2D targetTexture = null;
    private Resolution cameraResolution;

    private float EARTOFACE_RATIO = 1.4f;
    private float EYETOFACE_RATIO = 1.6f;
    string pictureFilePath => Application.persistentDataPath + "/faces/";
    string pictureFileName = "faceCap.png";


    void Start()
    {
        // BodyPix detector initialization
        _detector = new BodyDetector(_resources, _resolution.x, _resolution.y);

        if (useLive)
        {
            StartLiveCameraToTexture();
        }


        // // Texture
        //
        // // Marker population
        // for (var i = 0; i < Body.KeypointCount; i++)
        // {
        //     var marker = Instantiate(_markerPrefab, transform);
        //     marker.name = ((Body.KeypointID) i).ToString();
        //
        //     _markers[i] = (marker.transform, marker.GetComponentInChildren<UI.Text>());
        // }
    }

    void OnDestroy()
        => _detector.Dispose();

    void FixedUpdate()
    {
        // if (!setMarkerFlag)
        // {
        //     SetMarkers();
        //     setMarkerFlag = true;
        // }
        // TakePicture();

        // if (!setMarkerFlag)
        // {
        //     MarkFacePoints();
        //     UpdateRenderer();
        //     setMarkerFlag = true;
        // }

        // if (liveWebCam)
        // {
        //     MarkFacePoints();
        //     UpdateRenderer();
        // }
    }
    //
    // private void SetMarkers()
    // {
    //     // BodyPix detector update
    //     _detector.ProcessImage(_source);
    //     // _previewUI.texture = _source;
    //
    //     // Marker update
    //     var rectSize = inWorldResolution;
    //     Vector2 offset = -inWorldResolution / 2f;
    //     for (var i = 0; i < Body.KeypointCount; i++)
    //     {
    //         var key = _detector.Keypoints[i];
    //         var (marker, label) = _markers[i];
    //
    //         // Visibility
    //         var visible = key.Score > ScoreThreshold;
    //         marker.gameObject.SetActive(visible);
    //         if (!visible) continue;
    //
    //         // Position and label
    //         marker.localPosition = key.Position * rectSize + offset;
    //         Debug.Log($"{marker.ToString()} Position: {key.Position},");
    //     }
    // }


    public void MarkFacePoints(Texture image = null)
    {
        if (image)
        {
            _source = image;
        }

        _detector.ProcessImage(_source);


        List<Vector2> validPoints = new List<Vector2>();

        (Vector2, float) Nose = GetPoint(Body.KeypointID.Nose);
        if (Nose.Item2 > ScoreThreshold)
        {
            validPoints.Add(Nose.Item1);
        }

        (Vector2, float) LeftEar = GetPoint(Body.KeypointID.LeftEar);
        if (LeftEar.Item2 > ScoreThreshold)
        {
            validPoints.Add(LeftEar.Item1);
        }

        (Vector2, float) RightEar = GetPoint(Body.KeypointID.RightEar);
        if (RightEar.Item2 > ScoreThreshold)
        {
            validPoints.Add(RightEar.Item1);
        }

        (Vector2, float) LeftEye = GetPoint(Body.KeypointID.LeftEye);
        if (LeftEye.Item2 > ScoreThreshold)
        {
            validPoints.Add(LeftEye.Item1);
        }

        (Vector2, float) RightEye = GetPoint(Body.KeypointID.RightEye);
        if (RightEye.Item2 > ScoreThreshold)
        {
            validPoints.Add(RightEye.Item1);
        }

        float scale = 1;
        float rotation = 0;
        Vector2 dir;
        if (LeftEar.Item2 > ScoreThreshold && RightEar.Item2 > ScoreThreshold)
        {
            dir = LeftEar.Item1 - RightEar.Item1;
            scale = dir.magnitude;
            scale *= EARTOFACE_RATIO;
        }
        else
        {
            dir = LeftEye.Item1 - RightEye.Item1;
            scale = dir.magnitude;
            scale *= EYETOFACE_RATIO;
        }

        rotation = Vector2.SignedAngle(dir.normalized, Vector2.right);

        scale /= Mathf.Cos(rotation * Mathf.Deg2Rad);

        Debug.Log($"number of valid points: {validPoints.Count}. dir: {dir}");

        //Ratio
        float ratio = (float) _source.height / (float) _source.width;
        Debug.Log($"image size: {_source.height} {_source.width}");

        //Score
        float score = Nose.Item2 + LeftEar.Item2 + RightEar.Item2 + LeftEye.Item2 + RightEye.Item2;
        if (scale is >= 1f or <= 0f)
        {
            score = -1;
            Debug.LogError($"Face {_source} scaling error");
            scale = Mathf.Clamp(scale, 0, 1);
        }
        
        //CENTRE
        Vector2 centre = new Vector2();
        centre = Nose.Item1;
        centre -= new Vector2(0.5f, .5f);
        headImage = new HeadImage(_source, LeftEar.Item1, RightEar.Item1, LeftEye.Item1, RightEye.Item1,
            Nose.Item1, centre, rotation, scale, score, ratio);
    }

    public void UpdateRenderer()
    {
        textureDisplay.material.mainTexture = headImage.Face;
        textureDisplay.material.SetFloat("_Scale", headImage.Scale);
        textureDisplay.material.SetFloat("_Rotation", headImage.Rotation);
        textureDisplay.material.SetVector("_Center", headImage.Center);
        textureDisplay.material.SetVector("_Ratio", new Vector2(1, headImage.Ratio));
    }

    (Vector2, float) GetPoint(Body.KeypointID keypoint)
    {
        Keypoint detectorKeypoint = _detector.Keypoints[(int) keypoint];
        return (detectorKeypoint.Position, detectorKeypoint.Score);
    }


    //Camera stuff

    public void StartLiveCameraToTexture()
    {
        liveWebCam = new WebCamTexture();
        try
        {
            if (liveWebCam.isReadable)
            {
                liveWebCam.Play();
                _source = liveWebCam;
                webCamOn = true;
            }
            else
            {
                webCamOn = false;
            }
        }
        catch (Exception e)
        {
            webCamOn = false;
        }
    }

    private void OnApplicationQuit()
    {
        if (useLive && liveWebCam)
        {
            liveWebCam.Stop();
        }
    }

    private void InitialiseCamera()
    {
        cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);
    }

    public Texture TakePicture(bool savePNG)
    {
        takePictureLock = true;
        _resolution = new Vector2Int(liveWebCam.width, liveWebCam.height);

        StartCoroutine(TakePictureCoroutine(true, savePNG));

        //Encode to a PNG

        return processPhoto;
    }

    IEnumerator TakePictureCoroutine(bool setCaptureFlag, bool savePNG)
    {
        if (setCaptureFlag)
        {
            captureDoneFlag = false;
        }
        // if (liveWebCam && liveWebCam.isPlaying)
        // {
        //     liveWebCam.Stop();
        // }


        yield return new WaitForFixedUpdate();
        processPhoto = new Texture2D(liveWebCam.width, liveWebCam.height);
        processPhoto.SetPixels(liveWebCam.GetPixels());
        processPhoto.Apply();

        if (savePNG)
        {
            FileLoader.CreatePNG(pictureFilePath, pictureFileName, processPhoto);
        }

        yield return new WaitForFixedUpdate();
        if (setCaptureFlag)
        {
            captureDoneFlag = true;
        }

        takePictureLock = false;
    }

    public void CaptureFace()
    {
        if (takePictureLock)
        {
            return;
        }

        TakePicture(false);
        StartCoroutine(CaptureFaceCoroutine(processPhoto, true));
    }

    public void CaptureFace_SavePNG()
    {
        if (takePictureLock)
        {
            return;
        }

        TakePicture(true);
        Texture2D t = FileLoader.LoadTextureFromImage(new Texture2D(_resolution.x, _resolution.y), pictureFilePath,
            pictureFileName);
        StartCoroutine(CaptureFaceCoroutine(processPhoto, true));
    }

    IEnumerator CaptureFaceCoroutine(Texture t, bool setCaptureFlag)
    {
        if (setCaptureFlag)
        {
            yield return new WaitUntil(() => captureDoneFlag);
        }

        MarkFacePoints(t);
        UpdateRenderer();
    }

    public void RenderFace()
    {
        UpdateRenderer();
    }
}