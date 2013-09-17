<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SponsorshipSearchDisplay.ascx.cs"
    Inherits="Blackbaud.CustomFx.ChildSponsorship.WebParts.SponsorshipSearchDisplay" %>
<asp:Repeater ID="rptSearch" runat="server" OnItemDataBound="rptSearch_ItemDataBound">
    <HeaderTemplate>
        <table>
    </HeaderTemplate>
    <ItemTemplate>
        <tr>
            <td>
                <table>
                    <tbody>
                        <tr>
                            <td colspan="2">
                                <p style="color: #ff9900" align="left">
                                    <span style="color: #3366ff"><span style="color: #ff9900"><span style="font-size: 12pt;
                                        color: #800000; font-family: Verdana"><strong style="font-size: 10pt">
                                            <%#Eval("FULLNAME")%></strong></span></span></span></p>
                            </td>
                        </tr>
                        <tr>
                            <td rowspan="5">
                                <asp:ImageButton ID="imgThumbnail" runat="server" /></a>
                            </td>
                            <td align="left" style="padding-left: 10px">
                                <em>
                                    <%#Eval("GENDER")%>&nbsp;from
                                    <%#Eval("COUNTRYNAME")%></em>
                            </td>
                        </tr>
                        <tr>
                            <td align="left" style="padding-left: 10px">
                                <span style="color: #3366ff"><span style="color: #ce5106"><span style="color: #000000">
                                    <strong>Age:</strong></span>&nbsp;</span></span><%#Eval("AGE")%>
                            </td>
                        </tr>
                        <tr>
                            <td align="left" style="padding-left: 10px">
                                <span style="color: #ce5106"><span style="color: #000000"><strong>Birthdate:</strong></span>&nbsp;</span><%#Eval("BIRTHDATE")%>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding-left: 10px">
                                <p align="left">
                                    <strong>Child No.:</strong>&nbsp;
                                    <%#Eval("CHILDNO")%></p>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding-left: 10px">
                                <span style="color: #ce5106"><strong>
                                    <asp:LinkButton ID="lnkMoreInfo" runat="server" Text="More Info" /></strong></span>
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <strong style="font-size: 10pt">
                                    <asp:LinkButton ID="lnkSponsor" runat="server" Text="Sponsor" /></strong>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </td>
    </ItemTemplate>
    <AlternatingItemTemplate>
        <td>
            <table>
                <tbody>
                    <tr>
                        <td colspan="2">
                            <p style="color: #ff9900" align="left">
                                <span style="color: #3366ff"><span style="color: #ff9900"><span style="font-size: 12pt;
                                    color: #800000; font-family: Verdana"><strong style="font-size: 10pt">
                                        <%#Eval("FULLNAME")%></strong></span></span></span></p>
                        </td>
                    </tr>
                    <tr>
                        <td rowspan="5">
                            <asp:ImageButton ID="imgThumbnail" runat="server" />
                        </td>
                        <td align="left" style="padding-left: 10px">
                            <em>
                                <%#Eval("GENDER")%>&nbsp;from
                                <%#Eval("COUNTRYNAME")%></em>
                        </td>
                    </tr>
                    <tr>
                        <td align="left" style="padding-left: 10px">
                            <span style="color: #3366ff"><span style="color: #ce5106"><span style="color: #000000">
                                <strong>Age:</strong></span>&nbsp;</span></span><%#Eval("AGE")%>
                        </td>
                    </tr>
                    <tr>
                        <td align="left" style="padding-left: 10px">
                            <span style="color: #ce5106"><span style="color: #000000"><strong>Birthdate:</strong></span>&nbsp;</span><%#Eval("BIRTHDATE")%>
                        </td>
                    </tr>
                    <tr>
                        <td style="padding-left: 10px">
                            <p align="left">
                                <strong>Child No.:</strong>&nbsp;
                                <%#Eval("CHILDNO")%></p>
                        </td>
                    </tr>
                    <tr>
                        <td style="padding-left: 10px">
                            <span style="color: #ce5106"><strong>
                                <asp:LinkButton ID="lnkMoreInfo" runat="server" Text="More Info" /></strong></span>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <strong style="font-size: 10pt">
                                <asp:LinkButton ID="lnkSponsor" runat="server" Text="Sponsor" /></strong>
                        </td>
                    </tr>
                </tbody>
            </table>
        </td>
        </tr>
    </AlternatingItemTemplate>
    <FooterTemplate>
        </table>
    </FooterTemplate>
</asp:Repeater>
<asp:Panel ID="pnlNav" runat="server" Visible="true">
    <asp:LinkButton ID="lnkFirst" runat="server" Text="First" OnClick="lnkFirst_Click" />
    <asp:LinkButton ID="lnkPrevious" runat="server" Text="Previous" OnClick="lnkPrevious_Click" />
    &nbsp;<asp:Label ID="lblCount" runat="server" Text="Label"></asp:Label>
    &nbsp;&nbsp;<asp:LinkButton ID="lnkNext" runat="server" Text="Next" OnClick="lnkNext_Click" />
    &nbsp;<asp:LinkButton ID="lnkLast" runat="server" Text="Last" OnClick="lnkLast_Click" />
    &nbsp; Go To Page:
    <asp:DropDownList ID="cmbPages" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cmbPages_SelectedIndexChanged" />
</asp:Panel>
<asp:Panel ID="pnlNoResults" runat="server" Visible="false">
    There are no children that match your search criteria.
</asp:Panel>
<asp:Label ID="lblError" runat="server" Visible="false"></asp:Label>