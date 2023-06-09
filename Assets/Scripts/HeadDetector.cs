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
using UnityEngine.Serialization;
using UnityEngine.Windows.WebCam;

[Serializable]
public struct HeadImage : IComparable<HeadImage>
{
    [SerializeField]
    private Texture2D face;

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


    public Texture2D Face
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

    public Vector2 Ratio2_x
    {
        get => new Vector2(1 / Ratio, 1f);
    }

    public Vector2 Ratio2_y
    {
        get => new Vector2(1f, Ratio);
    }

    public HeadImage(Texture2D face, Vector2 earL, Vector2 earR, Vector2 eyeL, Vector2 eyeR, Vector2 nose,
        Vector2 center,
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

    public bool Equals(HeadImage other)
    {
        return score.Equals(other.score);
    }

    public int CompareTo(HeadImage other)
    {
        return score.CompareTo(other.score);
    }

    public override bool Equals(object obj)
    {
        return obj is HeadImage other && Equals(other);
    }

    public override int GetHashCode()
    {
        return score.GetHashCode();
    }

    private sealed class ScoreRelationalComparer : IComparer<HeadImage>
    {
        public int Compare(HeadImage x, HeadImage y)
        {
            return x.score.CompareTo(y.score);
        }
    }

    public static IComparer<HeadImage> ScoreComparer { get; } = new ScoreRelationalComparer();
}

public sealed class HeadDetector : MonoBehaviour
{
    public Texture2D _source = null;

    [SerializeField]
    private HeadImage headImage;

    [SerializeField]
    ResourceSet _resources = null;

    [SerializeField]
    Vector2Int _detectorResolution = new Vector2Int(512, 384);

    [SerializeField]
    Vector2Int _headResolution = new Vector2Int(960, 960);

    private Vector2 _sourceRes => new Vector2Int(_source.width, _source.height);

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
    private Vector2Int cameraResolution;

    public static  float EARTOFACE_RATIO = .9f;
    public static float EYETOFACE_RATIO = 1.7f;
    public static float OVERRIDE_SCALE_POS = 2f;
    public static float OVERRIDE_SCALE_OFFSET = 1.5f;

    string pictureFilePath => Application.persistentDataPath + "/faces/";
    string pictureFileName = "faceCap.png";
    

    [Header("Debug_Calibration")]
    [SerializeField]
    private bool saveOriginalWebCam = true;

    void Start()
    {
        if (useLive)
        {
            StartLiveCameraToTexture();
        }

        // BodyPix detector initialization
        _detector = new BodyDetector(_resources, _detectorResolution.x, _detectorResolution.y);


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


    public void MarkFacePoints(Texture2D image = null, bool resetScoreIfFail = true)
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
        bool analysisFailed = false;
        if (LeftEye.Item2 > ScoreThreshold && RightEye.Item2 > ScoreThreshold)
        {
            analysisFailed = true;

            dir = LeftEye.Item1 - RightEye.Item1;
            scale = dir.magnitude;
            scale *= EYETOFACE_RATIO;
        }
        else
        {
            Debug.LogError($"Face missing eye");
            scale = 1;
            dir = Vector2.right;
        }


        rotation = Vector2.SignedAngle(dir.normalized, Vector2.right);

        scale /= Mathf.Cos(rotation * Mathf.Deg2Rad);

        Debug.Log($"number of valid points: {validPoints.Count}. dir: {dir}");

        //Ratio
        //4:3, 16:9
        float ratio = (float) _source.width / (float) _source.height;
        Debug.Log($"image size: {_source.height} {_source.width}");

        //Score
        float score = Nose.Item2 + LeftEar.Item2 + RightEar.Item2 + LeftEye.Item2 + RightEye.Item2;
        if (scale is >= 1f or <= 0f)
        {
            if (resetScoreIfFail)
            {
                score = -1;
                Debug.LogError($"Face {_source} scaling error");
                scale = Mathf.Clamp(scale, 0, 1);
            }
            else
            {
                score = headImage.Score;
            }
        }

        //CENTRE
        Vector2 centre = new Vector2();
        centre = (LeftEye.Item1+RightEye.Item1+Nose.Item1)/3;
        centre -= new Vector2(0.5f, .5f);
        headImage = new HeadImage(_source, LeftEar.Item1, RightEar.Item1, LeftEye.Item1, RightEye.Item1,
            Nose.Item1, centre, rotation, scale, score, ratio);
        Debug.Log($"Face created: {headImage.Scale} {headImage.Score}");
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

    Vector2 GetHeadPos(Vector2 pos)
    {
        Quaternion rotation = Quaternion.Euler(0, 0, -headImage.Rotation);
        // headImageCenter = rotation * headImageCenter;
        Vector2 headImageScale = pos * (headImage.Scale*OVERRIDE_SCALE_POS);
        headImageScale = rotation * headImageScale;

        Vector2 returnVector = headImageScale + GetOffsetStart();
        // returnVector = rotation * returnVector;
        return returnVector;
    }

    Vector2 GetOffsetStart()
    {
        Quaternion rotation = Quaternion.Euler(0, 0, -headImage.Rotation);
        // Vector2 ratio = new Vector2( headImage.Ratio);
        Vector2 headImageCornerOffset =
            _sourceRes * headImage.Ratio2_x * (headImage.Scale *OVERRIDE_SCALE_OFFSET);
        headImageCornerOffset = rotation * headImageCornerOffset;
        Vector2 headImageCenter =
            (headImage.Center + new Vector2(0.5f, 0.5f)) *
            _sourceRes;
        return headImageCenter - headImageCornerOffset;
    }

    //Camera stuff

    public void StartLiveCameraToTexture()
    {
        try
        {
            liveWebCam = WebCamManager.Texture;

            if (liveWebCam.isPlaying)
            {
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

    // private void InitialiseCamera()
    // {
    //     cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
    //     targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);
    // }

    public Texture TakePicture(bool savePNG, bool processImage = false)
    {
        takePictureLock = true;
        _detectorResolution = new Vector2Int(liveWebCam.width, liveWebCam.height);
        processPhoto = CreatePicture(false, savePNG, processImage);
        // StartCoroutine(TakePictureCoroutine(true, savePNG, processImage));

        //Encode to a PNG

        return processPhoto;
    }

    IEnumerator TakePictureCoroutine(bool setCaptureFlag, bool savePNG, bool processImage = false)
    {
        CreatePicture(setCaptureFlag, savePNG, processImage);

        yield return new WaitForFixedUpdate();
        if (setCaptureFlag)
        {
            captureDoneFlag = true;
        }

        takePictureLock = false;
        // liveWebCam.Stop();
    }

    private Texture2D CreatePicture(bool setCaptureFlag, bool savePNG, bool processImage)
    {
        if (setCaptureFlag) captureDoneFlag = false;

        // yield return new WaitForFixedUpdate();

        processPhoto = new Texture2D(liveWebCam.width, liveWebCam.height);
        processPhoto.SetPixels(liveWebCam.GetPixels());
        processPhoto.Apply();

        if (saveOriginalWebCam) FileLoader.CreatePNG(pictureFilePath, "Original_" + pictureFileName, processPhoto);

        if (processImage)
        {
            MarkFacePoints(processPhoto);

            processPhoto = new Texture2D(_headResolution.x, _headResolution.y);
            var pos = new Vector2();
            var newPos = new Vector2();

            //sample from capture
            for (var i = 0; i < _headResolution.x; i++)
            for (var j = 0; j < _headResolution.y; j++)
            {
                pos = new Vector2(i, j);

                newPos = GetHeadPos(pos);

                // Vector4 c = pos;
                // c.x = c.x/_headResolution.x;
                // c.y = c.y/_headResolution.y;
                // c /= 255f;
                // newPhoto.SetPixel((int) pos.x, (int) pos.y, new Color(c.x,c.y,c.z,0));
                processPhoto.SetPixel((int) pos.x, (int) pos.y,
                    headImage.Face.GetPixel((int) newPos.x, (int) newPos.y));
            }

            processPhoto.Apply();


            Debug.Log("created new head texture");
        }

        if (savePNG)
        {
            FileLoader.CreatePNG(pictureFilePath, pictureFileName, processPhoto);
        }

        return processPhoto;
    }

    HeadImage ConvertHeadImage(Texture2D newHeadImage)
    {
        // headImage.EarL = ConvertPoints(headImage.EarL);
        // headImage.EarR = ConvertPoints(headImage.EarR);
        // headImage.EyeL = ConvertPoints(headImage.EyeL);
        // headImage.EyeR = ConvertPoints(headImage.EyeR);
        // headImage.Nose = ConvertPoints(headImage.Nose);
        headImage.Face = newHeadImage;
        headImage.Center = headImage.Nose;
        headImage.Rotation = 0;
        headImage.Scale = 1;
        headImage.Ratio = _headResolution.x/_headResolution.y;
        // MarkFacePoints(newHeadImage,false);

        return headImage;
    }

    Vector2 ConvertPoints(Vector2 point)
    {
        Quaternion rotation = Quaternion.Euler(0, 0, headImage.Rotation);

        Vector2 headResolution = (headImage.Scale * headImage.Ratio2_x *
                                  _sourceRes);
        Vector2 offsetStart = GetOffsetStart();
        Vector2 localPointPosition = (rotation*point * _sourceRes) - offsetStart;
        return localPointPosition / headResolution;
    }

    public void CaptureFace_Calibration()
    {
        if (takePictureLock)
        {
            return;
        }

        TakePicture(false, true);
        // UpdateRenderer();
        // StartCoroutine(CaptureFaceCoroutine(processPhoto, true));
    }

    public void CaptureFace_SavePNG()
    {
        if (takePictureLock)
        {
            return;
        }

        TakePicture(true);
        Texture2D t = FileLoader.LoadTextureFromImage(new Texture2D(cameraResolution.x, cameraResolution.y),
            pictureFilePath,
            pictureFileName);
        // StartCoroutine(CaptureFaceCoroutine(processPhoto, true));
    }

    public HeadImage TakePlayerPicture_HeadImage()
    {
        TakePicture(false || saveOriginalWebCam, true);
        // StartCoroutine(CaptureFaceCoroutine(processPhoto, false));
        // MarkFacePoints(processPhoto);
        // headImage.Face = processPhoto;
        Debug.Log("Re-analysis photo");
        // MarkFacePoints(processPhoto,false);
        ConvertHeadImage(processPhoto);

        return headImage;
    }

    // IEnumerator CaptureFaceCoroutine(Texture2D t, bool setCaptureFlag, bool updateRenderer = true)
    // {
    //     
    //     // if (setCaptureFlag)
    //     // {
    //     //     yield return new WaitUntil(() => captureDoneFlag);
    //     // }
    //     //
    //     // MarkFacePoints(t);
    //     // if (updateRenderer)
    //     // {
    //     //     UpdateRenderer();
    //     // }
    // }

    // public void RenderFace()
    // {
    //     UpdateRenderer();
    // }
}