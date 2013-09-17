<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmailAChildEdit.ascx.cs" Inherits="Blackbaud.CustomFx.ChildSponorship.WebParts.EmailAChildEdit" %>
<%@ Register TagPrefix="uc" TagName="HTMLEditor" Src="~/admin/HtmlEditorControl2.ascx" %>
<%@ Register TagPrefix="uc" TagName="PageLink" Src="~/admin/PageLink.ascx"  %>
<asp:Label ID="lblError" runat="server" Visible="false" ForeColor="Red" Font-Bold="true" />
<table>
    <tr>         
        <td>From Address:</td>
        <td><asp:TextBox ID="txtFromAddress" runat="server" MaxLength="100" /></td>
        <td><asp:RequiredFieldValidator ID="rfvFromAddress" ControlToValidate="txtFromAddress" runat="server" ErrorMessage="* From address required" ForeColor="Red" /></td>
    </tr>
    <tr>         
        <td>From Name:</td>
        <td><asp:TextBox ID="txtFromName" runat="server" MaxLength="100" /></td>
        <td><asp:RequiredFieldValidator ID="rfvFromName" ControlToValidate="txtFromName" runat="server" ErrorMessage="* From name required" ForeColor="Red" /></td>
    </tr>
    <tr>
        <td>To Address:</td>
        <td><asp:TextBox ID="txtToAddress" runat="server" MaxLength="100" /></td>
        <td><asp:RequiredFieldValidator ID="rfvToAddress" ControlToValidate="txtToAddress" runat="server" ErrorMessage="* To address required" ForeColor="Red" /></td>
    </tr>  
    <tr>
        <td>Subject:</td>
        <td><asp:TextBox ID="txtSubject" runat="server" MaxLength="100" /></td>
        <td><asp:RequiredFieldValidator ID="rfvTxtSubject" ControlToValidate="txtSubject" runat="server" ErrorMessage="* Subject required" ForeColor="Red" /></td>
    </tr>  
    <tr>
        <td>Error Page ID:</td>
        <td><uc:PageLink ID="plinkErrorPage" runat="server" /></td>
        <td></td>
    </tr>  
    <tr>
        <td>Success Page ID:</td>
        <td><uc:PageLink ID="plinkSuccessPage" runat="server" /></td>
        <td></td>
    </tr>  
    <tr>
        <td>My Sponsorships Page ID:</td>
        <td><uc:PageLink ID="plinkMySponsorshipsPage" runat="server" /></td>
        <td></td>
    </tr> 
    <tr>
        <td colspan="3">Please provide the link to the privacy page and the email preferences below</td>
    </tr>  
    <tr>
        <td colspan="3"><uc:HTMLEditor ID="ucHtmlControl" runat="server" /></td>
    </tr>
</table>