<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="microsoft_graph_files_web_forms._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <%--<div class="jumbotron">
        <h1>ASP.NET</h1>
        <p class="lead">ASP.NET is a free web framework for building great Web sites and Web applications using HTML, CSS, and JavaScript.</p>
        <p><a href="http://www.asp.net" class="btn btn-primary btn-lg">Learn more &raquo;</a></p>
    </div>

    <div class="row">
        <div class="col-md-4">
            <h2>Getting started</h2>
            <p>
                ASP.NET Web Forms lets you build dynamic websites using a familiar drag-and-drop, event-driven model.
            A design surface and hundreds of controls and components let you rapidly build sophisticated, powerful UI-driven sites with data access.
            </p>
            <p>
                <a class="btn btn-default" href="https://go.microsoft.com/fwlink/?LinkId=301948">Learn more &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Get more libraries</h2>
            <p>
                NuGet is a free Visual Studio extension that makes it easy to add, remove, and update libraries and tools in Visual Studio projects.
            </p>
            <p>
                <a class="btn btn-default" href="https://go.microsoft.com/fwlink/?LinkId=301949">Learn more &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Web Hosting</h2>
            <p>
                You can easily find a web hosting company that offers the right mix of features and price for your applications.
            </p>
            <p>
                <a class="btn btn-default" href="https://go.microsoft.com/fwlink/?LinkId=301950">Learn more &raquo;</a>
            </p>
        </div>
    </div>--%>

    <div class="row">
        <div class="col-xs-11 col-lg-8">
            <div class="form-group">
                <%--Documents--%>
                <br />
                <br />
                <br />
                <br />
                <br />
                <br />
                <label for="txtFolder">Folder Name</label>
                <asp:TextBox runat="server" ID="txtFolder" Text="FolderName" CssClass="form-control"></asp:TextBox>
                <asp:Button runat="server" ID="btnGetDocuments" Text="Get" CssClass="btn btn-primary" OnClick="btnGetDocuments_Click" /><br /><br />
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-md-4">
            <label for="fileRCA">Attachments</label><br />
            <asp:FileUpload ID="fileRCA" runat="server" AllowMultiple="true"  /> 
            <asp:Button ID="UploadFile" runat="server" CausesValidation="false" Text="Upload" CssClass="btn btn-sm btn-primary margin-bottom-6" OnClick="UploadFile_Click" />
        </div>
    </div>
    <div class="row">
        <div class="col-md-8">
            <Label ID="lblUploadStatus" hidden="hidden" runat="server" style="color:darkred">Please wait while we upload your documents</Label>
            <br />
            <asp:GridView ID="grdFiles" CssClass="table" AutoGenerateColumns="false" runat="server">
                <Columns>
                    <asp:TemplateField HeaderText="Name">
                        <ItemTemplate>
                            <asp:HyperLink ID="lnkName" runat="server" NavigateUrl='<%# Eval("URL") %>' Target="_blank" Text='<%# Eval("Title") %>'></asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Created" HeaderText="Created" />
                    <asp:BoundField DataField="Modified" HeaderText="Modified" />
                    <asp:BoundField DataField="ID" HeaderText="ID" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton ID="btnDeleteDocument" Text="Delete" runat="server" OnClick="btnDeleteDocument_Click" CausesValidation="false"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    No Files
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
    </div>

</asp:Content>
