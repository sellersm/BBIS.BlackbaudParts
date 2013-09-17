<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SponsorshipSearchEdit.ascx.cs" Inherits="Blackbaud.CustomFx.ChildSponsorship.WebParts.SponsorshipSearchEdit" %>
<%@ Register TagPrefix="uc" TagName="PageLink" Src="~/admin/PageLink.ascx"  %>
<table>
    <tr>
        <td>Thumnail Image Doc Type</td>
        <td><asp:TextBox ID="txtDocType" runat="server" /></td>
    </tr>
    <tr>
        <td>Results Per Page</td>
        <td><asp:TextBox ID="txtResultsPerPage" runat="server" /></td>
    </tr>
    <tr>
        <td>More Info Target Page</td>
        <td><uc:PageLink ID="plinkMoreInfoTargetPage" runat="server" /></td>
    </tr>
    <tr>
        <td>Sponsor Target Page</td>
        <td><uc:PageLink ID="plinkSponsorTargetPage" runat="server" /></td>
    </tr>
</table>