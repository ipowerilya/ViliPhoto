using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Vuforia;

public class SimpleCloudHandler : MonoBehaviour, IObjectRecoEventHandler
{
    private CloudRecoBehaviour mCloudRecoBehaviour;

    private bool mIsScanning;

    public string mTargetMetadata = "";

    public GameObject MainPlayer;

    public VideoPlayer videopl;

    public ImageTargetBehaviour ImageTargetTemplate;

    public Text txt;

    public MainRuntimeBehaviour urlget;

    public string url;

    public DefaultTrackableEventHandler scaleget;

    public GameObject img;

    public float newScale1;
    public Text txt2;
  

    private void Start()
    {
        mCloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();
        if ((bool)mCloudRecoBehaviour)
        {
            mCloudRecoBehaviour.RegisterEventHandler(this);
        }
        MainPlayer = GameObject.Find("Player");
        Hide(MainPlayer);
       
    }

    public void OnInitialized(TargetFinder targetFinder)
    {
        UnityEngine.Debug.Log("Cloud Reco initialized");
    }

    public void OnInitError(TargetFinder.InitState initError)
    {
        UnityEngine.Debug.Log("Cloud Reco init error " + initError.ToString());
    }

    public void OnUpdateError(TargetFinder.UpdateState updateError)
    {
        UnityEngine.Debug.Log("Cloud Reco update error " + updateError.ToString());
    }

    public void OnNewSearchResult(TargetFinder.TargetSearchResult targetSearchResult)
    {
        UnityEngine.Debug.Log("found");
        GameObject gameObject = UnityEngine.Object.Instantiate(ImageTargetTemplate.gameObject);
        MainPlayer = gameObject.transform.GetChild(0).gameObject;
        GameObject gameObject2 = null;
        if (gameObject2 != null)
        {
            gameObject2.transform.SetParent(gameObject.transform);
        }
        if ((bool)ImageTargetTemplate)
        {
            ImageTargetBehaviour imageTargetBehaviour = (ImageTargetBehaviour)TrackerManager.Instance.GetTracker<ObjectTracker>().GetTargetFinder<ImageTargetFinder>().EnableTracking(targetSearchResult, gameObject);
        }
        TargetFinder.CloudRecoSearchResult cloudRecoSearchResult = (TargetFinder.CloudRecoSearchResult)targetSearchResult;
        mTargetMetadata = cloudRecoSearchResult.MetaData;
        if (Directory.Exists(Path.Combine(Application.persistentDataPath, "YourUnzippedVideos")))
        {
            url = Path.Combine(Application.persistentDataPath, "YourUnzippedVideos");
            url = Path.Combine(url, mTargetMetadata);
        }
        txt.text = url;
        if (File.Exists(url))
        {
            float rotation = NativeGallery.GetVideoProperties(url).rotation;
            txt.text = rotation.ToString();
            ImageTarget imageTarget = gameObject.GetComponent<ImageTargetBehaviour>().ImageTarget;
            newScale1 = imageTarget.GetSize().x / imageTarget.GetSize().y;

            MainPlayer.transform.localScale = new Vector3(newScale1, 1f, newScale1);

            if (newScale1 > 1f && rotation == 0f)
            {
                float y = 1f / newScale1;
                MainPlayer.transform.localScale = new Vector3(1f, y, 1f);
            }
            if (newScale1 > 1f && rotation == 90f)
            {
                MainPlayer.transform.Rotate(new Vector3(0f, 0f, -90f));
                float y = 1f / newScale1;
                MainPlayer.transform.localScale = new Vector3(y, 1f, 1f);
            }
            if (newScale1 > 1f && rotation == 180f)
            {
                MainPlayer.transform.Rotate(new Vector3(0f, 0f, -180f));
                float y = 1f / newScale1;
                MainPlayer.transform.localScale = new Vector3(1f, y, 1f);
            }
            if (newScale1 > 1f && rotation == 270f)
            {
                MainPlayer.transform.Rotate(new Vector3(0f, 0f, -270f));
                float y = 1f / newScale1;
                MainPlayer.transform.localScale = new Vector3(y, 1f, 1f);
            }
            if (newScale1 > 1f && rotation == 360f)
            {
                MainPlayer.transform.Rotate(new Vector3(0f, 0f, -360f));
                float y = 1f / newScale1;
                MainPlayer.transform.localScale = new Vector3(1f, y, 1f);
            }
            if (newScale1 < 1f)
            {
                MainPlayer.transform.localScale = new Vector3(newScale1, 1f, 1f);
            }
            if (newScale1 < 1f && rotation == 180f)
            {
                MainPlayer.transform.Rotate(new Vector3(0f, 0f, -180f));
                MainPlayer.transform.localScale = new Vector3(newScale1, 1f, 1f);
            }
            if (newScale1 < 1f && rotation == 90f)
            {
                MainPlayer.transform.Rotate(new Vector3(0f, 0f, -90f));
                MainPlayer.transform.localScale = new Vector3(1f, newScale1, 1f);
            }
            if (newScale1 < 1f && rotation == 270f)
            {
                MainPlayer.transform.Rotate(new Vector3(0f, 0f, -270f));
                MainPlayer.transform.localScale = new Vector3(1f, newScale1, 1f);
            }
            if (newScale1 < 1f && rotation == 360f)
            {
                MainPlayer.transform.Rotate(new Vector3(0f, 0f, -360f));
                MainPlayer.transform.localScale = new Vector3(newScale1, 1f, 1f);
            }
            txt2.text = MainPlayer.transform.localScale.ToString();
            MainPlayer.GetComponent<VideoPlayer>().url = url;
        }
        else
        {
            MainPlayer.transform.localScale = new Vector3(0f, 0f, 0f);
        }
       
    }

    public void OnStateChanged(bool scanning)
    {
        mIsScanning = scanning;
        if (scanning)
        {
            TrackerManager.Instance.GetTracker<ObjectTracker>().GetTargetFinder<ImageTargetFinder>().ClearTrackables(destroyGameObjects: false);
        }
    }

    private void Hide(GameObject obj)
    {
        Renderer[] componentsInChildren = obj.GetComponentsInChildren<Renderer>();
        Collider[] componentsInChildren2 = obj.GetComponentsInChildren<Collider>();
        Renderer[] array = componentsInChildren;
        for (int i = 0; i < array.Length; i++)
        {
            array[i].enabled = false;
        }
        Collider[] array2 = componentsInChildren2;
        for (int i = 0; i < array2.Length; i++)
        {
            array2[i].enabled = false;
        }
    }

    private void Update()
    {
    }
}

