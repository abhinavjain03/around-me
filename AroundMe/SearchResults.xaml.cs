using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Animation;
using AroundMe.Core;

namespace AroundMe
{
    public partial class SearchResults : PhoneApplicationPage
    {
        private string _topic;
        private const string FlikrApiKey = "3485eafc818fe27ca5ebe727528810c8";
        private  double _latitude;
        private double _longitude;
        private double _radius;
        public SearchResults()
        {
            InitializeComponent();
            Loaded += SearchResults_Loaded;
            BuildLocalizedApplicationBar();
        }

        async void SearchResults_Loaded(object sender, RoutedEventArgs e)
        {
            //Overlay.Visibility = Visibility.Visible;
            //OverlayProgressBar.IsIndeterminate = true;
            SystemTray.ProgressIndicator = new ProgressIndicator();
            SystemTray.ProgressIndicator.IsIndeterminate = true;
            SystemTray.ProgressIndicator.IsVisible = true;
            SystemTray.ProgressIndicator.Text = "Getting Photos";

            var images = await FlikrImage.GetFlikrImages(FlikrApiKey,_topic, _latitude, _longitude,_radius);
            DataContext = images;

            SystemTray.ProgressIndicator.IsIndeterminate = false;
            SystemTray.ProgressIndicator.Text = "Here are your photos";
            SystemTray.ProgressIndicator.IsVisible = false;

            //if (images.Count == 0)
            //    NoPhotosFound.Visibility = Visibility.Visible;
            //else
            //    NoPhotosFound.Visibility = Visibility.Collapsed;

            //Overlay.Visibility = Visibility.Collapsed;
            //OverlayProgressBar.IsIndeterminate=false;

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
                base.OnNavigatedTo(e);
                _latitude = Convert.ToDouble(NavigationContext.QueryString["latitude"]);
                _longitude = Convert.ToDouble(NavigationContext.QueryString["longitude"]);
                _radius = Convert.ToDouble(NavigationContext.QueryString["radius"]);
                _topic = NavigationContext.QueryString["topic"];
        }

        private void PhotosForLockscreen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void Image_ImageOpened(object sender, RoutedEventArgs e)
        {
            Image img = sender as Image;
            if (img == null)
                return;

            Storyboard s = new Storyboard();
            DoubleAnimation doubleAni = new DoubleAnimation();
            doubleAni.To = 1;
            doubleAni.Duration = new Duration(TimeSpan.FromMilliseconds(2000));
            Storyboard.SetTarget(doubleAni, img);
            Storyboard.SetTargetProperty(doubleAni,new PropertyPath(OpacityProperty));
            s.Children.Add(doubleAni);
            s.Begin();



        }

        // Sample code for building a localized ApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();
            ApplicationBar.IsVisible = false;

            //    // Create a new button and set the text value to the localized string from AppResources.
            ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Toolkit.Content/ApplicationBar.Check.png", UriKind.Relative));
            appBarButton.Text = "Set";
            appBarButton.Click += appBarButton_Click;
            ApplicationBar.Buttons.Add(appBarButton);

           
        }

        async void appBarButton_Click(object sender, EventArgs e)
        {
            List<FlikrImage> imgs = new List<FlikrImage>();
            foreach (object item in PhotosForLockScreen.SelectedItems)
            {
                FlikrImage img = item as FlikrImage;
                if (img != null)
                    imgs.Add(img);
            }
           //Clean out aall images in isolated storage
            LockScreenHelpers.CleanStorage();

            //Save this new list in isolated storage
            LockScreenHelpers.SaveSelectedBackgroundScreens(imgs);

            //Randomly select one item and use it as lockscreen
            await LockScreenHelpers.SetRandomImageFormLocalStorage();


            MessageBox.Show("You have a new lockscreen image","Set!", MessageBoxButton.OK);
        }

    }
}