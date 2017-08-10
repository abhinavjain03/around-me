using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using AroundMe.Resources;
using System.Device.Location;
using Windows.Devices.Geolocation;
using System.Net.Http;
using Newtonsoft.Json;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Maps;


namespace AroundMe
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            BuildLocalizedApplicationBar();
            Loaded += MainPage_Loaded;
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            SystemTray.ProgressIndicator = new ProgressIndicator(); 
            UpdateMap();
        }
        private static void SetProgressIndicator(Boolean isVisible)
        {
            SystemTray.ProgressIndicator.IsIndeterminate = isVisible;
            SystemTray.ProgressIndicator.IsVisible = isVisible;
        }

        private async void UpdateMap()
        {
            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;

            SetProgressIndicator(true);

            try
            {
                SystemTray.ProgressIndicator.Text = "Getting GPS location";

                Geoposition position = await geolocator.GetGeopositionAsync(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(30));

                SystemTray.ProgressIndicator.Text = "Acquired";
                var gpsCoorCenter = new GeoCoordinate(position.Coordinate.Latitude, position.Coordinate.Longitude);
                //AroundMeMap.SetView(gpsCoorCenter, 17);
                // AroundMeMap.SetView(new GeoCoordinate(26.9381D, 75.7412D), 14D);


                AroundMeMap.Center = gpsCoorCenter;
                AroundMeMap.ZoomLevel = 15;
                SetProgressIndicator(false);

            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Loacation is disabled in the phone settings");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
            

            
                
            }

        // Sample code for building a localized ApplicationBar
        private void BuildLocalizedApplicationBar()
        {
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
             ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
            ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/feature.search.png", UriKind.Relative));
            appBarButton.Text = AppResources.AppBarButtonText;
            appBarButton.Click += SearchClick;
            ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
           ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
           ApplicationBar.MenuItems.Add(appBarMenuItem);
        }

        private void SearchClick(object sender, EventArgs e)
        {
            string topic = HttpUtility.HtmlEncode(SearchTopic.Text);
            string navTo = string.Format("/SearchResults.xaml?latitude={0}&longitude={1}&topic={2}&radius={3}",AroundMeMap.Center.Latitude,AroundMeMap.Center.Longitude,topic,10);
            NavigationService.Navigate(new Uri(navTo,UriKind.RelativeOrAbsolute));
        }

        private void AroundMeMap_Loaded(object sender, RoutedEventArgs e)
        {
            //MapsSettings.ApplicationContext.ApplicationId = "";
            //Get from publish on dev.windowsphone.com
            //MapsSettings.ApplicationContext.AuthenticationToken = "";

        }
    }
}