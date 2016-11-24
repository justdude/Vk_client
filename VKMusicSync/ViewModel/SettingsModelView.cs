﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VKMusicSync.Model;
using System.Windows.Input;
using System.Net;
using System.Collections.ObjectModel;
using MIP.MVVM;
using MIP.Commands;
using VkDay.vkontakte;
using VKMusicSync.Handlers.IoC;
using Microsoft.Practices.Unity;
using VKMusicSync.Handlers.Coloring;


namespace VKMusicSync.ViewModel
{
	class SettingsViewModel : AdwancedViewModelBase
	{

		#region Properties

		public string UserFullName
		{
			get { return wrapper.IsUserLoaded ? wrapper.UserProfile.ToString() : string.Empty; }
		}

		public int ThreadMax
		{
			get
			{
				return Environment.ProcessorCount * 9;
			}
		}

		public int ThreadCountToUse
		{
			get
			{
				return Properties.Settings.Default.ThreadCountToUse;
			}
			set
			{
				Properties.Settings.Default.ThreadCountToUse = value;
				Properties.Settings.Default.Save();
				RaisePropertyChanged("ThreadCountToUse");
			}
		}



		public string DownloadFolderPath
		{
			get
			{
				return Properties.Settings.Default.DownloadFolderPath;
			}
			set
			{
				Properties.Settings.Default.DownloadFolderPath = value;
				Properties.Settings.Default.Save();
				RaisePropertyChanged("DownloadFolderPath");
			}
		}

		public string BackgroundPath
		{
			get
			{
				return Properties.Settings.Default.BackgroundPath;
			}
			set
			{
				Properties.Settings.Default.BackgroundPath = value;
				Properties.Settings.Default.Save();
				RaisePropertyChanged("BackgroundPath");
			}
		}

		#region Proxy
		private WebProxy proxy;
		public WebProxy Proxy
		{
			get
			{
				if (proxy == null)
				{
					proxy = new WebProxy(Properties.Settings.Default.ProxyAdress,
										 int.Parse(Properties.Settings.Default.ProxyPort));
					if (Properties.Settings.Default.UseProxyCredintial)
					{
						proxy.Credentials = Credential;
					}
					RaisePropertyChanged("Proxy");
				}
				return proxy;
			}
			set
			{
				proxy = value;
				RaisePropertyChanged("Proxy");
			}
		}

		public bool UseCredintial
		{
			get
			{
				return Properties.Settings.Default.UseProxyCredintial;
			}
			set
			{
				Properties.Settings.Default.UseProxyCredintial = value;
				Properties.Settings.Default.Save();
				RaisePropertyChanged("UseCredintial");
			}
		}

		public bool UseProxy
		{
			get
			{
				return Properties.Settings.Default.UseProxy;
			}
			set
			{
				Properties.Settings.Default.UseProxy = value;
				Properties.Settings.Default.Save();
				RaisePropertyChanged("UseProxy");
			}
		}

		private NetworkCredential credential;
		public NetworkCredential Credential
		{
			get
			{
				if (credential == null)
				{
					credential = new NetworkCredential(Properties.Settings.Default.ProxyName,
													   Properties.Settings.Default.ProxyPassword);
					RaisePropertyChanged("Credential");
				}
				return credential;
			}
			set
			{
				credential = value;
				RaisePropertyChanged("Credential");
			}
		}


		public string Adress
		{
			get
			{
				return Properties.Settings.Default.ProxyAdress;
			}
			set
			{
				Properties.Settings.Default.ProxyAdress = value;
				Properties.Settings.Default.Save();
				RaisePropertyChanged("Adress");
			}
		}

		public string Port
		{
			get
			{
				return Properties.Settings.Default.ProxyPort;
			}
			set
			{
				Properties.Settings.Default.ProxyPort = value;
				Properties.Settings.Default.Save();
				RaisePropertyChanged("Port");
			}
		}

		public string Login
		{
			get
			{
				return Properties.Settings.Default.ProxyName;
			}
			set
			{
				Properties.Settings.Default.ProxyName = value;
				Properties.Settings.Default.Save();
				RaisePropertyChanged("Login");
			}
		}


		public string Password
		{
			get
			{
				return Properties.Settings.Default.ProxyPassword;
			}
			set
			{
				Properties.Settings.Default.ProxyPassword = value;
				Properties.Settings.Default.Save();
				RaisePropertyChanged("Password");
			}
		}
		#endregion


		public List<System.Windows.Media.SolidColorBrush> ColorsScheme
		{
			get
			{
				return ColorJam.AllCollors;
			}
		}
		#endregion Properties

		#region Commands
		private DelegateCommand selectDownloadFolderClick;
		public ICommand SelectDownloadFolderClick
		{
			get
			{
				if (selectDownloadFolderClick == null)
				{
					selectDownloadFolderClick = new DelegateCommand(OnSelectDownloadFolderClick);
				}
				return selectDownloadFolderClick;
			}
		}

		private DelegateCommand selectBackgroundPathClick;
		public ICommand SelectBackgroundPathClick
		{
			get
			{
				if (selectBackgroundPathClick == null)
				{
					selectBackgroundPathClick = new DelegateCommand(OnSelectBackgroundPathClick);
				}
				return selectBackgroundPathClick;
			}
		}


		private DelegateCommand applySelectedBackground;
		public ICommand ApplySelectedBackground
		{
			get
			{
				if (applySelectedBackground == null)
				{
					applySelectedBackground = new DelegateCommand(OnApplySelectedBackground);
				}
				return applySelectedBackground;
			}
		}

		private DelegateCommand loginClick;
		public ICommand LoginClick
		{
			get
			{
				if (loginClick == null)
				{
					loginClick = new DelegateCommand(OnLoginClick, CanLogin);
				}
				return loginClick;
			}
		}

		private DelegateCommand exitClick;
		private IVkWrapper wrapper;
		public ICommand ExitClick
		{
			get
			{
				if (exitClick == null)
				{
					exitClick = new DelegateCommand(OnExitClick, CanExit);
				}
				return exitClick;
			}
		}
		#endregion

		#region Checkers
		private bool CanLogin()
		{
			return !wrapper.IsConnected;
		}

		private bool CanExit()
		{
			return !wrapper.IsConnected;
		}
		#endregion

		private void OnExitClick()
		{
			wrapper.Clear();
		}

		private void OnLoginClick()
		{
			wrapper.ShowAutorizationWindow();

			wrapper.AutorizedAction += (s, e) => wrapper.InitUser();
		}


		private void OnApplySelectedBackground()
		{

		}

		private void OnSelectBackgroundPathClick()
		{
			System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
			dialog.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg";
			dialog.ShowDialog();
			if (dialog.CheckFileExists)
			{
				this.BackgroundPath = dialog.FileName;
			}
		}
		private void OnSelectDownloadFolderClick()
		{
			System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
			dialog.ShowDialog();
			if (System.IO.Directory.Exists(dialog.SelectedPath) == true)
				this.DownloadFolderPath = dialog.SelectedPath + @"\Audio\";

			Refresh();
		}

		protected override void RefreshPrivate()
		{
			base.RefreshPrivate();

			RaisePropertyChanged(() => ColorsScheme);
			RaisePropertyChanged(() => BackgroundPath);
			RaisePropertyChanged(() => DownloadFolderPath);
			RaisePropertyChanged(() => ThreadCountToUse);
			RaisePropertyChanged(() => ThreadMax);
			RaisePropertyChanged(() => UserFullName);
			RaisePropertyChanged(() => UseCredintial);
			RaisePropertyChanged(() => UseProxy);
		}

		protected override void OnTokenChanged()
		{
			base.OnTokenChanged();

			wrapper = Unity.Instance.Resolve<IVkWrapper>();
		}
	}
}
