using microsoft_graph_files_web_forms.Classes;
using System;
using System.Collections.Generic;
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
            Auth auth = new Auth();
            
            if (auth.GetAppAccessToken())
            {
                // You success retreived an access token.
                string token = auth.AppAccessToken;
            }
            else
            {
                // Something didn't work.
            }
        }
    }
}