using MedicationMngApp.Models;
using MedicationMngApp.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace MedicationMngApp.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public IServiceItem<Item> DataItem => DependencyService.Get<IServiceItem<Item>>();

        string errormessage = string.Empty;
        bool isBusy = false;
        bool errorvisibility = false;

        public string ErrorMessage
        {
            get { return errormessage; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    ErrorVisibility = true;
                else
                    ErrorVisibility = false;

                SetProperty(ref errormessage, value);
            }
        }

        public bool ErrorVisibility
        {
            get { return errorvisibility; }
            set { SetProperty(ref errorvisibility, value); }
        }

        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        public bool CanSubmit
        {
            get { return !isBusy; }
        }

        string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
