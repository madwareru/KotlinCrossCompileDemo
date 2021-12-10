using DemoApp.Utils;

using UnityEngine;

namespace DemoApp
{
    public class DatePickerListener: AndroidJavaProxy
    {
        private readonly IDatePickerListener _inner;
        public DatePickerListener(IDatePickerListener inner) : base("com.madwaremade.toaster.IDatePickerListener")
        {
            _inner = inner;
        }
        // ReSharper disable once InconsistentNaming, UnusedMember.Global
        public void onDatePick(int year, int month, int dayOfMonth)
        {
            AsyncManager.ExecuteOnMainThread(() =>
            {
                _inner.OnDatePick(year, month, dayOfMonth);
            });
        }
    }
    
    public interface IDatePickerListener
    {
        void OnDatePick(int year, int month, int dayOfMonth);
    }
}