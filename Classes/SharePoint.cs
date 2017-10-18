using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace microsoft_graph_files_web_forms.Classes
{
    public class SharePoint
    {
        // helper classes for responses and file data
        public class GraphSessionResponse
        {
            public DateTime expirationDateTime { get; set; }
            public string[] nextExpectedRanges { get; set; }
            public string uploadUrl { get; set; }
        }
        public class FileData
        {
            public FileMetaData[] value { get; set; }

            public class FileMetaData
            {
                public DateTime createdDateTime { get; set; }
                public string id { get; set; }
                public DateTime lastModifiedDateTime { get; set; }
                public string name { get; set; }
                public string webUrl { get; set; }
                public string size { get; set; }
            }
        }
        public class FileTransfer
        {
            public HttpPostedFile File { get; set; }
            public int Size { get; set; }
            public int SliceSize { get; set; }
            public int Start { get; set; }
            public string UploadURL { get; set; }

        }

        // Attributes
        private string GRAPH_HOST_URL = "https://graph.microsoft.com/v1.0";
        private string DRIVE_ID = "{drive-id}";
        Auth auth = new Auth();

        // Constructor
        public SharePoint()
        {

        }

        // Behaviors
        //************************************************************************//
        //                       MICROSOFT GRAPH V1.0                             //
        //************************************************************************//

        /// <summary>
        /// Retreives SharePoint documents using Microsoft Graph v1.0
        /// </summary>
        /// <param name="FolderPath">Path after host name in the following format "/FolderName"</param>
        /// <returns></returns>
        public DataTable GetDocuments(string FolderPath)
        {

            DataTable dt = new DataTable("Documents");
            DataRow dr = dt.NewRow();
            dt.Columns.Add("Title");
            dt.Columns.Add("URL");
            dt.Columns.Add("Created");
            dt.Columns.Add("Modified");
            dt.Columns.Add("ID");

            if (auth.GetAppAccessToken())
            {
                string result = "";
                string URL = GRAPH_HOST_URL + "/drives/" + DRIVE_ID + "/root:" + FolderPath + ":/children?$Select=size,name,createdDateTime,id,lastModifiedDateTime,webUrl";


                HttpClient client = new HttpClient();
                client.BaseAddress = new System.Uri(URL);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AppAccessToken);
                HttpResponseMessage message = client.GetAsync(URL).Result;

                result = message.Content.ReadAsStringAsync().Result;

                if (message.IsSuccessStatusCode)
                {
                    SharePoint.FileData data = JsonConvert.DeserializeObject<SharePoint.FileData>(result);

                    foreach (var item in data.value)
                    {
                        dr = dt.NewRow();

                        dr["Title"] = item.name;
                        dr["URL"] = item.webUrl;
                        dr["Created"] = item.createdDateTime;
                        dr["Modified"] = item.lastModifiedDateTime;
                        dr["ID"] = item.id;

                        dt.Rows.Add(dr);
                    }
                }
                else
                {

                }
            }

            return dt;

        }

        /// <summary>
        /// Delete a SharePoint document using Microsoft Graph v1.0
        /// </summary>
        /// <param name="itemID">ID of the item in the drive</param>
        /// <returns></returns>
        public bool DeleteDocument(string itemID)
        {
            bool success = false;
            var URL = GRAPH_HOST_URL + "/drives/" + DRIVE_ID + "/items/" + itemID;

            if (auth.GetAppAccessToken())
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new System.Uri(URL);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + auth.AppAccessToken);
                HttpResponseMessage message = client.DeleteAsync(URL).Result;

                if (message.IsSuccessStatusCode)
                {
                    success = true;
                }
                else
                {
                    success = false;
                }
            }



            return success;
        }

        /// <summary>
        /// Upload a document to SharePoint using Microsoft Graph v1.0
        /// </summary>
        /// <param name="File">HttpPostedFile of File Upload Control</param>
        /// <param name="Folder">Folder Path to put the file. IE: "/Folder/SubFolder"</param>
        /// <returns></returns>
        public bool UploadDocument(HttpPostedFile File, string Folder)
        {
            // Function Variables
            Auth auth = new Auth();
            SharePoint.FileTransfer fileTransfer = new SharePoint.FileTransfer();
            string FileName = Path.GetFileName(File.FileName);
            bool success = false;
            fileTransfer.File = File;
            fileTransfer.Size = File.ContentLength;
            fileTransfer.SliceSize = 320 * 180000;
            fileTransfer.Start = 0;

            // Get access token
            if (auth.GetAppAccessToken())
            {
                // Create Upload Session
                fileTransfer.UploadURL = CreateUploadSession(FileName, Folder, auth.AppAccessToken);

                // Save file to local server                
                File.SaveAs(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/SupportDocuments/") + FileName);

                // upload chunk to session
                success = UploadToSession(fileTransfer);
            }
            else
            {
                // Failed to retrieve an access token... Handle error.
            }

            return success;

        }

        private bool UploadToSession(FileTransfer ft)
        {
            bool success = false;
            bool finished = false;
            int count = 0;
            byte[] chunk;
            var fileName = Path.GetFileName(ft.File.FileName);
            var filePath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/SupportDocuments/") + fileName;

            try
            {
                while (!finished || count == 20)
                {
                    int end = ft.Start + ft.SliceSize;

                    if ((ft.Size - end) < 0)
                    {
                        end = ft.Size;
                        ft.SliceSize = ft.Size;
                    }

                    chunk = GetChunk(filePath, ft);
                    string range = "bytes " + ft.Start + "-" + (end - 1) + "/" + ft.Size;

                    HttpClient client = new HttpClient();
                    client.BaseAddress = new System.Uri(ft.UploadURL);

                    HttpContent content = new ByteArrayContent(chunk);
                    content.Headers.Add("Content-Range", range);
                    HttpResponseMessage message = client.PutAsync(ft.UploadURL, content).Result;

                    if (message.IsSuccessStatusCode)
                    {
                        if (message.StatusCode == HttpStatusCode.Created)
                        {
                            System.IO.File.Delete(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/SupportDocuments/") + fileName);
                            finished = true;
                            success = true;
                        }
                        else
                        {
                            finished = false;
                            ft.Start += ft.SliceSize;
                        }
                    }
                    else
                    {
                        // Something is wrong
                        finished = true;
                        success = false;
                    }



                    count++;
                    string result = message.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception ex)
            {
                var message = ex;
            }


            return success;
        }

        private byte[] GetChunk(string FilePath, FileTransfer ft)
        {
            using (BinaryReader b = new BinaryReader(System.IO.File.Open(FilePath, FileMode.Open)))
            {
                b.BaseStream.Seek(ft.Start, SeekOrigin.Begin);
                byte[] chunk = b.ReadBytes(ft.SliceSize);
                return chunk;
            }

        }

        /// <summary>
        /// Create an upload session using Microsoft Graph v1.0 endpoint.
        /// </summary>
        /// <param name="FileName">Name and extension of the file.</param>
        /// <param name="FolderPath">Folder Path. IE: "/Folder/Subfolder"</param>
        /// <param name="token">Bearer Access Token to communicate with MS Graph.</param>
        /// <returns></returns>
        private string CreateUploadSession(string FileName, string FolderPath, string token)
        {
            var URL = GRAPH_HOST_URL + "/drives/" + DRIVE_ID + "/root:" + FolderPath + "/" + FileName + ":/createUploadSession";
            var body = "";

            HttpClient client = new HttpClient();
            client.BaseAddress = new System.Uri(URL);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + token);

            System.Net.Http.HttpContent content = new StringContent(body, UTF8Encoding.UTF8, "application/json");
            HttpResponseMessage message = client.PostAsync(URL, content).Result;

            SharePoint.GraphSessionResponse res = JsonConvert.DeserializeObject<SharePoint.GraphSessionResponse>(message.Content.ReadAsStringAsync().Result);

            return res.uploadUrl;
        }
    }
}