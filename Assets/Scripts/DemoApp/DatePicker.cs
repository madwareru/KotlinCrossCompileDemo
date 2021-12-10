using UnityEngine;


namespace DemoApp
{
    public class DatePicker
    {
        private readonly AndroidJavaObject _androidJavaObject;

        public AndroidJavaObject Inner => _androidJavaObject;

        public DatePicker(AndroidJavaObject activity, AndroidJavaProxy listener)
        {
            _androidJavaObject = new AndroidJavaObject("com.madwaremade.toaster.DatePicker", activity, listener);
        }

        public void Show() => _androidJavaObject.Call("show");
    }
}