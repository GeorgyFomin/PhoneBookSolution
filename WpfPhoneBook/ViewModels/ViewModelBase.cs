using System;
using System.ComponentModel;
using System.Net.Http;

using UseCases.API.Core;

namespace WpfPhoneBook.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Хранит ссылку на обработчик события изменения свойства.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
            {
                return;
            }
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
