<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MyFinancialCommitmentsEdit.ascx.cs" Inherits="Blackbaud.CustomFx.ChildSponsorship.WebParts.MyFinancialCommitmentsEdit" %>
<table>
    <tr>
        <td>Demo Mode:</td>
        <td><asp:CheckBox ID="chkDemo" runat="server" /></td>
    </tr>
    <tr>
        <td>
            Merchant Account:
        </td>
        <td>
            <asp:DropDownList ID="ddlMerchantAccounts" runat="server" />
        </td>
    </tr>
    <tr>
        <td valign="top">Thank You Message:</td>
        <td><asp:TextBox ID="txtMessage" runat="server" TextMode="MultiLine" Height="317px" 
                Width="570px" /></td>
    </tr>
</table>