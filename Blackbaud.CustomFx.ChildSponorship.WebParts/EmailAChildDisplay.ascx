<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmailAChildDisplay.ascx.cs" Inherits="Blackbaud.CustomFx.ChildSponorship.WebParts.EmailAChildDisplay" %>
<asp:Label ID="lblError" runat="server" Visible="false" ForeColor="Red" Font-Bold="true" />
<table>
    <tr>
        <td><asp:Label ID="lblChildID" runat="server" /></td>
        <td><asp:Label ID="lblChildName" runat="server" /></td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:Label ID="lblMessageHeader" runat="server" />
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:TextBox ID="txtMessage" runat="server" TextMode="MultiLine" Height="443px" Width="466px" ValidationGroup="0" /><asp:RequiredFieldValidator ID="rfvMessage" runat="server" ControlToValidate="txtMessage" ErrorMessage="* Message is required" ForeColor="Red" Display="Dynamic" ValidationGroup="0" />
        </td>
    </tr>
    <tr>
        <td colspan="2" style="text-align:right">
            <asp:Button ID="btnSend" runat="server" Text="Send" OnClick="btnSend_Click" ValidationGroup="0" /><asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" /></td>
    </tr>
</table>
