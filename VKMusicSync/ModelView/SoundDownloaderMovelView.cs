﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Threading;
using System.ComponentModel;
using System.Net;

using MVVM;
using VKMusicSync.Model;
using VKMusicSync.ModelView;
using VKMusicSync.Handlers.Synchronize;
using vkontakte;
using VKMusicSync.Handlers;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Security.Cryptography;

namespace VKMusicSync.ModelView
{
    class SoundDownloaderMovelView: ViewModelBase
    {
        #region Private variables

        private Profile Profile = new Profile();
        private List<Sound> sounds = new List<Sound>();

        #endregion

        #region Binding variables


        //public System.Windows.Visibility ProgressVisibility = System.Windows.Visibility.Visible;


        public ObservableCollection<SoundModelView> Sounds { get; set; }


        public string UserFullName
        {
            get
            { return Profile.first_name + " " + Profile.last_name; }
            set
            {
                OnPropertyChanged("UserFullName");
            }
        }

        //private System.Windows.Media.Imaging.BitmapFrame avatar;
        private string avatar;
       // public System.Windows.Media.Imaging.BitmapFrame Avatar
        public string Avatar 
        {
            get
            {
                return avatar;
            }
            set
            {
                avatar = value;
                OnPropertyChanged("Avatar");
            }
        }

        private double progressPercentage = 0;
        public double ProgressPercentage
        {
            get
            {
                return progressPercentage;
            }
            set
            {
                progressPercentage = value;
                OnPropertyChanged("ProgressPercentage");
            }
        }
        #endregion

        #region Click Commands

        private DelegateCommand downloadFiles;
        public ICommand DownloadFiles
        {
            get
            {
                if (downloadFiles == null)
                {
                    downloadFiles = new DelegateCommand(OnDownloadFiles);
                }
                return downloadFiles;
            }

        }

        private DelegateCommand settings;
        public ICommand SettingsClick
        {
            get
            {
                if (settings == null)
                {
                    settings = new DelegateCommand(OnSettingsClick);
                }
                return settings;
            }

        }

        private DelegateCommand sync;
        public ICommand SyncClick
        {
            get
            {
                if (sync == null)
                {
                    sync = new DelegateCommand(OnSyncClick);
                }
                return sync;
            }

        }
        #endregion

        #region Constructor
        public SoundDownloaderMovelView(List<Sound> sounds)
        {
            SetSounds(sounds);
        }

        public SoundDownloaderMovelView()
        {
            sounds = new List<Sound>();
            this.Sounds = new ObservableCollection<SoundModelView>();
        }

        private void SetSounds(List<Sound> sounds)
        {
            Sounds = new ObservableCollection<SoundModelView>(sounds.Select(s => new SoundModelView(s)));
        }
        #endregion

        #region FormsActions

        private void OnSyncClick()
        {
            ComputeDataToSync();
        }

        private void OnSettingsClick()
        {
            System.Windows.Forms.MessageBox.Show("SettingsClick");
        }

        private void OnAuthClick()
        {
            var authWindow = new Auth();
            authWindow.ShowDialog();
            //System.Windows.Forms.MessageBox.Show("OnSyncClick");
        }

        public void Window_Loaded()
        {
            OnAuthClick();
            ComputeDataToSync();
        }

        #endregion

        #region Process vk data to forms

        private void ComputeDataToSync()
        {
            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += this.Init;
            backgroundWorker.RunWorkerCompleted += this.InitDone;
            backgroundWorker.RunWorkerAsync();
        }

        

        private void Init(object sender, DoWorkEventArgs e)
        {

            int count = int.Parse(APIManager.vk.GetAudioCountFromUser(APIManager.vk.UserId, 
                                                                      false).SelectSingleNode("response")
                                                                            .InnerText);
            ProfileCommand profCommand = vkontakte.CommandsGenerator.GetUsers(APIManager.AccessData.UserId);
            this.Profile = profCommand.ExecuteForList().FirstOrDefault();
            var paths = (new List<string>() { Profile.photo, Profile.photoMedium, Profile.photoBig });
            var path = string.Empty;
            foreach(var p in paths)
                if (p.Count()>0) 
                {
                    path = p;
                    break;
                }
            if (path != string.Empty)
                Avatar = path;
            
            this.UserFullName = Profile.last_name;

            if (count > 0)
            {
                AudioCommand command = vkontakte.CommandsGenerator.GetAudioFromUser(APIManager.vk.UserId, false, 0, count);
                command.OnCommandExecuting += OnCommandLoading;
                sounds = command.ExecuteForList();
            }
        }

        private void OnCommandLoading(Object sender, DownloadProgressChangedEventArgs e)
        {
            this.ProgressPercentage = (double)Math.Abs(1 - e.ProgressPercentage);
        }

        private void InitDone(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Sounds.Clear();
            for(int i=0; i<sounds.Count; i++)
                this.Sounds.Add(new SoundModelView(sounds[i]));
            SetSounds(sounds);
            this.ProgressPercentage = 100;
        }

        #endregion
        public static string DecodeUrlString(string url)
        {
            string newUrl;
            while ((newUrl = Uri.UnescapeDataString(url)) != url)
                url = newUrl;
            return newUrl;
        }
        #region Load audio

        private SynhronizeAdapter adapter = new SynhronizeAdapter();

        private void OnDownloadFiles()
        {
            AudioUploadComman comm = CommandsGenerator.GetUploadServer();
            comm.Execute();
            var info = comm.UploadAudio(@"D:\Musik\", @"myzuka.ru_08_nobody_but_me.mp3");
  

            /*var audio = "%7B%22audio%22%3A%222e26475e89%22%2C%22time%22%3A138%2C%22artist%22%3A%22The+Human+Beinz%22%2C%22title%22%3A%22Nobody+But+Me%22%2C%22genre%22%3A24%2C%22album%22%3A%22The+Departed%22%2C%22bitrate%22%3A320%2C%22md5%22%3A%2292c8c6fdcd25c6998b3b86a1deaa99fa%22%2C%22kad%22%3A%2212005fec2aabe7208a349f46b98e96ff%5Cn%22%7D";
            audio = DecodeUrlString(audio);

            var info = new AudioUploadedInfo("536214", audio, "a71d783bad416ff57f703438eeaacf37");*/

            AudioUploadComman audioCommand = CommandsGenerator.SaveAudio(info);
            var fullstr = audioCommand.QueryString;
            var paramsAndtoken = @"?"+audioCommand.GetParamsWithToken();
            //paramsAndtoken = @"?uid=15852307&fields=uid, first_name, last_name, nickname, sex, bdate, city, countryphoto, photo_medium, photo_big&access_token=f734a0848b5e7843e423e3acf72fd35736dfcff1e87b0e8aa5d22ddafd778c95c784dd8995b454d6e6b48";
            //paramsAndtoken = @"?server=536518&audio=%7B%22audio%22%3A%22a0e144777d%22%2C%22time%22%3A138%2C%22artist%22%3A%22The+Human+Beinz%22%2C%22title%22%3A%22Nobody+But+Me%22%2C%22genre%22%3A24%2C%22album%22%3A%22The+Departed%22%2C%22bitrate%22%3A320%2C%22md5%22%3A%2292c8c6fdcd25c6998b3b86a1deaa99fa%22%2C%22kad%22%3A%2212005fec2aabe7208a349f46b98e96ff%5Cn%22%7D&hash=21f7deda067d8930bf963f0159308348&artist=artist&title=title&access_token=69fedfbf0e43a0e942c5e1bf7158f25204224349a0e005890a9db1b63c181c9a69bc40ef3043b0ca1585a";
            string asnwer = Reqeust.POST("https://api.vk.com/method/audio.save", paramsAndtoken);
            //audioCommand.Execute();
            /*BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += this.DoWork;
            backgroundWorker.RunWorkerCompleted += this.OnCompletedAllLoad;
            backgroundWorker.RunWorkerAsync(sounds);*/
        }

        private void OnChangeLoadFile(object sender, DownloadProgressChangedEventArgs e)
        {
            this.ProgressPercentage = ((float)adapter.CurrentFileNumber / (float)adapter.FilesCount * 100);
        }

        private void OnCompleteLoadFile(object sender, DownloadDataCompletedEventArgs e)
        {
            this.ProgressPercentage = 100;
        }

        private void OnCompletedAllLoad(object sender, RunWorkerCompletedEventArgs e)
        {
            string directory = @"audio\";
            IOHandler.OpenPath(directory);
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            
            adapter.OnLoaded += this.OnCompleteLoadFile;
            adapter.OnProgress += this.OnChangeLoadFile;
            List<SoundModelView> soundModelList = new List<SoundModelView>();
            soundModelList.AddRange(Sounds);
            adapter.SyncFolderWithList<SoundModelView>(soundModelList, @"audio\");
        }
        #endregion


    }
}
