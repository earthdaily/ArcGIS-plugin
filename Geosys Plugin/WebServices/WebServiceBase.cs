using Geosys_Plugin.Models.APIResponses;
using Geosys_Plugin.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Geosys_Plugin
{

    public abstract class WebServiceBase
    {
        static readonly HttpClient client = new HttpClient();

        protected async Task<T> GetRequestByAuthentication<T>(string url) where T : class
        {
            T responseJSON = null;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Properties.Settings.Default.AuthToken);
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                responseJSON = JsonConvert.DeserializeObject<T>(responseBody);
            }
            else
            {
                throw new Exception(response.StatusCode.ToString());
            }
            return responseJSON;
        }

        protected async Task<BitmapImage> GetImageRequestByAuthentication<T>(string url) where T : class
        {
            BitmapImage bitmap = null;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Properties.Settings.Default.AuthToken);
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                Stream imageSource = await response.Content.ReadAsStreamAsync();
                bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = imageSource;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
            }
            else
            {
                return null;
            }

            return bitmap;
        }

        protected async Task<string> GetTifZipAndSaveAsTif<T>(string url, string fileName) where T : class
        {
            string savedPath = null;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Properties.Settings.Default.AuthToken);
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                Stream imageSource = await response.Content.ReadAsStreamAsync();

                using (ZipArchive zip = new ZipArchive(imageSource))
                {
                    if (zip.Entries.Count > 0)
                    {
                        savedPath = string.Format(@"{0}\{1}{2}", Properties.Settings.Default.OutputDirectory, fileName, ".tif");
                        FileInfo fileInfo = FileUtils.incrementNameIfFileExists(savedPath);
                        fileInfo.Directory.Create();
                        savedPath = fileInfo.FullName;
                        using (var tifFile = zip.Entries[0].Open())
                        {
                            using (FileStream fs = File.OpenWrite(savedPath))
                            {
                                tifFile.CopyTo(fs);
                            }
                        }
                    }

                }

            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }

            return savedPath;
        }

        protected async Task<string> GetShapeZipAndSaveLocally<T>(string url, string fileName) where T : class
        {
            string savedShapePath = null;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Properties.Settings.Default.AuthToken);
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                Stream imageSource = await response.Content.ReadAsStreamAsync();

                using (ZipArchive zip = new ZipArchive(imageSource))
                {
                    if (zip.Entries.Count > 0)
                    {
                        foreach (var file in zip.Entries)

                        {
                            using (var openedFile = file.Open())
                            {

                                string savedPath = string.Format(@"{0}\{1}{2}", Properties.Settings.Default.OutputDirectory, fileName, Path.GetExtension(file.FullName));
                                FileInfo fileInfo = FileUtils.incrementNameIfFileExists(savedPath);
                                fileInfo.Directory.Create();
                                savedPath = fileInfo.FullName;
                                if (file.FullName.EndsWith(".shp"))
                                {
                                    savedShapePath = string.Format(savedPath);
                                }
                                using (FileStream fs = File.OpenWrite(savedPath))
                                {
                                    openedFile.CopyTo(fs);
                                }
                            }
                        }
                    }

                }

            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }

            return savedShapePath;
        }

        protected async Task<string> GetPngAndSaveLocally<T>(string pngUrl, string pgwUrl, string legendUrl, string fileName) where T : class
        {
            BitmapImage bitmap = null;
            string savedShapePath = null;
            string savedFolder = null;
            string fullPngPath = null;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Properties.Settings.Default.AuthToken);
            HttpResponseMessage response = await client.GetAsync(pngUrl);
            if (response.IsSuccessStatusCode)
            {

                Stream imageSource = await response.Content.ReadAsStreamAsync();
                fullPngPath = string.Format(@"{0}\{1}{2}", Properties.Settings.Default.OutputDirectory, fileName, ".png");
                FileInfo fileInfo = FileUtils.incrementNameIfFileExists(fullPngPath);
                fileInfo.Directory.Create();
                fullPngPath = fileInfo.FullName;
                using (Stream output = File.OpenWrite(fullPngPath))
                {
                    imageSource.CopyTo(output);
                }

            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }

            HttpResponseMessage response2 = await client.GetAsync(pgwUrl);
            if (response2.IsSuccessStatusCode)
            {
                Stream imageSource = await response2.Content.ReadAsStreamAsync();
                string fullPath = string.Format(Path.ChangeExtension(fullPngPath, "pgw"));
                FileInfo fileInfo = new FileInfo(fullPath);
                fileInfo.Directory.Create();
                using (Stream output = File.OpenWrite(fullPath))
                {
                    imageSource.CopyTo(output);
                }

            }
            else
            {
                throw new Exception(response2.ReasonPhrase);
            }

            HttpResponseMessage response3 = await client.GetAsync(legendUrl);
            if (response2.IsSuccessStatusCode)
            {
                Stream imageSource = await response3.Content.ReadAsStreamAsync();
                string fullPath = string.Format(Path.ChangeExtension(fullPngPath, null));
                fullPath = fullPath + "_legend.png";
                FileInfo fileInfo = new FileInfo(fullPath);
                fileInfo.Directory.Create();
                using (Stream output = File.OpenWrite(fullPath))
                {
                    imageSource.CopyTo(output);
                }

            }
            else
            {
                throw new Exception(response3.ReasonPhrase);
            }

            return fullPngPath;
        }

        protected async Task<string> GetKmzAndSaveLocally<T>(string kmzUrl, string fileName) where T : class
        {
            BitmapImage bitmap = null;
            string savedShapePath = null;
            string savedFolder = null;
            string fullPath = null;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Properties.Settings.Default.AuthToken);
            HttpResponseMessage response = await client.GetAsync(kmzUrl);
            if (response.IsSuccessStatusCode)
            {

                Stream imageSource = await response.Content.ReadAsStreamAsync();
                fullPath = string.Format(@"{0}\{1}{2}", Properties.Settings.Default.OutputDirectory, fileName, ".kmz");
                FileInfo fileInfo = FileUtils.incrementNameIfFileExists(fullPath);
                fileInfo.Directory.Create();
                fullPath = fileInfo.FullName;
                using (Stream output = File.OpenWrite(fullPath))
                {
                    imageSource.CopyTo(output);
                }
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }

            return fullPath;
        }

        public async Task<T> PostRequest<T>(string url, Dictionary<string, string> body) where T : class
        {
            client.DefaultRequestHeaders.Authorization = null;
            T responseJSON = null;
            try
            {
                HttpResponseMessage response = await client.PostAsync(url, new FormUrlEncodedContent(body));
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                responseJSON = JsonConvert.DeserializeObject<T>(responseBody);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            return responseJSON;
        }

        public async Task<T> PostRequestWithAuthentication<T>(string url, string body) where T : class
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Properties.Settings.Default.AuthToken);
            T responseJSON = null;
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            HttpResponseMessage response = await client.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                responseJSON = JsonConvert.DeserializeObject<T>(responseBody);
            }
            else
            {
                throw new Exception(response.StatusCode.ToString());
            }

            return responseJSON;
        }
    }
}
