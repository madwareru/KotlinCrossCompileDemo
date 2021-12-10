using UnityEngine;

namespace DemoApp.Utils
{
    public static class UnityPlayerActivityProvider
    {
        public static AndroidJavaObject CurrentActivity 
        {
            get
            {
                var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                return unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            }
        }
    }
}