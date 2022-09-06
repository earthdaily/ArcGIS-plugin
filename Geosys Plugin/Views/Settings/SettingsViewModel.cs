using ArcGIS.Desktop.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Web.WebView2;
using Microsoft.Web.WebView2.Wpf;
using System.Diagnostics;
using Geosys_Plugin.WebServices;
using Geosys_Plugin.Models.APIResponses;
using Geosys_Plugin.Views.Plugin;
using System.Windows;
using System.Windows.Controls;
using System.Security;
using ArcGIS.Desktop.Framework.Controls;
using Geosys_Plugin.Models;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace Geosys_Plugin.Views.Settings
{
    internal class SettingsViewModel : BaseViewModel
    {
        private string _username;
        private string _password;
        private string _clientId;
        private string _clientSecret;
        private string _cropType;
        private string _outputDirectory;
        private int _maxItemsPerRequest;
        private bool _useTestService;
        private DateTime _sowingDate;
        WebUrls _webUrls;

        private List<string> _cropTypesLOV;

        private AuthenticationService _webService;
        public SettingsViewModel()
        {
            this.Username = Properties.Settings.Default.Username;
            this.ClientId = Properties.Settings.Default.ClientId;
            this.ClientSecret = Properties.Settings.Default.ClientSecret;
            _webService = new AuthenticationService();
            _cropType = Properties.Settings.Default.CropType;
            _sowingDate = Properties.Settings.Default.SowingDate;
            _outputDirectory = Properties.Settings.Default.OutputDirectory;
            _maxItemsPerRequest = Properties.Settings.Default.MaxItemsPerRequest;
            _cropTypesLOV = new List<string>();
            LOV listOfValues = ReadLovFile();
            _webUrls = new APIUrls().web;
            _cropTypesLOV = listOfValues.CropTypes;
            _useTestService = Properties.Settings.Default.UseTestingService;
        }

        public string Username
        {
            get { return _username; }
            set
            {
                SetProperty(ref _username, value, () => Username);
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                SetProperty(ref _password, value, () => Password);
            }
        }
        public string ClientSecret
        {
            get { return _clientSecret; }
            set
            {
                SetProperty(ref _clientSecret, value, () => ClientSecret);
            }
        }
        public string ClientId
        {
            get { return _clientId; }
            set
            {
                SetProperty(ref _clientId, value, () => ClientId);
            }
        }

        public string CropType
        {
            get { return _cropType; }
            set
            {
                SetProperty(ref _cropType, value, () => CropType);
            }
        }

        public bool UseTestService
        {
            get { return _useTestService; }
            set
            {
                SetProperty(ref _useTestService, value, () => UseTestService);
            }
        }

        public string OutputDirectory
        {
            get { return _outputDirectory; }
            set
            {
                SetProperty(ref _outputDirectory, value, () => OutputDirectory);
            }
        }

        public int MaxItemsPerRequest
        {
            get { return _maxItemsPerRequest; }
            set
            {
                SetProperty(ref _maxItemsPerRequest, value, () => MaxItemsPerRequest);
            }
        }

        public DateTime SowingDate
        {
            get { return _sowingDate; }
            set
            {
                SetProperty(ref _sowingDate, value, () => SowingDate);
            }
        }

        public List<string> CropTypesLOV
        {
            get { return _cropTypesLOV; }
        }

        #region Commands

        public ICommand OpenAboutCmd
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    System.Diagnostics.Process.Start(_webUrls.AboutUrl);
                }, true);
            }
        }

        public ICommand OpenCredentialsCmd
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    System.Diagnostics.Process.Start(_webUrls.CredentialUrl);
                }, true);
            }
        }


        /// <summary>
        /// Command to browse for output directory
        /// </summary>
        private RelayCommand _browseCommand;
        public ICommand BrowseCommand
        {
            get
            {
                if (_browseCommand == null)
                    _browseCommand = new RelayCommand(BrowseImpl, () => true);
                return _browseCommand;
            }
        }

        /// <summary>
        /// Displayes the Browse dialog. 
        /// </summary>
        /// <param name="param"></param>
        private void BrowseImpl(object param)
        {
            var filename = OpenBrowseDialog();
            OutputDirectory = filename;
        }

        /// <summary>
        /// Display the browse dialog
        /// </summary>
        /// <returns></returns>
        private string OpenBrowseDialog()
        {
            using (var dlg = new System.Windows.Forms.FolderBrowserDialog())
            {
                dlg.Description = "Select Output folder";
                dlg.SelectedPath = OutputDirectory;
                dlg.ShowNewFolderButton = false;
                System.Windows.Forms.DialogResult result = dlg.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    return dlg.SelectedPath;
                }
                else
                {
                    return OutputDirectory;
                }
            }
        }

        public ICommand ConnectCmd
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    AuthenticationTokenInput input = new AuthenticationTokenInput(ClientId, Username, Password, ClientSecret);
                    Properties.Settings.Default.UseTestingService = UseTestService;
                    AuthenticationTokenResponse token = await _webService.GetAuthenticationToken(input);
                    string messageBoxText = "AuthenticationError: Ensure your username, password, client id, and client secret are valid for the selected region service and then try again.";
                    string caption = "Geosys API Authentication status";
                    MessageBoxImage icon = MessageBoxImage.Error;
                    MessageBoxButton button = MessageBoxButton.OK;
                    if (token != null && token.access_token != null)
                    {
                        Properties.Settings.Default.AuthToken = token.access_token;
                        messageBoxText = "Authentication succeeded";
                        icon = MessageBoxImage.Information;
                    }
                    MessageBox.Show(messageBoxText, caption, button, icon);
                }, true);
            }
        }

        public ICommand OkCmd => new RelayCommand(async (proWindow) =>
                {
                    Properties.Settings.Default.Username = Username;
                    Properties.Settings.Default.ClientId = ClientId;
                    Properties.Settings.Default.ClientSecret = ClientSecret;
                    Properties.Settings.Default.CropType = CropType;
                    Properties.Settings.Default.Password = Password;
                    Properties.Settings.Default.OutputDirectory = OutputDirectory;
                    Properties.Settings.Default.UseTestingService = UseTestService;
                    if (SowingDate == null)
                    {
                        SowingDate = new DateTime();
                    }
                    if (ClientId != null && Username != null && Password != null && ClientSecret != null)
                    {
                        AuthenticationTokenInput input = new AuthenticationTokenInput(ClientId, Username, Password, ClientSecret);
                        AuthenticationTokenResponse token = await _webService.GetAuthenticationToken(input);
                        string messageBoxText = "AuthenticationError: Ensure your username, password, client id, and client secret are valid for the selected region service and then try again.";
                        string caption = "Geosys API Authentication status";
                        MessageBoxImage icon = MessageBoxImage.Error;
                        MessageBoxButton button = MessageBoxButton.OK;
                        if (token != null && token.access_token != null)
                        {
                            Properties.Settings.Default.AuthToken = token.access_token;
                        }
                        else
                        {
                            MessageBox.Show(messageBoxText, caption, button, icon);
                            return;
                        }
                    }
                    Properties.Settings.Default.SowingDate = SowingDate;
                    Properties.Settings.Default.MaxItemsPerRequest = MaxItemsPerRequest;
                    Properties.Settings.Default.Save();
                    (proWindow as ProWindow).Close();
                }, () => true);


        public ICommand CancelCmd => new RelayCommand((proWindow) =>
        {
            (proWindow as ProWindow).Close();
        }, () => true);

        #endregion Commands

        private static LOV ReadLovFile()
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Geosys_Plugin.Config.LOV.json"))
            using (StreamReader reader = new StreamReader(stream))
            {
                var jsonContent = reader.ReadToEnd();
                var config = JsonConvert.DeserializeObject<LOV>(jsonContent);
                return config;
            }
        }

    }

}
