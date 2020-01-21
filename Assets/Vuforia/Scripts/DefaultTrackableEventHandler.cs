/*==============================================================================
Copyright (c) 2017 PTC Inc. All Rights Reserved.

Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.
==============================================================================*/

using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using Vuforia;

public class DefaultTrackableEventHandler : MonoBehaviour, ITrackableEventHandler
{
    public VideoPlayer videopl;

    public GameObject Mainpl;

    public GameObject cubeUI;

    public Vector2 trackedCloudImageWH;

    public float scale;

    protected TrackableBehaviour mTrackableBehaviour;

    protected TrackableBehaviour.Status m_PreviousStatus;

    protected TrackableBehaviour.Status m_NewStatus;

    protected virtual void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if ((bool)mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
    }

    protected virtual void OnDestroy()
    {
        if ((bool)mTrackableBehaviour)
        {
            mTrackableBehaviour.UnregisterTrackableEventHandler(this);
        }
    }

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        m_PreviousStatus = previousStatus;
        m_NewStatus = newStatus;
        if (newStatus == TrackableBehaviour.Status.DETECTED || newStatus == TrackableBehaviour.Status.TRACKED || newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            UnityEngine.Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
            OnTrackingFound();
        }
        else if (previousStatus == TrackableBehaviour.Status.TRACKED && newStatus == TrackableBehaviour.Status.NO_POSE)
        {
            UnityEngine.Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
            OnTrackingLost();
        }
        else
        {
            OnTrackingLost();
        }
    }

    protected virtual void OnTrackingFound()
    {
        ImageTarget imageTarget = GetComponent<ImageTargetBehaviour>().ImageTarget;
        UnityEngine.Debug.Log(imageTarget.GetSize());
        UnityEngine.Debug.Log(imageTarget.GetSize().x);
        UnityEngine.Debug.Log(imageTarget.GetSize().y);
        UnityEngine.Debug.Log(imageTarget.GetSize().z);
        scale = imageTarget.GetSize().x / imageTarget.GetSize().y;
        UnityEngine.Debug.Log(scale);
        StartCoroutine(UpdateCoroutine());
    }

    private IEnumerator UpdateCoroutine()
    {
        UnityEngine.Debug.Log(PlayerPrefs.GetFloat("rotation"));
        videopl.Prepare();
        cubeUI.gameObject.SetActive(value: false);
        UnityEngine.Debug.Log("wait is over");
        Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(includeInactive: true);
        Collider[] colliderComponents = GetComponentsInChildren<Collider>(includeInactive: true);
        Canvas[] canvasComponents = GetComponentsInChildren<Canvas>(includeInactive: true);
        yield return new WaitForSeconds(1.7f);
        Renderer[] array = rendererComponents;
        for (int i = 0; i < array.Length; i++)
        {
            array[i].enabled = true;
        }
        Collider[] array2 = colliderComponents;
        for (int j = 0; j < array2.Length; j++)
        {
            array2[j].enabled = true;
        }
        Canvas[] array3 = canvasComponents;
        for (int k = 0; k < array3.Length; k++)
        {
            array3[k].enabled = true;
        }
        videopl.Play();
    }

    protected virtual void OnTrackingLost()
    {
        videopl.Stop();
        cubeUI.gameObject.SetActive(value: true);
        Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>(includeInactive: true);
        Collider[] componentsInChildren2 = GetComponentsInChildren<Collider>(includeInactive: true);
        Canvas[] componentsInChildren3 = GetComponentsInChildren<Canvas>(includeInactive: true);
        Renderer[] array = componentsInChildren;
        for (int i = 0; i < array.Length; i++)
        {
            array[i].enabled = false;
        }
        Collider[] array2 = componentsInChildren2;
        for (int j = 0; j < array2.Length; j++)
        {
            array2[j].enabled = false;
        }
        Canvas[] array3 = componentsInChildren3;
        for (int k = 0; k < array3.Length; k++)
        {
            array3[k].enabled = false;
        }
    }
}

