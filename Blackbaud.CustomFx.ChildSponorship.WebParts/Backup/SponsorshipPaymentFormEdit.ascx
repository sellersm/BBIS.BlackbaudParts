<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SponsorshipPaymentFormEdit.ascx.cs"
    Inherits="Blackbaud.CustomFx.ChildSponsorship.WebParts.SponsorshipPaymentFormEdit" %>
<%@ Register TagPrefix="uc" TagName="HTMLEditor" Src="~/admin/HtmlEditorControl2.ascx" %>
<%@ Register TagPrefix="uc" TagName="PageLink" Src="~/admin/PageLink.ascx"  %>
<table>
    <tr>
        <td>
            Merchant Account:
        </td>
        <td>
            <asp:DropDownList ID="ddlMerchantAccounts" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            Thank You Page:
        </td>
        <td>
            <uc:PageLink ID="plinkThankYouPage" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            Demo Mode:
        </td>
        <td>
            <asp:CheckBox ID="chkDemo" runat="server" />
        </td>
    </tr>
    <tr>
        <td>Batch Number Prefix:</td>
        <td>
            <asp:TextBox ID="txtPrefix" runat="server" />
        </td>
    </tr>
</table>
<br />
<hr />
<br />
<table>    
    <tr>
        <td>
            <asp:Label ID="lblConfirmationEmail" Text="Design your confirmation email:" runat="server"></asp:Label>
            <table>
                <tr>
                    <td>
                        From Address
                    </td>
                    <td style="color: Red">
                        <asp:TextBox ID="txtFromAddress" runat="server" />
                        *<asp:RequiredFieldValidator ID="rfvTxtFromAddress" runat="server" ErrorMessage="From address required"
                            ForeColor="Red" Display="Dynamic" ControlToValidate="txtFromAddress" />
                        <asp:RegularExpressionValidator ID="revTxtFromAddress" runat="server" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                            ControlToValidate="txtFromAddress" ErrorMessage="Invalid email address" ForeColor="Red"
                            Display="Dynamic" />
                    </td>
                </tr>
                <tr>
                    <td>
                        From Name
                    </td>
                    <td style="color: Red">
                        <asp:TextBox ID="txtFromName" runat="server" MaxLength="50" />
                        *<asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="From name required"
                            ForeColor="Red" Display="Dynamic" ControlToValidate="txtFromName" />
                    </td>
                </tr>
                <tr>
                    <td>
                        Reply Address
                    </td>
                    <td style="color: Red">
                        <asp:TextBox ID="txtReplyAddress" runat="server" />
                        *<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Reply address required"
                            ForeColor="Red" Display="Dynamic" ControlToValidate="txtReplyAddress" />
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                            ControlToValidate="txtReplyAddress" ErrorMessage="Invalid email address" ForeColor="Red"
                            Display="Dynamic" />
                    </td>
                </tr>
                <tr>
                    <td>
                        Subject
                    </td>
                    <td style="color: Red">
                        <asp:TextBox ID="txtSubject" runat="server" MaxLength="100" />
                        *<asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Subject required"
                            ForeColor="Red" Display="Dynamic" ControlToValidate="txtSubject" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <uc:HTMLEditor id="htmledConfirmationEmail" runat="server" oninit="htmled2ConfirmationEmail_Init" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
