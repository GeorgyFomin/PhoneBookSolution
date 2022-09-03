using System.Windows.Input;
using WpfPhoneBook.Commands;
using System.Net.Http;
using System.Net.Http.Json;
using UseCases.API.Authentication;
using Newtonsoft.Json;
using System.Windows;
using System.Windows.Controls;
using UseCases.API.Core;

namespace WpfPhoneBook.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Fields
        private ViewModelBase? viewModel;
        private RelayCommand? phonesCommand;
        private RelayCommand? regLogCommand;
        private RelayCommand? regCommand;
        private RelayCommand? logInCommand;
        private Visibility? userBtnVisibility = Visibility.Collapsed;
        private Visibility? adminBtnVisibility = Visibility.Visible;
        #endregion
        #region Properties
        public Visibility? UserBtnVisibility { get => userBtnVisibility; set { userBtnVisibility = value; RaisePropertyChanged(nameof(UserBtnVisibility)); } }
        public Visibility? AdminBtnVisibility { get => adminBtnVisibility; set { adminBtnVisibility = value; RaisePropertyChanged(nameof(AdminBtnVisibility)); } }
        public ViewModelBase? ViewModel { get => viewModel; set { viewModel = value; RaisePropertyChanged(nameof(ViewModel)); } }
        public ICommand PhonesCommand => phonesCommand ??= new RelayCommand(e => ViewModel = new PhonesViewModel());
        public ICommand RegCommand => regCommand ??= new RelayCommand(Register);
        public ICommand RegLogCommand => regLogCommand ??= new RelayCommand(RegLog);
        public ICommand LogInCommand => logInCommand ??= new RelayCommand(LogIn);
        #endregion
        private async void Register(object? e)
        {
            if (e == null || e is not Button regButton || regButton == null)
                return;
            RegLogWindow regLogWin = new() { DataContext = this };
            regLogWin.RegBtn.Content = "Регистрация";
            regLogWin.Email.Visibility = Visibility.Visible;
            string role = regButton.Name == "User" ? UserRoles.User : UserRoles.Admin;
            regLogWin.regBlock.Text = $"Register {role}";
            bool? res = regLogWin.ShowDialog();
            string errMsg = $"Error! {role} creation failed!";
            if (!res.HasValue || !res.Value)
            {
                MessageBox.Show(errMsg);
                return;
            }
            HttpResponseMessage? response;
            if (role == UserRoles.User)
            {
                if (ApiClient.JwtToken != null)
                {
                    ApiClient.Http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", ApiClient.JwtToken);
                }
                response = await ApiClient.Http.PostAsJsonAsync(ApiClient.authPath + "/register-user",
                    new RegisterModel() { Email = regLogWin.Email.Text, Password = regLogWin.Password.Password, Username = regLogWin.UserName.Text });
                if (!response.IsSuccessStatusCode)
                {
                    HttpContent? content = response.Content;
                    if (content != null)
                    {
                        RegisterResponse? registerResponse = JsonConvert.DeserializeObject<RegisterResponse>(await content.ReadAsStringAsync());
                        if (registerResponse != null && registerResponse.Message != null)
                        {
                            MessageBox.Show(registerResponse.Message);
                            return;
                        }
                    }
                    MessageBox.Show(response.StatusCode.ToString());
                }
            }
            else
            {
                response = await ApiClient.Http.PostAsJsonAsync(ApiClient.authPath + "/register-admin",
                    new RegisterModel() { Email = regLogWin.Email.Text, Password = regLogWin.Password.Password, Username = regLogWin.UserName.Text });
                if (!response.IsSuccessStatusCode)
                {
                    RegisterResponse? registerResponse = JsonConvert.DeserializeObject<RegisterResponse>(await response.Content.ReadAsStringAsync());
                    MessageBox.Show(registerResponse != null && registerResponse.Message != null ?
                        registerResponse.Message : (errMsg + await response.Content.ReadAsStringAsync()));
                }
            }
        }
        private void RegLog(object? e)
        {
            if (e == null || e is not RegLogWindow regLogWin || regLogWin == null)
                return;
            regLogWin.DialogResult = true;
        }
        private async void LogIn(object? e)
        {
            if (e == null || e is not Button regButton || regButton == null)
                return;
            RegLogWindow regLogWin = new() { DataContext = this };
            regLogWin.RegBtn.Content = "Войти";
            regLogWin.Email.Visibility = Visibility.Collapsed;
            string role = regButton.Name == "User" ? UserRoles.User : UserRoles.Admin;
            regLogWin.regBlock.Text = $"Login {role}";
            bool? res = regLogWin.ShowDialog();
            string errMsg = $"Error! {role} login failed!";
            if (res.HasValue && res.Value)
            {
                HttpResponseMessage response = await ApiClient.Http.PostAsJsonAsync(ApiClient.authPath + "/Login",
                    new LoginModel() { Password = regLogWin.Password.Password, Username = regLogWin.UserName.Text });
                if (response.IsSuccessStatusCode)
                {
                    ApiClient.JwtToken = JsonConvert.DeserializeObject<LoginResponse>(await response.Content.ReadAsStringAsync())?.Token;
                    ApiClient.UsedRole = role;
                    AdminBtnVisibility = Visibility.Collapsed;
                    UserBtnVisibility = role == UserRoles.User ? Visibility.Collapsed : Visibility.Visible;
                    ViewModel = new PhonesViewModel() { CanAdd = true, CanRemove = role == UserRoles.Admin, IsReadOnly = false };
                }
                else
                {
                    LoginResponse? loginResponse = JsonConvert.DeserializeObject<LoginResponse>(await response.Content.ReadAsStringAsync());
                    MessageBox.Show(loginResponse != null && loginResponse.Token != null ?
                        (loginResponse.Token + "  " + loginResponse.Expiration) : (errMsg + await response.Content.ReadAsStringAsync()));
                }
            }
            else
                MessageBox.Show(errMsg);
        }
    }
}
