using microsoft_graph_files_web_forms.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace microsoft_graph_files_web_forms
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string path = "/" + txtFolder.Text;
            GetDocuments(path);
        }

        protected void UploadFile_Click(object sender, EventArgs e)
        {
            lblUploadStatus.Visible = true;
            string path = "/" + txtFolder.Text;
            if (UploadDocuments(path))
            {
                // Success
                lblUploadStatus.Visible = false;
                GetDocuments(path);
            }
            else
            {
                // Something is wrong
            }
        }

        private bool UploadDocuments(string FolderPath)
        {
            bool success = false;
            if (fileRCA.HasFile)
            {
                foreach (var file in fileRCA.PostedFiles)
                {
                    SharePoint sp = new SharePoint();
                    success = sp.UploadDocument(file, FolderPath);
                }
            }
            else
            {
                // There isn't any files in the upload control. 
            }

            return success;
        }

        private void GetDocuments(string FolderPath)
        {
            SharePoint sp = new SharePoint();
            DataTable dt = sp.GetDocuments(FolderPath);

            grdFiles.DataSource = dt;
            grdFiles.DataBind();
        }

        protected void btnDeleteDocument_Click(object sender, EventArgs e)
        {
            GridViewRow row = (GridViewRow)((LinkButton)sender).NamingContainer;
            string itemID = row.Cells[3].Text;

            SharePoint sp = new SharePoint();
            if (sp.DeleteDocument(itemID))
            {
                string path = "/" + txtFolder.Text;
                GetDocuments(path);
            }
            else
            {
                //Something is wrong
            }
        }

        protected void btnGetDocuments_Click(object sender, EventArgs e)
        {
            string path = "/" + txtFolder.Text;
            GetDocuments(path);
        }
    }
}