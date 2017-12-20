using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DesktopProjectDebug
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName]string name = null)
        {
            Assumes.Assert(!string.IsNullOrEmpty(name), nameof(name));

            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        /// <summary>
        /// Raises a property change event for the given <paramref name="name"/> only if the <paramref name="currentValue"/>
        /// is different from the <paramref name="newValue"/>. Object equality is used to determine whether the
        /// two values are equal. <paramref name="currentValue"/> is unconditionally set to <paramref name="newValue"/>.
        /// </summary>
        /// <typeparam name="T">The type for the given values.</typeparam>
        /// <param name="currentValue">The current value of the property.</param>
        /// <param name="newValue">The new value of the property.</param>
        /// <param name="name">The property name.</param>
        protected void ChangePropertyValue<T>(ref T currentValue, T newValue, [CallerMemberName]string name = null)
        {
            Assumes.Assert(!string.IsNullOrEmpty(name), nameof(name));

            if (!object.Equals(currentValue, newValue))
            {
                currentValue = newValue;
                OnPropertyChanged(name);
            }
        }
    }
}
