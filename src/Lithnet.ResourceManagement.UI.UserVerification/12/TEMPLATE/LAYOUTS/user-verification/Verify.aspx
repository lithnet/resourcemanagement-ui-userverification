<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Verify.aspx.cs" Inherits="Lithnet.ResourceManagement.UI.UserVerification.Verify" UICulture="auto" Culture="auto" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <title runat="server" id="pageTitle">User verification</title>
    <link rel="stylesheet" href="styles.css" />
</head>

<body>
    <div class="main">
        <div class="wrapper">
            <div id="header">
                <img src="lithnet16.png" alt="Lithnet" />
            </div>
            <h1>
                <asp:Label ID="lbHeader" runat="server" meta:resourcekey="lbHeader"></asp:Label></h1>
            <form id="form1" runat="server">
                <asp:Table runat="server">
                    <asp:TableRow>
                        <asp:TableHeaderCell>
                            <asp:Label ID="lbCaptionUsername" runat="server" meta:resourcekey="lbCaptionUsername"></asp:Label>
                        </asp:TableHeaderCell>
                        <asp:TableCell>
                            <asp:Label ID="lbUser" runat="server" />
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableHeaderCell>
                            <asp:Label ID="lbCaptionName" runat="server" meta:resourcekey="lbCaptionName"></asp:Label>
                        </asp:TableHeaderCell>
                        <asp:TableCell>
                            <asp:Label ID="lbName" runat="server" />
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableHeaderCell>
                            <asp:Label ID="lbCaptionPhoneNumber" runat="server" meta:resourcekey="lbCaptionPhoneNumber"></asp:Label>
                        </asp:TableHeaderCell><asp:TableCell>
                            <asp:Label ID="lbPhone" runat="server" />
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow ID="rowSecurityCode" runat="server" Visible="false">
                        <asp:TableHeaderCell>
                            <asp:Label ID="lbCaptionSecurityCode" runat="server" meta:resourcekey="lbCaptionSecurityCode"></asp:Label>
                        </asp:TableHeaderCell><asp:TableCell>
                            <asp:Label ID="lbSecurityCode" runat="server" />
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>


                <div id="warning" runat="server">
                    <asp:Label ID="lbWarning" runat="server" Text="error" />
                </div>

                <asp:Button ID="btSend" runat="server" OnClick="btSend_Click" meta:resourcekey="btSend" CssClass="button" />
            </form>



        </div>
    </div>
</body>
</html>
