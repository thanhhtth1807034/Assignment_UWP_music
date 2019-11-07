using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Newtonsoft.Json;
using T1808aUWP.Entity;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace T1808aUWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {

        private const string LoginUrl = "https://2-dot-backup-server-003.appspot.com/_api/v2/members/authentication";
        public LoginPage()
        {
            Debug.WriteLine("Init Login.");
            this.InitializeComponent();
        }

        private void ButtonLogin_OnClick(object sender, RoutedEventArgs e)
        {
            var loginMember = new LoginMember()
            {
                email = this.Email.Text,
                password = this.Password.Password,
            };

            Dictionary<string, string> errors = loginMember.Validate();
            if (errors.Count == 0)
            {
                var httpClient = new HttpClient();
                var dataToSend = JsonConvert.SerializeObject(loginMember);
                var content = new StringContent(dataToSend, Encoding.UTF8, "application/json");
                var response = httpClient.PostAsync(LoginUrl, content).GetAwaiter().GetResult();
                var jsonContent = response.Content.ReadAsStringAsync().Result;
                var responseLoginMember = JsonConvert.DeserializeObject<MemberCredential>(jsonContent);
                Debug.WriteLine(response);
                SaveTokenToFile(responseLoginMember);
            }
            else
            {
                if (errors.ContainsKey("email"))
                {
                    EmailError.Text = errors["email"];
                    EmailError.Visibility = Visibility.Visible;
                }
                else
                {
                    EmailError.Visibility = Visibility.Collapsed;
                }

                if (errors.ContainsKey("password"))
                {
                    PwdError.Text = errors["password"];
                    PwdError.Visibility = Visibility.Visible;
                }
                else
                {
                    PwdError.Visibility = Visibility.Collapsed;
                }
            }
        }

        private async void SaveTokenToFile(MemberCredential responseLoginMember)
        {
            StorageFolder storageFolder =
                await ApplicationData.Current.LocalFolder.CreateFolderAsync("SavedFile",
                    CreationCollisionOption.OpenIfExists);
            StorageFile storageFile =
                await storageFolder.CreateFileAsync("token.txt", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(storageFile, JsonConvert.SerializeObject(responseLoginMember));
        }

        private void ButtonRegister_OnClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(RegisterPage2));
        }
    }
}
