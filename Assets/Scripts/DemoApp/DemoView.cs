using System;
using DemoApp.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace DemoApp
{
    public class DemoView : MonoBehaviour, IDatePickerListener
    {
        [SerializeField] private Button _calendarButton;
        [SerializeField] private int _toastDuration = 10;
        
        private ToastWrapper _toastWrapper;
        private DatePicker _datePicker;
        private DatePickerListener _datePickerListener;

        private void Awake()
        {
            var currentActivity = UnityPlayerActivityProvider.CurrentActivity;
            _toastWrapper = new ToastWrapper(currentActivity);
            _datePickerListener = new DatePickerListener(this);
            _datePicker = new DatePicker(currentActivity, _datePickerListener);
            
            _calendarButton.onClick.AddListener(ShowDatePicker);
        }

        private void ShowDatePicker()
        {
            _datePicker.Show();
        }

        private void OnDestroy()
        {
            _calendarButton.onClick.RemoveListener(ShowDatePicker);
        }

        public void OnDatePick(int year, int month, int dayOfMonth)
        {
            _toastWrapper.ShowText(
                $"Received a date: {year.ToString()}-{month.ToString()}-{dayOfMonth.ToString()}", 
                _toastDuration
            );
        }
    }
}