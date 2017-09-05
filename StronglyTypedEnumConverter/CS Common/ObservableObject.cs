using System.Runtime.CompilerServices;

namespace StronglyTypedEnumConverter
{
    public abstract class ObservableObject : System.ComponentModel.INotifyPropertyChanged
    {
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName]string propertyName = "")
        {
            return DoSetProperty(ref storage, value, propertyName);
        }

        protected void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            DoNotifyPropertyChanged(propertyName);
        }

        private bool DoSetProperty<T>(ref T storage, T value, string propertyName)
        {
            if (Equals(storage, value)) return false;

            storage = value;

            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));

            return true;
        }

        private void DoNotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}
