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
        private RelayCommand? phoneRowEditEndCommand;
        private RelayCommand? phoneBeginningEditCommand;
        private bool canAdd;
        private bool canRemove;
        private bool isReadOnly = true;
        #endregion
        #region Properties
        public PhoneDto? SelectedPhone { get => selectedPhone; set { selectedPhone = value; RaisePropertyChanged(nameof(SelectedPhone)); } }
        public bool CanAdd { get => canAdd; set { canAdd = value; RaisePropertyChanged(nameof(CanAdd)); } }
        public bool CanRemove { get => canRemove; set { canRemove = value; RaisePropertyChanged(nameof(CanRemove)); } }
        public bool IsReadOnly { get => isReadOnly; set { isReadOnly = value; RaisePropertyChanged(nameof(IsReadOnly)); } }
        /// <summary>
        /// Устанавливает и возвращает коллекцию объектов модели.
        /// </summary>
        public ObservableCollection<PhoneDto> Phones { get => phones; set { phones = value; RaisePropertyChanged(nameof(Phones)); } }
        public ICommand PhoneRemoveCommand => phoneRemoveCommand ??= new RelayCommand(PhoneRemove);
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
            // Посылаем клиенту запрос о т. книге.
            HttpResponseMessage response = await ApiClient.Http.GetAsync(ApiClient.phonesPath);
            //Возвращаем полученый из базы данных т. книгу либо null.
            List<PhoneDto>? phonesDto = response.IsSuccessStatusCode ? JsonConvert.DeserializeObject<List<PhoneDto>>(response.Content.ReadAsStringAsync().Result) : null;
            // Создаем коллекцию записей, если т. книга существует.
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
        private async void PhoneRowEditEnd(object? e)
        {
            if (selectedPhone == null)
                return;
            // Здесь требуется создание локального клиента. 
            // Если использовать клиента Http, созданного вне метода, ни редактирование, ни создание новой записи работать не будут.
            // Отредактированная версия записи не будет поступать по адресу. Будет поступать прежняя версия.
            using HttpClient httpClient = new() { BaseAddress = new Uri(ApiClient.address) };
            if (ApiClient.JwtToken != null)// Если токен есть
                // Формируем заголовок запроса, подключая имеющийся токен.
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", ApiClient.JwtToken);
            // Формируем запрос на редактирование или создания записи.
            HttpResponseMessage? response = selectedPhone.Id == 0 ? await httpClient.PostAsJsonAsync(ApiClient.phonesPath, selectedPhone) :
                await httpClient.PutAsJsonAsync(ApiClient.phonesPath + $"/{selectedPhone.Id}", selectedPhone);
            if (response.IsSuccessStatusCode)
                // При удачном запросе обновляем т. книгу в UI.
                ResetPhones();
            else
                // Формируем сообщение при неудачном результате запроса.
                MessageBox.Show(response.StatusCode.ToString());
        }
        private void PhoneBeginningEdit(object? e)
        {
            if (e == null || e is not DataGridBeginningEditEventArgs eventArgs)
                return;
            // Роль UserRoles.User имеет право только добавлять запись в телю книгу, но не редактировать уже имеющиеся записи.
            eventArgs.Cancel = ApiClient.UsedRole == UserRoles.User && selectedPhone != null && selectedPhone.Id != 0;
        }
        #endregion
    }
}
