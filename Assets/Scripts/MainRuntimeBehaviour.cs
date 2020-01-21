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

using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

using Object = UnityEngine.Object;
using Image = Vuforia.Image;
using UnityEditor;
using UnityEngine.SceneManagement;



#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
using NativeGalleryNamespace;
#endif






public class MainRuntimeBehaviour : MonoBehaviour
{
    
    // EXIF orientation: http://sylvana.net/jpegcrop/exif_orientation.html (indices are reordered)
    public enum ImageOrientation { Unknown = -1, Normal = 0, Rotate90 = 1, Rotate180 = 2, Rotate270 = 3, FlipHorizontal = 4, Transpose = 5, FlipVertical = 6, Transverse = 7 };
    private bool cameraInitialized;
    private BarcodeReader barCodeReader;
    public Text txt;
    public string url;
    public string dirName;
    public GameObject ready;
    public GameObject cube;
    bool isLoading = false;
    private bool isDecoding = false;
    public bool isDecoding1 = false;
    public bool checkloading = false;
    public string dirEnd;
    public string WhereFile;
    public bool another;
    





 
    void OnEnable()
    {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }
    void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        
    }
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        cameraInitialized = false;
        Debug.Log("Level Loaded");
        Debug.Log(scene.name);
        Debug.Log(mode);
        barCodeReader = new BarcodeReader();
        StartCoroutine(InitializeCamera());
        ready.gameObject.SetActive(false);
        cube.gameObject.SetActive(true);
        checkloading = false;

    }
   


    private IEnumerator InitializeCamera()
    {
        yield return new WaitForSeconds(1.25f);

        var isFrameFormatSet = CameraDevice.Instance.SetFrameFormat(PIXEL_FORMAT.RGB888, true);
        Debug.Log(String.Format("FormatSet : {0}", isFrameFormatSet));

        //Force autofocus.
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
        
        if (isDecoding1)
        {
            cube.gameObject.transform.Rotate(new Vector3(0f, 0f, -3f));
        }
        if (cameraInitialized)
        {
            try
            {
                var cameraFeed = CameraDevice.Instance.GetCameraImage(PIXEL_FORMAT.RGB888);
                if (cameraFeed == null)
                {
                    return;
                }
                var data = barCodeReader.Decode(cameraFeed.Pixels, cameraFeed.BufferWidth, cameraFeed.BufferHeight, RGBLuminanceSource.BitmapFormat.RGB24);
                if (data != null)
                {
                    
                    // QRCode detected.
                    Debug.Log(data.Text);
                    isDecoding1 = true;
                    if (checkloading == false)
                    {
                        checkloading = true;
                        url = "https://ides.ru/ya/" + data.Text;
                        txt.text = url;
                        StartCoroutine(downloadVideo(url, "videos.zip", true));
                    }
                    
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
    IEnumerator downloadVideo(string videoUrl, string saveFileName, bool overwriteVideo)
    {
        //Where to Save the Video
        string saveDir = Path.Combine(Application.persistentDataPath, saveFileName);

        //Play back Directory
        string playbackDir = saveDir;
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "YourUnzippedVideos")))
        {
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "YourUnzippedVideos"));
        }
        dirEnd = Path.Combine(Application.persistentDataPath, "YourUnzippedVideos");
        //For Iphone
        playbackDir = "file://" + saveDir;


        bool downloadSuccess = false;
        byte[] vidData = null;
        

        
        string[] persistantData = Directory.GetFiles(Application.persistentDataPath);
        if (persistantData.Contains(playbackDir) && !overwriteVideo)
        {
            Debug.Log("Video already exist.");
            //Play Video
            txt.text = "loaded";
            
            ExtractZipFile(playbackDir, dirEnd);
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
           
            ExtractZipFile(playbackDir, dirEnd);

        }
    }

    //Downloads the Video
    IEnumerator downloadData(string videoUrl, Action<bool, byte[]> result)
    {
        //Download Video
        UnityWebRequest webRequest = UnityWebRequest.Get(videoUrl);
        webRequest.SendWebRequest();

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
    public void ExtractZipFile(string archiveFilenameIn, string outFolder)
    {
        ZipFile zf = null;
        try
        {
            FileStream fs = File.OpenRead(archiveFilenameIn);
            zf = new ZipFile(fs);

            foreach (ZipEntry zipEntry in zf)
            {
                if (!zipEntry.IsFile) continue; // Ignore directories

                String entryFileName = zipEntry.Name;

                // to remove the folder from the entry:
                entryFileName = Path.GetFileName(entryFileName);

                byte[] buffer = new byte[4096];     // 4K is optimum
                Stream zipStream = zf.GetInputStream(zipEntry);

                // Manipulate the output filename here as desired.
                String fullZipToPath = Path.Combine(outFolder, entryFileName);
                string directoryName = Path.GetDirectoryName(fullZipToPath);
                if (directoryName.Length > 0)
                    Directory.CreateDirectory(directoryName);

                using (FileStream streamWriter = File.Create(fullZipToPath))
                {
                    StreamUtils.Copy(zipStream, streamWriter, buffer);
                }
            }
        }
        finally
        {
            if (zf != null)
            {
                zf.IsStreamOwner = true;
                zf.Close();
            }
            ready.gameObject.SetActive(true);
            cube.gameObject.SetActive(false);
            isDecoding1 = false;
            checkloading = false;
           
            //ZipUtil.Unzip(playbackDir, dirEnd);
           
        }
    }
    /*
        public void UnZipp(string srcDirPath, string destDirPath)
        {
            ZipInputStream zipIn = null;
            FileStream streamWriter = null;
            txt.text = "start unzipping";

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(destDirPath));

                zipIn = new ZipInputStream(File.OpenRead(srcDirPath));
                txt.text = "zip: " + srcDirPath;
                ZipEntry entry;
                txt.text = "dir created if you see this";
                while ((entry = zipIn.GetNextEntry()) != null)
                {
                    string dirPath = Path.GetDirectoryName(destDirPath + entry.Name);

                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                        dirName = Path.GetDirectoryName(destDirPath + entry.Name);
                        PlayerPrefs.SetString("url", dirName);
                        PlayerPrefs.Save();
                    }

                    if (!entry.IsDirectory)
                    {
                        streamWriter = File.Create(destDirPath + entry.Name);
                        int size = 2048;
                        byte[] buffer = new byte[size];

                        while ((size = zipIn.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            streamWriter.Write(buffer, 0, size);
                        }
                    }

                    streamWriter.Close();
                }
            }
            catch (System.Threading.ThreadAbortException lException)
            {
                // do nothing
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (zipIn != null)
                {
                    zipIn.Close();

                    PlayerPrefs.SetString("url", dirName);
                    PlayerPrefs.Save();


                }

                if (streamWriter != null)
                {
                    streamWriter.Close();

                    PlayerPrefs.SetString("url", dirName);
                    PlayerPrefs.Save();

                }


                ready.gameObject.SetActive(true);
                    cube.gameObject.SetActive(false);
                    isDecoding1 = false;
                    checkloading = false;
                PlayerPrefs.Save();
                ZipUtil.Unzip(playbackDir, dirEnd);
               WhereFile = Directory.GetFiles(Application.persistentDataPath, "lover.mp4", SearchOption.AllDirectories).ToString();
                txt.text = WhereFile;

            }



        }
        */
}




/*
private IEnumerator InitializeCamera()
{
    // Waiting a little seem to avoid the Vuforia's crashes.
    yield return new WaitForSeconds(3f);

    var isFrameFormatSet = CameraDevice.Instance.SetFrameFormat(Vuforia.Image.PIXEL_FORMAT.RGB888, true);
    Debug.Log(String.Format("FormatSet : {0}", isFrameFormatSet));

    // Force autofocus.
    //        var isAutoFocus = CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
    //        if (!isAutoFocus)
    //        {
    //            CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_NORMAL);
    //        }
    //        Debug.Log(String.Format("AutoFocus : {0}", isAutoFocus));

    cameraInitialized = true;
}
*/
/*
private void Update()
{
    if (isDecoding)
    {
        cube.gameObject.transform.Rotate(new Vector3(0f, 0f, -3f));
    }

    if (cameraInitialized)
    {
        try
        {
            var cameraFeed = CameraDevice.Instance.GetCameraImage(Vuforia.Image.PIXEL_FORMAT.RGB888);
            if (cameraFeed == null)
            {
                return;
            }



            var data = barCodeReader.Decode(cameraFeed.Pixels, cameraFeed.BufferWidth, cameraFeed.BufferHeight, RGBLuminanceSource.BitmapFormat.RGB24);
            if (data != null)
            {

                // QRCode detected.
                isDecoding = true;

                Debug.Log(data.Text);
                url = "https://ides.ru/ya/" + data.Text;
                if (isLoading == false)
                {
                    StartCoroutine(downloadVideo(url, "videos.zip", true));
                }
                // our function to call and pass url as text
                data = null;        // clear data
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
*/






