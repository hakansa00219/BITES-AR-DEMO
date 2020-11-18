using UnityEngine;
using Vuforia;

// This class is used because I added ARCORE library to Vuforia to increase device tracking and ground detection abilities.
// When you add ARCORE library the auto focus script in the Vuforia library will be overwritten by ARCORE library.
// This script will make the mobile device camera to focus again.

public class CameraFocus : MonoBehaviour 
{
    void Start()
    {
        var vuforia = VuforiaARController.Instance; 
        vuforia.RegisterVuforiaStartedCallback(OnVuforiaStarted);
        vuforia.RegisterOnPauseCallback(OnPaused);
    }

    private void OnVuforiaStarted()
    {
        CameraDevice.Instance.SetFocusMode(
            CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
    }

    private void OnPaused(bool paused)
    {
        if (!paused) // Resumed
        {
            // Set again autofocus mode when app is resumed
            CameraDevice.Instance.SetFocusMode(
               CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
        }
    }
}

