using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
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
        private RelayCommand? resetCommand;
        private RelayCommand? saveCommand;
        private bool canAdd;
        private bool canRemove;
        private bool isReadOnly = true;
        private readonly List<int> createdPhones = new();
        private readonly List<int> editedPhones = new();
        private Visibility saveVisibility = Visibility.Hidden;
        #endregion
        #region Properties
        public Visibility SaveVisibility { get => saveVisibility; set { saveVisibility = value; RaisePropertyChanged(nameof(SaveVisibility)); } }
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
        public ICommand ResetCommand => resetCommand ??= new RelayCommand(e => ResetPhones());
        public ICommand SaveCommand => saveCommand ??= new RelayCommand(Save);
        #endregion
        #region Methods
        public PhonesViewModel() => ResetPhones();
        public async void ResetPhones()
        {
            // Посылаем клиенту запрос о т. книге.
            HttpResponseMessage response = await ApiClient.Http.GetAsync(ApiClient.phonesPath);
            //Возвращаем полученый из базы данных т. книгу либо null.
            List<PhoneDto>? phonesDto = response.IsSuccessStatusCode ? JsonConvert.DeserializeObject<List<PhoneDto>>(response.Content.ReadAsStringAsync().Result) : null;
            // Создаем коллекцию записей, если т. книга существует.
            Phones = phonesDto != null ? new ObservableCollection<PhoneDto>(phonesDto) : new();
            SaveVisibility = Visibility.Hidden;
            createdPhones.Clear(); editedPhones.Clear();
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
        private void PhoneRowEditEnd(object? e)
        {
            if (selectedPhone == null)
                return;
            // Добавляем индекс выделенной записи в список отредактированных или вновь созданных записей с тем, чтобы в дальнейшем сохранить их в БД.
            if (selectedPhone.Id == 0)
                createdPhones.Add(Phones.IndexOf(selectedPhone));
            else
                editedPhones.Add(Phones.IndexOf(selectedPhone));
            // Кнопка сохранения становится видимой.
            SaveVisibility = Visibility.Visible;
            //HttpResponseMessage response;

            //// Если использовать клиента Http, созданного в куче, необходимо дожидаться его уничтожения в памяти механизмом GC.
            //// В противном случае ни редактирование, ни создание новой записи работать не будут.
            //// По адресу будет поступать прежняя версия.
            //if (ApiClient.JwtToken != null)// Если токен есть
            //    // Формируем заголовок запроса, подключая имеющийся токен.
            //    ApiClient.Http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", ApiClient.JwtToken);
            //// Формируем запрос на редактирование или создание записи.
            //response = selectedPhone.Id == 0 ? await ApiClient.Http.PostAsJsonAsync(ApiClient.phonesPath, selectedPhone) :
            //    await ApiClient.Http.PutAsJsonAsync(ApiClient.phonesPath + $"/{selectedPhone.Id}", selectedPhone);

            ////// Повторяем тот же код для локально воссоздаваемого клиента.
            ////using HttpClient httpClient = new() { BaseAddress = new Uri(ApiClient.address) };
            ////if (ApiClient.JwtToken != null)
            ////    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", ApiClient.JwtToken);
            ////response = selectedPhone.Id == 0 ? await httpClient.PostAsJsonAsync(ApiClient.phonesPath, selectedPhone) :
            ////    await httpClient.PutAsJsonAsync(ApiClient.phonesPath + $"/{selectedPhone.Id}", selectedPhone);

            //if (response.IsSuccessStatusCode)
            //    // При удачном запросе обновляем тел. книгу в UI.
            //    ResetPhones();
            //else
            //    // Формируем сообщение при неудачном результате запроса.
            //    MessageBox.Show(response.StatusCode.ToString());
        }
        private void PhoneBeginningEdit(object? e)
        {
            if (e == null || e is not DataGridBeginningEditEventArgs eventArgs)
                return;
            // Роль UserRoles.User имеет право только добавлять запись в тел. книгу, но не редактировать уже имеющиеся записи.
            eventArgs.Cancel = ApiClient.UsedRole == UserRoles.User && selectedPhone != null && selectedPhone.Id != 0;
        }
        private async void Save(object? e)
        {
            HttpResponseMessage? response = null;
            bool isSuccess = true;
            // Если использовать клиента Http, созданного в куче, необходимо дожидаться его уничтожения в памяти механизмом GC.
            // В противном случае ни редактирование, ни создание новой записи работать не будут.
            // По адресу будет поступать прежняя версия.
            if (ApiClient.JwtToken != null)// Если токен есть
                // Формируем заголовок запроса, подключая имеющийся токен.
                ApiClient.Http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", ApiClient.JwtToken);
            // Формируем запрос на редактирование или создание записи для каждой записи из списков.
            foreach (int item in createdPhones)
            {
                response = await ApiClient.Http.PostAsJsonAsync(ApiClient.phonesPath, Phones[item]);
                if (response == null)
                    return;
                isSuccess &= response.IsSuccessStatusCode;
            }
            foreach (int item in editedPhones)
            {
                response = await ApiClient.Http.PutAsJsonAsync(ApiClient.phonesPath + $"/{Phones[item].Id}", Phones[item]);
                if (response == null)
                    return;
                isSuccess &= response.IsSuccessStatusCode;
            }
            if (response == null)
                return;
            if (isSuccess)
            {
                // При удачном запросе обновляем тел. книгу в UI.
                ResetPhones();
            }
            else
                // Формируем сообщение при неудачном результате запроса.
                MessageBox.Show(response.StatusCode.ToString());
        }
        #endregion
    }
}
