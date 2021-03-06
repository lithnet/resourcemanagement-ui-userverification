﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Verify.aspx.cs" Inherits="Lithnet.ResourceManagement.UI.UserVerification.Verify" UICulture="auto" Culture="auto" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <title runat="server" id="pageTitle">
        <asp:Literal runat="server" Text="<%$Resources:PageTitle%>" /></title>
    <link rel="stylesheet" href="styles.css" />
    <link rel="stylesheet" href="common-layout.css" />
</head>

<body>
    <div class="main">
        <div class="wrapper">
            <div id="header" class="lithnet-header">
                <img src="lithnet16.png" alt="Lithnet" />
            </div>
            <h1>
                <asp:Label ID="lbHeader" runat="server" Text="<%$Resources:PageTitle%>" ></asp:Label>
            </h1>
            <form id="form1" runat="server" class="formcontent">
                <asp:Table ID="attributeTable" runat="server" CssClass="dataTable" />

                <div id="divWarning" class="warning" runat="server">
                    <asp:Label ID="lbWarning" runat="server" Text="" />
                </div>

                <div class="buttonRow">
                    <asp:Button ID="btSend" runat="server" OnClick="btSend_Click" CssClass="button" />
                    <asp:Button ID="btClose"
                        runat="server"
                        OnClientClick="ClosePage(); return false;"
                        CssClass="button"
                        Visible="true"
                        Text="<%$Resources:PageButtonClose%>" />
                </div>
            </form>
        </div>
    </div>
    
     <script>
        function ClosePage() {
            open(location, '_self').close();
            return false;
        }

        function ResetPage() {
            window.location.href = "<%=this.Request.RawUrl%>";
        }
    </script>
</body>
</html>
