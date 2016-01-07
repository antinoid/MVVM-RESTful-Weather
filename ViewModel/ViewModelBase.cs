// C# 5.0 or higher for CallerMemberName
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ViewModel
{
    /// <summary>
    /// Base view model class that implements INPC, OnPropertyChanged and SetValue
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Sets a new value for a property and notifies about the change.
        /// </summary>
        /// <typeparam name="T">The property type.</typeparam>
        /// <param name="property">Reference to the field</param>
        /// <param name="value">The value to set</param>
        /// <param name="propertyName">The name of the property that has a new value.</param>
        /// <remarks>
        /// The order of actions is defined as the following:
        /// <list type="number">
        ///   <item>Change property value</item>
        ///   <item>Call OnPropertyChanged method</item>
        ///   <item>Raise <see cref="PropertyChanged"/> event for the property</item>
        /// </list>
        /// </remarks>
        protected void SetValue<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (property != null)
            {
                if (property.Equals(value)) { return; }
            }
            property = value;
            OnPropertyChanged(propertyName);
        }

        /// <summary>
        /// Raises this object's PropertyChanged event for one property and all its dependent
        /// properties.
        /// </summary>
        /// <param name="propertyName">The name of the property that has a new value.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
