using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace AroundMe.Core
{
    public class FlikrImage
    {
        public Uri Image320 { get; set; }
        public Uri Image1024 { get; set; }
        public async static Task<List<FlikrImage>> GetFlikrImages(string flikrApiKey,string topic,double latitude=double.NaN, double longitude=double.NaN,double radius=double.NaN)
        {
            HttpClient client = new HttpClient();

            var baseUrl = getBaseUrl(flikrApiKey,topic,latitude,longitude,radius);
            
            string flikrResult = await client.GetStringAsync(baseUrl);
               
            FlikrData apiData = JsonConvert.DeserializeObject<FlikrData>(flikrResult);
            List<FlikrImage> images=new List<FlikrImage>();

            if (apiData.stat == "ok")
            {
                foreach (Photo data in apiData.photos.photo)
                {
                    FlikrImage img = new FlikrImage();
                    //get url for each photo
                    //http://farm{farmid}.staticflikr.com/{server-id}/{id}_{secret}{size}.jpg

                    string photoUrl = "http://farm{0}.staticflickr.com/{1}/{2}_{3}";
                    string baseFlikrUrl = string.Format(photoUrl, data.farm, data.server, data.id, data.secret);

                    img.Image320 = new Uri(baseFlikrUrl + "_n.jpg");
                    img.Image1024 = new Uri(baseFlikrUrl + "_b.jpg");

                    images.Add(img);
                }
            }
            return images;
 
        }
        private static string getBaseUrl(string flikrApiKey,string topic,double latitude=double.NaN,double longitude=double.NaN,double radius=double.NaN)
        {
            //Licenses
            //https://www.flickr.com/services/api/flickr.photos.licenses.getInfo.html
            /*<license id="4" name="Attribution License" url="http://creativecommons.org/licenses/by/2.0/" />
                  <license id="5" name="Attribution-ShareAlike License" url="http://creativecommons.org/licenses/by-sa/2.0/" />
                <license id="6" name="Attribution-NoDerivs License" url="http://creativecommons.org/licenses/by-nd/2.0/" />
               <license id="7" name="No known copyright restrictions" url="http://flickr.com/commons/usage/" />
              */

            
            string license = "4,5,6,7";
            license=license.Replace(",", "%2C");

            if (!double.IsNaN(latitude))
            latitude = Math.Round(latitude,4);

            if(!double.IsNaN(longitude))
            longitude = Math.Round(longitude,4);

            string url = "http://api.flickr.com/services/rest/" +
                "?method=flickr.photos.search" +
                "&license={0}" +
                "&api_key={1}" +
                "&format=json" +
                "&nojsoncallback=1";
            
            var baseUrl = string.Format(url, license, flikrApiKey);
            if (!string.IsNullOrWhiteSpace(topic))
                baseUrl += string.Format("&text=%22{0}%22", topic);
            if (!double.IsNaN(latitude) && !double.IsNaN(longitude))
                baseUrl += string.Format("&lat{0}&lon={1}", latitude, longitude);

            if (!double.IsNaN(radius))
                baseUrl += string.Format("&radius={0}", radius);

            return baseUrl;
        }
    }
}
