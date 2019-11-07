using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using Windows.Storage;
using Newtonsoft.Json;
using SQLitePCL;
using T1808aUWP.Entity;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace T1808aUWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ScrollViewDemo
    {
        private const string GetInforUrl = "https://2-dot-backup-server-003.appspot.com/_api/v2/members/information";
        private List<Student> listStudents;
        public ScrollViewDemo()
        {
            //LoadDatabase();
            Debug.WriteLine("Init Infor User.");
            ReadTokenFromFile();

            InitializeComponent();
        }

        private async void ReadTokenFromFile()
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
                    TokenKey.Text = memberCredential.token;

                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(string.Format(GetInforUrl));
                    //httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "GET";
                    httpWebRequest.Headers.Add("Authorization", "Basic" + " " + memberCredential.token);
                    var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    Debug.WriteLine(httpWebResponse.StatusCode);
                    Debug.WriteLine(httpWebResponse.Server);

                    string jsonString;
                    using (Stream stream = httpWebResponse.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream ?? throw new InvalidOperationException(), Encoding.UTF8);
                        jsonString = reader.ReadToEnd();
                    }

                    Member member = JsonConvert.DeserializeObject<Member>(jsonString);
                    UId.Text = member.id.ToString();
                    FirstName.Text = member.firstName;
                    LastName.Text = member.lastName;
                    Avatar.Text = member.avatar;
                    Phone.Text = member.phone;
                    Address.Text = member.address;
                    Gender.Text = member.gender.ToString();
                    Birthday.Text = member.birthday;
                    Email.Text = member.email;
                    //Debug.WriteLine(jsonString);
                    // Lấy thông tin người dùng\

                    // qua api https://2-dot-backup-server-003.appspot.com/_api/v2/members/information
                }
            }
            catch (Exception)
            {
                Frame.Navigate(typeof(LoginPage));
            }


            //Debug.WriteLine(memberCredential.token);

            //Debug.WriteLine(content);
            //var memberCredential = new MemberCredential();
            //memberCredential.token = Guid.NewGuid().ToString();
            //memberCredential.secretToken = memberCredential.token;
            //memberCredential.createdTimeMLS = DateTime.Now.Millisecond;
            //memberCredential.expiredTimeMLS = DateTime.Today.AddDays(7).Millisecond;
            //memberCredential.status = 1;
            //Debug.WriteLine(memberCredential.token);
        }

        //private void LoadDatabase()
        //{

        //    SQLiteConnection conn;
        //    //Get a reference to the SQLite database    
        //    conn = new SQLiteConnection("sqlitepcldemo.db");
        //    string sql = @"CREATE TABLE IF NOT EXISTS Student (Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,Name VARCHAR( 140 ),City VARCHAR( 140 ),Contact VARCHAR( 140 ));";
        //    using (var statement = conn.Prepare(sql)) { statement.Step(); }


        //    try
        //    {
        //        using (var custstmt = conn.Prepare("INSERT INTO Student (Name, City, Contact) VALUES (?, ?, ?)"))
        //        {
        //            custstmt.Bind(1, "Dat");
        //            custstmt.Bind(2, "Hai duong");
        //            custstmt.Bind(3, "Dep trai ");
        //            custstmt.Step();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // TODO: Handle error}
        //    }

        //}

        //private void Menu_Click(object sender, TappedRoutedEventArgs e)
        //{
        //    this.MySplitView.IsPaneOpen = !this.MySplitView.IsPaneOpen;
        //}

        //private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        //{
        //    if (sender is HyperlinkButton hyperlink)
        //    {
        //        switch (hyperlink.Tag)
        //        {
        //            case "Register":
        //                MainContent.Navigate(typeof(RegisterPage2));
        //                break;
        //            case "Login":
        //                MainContent.Navigate(typeof(LoginPage));
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //}
    }
}
