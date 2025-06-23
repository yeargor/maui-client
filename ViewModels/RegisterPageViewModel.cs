using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiDemo2.Dtos;
using MauiDemo2.Services;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MauiDemo2.Views;

namespace MauiDemo2.ViewModels
{
    public partial class RegisterPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private RegisterRequestDto request = new RegisterRequestDto {
            Username = string.Empty,
            LastName = string.Empty,
            FirstName = string.Empty,
            SecondName = string.Empty,
            Email = string.Empty,
            Password = string.Empty
        };

        [ObservableProperty]
        private string? repeatPassword = null;

        [ObservableProperty]
        private bool isPassword = true;
        [ObservableProperty]
        private bool isRepeatPassword = true;
        [ObservableProperty]
        private string eyeIcon = "eyeoff.svg";
        [ObservableProperty]
        private string repeatEyeIcon = "eyeoff.svg";
        [ObservableProperty]
        private bool isPasswordMismatch;
        [ObservableProperty]
        private List<string> validationErrors = new();
        [ObservableProperty]
        private ObservableCollection<string> usernameErrors = new();
        [ObservableProperty]
        private ObservableCollection<string> lastNameErrors = new();
        [ObservableProperty]
        private ObservableCollection<string> firstNameErrors = new();
        [ObservableProperty]
        private ObservableCollection<string> secondNameErrors = new();
        [ObservableProperty]
        private ObservableCollection<string> emailErrors = new();
        [ObservableProperty]
        private ObservableCollection<string> passwordErrors = new();
        [ObservableProperty]
        private ObservableCollection<string> repeatPasswordErrors = new();
        [ObservableProperty]
        private bool showValidationErrors;

        [ObservableProperty]
        private bool isErrorPopupVisible;
        [ObservableProperty]
        private string? errorPopupText = null;

        [ObservableProperty]
        private string selectedPhoto = "man.png";

        [ObservableProperty]
        private FileResult? pickedPhotoFile;

        [ObservableProperty]
        private ImageSource? pickedPhotoImageSource;

        [ObservableProperty]
        private bool isBusy;
        [ObservableProperty]
        private string? registerError;

        private readonly AuthService _authService = ServiceHelper.GetService<AuthService>();

        partial void OnRepeatPasswordChanged(string? value)
        {
            CheckPasswordMatch();
        }
        partial void OnRequestChanged(RegisterRequestDto value)
        {
            CheckPasswordMatch();
        }
        private void CheckPasswordMatch()
        {
            IsPasswordMismatch = !string.IsNullOrEmpty(Request.Password) && !string.IsNullOrEmpty(RepeatPassword) && Request.Password != RepeatPassword;
        }

        [RelayCommand]
        private void TogglePasswordVisibility()
        {
            IsPassword = !IsPassword;
            EyeIcon = IsPassword ? "eyeoff.svg" : "eyeon.svg";
        }
        [RelayCommand]
        private void ToggleRepeatPasswordVisibility()
        {
            IsRepeatPassword = !IsRepeatPassword;
            RepeatEyeIcon = IsRepeatPassword ? "eyeoff.svg" : "eyeon.svg";
        }

        [RelayCommand]
        private void Register()
        {
            ShowValidationErrors = false;
            UsernameErrors.Clear();
            LastNameErrors.Clear();
            FirstNameErrors.Clear();
            SecondNameErrors.Clear();
            EmailErrors.Clear();
            PasswordErrors.Clear();
            RepeatPasswordErrors.Clear();
            ValidationErrors = new List<string>();
            IsErrorPopupVisible = false;
            ErrorPopupText = string.Empty;

            var req = new RegisterRequestDto {
                Username = Request.Username,
                LastName = Request.LastName,
                FirstName = Request.FirstName,
                SecondName = Request.SecondName,
                Email = Request.Email,
                Password = Request.Password
            };
            var context = new ValidationContext(req);
            var results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(req, context, results, true);

            // Пароли
            if (string.IsNullOrWhiteSpace(Request.Password))
                PasswordErrors.Add("Password cannot be empty");
            if (string.IsNullOrWhiteSpace(RepeatPassword))
                RepeatPasswordErrors.Add("Please repeat password");
            if (!string.IsNullOrWhiteSpace(Request.Password) && !string.IsNullOrWhiteSpace(RepeatPassword) && Request.Password != RepeatPassword)
                RepeatPasswordErrors.Add("Passwords do not match");

            // DataAnnotations
            foreach (var error in results)
            {
                foreach (var member in error.MemberNames)
                {
                    switch (member)
                    {
                        case nameof(RegisterRequestDto.Username): AddError(UsernameErrors, error.ErrorMessage); break;
                        case nameof(RegisterRequestDto.LastName): AddError(LastNameErrors, error.ErrorMessage); break;
                        case nameof(RegisterRequestDto.FirstName): AddError(FirstNameErrors, error.ErrorMessage); break;
                        case nameof(RegisterRequestDto.SecondName): AddError(SecondNameErrors, error.ErrorMessage); break;
                        case nameof(RegisterRequestDto.Email): AddError(EmailErrors, error.ErrorMessage); break;
                        case nameof(RegisterRequestDto.Password): AddError(PasswordErrors, error.ErrorMessage); break;
                    }
                }
            }

            ShowValidationErrors = true;

            var allErrors = new List<string>();
            allErrors.AddRange(UsernameErrors);
            allErrors.AddRange(LastNameErrors);
            allErrors.AddRange(FirstNameErrors);
            allErrors.AddRange(SecondNameErrors);
            allErrors.AddRange(EmailErrors);
            allErrors.AddRange(PasswordErrors);
            allErrors.AddRange(RepeatPasswordErrors);

            if (allErrors.Count > 0)
            {
                ErrorPopupText = string.Join("\n", allErrors);
                IsErrorPopupVisible = true;
                return;
            }

            Debug.WriteLine($"[Register] Username: {req.Username}, LastName: {req.LastName}, FirstName: {req.FirstName}, SecondName: {req.SecondName}, Email: {req.Email}, Password: {req.Password}");
            _ = GoToPhotoPage();
        }

        [RelayCommand]
        private void CloseErrorPopup()
        {
            IsErrorPopupVisible = false;
        }

        [RelayCommand]
        private async Task ToLoginPage()
        {
            if (Application.Current?.MainPage?.Navigation != null)
            {
                await Application.Current.MainPage.Navigation.PopModalAsync();
                await Application.Current.MainPage.Navigation.PushModalAsync(new MauiDemo2.Views.LoginPage());
            }
        }

        [RelayCommand]
        private async Task GoToPhotoPage()
        {
            if (Application.Current?.MainPage?.Navigation != null)
                await Application.Current.MainPage.Navigation.PushModalAsync(new MauiDemo2.Views.PhotoPage(this));
        }

        [RelayCommand]
        private void SelectPhoto(string photo)
        {
            SelectedPhoto = photo;
            if (photo == "man.png" || photo == "woman.png")
            {
                // Не трогаем PickedPhotoImageSource и PickedPhotoFile, чтобы третий кружок всегда был myphoto.png
                PickedPhotoImageSource = null;
                PickedPhotoFile = null;
            }
        }

        [RelayCommand]
        private async Task PickPhoto()
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Выберите фото",
                    FileTypes = FilePickerFileType.Images
                });
                if (result != null)
                {
                    PickedPhotoFile = result;
                    SelectedPhoto = "myphoto.png";
                    PickedPhotoImageSource = ImageSource.FromStream(() => result.OpenReadAsync().Result);
                }
            }
            catch { /* обработка ошибок по желанию */ }
        }

        [RelayCommand]
        private async Task ConfirmPhoto()
        {
            FileResult? imageResult = null;
            Dtos.DefaultPhotoType? defaultType = null;
            bool isUsingDefault = false;
            if (SelectedPhoto == "myphoto.png" && PickedPhotoFile != null)
            {
                imageResult = PickedPhotoFile;
                isUsingDefault = false;
            }
            else if (SelectedPhoto == "man.png")
            {
                defaultType = Dtos.DefaultPhotoType.MAN;
                isUsingDefault = true;
                imageResult = null;
            }
            else if (SelectedPhoto == "woman.png")
            {
                defaultType = Dtos.DefaultPhotoType.WOMAN;
                isUsingDefault = true;
                imageResult = null;
            }
            Request = new RegisterRequestDto {
                Username = Request.Username,
                LastName = Request.LastName,
                FirstName = Request.FirstName,
                SecondName = Request.SecondName,
                Email = Request.Email,
                Password = Request.Password,
                DefaultType = defaultType,
                IsUsingDefault = isUsingDefault,
                Image = imageResult
            };
            var imagePath = Request.Image?.FullPath ?? (Request.DefaultType?.ToString() ?? "no file");
            Debug.WriteLine($"[Photo Confirmed] DTO: {{ Username: {Request.Username}, LastName: {Request.LastName}, FirstName: {Request.FirstName}, SecondName: {Request.SecondName}, Email: {Request.Email}, Password: {Request.Password}, DefaultType: {Request.DefaultType}, IsUsingDefault: {Request.IsUsingDefault}, Image: {imagePath} }}");
            await RegisterAndSendAsync();
        }

        public async Task RegisterAndSendAsync()
        {
            Debug.WriteLine($"[RegisterAndSendAsync] START. Request: {{ Username: {Request?.Username}, Email: {Request?.Email}, Password: {Request?.Password}, DefaultType: {Request?.DefaultType}, IsUsingDefault: {Request?.IsUsingDefault}, Image: {Request?.Image?.FullPath} }}");
            IsBusy = true;
            RegisterError = null;
            using var client = new HttpClient();
            using var content = new MultipartFormDataContent();
            Debug.WriteLine("[RegisterAndSendAsync] Adding form data...");
            content.Add(new StringContent(Request.Username), "Username");
            content.Add(new StringContent(Request.LastName ?? string.Empty), "LastName");
            content.Add(new StringContent(Request.FirstName ?? string.Empty), "FirstName");
            content.Add(new StringContent(Request.SecondName ?? string.Empty), "SecondName");
            content.Add(new StringContent(Request.Email), "Email");
            content.Add(new StringContent(Request.Password), "Password");
            content.Add(new StringContent(Request.DefaultType?.ToString() ?? string.Empty), "DefaultType");
            content.Add(new StringContent(Request.IsUsingDefault.ToString()), "IsUsingDefault");
            if (Request.Image is FileResult fileResult)
            {
                Debug.WriteLine($"[RegisterAndSendAsync] Adding image: {fileResult.FileName}");
                var stream = await fileResult.OpenReadAsync();
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                content.Add(fileContent, "Image", fileResult.FileName);
            }
            try
            {
                Debug.WriteLine("[RegisterAndSendAsync] Sending POST request...");
                var response = await client.PostAsync("http://10.0.2.2:5246/api/auth/register", content);
                var responseString = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"[RegisterAndSendAsync] Response: {response.StatusCode}, Body: {responseString}");
                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine("[RegisterAndSendAsync] Registration success, calling LoginAfterRegister...");
                    await LoginAfterRegister(Request.Email, Request.Password);
                }
                else
                {
                    try
                    {
                        using var doc = JsonDocument.Parse(responseString);
                        var root = doc.RootElement;
                        var error = root.TryGetProperty("error", out var e) ? e.GetString() : responseString;
                        RegisterError = error;
                        Debug.WriteLine($"[RegisterAndSendAsync] Registration error: {RegisterError}");
                    }
                    catch
                    {
                        RegisterError = responseString;
                        Debug.WriteLine($"[RegisterAndSendAsync] Registration error (raw): {RegisterError}");
                    }
                }
            }
            catch (Exception ex)
            {
                RegisterError = ex.Message;
                Debug.WriteLine($"[RegisterAndSendAsync] Exception: {ex}");
            }
            finally
            {
                IsBusy = false;
                Debug.WriteLine("[RegisterAndSendAsync] END");
            }
            if (Request == null ||
                string.IsNullOrWhiteSpace(Request.Username) ||
                string.IsNullOrWhiteSpace(Request.Email) ||
                string.IsNullOrWhiteSpace(Request.Password))
            {
                RegisterError = "Проверьте, что все обязательные поля заполнены.";
                Debug.WriteLine("[RegisterAndSendAsync] Request or required fields are null/empty");
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка регистрации", RegisterError, "OK");
                }
                return;
            }
            if (!string.IsNullOrEmpty(RegisterError) && Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка регистрации", RegisterError, "OK");
            }
        }

        private async Task LoginAfterRegister(string email, string password)
        {
            Debug.WriteLine($"[LoginAfterRegister] START. Email: {email}, Password: {password}");
            if (_authService == null)
            {
                RegisterError = "_authService is null!";
                Debug.WriteLine("[LoginAfterRegister] _authService is null!");
                if (Application.Current?.MainPage != null)
                    await Application.Current.MainPage.DisplayAlert("Ошибка входа", RegisterError, "OK");
                return;
            }
            var result = await _authService.LoginAsync(email, password);
            var (success, token, error) = result;
            Debug.WriteLine($"[LoginAfterRegister] Result: success={success}, token={token}, error={error}");
            if (success && !string.IsNullOrEmpty(token))
            {
                Debug.WriteLine("[LoginAfterRegister] Login success, navigating to CardPage...");
                if (Application.Current?.MainPage != null)
                    Application.Current.MainPage = new AppShell();
                if (Shell.Current != null)
                    await Shell.Current.GoToAsync(nameof(CardPage));
                Debug.WriteLine("[LoginAfterRegister] END (success)");
                return;
            }
            else
            {
                RegisterError = error ?? "Ошибка авторизации после регистрации. Попробуйте войти вручную.";
                Debug.WriteLine($"[LoginAfterRegister] Login failed: {RegisterError}");
            }
            if (!string.IsNullOrEmpty(RegisterError) && Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка входа", RegisterError, "OK");
            }
            Debug.WriteLine("[LoginAfterRegister] END (fail)");
        }

        private void AddError(ObservableCollection<string> collection, string? error)
        {
            if (!string.IsNullOrWhiteSpace(error))
                collection.Add(error);
        }
    }
}
