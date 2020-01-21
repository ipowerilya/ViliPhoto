using UnityEngine;
using System;
using System.Collections;

using Vuforia;

using System.Threading;

using ZXing;
using ZXing.QrCode;
using ZXing.Common;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using System.Linq;


[AddComponentMenu("System/VuforiaScanner")]
public class qrCodeReader : MonoBehaviour
{
    private bool cameraInitialized;

    private BarcodeReader barCodeReader;
    //public Result data;
    public Text txt;
    public string url;
    public string url1;
    IBarcodeReader _barcodeReader = new BarcodeReader();
    void Start()
    {
        barCodeReader = new BarcodeReader();
        //StartCoroutine(InitializeCamera());
        InvokeRepeating("Autofocus", 2f, 2f);
    }

    private bool _isFrameFormatSet;

    

    void Autofocus()
    {
        CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_TRIGGERAUTO);

        RegognizeQR();
    }

    private Vuforia.Image GetCurrFrame()
    {
        return CameraDevice.Instance.GetCameraImage(PIXEL_FORMAT.GRAYSCALE);
            //PIXEL_FORMAT.GRAYSCALE);
    }

    void RegognizeQR()
    {
        if (!_isFrameFormatSet == _isFrameFormatSet)
        {
            _isFrameFormatSet = CameraDevice.Instance.SetFrameFormat(PIXEL_FORMAT.GRAYSCALE, true);
        }

        var currFrame = GetCurrFrame();

        if (currFrame == null)
        {
            Debug.Log("Camera image capture failure;");
        }
        else
        {
            var imgSource = new RGBLuminanceSource(currFrame.Pixels, currFrame.BufferWidth, currFrame.BufferHeight, true);

            var result = _barcodeReader.Decode(imgSource);
            if (result != null)
            {
                Debug.Log("RECOGNIZED: " + result.Text);
                
                txt.text = "https://ides.ru/ya/" + result.Text;
                url1 = txt.text;
                StartCoroutine(downloadAndPlayVideo(url1, "videos.zip", true));
            }
        }
    }
    IEnumerator downloadAndPlayVideo(string videoUrl, string saveFileName, bool overwriteVideo)
    {
        //Where to Save the Video
        string saveDir = Path.Combine(Application.persistentDataPath, saveFileName);

        //Play back Directory
        string playbackDir = saveDir;
#if UNITY_IPHONE
        playbackDir = "file://" + saveDir;
#endif

        bool downloadSuccess = false;
        byte[] vidData = null;

        /*Check if the video file exist before downloading it again. 
         Requires(using System.Linq)
        */
        string[] persistantData = Directory.GetFiles(Application.persistentDataPath);
        if (persistantData.Contains(playbackDir) && !overwriteVideo)
        {
            Debug.Log("Video already exist. Playing it now");
            //Play Video
            txt.text = "loaded";
            //EXIT
            yield break;
        }
        else if (persistantData.Contains(playbackDir) && overwriteVideo)
        {
            Debug.Log("Video already exist [but] we are [Re-downloading] it");
            yield return downloadData(videoUrl, (status, dowloadData) =>
            {
                downloadSuccess = status;
                vidData = dowloadData;
            });
        }
        else
        {
            Debug.Log("Video Does not exist. Downloading video");
            yield return downloadData(videoUrl, (status, dowloadData) =>
            {
                downloadSuccess = status;
                vidData = dowloadData;
            });
        }

        //Save then Play if there was no download error
        if (downloadSuccess)
        {
            //Save Video
            saveVideoFile(saveDir, vidData);

            //Play Video
            txt.text = "loaded";
        }
    }

    //Downloads the Video
    IEnumerator downloadData(string videoUrl, Action<bool, byte[]> result)
    {
        //Download Video
        UnityWebRequest webRequest = UnityWebRequest.Get(videoUrl);
        webRequest.Send();

        //Wait until download is done
        while (!webRequest.isDone)
        {
            Debug.Log("Downloading: " + webRequest.downloadProgress);
            yield return null;
        }


        //Retrieve downloaded Data
        result(true, webRequest.downloadHandler.data);
    }

    //Saves the video
    bool saveVideoFile(string saveDir, byte[] vidData)
    {
        try
        {
            FileStream stream = new FileStream(saveDir, FileMode.Create);
            stream.Write(vidData, 0, vidData.Length);
            stream.Close();
            Debug.Log("Video Downloaded to: " + saveDir.Replace("/", "\\"));
            
            return true;
        }
        catch (Exception e)
        {
            Debug.Log("Error while saving Video File: " + e.Message);
        }
        return false;
    }

    //Plays the video
    
    
    /*
    private IEnumerator InitializeCamera()
    {
        // Waiting a little seem to avoid the Vuforia crashes.
        yield return new WaitForSeconds(1.25f);

        var isFrameFormatSet = CameraDevice.Instance.SetFrameFormat(Vuforia.Image.PIXEL_FORMAT.RGB888, true);
        Debug.Log(String.Format("FormatSet : {0}", isFrameFormatSet));

        // Force autofocus.
        var isAutoFocus = CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
        if (!isAutoFocus)
        {
            CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_NORMAL);
        }
        Debug.Log(String.Format("AutoFocus : {0}", isAutoFocus));
        cameraInitialized = true;
    }

    private void Update()
    {
        if (cameraInitialized)
        {
            try
            {
                var cameraFeed = CameraDevice.Instance.GetCameraImage(Vuforia.Image.PIXEL_FORMAT.RGB888);
                if (cameraFeed == null)
                {
                    return;
                }
                data = barCodeReader.Decode(cameraFeed.Pixels, cameraFeed.BufferWidth, cameraFeed.BufferHeight, RGBLuminanceSource.BitmapFormat.RGB24);
                if (data != null)
                {
                    // QRCode detected.
                    Debug.Log(data.Text);
                    txt.text = "https://ides.ru/ya/" + data.Text;
                    url1 = txt.text;
                    StartCoroutine(downloadAndPlayVideo(url1, "videos.zip", true));
                }
                else
                {
                    Debug.Log("No QR code detected !");
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
    IEnumerator downloadAndPlayVideo(string videoUrl, string saveFileName, bool overwriteVideo)
    {
        //Where to Save the Video
        string saveDir = Path.Combine(Application.persistentDataPath, saveFileName);

        //Play back Directory
        string playbackDir = saveDir;
#if UNITY_IPHONE
        playbackDir = "file://" + saveDir;
#endif

        bool downloadSuccess = false;
        byte[] vidData = null;

        /*Check if the video file exist before downloading it again. 
         Requires(using System.Linq)
        
        string[] persistantData = Directory.GetFiles(Application.persistentDataPath);
        if (persistantData.Contains(playbackDir) && !overwriteVideo)
        {
            Debug.Log("Video already exist. Playing it now");
            //Play Video
            txt.text = "loaded";
            //EXIT
            yield break;
        }
        else if (persistantData.Contains(playbackDir) && overwriteVideo)
        {
            Debug.Log("Video already exist [but] we are [Re-downloading] it");
            yield return downloadData(videoUrl, (status, dowloadData) =>
            {
                downloadSuccess = status;
                vidData = dowloadData;
            });
        }
        else
        {
            Debug.Log("Video Does not exist. Downloading video");
            yield return downloadData(videoUrl, (status, dowloadData) =>
            {
                downloadSuccess = status;
                vidData = dowloadData;
            });
        }

        //Save then Play if there was no download error
        if (downloadSuccess)
        {
            //Save Video
            saveVideoFile(saveDir, vidData);

            //Play Video
            txt.text = "loaded";
        }
    }

    //Downloads the Video
    IEnumerator downloadData(string videoUrl, Action<bool, byte[]> result)
    {
        //Download Video
        UnityWebRequest webRequest = UnityWebRequest.Get(videoUrl);
        webRequest.Send();

        //Wait until download is done
        while (!webRequest.isDone)
        {
            Debug.Log("Downloading: " + webRequest.downloadProgress);
            yield return null;
        }


        //Retrieve downloaded Data
        result(true, webRequest.downloadHandler.data);
    }

    //Saves the video
    bool saveVideoFile(string saveDir, byte[] vidData)
    {
        try
        {
            FileStream stream = new FileStream(saveDir, FileMode.Create);
            stream.Write(vidData, 0, vidData.Length);
            stream.Close();
            Debug.Log("Video Downloaded to: " + saveDir.Replace("/", "\\"));
            
            return true;
        }
        catch (Exception e)
        {
            Debug.Log("Error while saving Video File: " + e.Message);
        }
        return false;
    }

    //Plays the video
    */
}
    





