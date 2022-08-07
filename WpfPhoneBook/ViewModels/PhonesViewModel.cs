using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UseCases.API.Authentication;
using UseCases.API.Core;
using UseCases.API.Dto;
using WebApiJwt.Data;
using WpfPhoneBook.Commands;

namespace WpfPhoneBook.ViewModels
{
    internal class PhonesViewModel : ViewModelBase
    {
        #region Fields
        /// <summary>
        /// Хранит ссылку на коллекцию объектов модели.
        /// </summary>
        private ObservableCollection<PhoneDto> phones = new();
        /// <summary>
        /// Хранит ссылку на текущий выделенный объект модели.
        /// </summary>
        private PhoneDto? selectedPhone;
        private RelayCommand? phoneRemoveCommand;
        private RelayCommand? phoneSelectionCommand;
        private RelayCommand? phoneRowEditEndCommand;
        private RelayCommand? phoneBeginningEditCommand;
        private bool canAdd;
        private bool canRemove;
        private bool isReadOnly = true;
        #endregion
        #region Properties
        public bool CanAdd { get => canAdd; set { canAdd = value; RaisePropertyChanged(nameof(CanAdd)); } }
        public bool CanRemove { get => canRemove; set { canRemove = value; RaisePropertyChanged(nameof(CanRemove)); } }
        public bool IsReadOnly { get => isReadOnly; set { isReadOnly = value; RaisePropertyChanged(nameof(IsReadOnly)); } }
        /// <summary>
        /// Устанавливает и возвращает коллекцию объектов модели.
        /// </summary>
        public ObservableCollection<PhoneDto> Phones { get => phones; set { phones = value; RaisePropertyChanged(nameof(Phones)); } }
        public ICommand PhoneRemoveCommand => phoneRemoveCommand ??= new RelayCommand(PhoneRemove);
        public ICommand PhoneSelectionCommand => phoneSelectionCommand ??= new RelayCommand(PhoneSelection);
        public ICommand PhoneRowEditEndCommand => phoneRowEditEndCommand ??= new RelayCommand(PhoneRowEditEnd);
        public ICommand PhoneBeginningEditCommand => phoneBeginningEditCommand ??= new RelayCommand(PhoneBeginningEdit);
        #endregion
        #region Methods
        public PhonesViewModel()
        {
            ResetPhones();
        }
        public async void ResetPhones()
        {
            // Получаем из базы список заказов или null.
            // Посылаем клиенту запрос о заказах.
            HttpResponseMessage response = await ApiClient.Http.GetAsync(ApiClient.phonesPath);
            //Возвращаем полученый из базы данных список заказов либо null.
            List<PhoneDto>? phonesDto = response.IsSuccessStatusCode ? JsonConvert.DeserializeObject<List<PhoneDto>>(response.Content.ReadAsStringAsync().Result) : null;
            // Создаем коллекцию заказов, если список существует.
            Phones = phonesDto != null ? new ObservableCollection<PhoneDto>(phonesDto) : new();
        }
        private async void PhoneRemove(object? e)
        {
            if (selectedPhone == null)
                return;
            //Здесь клиент ApiClient.Http описан в отдельном классе и в отдельной библиотеке. Проблем нет.
            if (ApiClient.JwtToken != null)
                ApiClient.Http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", ApiClient.JwtToken);
            HttpResponseMessage? response = await ApiClient.Http.DeleteAsync(ApiClient.phonesPath + $"/{selectedPhone.Id}");
            if (response.IsSuccessStatusCode)
                Phones.Remove(selectedPhone);
            else
            if (ApiClient.UsedRole == UserRoles.User)
                MessageBox.Show(response.StatusCode.ToString() + $". У роли {UserRoles.User} нет прав на удаление записи!");
        }
        private void PhoneSelection(object? e)
        {
            if (e == null || e is not DataGrid grid || grid.SelectedItem == null)
                return;
            selectedPhone = grid.SelectedItem is PhoneDto phoneDto ? phoneDto : null;
        }
        private async void PhoneRowEditEnd(object? e)
        {
            if (selectedPhone == null)
                return;
            HttpResponseMessage? response;
            // Здесь требуется локальное описание клиента. 
            // Если использовать клиента ApiClient.Http, описанного во внешнем классе, ни редактирование ни создание новой записи работать не будут.
            HttpClient client = new() { BaseAddress = new Uri(ApiClient.address) };
            if (ApiClient.JwtToken != null)
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", ApiClient.JwtToken);
            response = selectedPhone.Id == 0 ? await client.PostAsJsonAsync(ApiClient.phonesPath, selectedPhone) :
                await client.PutAsJsonAsync(ApiClient.phonesPath + $"/{selectedPhone.Id}", selectedPhone);
            if (response.IsSuccessStatusCode)
                ResetPhones();
            else
                MessageBox.Show(response.StatusCode.ToString());
        }
        private void PhoneBeginningEdit(object? e)
        {
            if (e == null || e is not DataGridBeginningEditEventArgs eventArgs)
                return;
            eventArgs.Cancel = ApiClient.UsedRole == UserRoles.User && selectedPhone != null && selectedPhone.Id != 0;
        }
        #endregion
    }
}
