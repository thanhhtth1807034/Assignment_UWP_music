using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using T1808aUWP.Entity;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace T1808aUWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RegisterPage2 : Page
    {
        private const string RegisterUrl = "https://2-dot-backup-server-003.appspot.com/_api/v2/members";
        private const string GET_UPLOAD_URL = "https://2-dot-backup-server-003.appspot.com/get-upload-token";
        private string _gender = "Gender";
        private StorageFile photo;
        public RegisterPage2()
        {
            Debug.WriteLine("Init Register.");
            this.InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var birthdaySelectedDate = this.Birthday.SelectedDate;
            if (birthdaySelectedDate == null)
            {
                birthdaySelectedDate = DateTime.Now;
            }

            var birthday = birthdaySelectedDate.Value.ToString("yyyy-MM-dd");

            var member = new Member
            {
                firstName = this.FirstName.Text,
                lastName = this.LastName.Text,
                avatar = AvatarUrl.Text,
                phone = this.Phone.Text,
                address = this.Address.Text,
                introduction = this.Introduction.Text,
                email = this.Email.Text,
                password = this.Password.Password,
                gender = _gender.Equals("Male") ? 1 : (_gender.Equals("Female") ? 0 : 2),
                birthday = birthday,
            };

            Dictionary<String, String> errors = member.Validate();
            if (errors.Count == 0)
            {
                var httpClient = new HttpClient();

                //chuyển kiểu dữ liệu C# thành kiểu dữ liệu Json
                var dataToSend = JsonConvert.SerializeObject(member);

                //gói, gắn mác cho kiểu dữ liệu, xác định kiểu dữ liệu là json, utf8 
                var content = new StringContent(dataToSend, Encoding.UTF8, "application/json");

                // thực hiện gửi dữ liệu với phương thức POST 
                var resposne = httpClient.PostAsync(RegisterUrl, content).GetAwaiter().GetResult();

                //lấy kết quả trả về từ serve 
                var jsonContent = resposne.Content.ReadAsStringAsync().Result;

                //ép kiểu kết quả từ dữ liệu json sang dữ liệu của C#
                var responseMember = JsonConvert.DeserializeObject<Member>(jsonContent);

                // in ra giá trị Member trả về
                Debug.WriteLine("Register success with id: " + responseMember.id);
            }
            else
            {
                if (errors.ContainsKey("firstName"))
                {
                    FNameError.Text = errors["firstName"];
                    FNameError.Visibility = Visibility.Visible;
                }
                else
                {
                    FNameError.Visibility = Visibility.Collapsed;
                }

                if (errors.ContainsKey("lastName"))
                {
                    LNameError.Text = errors["lastName"];
                    LNameError.Visibility = Visibility.Visible;
                }
                else
                {
                    LNameError.Visibility = Visibility.Collapsed;
                }

                if (errors.ContainsKey("phone"))
                {
                    PhoneError.Text = errors["phone"];
                    PhoneError.Visibility = Visibility.Visible;
                }
                else
                {
                    PhoneError.Visibility = Visibility.Collapsed;
                }

                if (errors.ContainsKey("address"))
                {
                    AddressError.Text = errors["address"];
                    AddressError.Visibility = Visibility.Visible;
                }
                else
                {
                    AddressError.Visibility = Visibility.Collapsed;
                }

                if (errors.ContainsKey("introduction"))
                {
                    IntroductionError.Text = errors["introduction"];
                    IntroductionError.Visibility = Visibility.Visible;
                }
                else
                {
                    IntroductionError.Visibility = Visibility.Collapsed;
                }

                if (errors.ContainsKey("birthday"))
                {
                    BirthdayError.Text = errors["birthday"];
                    BirthdayError.Visibility = Visibility.Visible;
                }
                else
                {
                    BirthdayError.Visibility = Visibility.Collapsed;
                }

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

        private async void ProcessCaptureImage()
        {
            //StorageFolder storageFolder =
            //    await ApplicationData.Current.LocalFolder.CreateFolderAsync("SaveFile",
            //        CreationCollisionOption.OpenIfExists);
            //StorageFile storageFile =
            //    await storageFolder.CreateFileAsync("token.txt", CreationCollisionOption.OpenIfExists);

            //var JsonContent = await FileIO.ReadTextAsync(storageFile);
            //MemberCredential memberCredential = JsonConvert.DeserializeObject<MemberCredential>(JsonContent);
            //Debug.WriteLine(memberCredential.token);
            // ghi file
            //var memberCredential = new MemberCredential();
            //memberCredential.token = Guid.NewGuid().ToString();
            //memberCredential.secretToken = memberCredential.token;
            //memberCredential.createdTimeMLS = DateTime.Now.Millisecond;
            //memberCredential.expiredTimeMLS = DateTime.Today.AddDays(7).Millisecond;
            //memberCredential.status = 1;
            //Debug.WriteLine(memberCredential.token);
            //await FileIO.WriteTextAsync(storageFile, JsonConvert.SerializeObject(memberCredential));
            CameraCaptureUI captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            captureUI.PhotoSettings.CroppedSizeInPixels = new Size(200, 200);

            this.photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

            if (this.photo == null)
            {
                // User cancelled photo capture
                return;
            }

            string uploadUrl = GetUploadUrl();
            HttpUploadFile(uploadUrl, "myFile", "image/jpeg");
        }

        public async void HttpUploadFile(string url, string paramName, string contentType)
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";

            Stream rs = await wr.GetRequestStreamAsync();
            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string header = string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n", paramName, "path_file", contentType);
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            // write file.
            Stream fileStream = await this.photo.OpenStreamForReadAsync();
            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                rs.Write(buffer, 0, bytesRead);
            }

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);

            WebResponse wresp = null;
            try
            {
                wresp = await wr.GetResponseAsync();
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                //Debug.WriteLine(string.Format("File uploaded, server response is: @{0}@", reader2.ReadToEnd()));
                //string imgUrl = reader2.ReadToEnd();
                //Uri u = new Uri(reader2.ReadToEnd(), UriKind.Absolute);
                //Debug.WriteLine(u.AbsoluteUri);
                //ImageUrl.Text = u.AbsoluteUri;
                //MyAvatar.Source = new BitmapImage(u);
                //Debug.WriteLine(reader2.ReadToEnd());
                string imageUrl = reader2.ReadToEnd();
                Avatar.Source = new BitmapImage(new Uri(imageUrl, UriKind.Absolute));
                AvatarUrl.Text = imageUrl;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error uploading file", ex.StackTrace);
                Debug.WriteLine("Error uploading file", ex.InnerException);
                if (wresp != null)
                {
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }
        }

        private string GetUploadUrl()
        {
            var httpClient = new HttpClient();
            // thực hiện gửi dữ liệu với phương thức post.
            var response = httpClient.GetAsync(GET_UPLOAD_URL).GetAwaiter().GetResult();
            // lấy kết quả trả về từ server.
            var uploadUrl = response.Content.ReadAsStringAsync().Result;
            Debug.WriteLine("Upload url: " + uploadUrl);
            return uploadUrl;
        }


        private void RadioBtn_OnChecked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton checkedRadioButton)
            {
                this._gender = checkedRadioButton.Tag.ToString();
            }
        }

        private void ButtonLogin_OnClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LoginPage));
        }
        private void CapturePhoto(object sender, RoutedEventArgs e)
        {
            ProcessCaptureImage();
        }
    }
}
