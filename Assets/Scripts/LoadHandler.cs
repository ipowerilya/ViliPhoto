using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

public class LoadHandler : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        string url = "https://ides.ru/ya/https://yadi.sk/d/46EUKMXJ2iDAXQ";


        StartCoroutine(downloadAndPlayVideo(url, "videos.zip", true));
    }
    //Downloads, Saves and Plays the Video
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
            playVideo(playbackDir);
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
            playVideo(playbackDir);
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

        Debug.Log("Video Downloaded");
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
    void playVideo(string path)
    {
        Handheld.PlayFullScreenMovie(path, Color.black,
            FullScreenMovieControlMode.Full, FullScreenMovieScalingMode.AspectFill);
    }
   
 

}
