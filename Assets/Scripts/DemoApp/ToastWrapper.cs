using DemoApp.Utils;
using UnityEngine;

namespace DemoApp
{
    public class ToastWrapper
    {
        private readonly AndroidJavaObject _androidJavaObject;

        public AndroidJavaObject Inner => _androidJavaObject;

        public ToastWrapper(AndroidJavaObject activity)
        {
            _androidJavaObject = new AndroidJavaObject(
                "com.madwaremade.toaster.ToastWrapper", 
                UnityPlayerActivityProvider.CurrentActivity
            );
        }
        
        public void ShowText(string text, int duration) => _androidJavaObject.Call("showText", text, duration);
    }
}