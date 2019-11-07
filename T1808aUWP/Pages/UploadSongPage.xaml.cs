using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
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
    public sealed partial class UploadSongPage : Page
    {
        private const string UploadSongUrl = "https://2-dot-backup-server-003.appspot.com/_api/v2/songs";
        //private const string GET_UPLOAD_URL = "https://2-dot-backup-server-003.appspot.com/get-upload-token";
        //private StorageFile photo;
        public UploadSongPage()
        {
            Debug.WriteLine("Init Upload song.");
            this.InitializeComponent();
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var song = new Song
            {
                name = this.Name.Text,
                description = this.Description.Text,
                singer = this.Singer.Text,
                author = this.Author.Text,
                thumbnail = this.Thumbnail.Text,
                link = this.Link.Text,
            };
            Dictionary<String, String> errors = song.Validate();
            if (errors.Count == 0)
            {
                var storageFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("SavedFile",
                    CreationCollisionOption.OpenIfExists);
                try
                {
                    var storageFile =
                        await storageFolder.GetFileAsync("token.txt");
                    if (storageFile != null)
                    {
                        var jsonContent = await FileIO.ReadTextAsync(storageFile);
                        MemberCredential memberCredential = JsonConvert.DeserializeObject<MemberCredential>(jsonContent);
                        var httpWebRequest = (HttpWebRequest)WebRequest.Create(UploadSongUrl);
                        httpWebRequest.ContentType = "application/json";
                        httpWebRequest.Method = "POST";
                        httpWebRequest.Headers.Add("Authorization", "Basic" + " " + memberCredential.token);

                        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                        {
                            var json = JsonConvert.SerializeObject(song);

                            streamWriter.Write(json);
                        }

                        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream() ?? throw new InvalidOperationException()))
                        {
                            var result = streamReader.ReadToEnd();
                            Debug.WriteLine(result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception Occurred: " + ex.Message);
                }
            }
            else
            {
                if (errors.ContainsKey("name"))
                {
                    NameError.Text = errors["name"];
                    NameError.Visibility = Visibility.Visible;
                }
                else
                {
                    NameError.Visibility = Visibility.Collapsed;
                }

                if (errors.ContainsKey("singer"))
                {
                    SingerError.Text = errors["singer"];
                    SingerError.Visibility = Visibility.Visible;
                }
                else
                {
                    SingerError.Visibility = Visibility.Collapsed;
                }

                if (errors.ContainsKey("author"))
                {
                    AuthorError.Text = errors["author"];
                    AuthorError.Visibility = Visibility.Visible;
                }
                else
                {
                    AuthorError.Visibility = Visibility.Collapsed;
                }

                if (errors.ContainsKey("description"))
                {
                    DescriptionError.Text = errors["description"];
                    DescriptionError.Visibility = Visibility.Visible;
                }
                else
                {
                    DescriptionError.Visibility = Visibility.Collapsed;
                }

                if (errors.ContainsKey("link"))
                {
                    LinkError.Text = errors["link"];
                    LinkError.Visibility = Visibility.Visible;
                }
                else
                {
                    LinkError.Visibility = Visibility.Collapsed;
                }

                if (errors.ContainsKey("thumbnail"))
                {
                    ThumbnailError.Text = errors["thumbnail"];
                    ThumbnailError.Visibility = Visibility.Visible;
                }
                else
                {
                    ThumbnailError.Visibility = Visibility.Collapsed;
                }
            }
        }

        //private async void ProcessCaptureImage()
        //{
        //    CameraCaptureUI captureUI = new CameraCaptureUI();
        //    captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
        //    captureUI.PhotoSettings.CroppedSizeInPixels = new Size(200, 200);

        //    this.photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

        //    if (this.photo == null)
        //    {
        //        // User cancelled photo capture
        //        return;
        //    }

        //    string uploadUrl = GetUploadUrl();
        //    HttpUploadFile(uploadUrl, "myFile", "image/jpeg");
        //}

        //public async void HttpUploadFile(string url, string paramName, string contentType)
        //{
        //    string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
        //    byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

        //    HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
        //    wr.ContentType = "multipart/form-data; boundary=" + boundary;
        //    wr.Method = "POST";

        //    Stream rs = await wr.GetRequestStreamAsync();
        //    rs.Write(boundarybytes, 0, boundarybytes.Length);

        //    string header = string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n", paramName, "path_file", contentType);
        //    byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
        //    rs.Write(headerbytes, 0, headerbytes.Length);

        //    // write file.
        //    Stream fileStream = await this.photo.OpenStreamForReadAsync();
        //    byte[] buffer = new byte[4096];
        //    int bytesRead = 0;
        //    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
        //    {
        //        rs.Write(buffer, 0, bytesRead);
        //    }

        //    byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
        //    rs.Write(trailer, 0, trailer.Length);

        //    WebResponse wresp = null;
        //    try
        //    {
        //        wresp = await wr.GetResponseAsync();
        //        Stream stream2 = wresp.GetResponseStream();
        //        StreamReader reader2 = new StreamReader(stream2);
        //        //Debug.WriteLine(string.Format("File uploaded, server response is: @{0}@", reader2.ReadToEnd()));
        //        //string imgUrl = reader2.ReadToEnd();
        //        //Uri u = new Uri(reader2.ReadToEnd(), UriKind.Absolute);
        //        //Debug.WriteLine(u.AbsoluteUri);
        //        //ImageUrl.Text = u.AbsoluteUri;
        //        //MyAvatar.Source = new BitmapImage(u);
        //        //Debug.WriteLine(reader2.ReadToEnd());
        //        string imageUrl = reader2.ReadToEnd();
        //        ImageSong.Source = new BitmapImage(new Uri(imageUrl, UriKind.Absolute));
        //        ImageUrl.Text = imageUrl;
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("Error uploading file", ex.StackTrace);
        //        Debug.WriteLine("Error uploading file", ex.InnerException);
        //        if (wresp != null)
        //        {
        //            wresp = null;
        //        }
        //    }
        //    finally
        //    {
        //        wr = null;
        //    }
        //}

        //private string GetUploadUrl()
        //{
        //    var httpClient = new HttpClient();
        //    // thực hiện gửi dữ liệu với phương thức post.
        //    var response = httpClient.GetAsync(GET_UPLOAD_URL).GetAwaiter().GetResult();
        //    // lấy kết quả trả về từ server.
        //    var uploadUrl = response.Content.ReadAsStringAsync().Result;
        //    Debug.WriteLine("Upload url: " + uploadUrl);
        //    return uploadUrl;
        //}
        //private void CapturePhoto(object sender, RoutedEventArgs e)
        //{
        //    ProcessCaptureImage();
        //}
    }
}
