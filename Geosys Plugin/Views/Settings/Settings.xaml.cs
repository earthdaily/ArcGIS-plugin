using ArcGIS.Desktop.Framework;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Geosys_Plugin.Views.Settings
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : ArcGIS.Desktop.Framework.Controls.ProWindow
    {
        public Settings()
        {
            InitializeComponent();
            this.DataContext = new SettingsViewModel();
            this.passwordBoxInput.Password = Properties.Settings.Default.Password;
        }


        #region Commands

 
        #endregion Commands

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            { ((dynamic)this.DataContext).Password = ((PasswordBox)sender).Password; }
        }

    }
}
