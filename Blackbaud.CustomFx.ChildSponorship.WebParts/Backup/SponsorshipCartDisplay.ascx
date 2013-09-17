<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SponsorshipCartDisplay.ascx.cs" Inherits="Blackbaud.CustomFx.ChildSponsorship.WebParts.SponsorshipCartDisplay" %>
<asp:GridView ID="gvCart" runat="server" AutoGenerateColumns="False" 
    BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" 
    ShowFooter="True" onrowdatabound="gvCart_RowDataBound" Width="600px" 
    onrowupdated="gvCart_RowUpdated" DataKeyNames="Id" 
    onrowediting="gvCart_RowEditing" onrowdeleting="gvCart_RowDeleting" 
    onrowupdating="gvCart_RowUpdating">
    <Columns>
        <asp:TemplateField HeaderText="Child No.">
            <ItemTemplate>
                <asp:Label ID="Label1" runat="server" Text='<%# Bind("Number") %>'></asp:Label>
            </ItemTemplate>
            <ItemStyle Width="80px" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Child's Name">
            <ItemTemplate>
                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
            </ItemTemplate>
            <ItemStyle Width="160px" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Months">
            <EditItemTemplate>
                <asp:TextBox ID="txtMonths" runat="server" Text='<%# Bind("Months") %>'></asp:TextBox>
            </EditItemTemplate>
            <ItemTemplate>
                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Months") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Amount">
            <ItemTemplate>
                <asp:Label ID="Label4" runat="server" Text='<%# Bind("Amount", "{0:c}") %>'></asp:Label>
            </ItemTemplate>
            <ItemStyle Width="70px" />            
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Edit Cart" ShowHeader="False">
            <ItemTemplate>
                <asp:Button ID="Button1" runat="server" CausesValidation="False" CommandName="Edit" Text="Edit" />
                &nbsp;<asp:Button ID="Button2" runat="server" CausesValidation="False" CommandName="Delete" Text="Delete" />
            </ItemTemplate>
            <FooterTemplate>
                <asp:Button ID="btnCheckout" runat="server" Text="Continue >>" onclick="btnContinue_Click"  />
            </FooterTemplate>
            <FooterStyle BackColor="#FFBA58" />
            <ItemStyle Width="130px" />
        </asp:TemplateField>
    </Columns>
    <HeaderStyle BackColor="#FFBA58" Height="25px" HorizontalAlign="Center" />    
</asp:GridView>
<asp:Label ID="lblError" runat="server" Visible="false" />