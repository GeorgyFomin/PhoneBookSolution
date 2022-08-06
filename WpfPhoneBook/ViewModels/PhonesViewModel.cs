using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
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
        /// Устанавливает и возвращает коллекцию заказов.
        /// </summary>
        public ObservableCollection<PhoneDto> Phones { get => phones; set { phones = value; RaisePropertyChanged(nameof(Phones)); } }

        public ICommand PhoneRemoveCommand => phoneRemoveCommand ??= new RelayCommand(PhoneRemove);
        public ICommand PhoneSelectionCommand => phoneSelectionCommand ??= new RelayCommand(PhoneSelection);
        public ICommand PhoneRowEditEndCommand => phoneRowEditEndCommand ??= new RelayCommand(PhoneRowEditEnd);
        #endregion
        public PhonesViewModel()
        {
            ResetPhones();
        }
        static async Task<List<PhoneDto>?> GetPhones()
        {
            HttpClient? client = new() { BaseAddress = new Uri(ApiClient.address) };
            // Посылаем клиенту запрос о заказах.
            HttpResponseMessage response = await client.GetAsync(ApiClient.phonesPath);
            //Возвращаем полученый из базы данных список заказов либо null.
            return response.IsSuccessStatusCode ? JsonConvert.DeserializeObject<List<PhoneDto>>(response.Content.ReadAsStringAsync().Result) : null;
        }
        public async void ResetPhones()
        {
            // Получаем из базы список заказов или null.
            List<PhoneDto>? phonesDto = await GetPhones();
            // Создаем коллекцию заказов, если список существует.
            Phones = phonesDto != null ? new ObservableCollection<PhoneDto>(phonesDto) : new();
        }

        #region Methods
        private async void PhoneRemove(object? e)
        {
            if (selectedPhone == null)
            {
                return;
            }

            HttpClient? client = new() { BaseAddress = new Uri(ApiClient.address) };
            if (ApiClient.JwtToken != null)
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", ApiClient.JwtToken);
            HttpResponseMessage? response = await client.DeleteAsync(ApiClient.phonesPath + $"/{selectedPhone.Id}");
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
            HttpClient? client = new() { BaseAddress = new Uri(ApiClient.address) };
            HttpResponseMessage? response;
            if (ApiClient.JwtToken != null)
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", ApiClient.JwtToken);
            response = selectedPhone.Id == 0 ? await client.PostAsJsonAsync(ApiClient.phonesPath, selectedPhone) :
                await client.PutAsJsonAsync(ApiClient.phonesPath + $"/{selectedPhone.Id}", selectedPhone);
            if (response.IsSuccessStatusCode)
                ResetPhones();
            else
                MessageBox.Show(response.StatusCode.ToString());
        }
        public RelayCommand PhoneBeginningEditCommand => phoneBeginningEditCommand ??= new RelayCommand(PhoneBeginningEdit);
        private void PhoneBeginningEdit(object? e)
        {
            if (e == null || e is not DataGridBeginningEditEventArgs eventArgs)
                return;
            eventArgs.Cancel = ApiClient.UsedRole == UserRoles.User && selectedPhone != null && selectedPhone.Id != 0;
        }
        #endregion
    }
}
