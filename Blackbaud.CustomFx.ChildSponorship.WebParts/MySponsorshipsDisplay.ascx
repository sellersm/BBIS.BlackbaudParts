<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MySponsorshipsDisplay.ascx.cs" Inherits="Blackbaud.CustomFx.ChildSponsorship.WebParts.MySponsorshipsDisplay" %>
<script type="text/javascript">
    $(document).ready(function () {
        $("#<%= radPayment.ClientID %>").change(function () {
            var enable = false;

            if ($("#<%= radPayment.ClientID %>").find('input:checked').val() == "CC") {
                $("#tblCC").css("display", "table");
                $("#tblCheck").css("display", "none");
                enable = true;
            } else {
                $("#tblCC").css("display", "none");
                $("#tblCheck").css("display", "table");
                $("#<%= radCcRecurrence.ClientID %>").val("OneTimeGift");
            }

            ValidatorEnable($("#<%= reqCcName.ClientID %>")[0], enable);
            ValidatorEnable($("#<%= RequiredFieldValidator12.ClientID %>")[0], enable);
            ValidatorEnable($("#<%= RequiredFieldValidator15.ClientID %>")[0], enable);
            ValidatorEnable($("#<%= RequiredFieldValidator16.ClientID %>")[0], enable);
            ValidatorEnable($("#<%= RequiredFieldValidator17.ClientID %>")[0], enable);
            ValidatorEnable($("#<%= RequiredFieldValidator18.ClientID %>")[0], enable);
        });
    });
</script>
<style type="text/css">
    .Validation
    {
        color: Red;
        font-weight: bold;
    }
</style>
<asp:MultiView ID="mvMain" runat="server" ActiveViewIndex="0">
    <asp:View ID="viewSponsorships" runat="server">
        <asp:GridView ID="gvSponsorships" runat="server" AutoGenerateColumns="False" 
        DataKeyNames="RevenueId" onrowdatabound="gvSponsorships_RowDataBound" 
        BorderStyle="None" GridLines="None" HorizontalAlign="Center" Width="100%" 
        onrowcommand="gvSponsorships_RowCommand">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton ID="lnkPayments" runat="server" Text="View Payments" CommandName="ViewPayments" CommandArgument='<%#Eval("RevenueId") %>' /><br /><br />
                    <asp:LinkButton ID="lnkPay" runat="server" Text="Make Payment" CommandName="MakePayment" CommandArgument='<%#Eval("RevenueId") %>' /><br /><br />
                    <asp:HyperLink ID="lnkEmail" runat="server" Text="Write a Letter" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:ImageButton ID="imgThumbnail" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Child No">
                <ItemTemplate>
                    <asp:LinkButton ID="lnkNo" runat="server" Text='<%# Bind("[Child No]") %>' />
                </ItemTemplate>
                <HeaderStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Child Name">
                <ItemTemplate>
                    <asp:LinkButton ID="lnkName" runat="server" Text='<%# Bind("[Child Name]") %>' />
                </ItemTemplate>
                <FooterStyle HorizontalAlign="Center" />
                <HeaderStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:BoundField DataField="Age" HeaderText="Age" >
                <HeaderStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:BoundField DataField="Birthdate" HeaderText="Birthdate" >
                <HeaderStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Make a Payment">
                <ItemTemplate>
                    <asp:CheckBox ID="chkPayment" runat="server" />
                </ItemTemplate>
                <HeaderStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:LinkButton ID="lnkMakePayment" runat="server" onclick="lnkMakePayment_Click">Make Payment for Selected Sponsorships</asp:LinkButton>
    </asp:View>
    <asp:View ID="viewPayments" runat="server">
        <asp:LinkButton ID="lnkBack" runat="server" Text="Back" onclick="lnkBack_Click" />
        <asp:GridView ID="gvPayments" runat="server" AutoGenerateColumns="False" GridLines="None">
            <Columns>
                <asp:BoundField DataField="ACTIVITYTYPE" HeaderText="Activity" />
                <asp:BoundField DataField="AMOUNT" DataFormatString="{0:c}" HeaderText="Amount" />
                <asp:BoundField DataField="BALANCE" DataFormatString="{0:c}" HeaderText="Balance" />
                <asp:BoundField DataField="DATE" DataFormatString="{0:M/dd/yyyy}" HeaderText="Date" />
            </Columns>
    
        </asp:GridView>        
    </asp:View>
    <asp:View ID="viewPay" runat="server">
        <asp:LinkButton ID="lnkPayBack" runat="server" Text="Back" onclick="lnkPayBack_Click" />
        <asp:Label ID="lblSummary" runat="server" Visible="false" />
        <asp:Label ID="lblError" runat="server" ForeColor="Red" />
        <asp:ValidationSummary ID="ValidationSummary1" runat="server"  ValidationGroup="Checkout" />
        <table>
            <tr>
                <td class="NCC_ScholarshipApp_subSectionHeader">Billing Address:</td>
            </tr>
            <tr>
                <td>
                    <table>
                        <tr>
                            <td><span id="Span1" class="BBFieldCaption ChildSearchFieldCaption">Amount</span><span class="NCC_ScholarshipApp_requiredIndicator">&nbsp;*</span></td>
                            <td>
                                <asp:TextBox ID="txtAmount" runat="server" />
                                <asp:CompareValidator ID="vldAmount" runat="server" ControlToValidate="txtAmount" Operator="DataTypeCheck" Type="Currency" ValidationGroup="Checkout" ErrorMessage="You must enter a dollar amount for Amount field." /> 
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtAmount" ErrorMessage="Amount" ForeColor="Red" ValidationGroup="Checkout">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>Billing Address:</td>
                            <td></td>
                        </tr>
                        <tr>
                            <td><span class="BBFieldCaption ChildSearchFieldCaption">Address 1: *</span></td>
                            <td class="style4">
                                <asp:TextBox ID="txtBillingAddress" runat="server"></asp:TextBox>
                                &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                                    ControlToValidate="txtBillingAddress" ErrorMessage="Billing Address 1" ForeColor="Red" 
                                    ValidationGroup="Checkout">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td><span class="BBFieldCaption ChildSearchFieldCaption">Address 2:</span>
                            </td>
                            <td class="style4">
                                <asp:TextBox ID="txtBillingAddress2" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td><span class="BBFieldCaption ChildSearchFieldCaption">City: *</span></td>
                            <td class="style4">
                                <asp:TextBox ID="txtBillingCity" runat="server"></asp:TextBox>
                                &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="txtBillingCity" ErrorMessage="Billing City" ForeColor="Red" ValidationGroup="Checkout">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td><span class="BBFieldCaption ChildSearchFieldCaption">State: *</span></td>
                            <td class="style4">
                                <asp:DropDownList ID="cmbBillingState" runat="server">
                                    <asp:ListItem Value="">[Select State]</asp:ListItem>
                                    <asp:ListItem Value="AL">Alabama</asp:ListItem>
                                    <asp:ListItem Value="AK">Alaska</asp:ListItem>
                                    <asp:ListItem Value="AZ">Arizona</asp:ListItem>
                                    <asp:ListItem Value="AR">Arkansas</asp:ListItem>
                                    <asp:ListItem Value="CA">California</asp:ListItem>
                                    <asp:ListItem Value="CO">Colorado</asp:ListItem>
                                    <asp:ListItem Value="CT">Connecticut</asp:ListItem>
                                    <asp:ListItem Value="DC">District of Columbia</asp:ListItem>
                                    <asp:ListItem Value="DE">Delaware</asp:ListItem>
                                    <asp:ListItem Value="FL">Florida</asp:ListItem>
                                    <asp:ListItem Value="GA">Georgia</asp:ListItem>
                                    <asp:ListItem Value="HI">Hawaii</asp:ListItem>
                                    <asp:ListItem Value="ID">Idaho</asp:ListItem>
                                    <asp:ListItem Value="IL">Illinois</asp:ListItem>
                                    <asp:ListItem Value="IN">Indiana</asp:ListItem>
                                    <asp:ListItem Value="IA">Iowa</asp:ListItem>
                                    <asp:ListItem Value="KS">Kansas</asp:ListItem>
                                    <asp:ListItem Value="KY">Kentucky</asp:ListItem>
                                    <asp:ListItem Value="LA">Louisiana</asp:ListItem>
                                    <asp:ListItem Value="ME">Maine</asp:ListItem>
                                    <asp:ListItem Value="MD">Maryland</asp:ListItem>
                                    <asp:ListItem Value="MA">Massachusetts</asp:ListItem>
                                    <asp:ListItem Value="MI">Michigan</asp:ListItem>
                                    <asp:ListItem Value="MN">Minnesota</asp:ListItem>
                                    <asp:ListItem Value="MS">Mississippi</asp:ListItem>
                                    <asp:ListItem Value="MO">Missouri</asp:ListItem>
                                    <asp:ListItem Value="MT">Montana</asp:ListItem>
                                    <asp:ListItem Value="NE">Nebraska</asp:ListItem>
                                    <asp:ListItem Value="NV">Nevada</asp:ListItem>
                                    <asp:ListItem Value="NH">New Hampshire</asp:ListItem>
                                    <asp:ListItem Value="NJ">New Jersey</asp:ListItem>
                                    <asp:ListItem Value="NM">New Mexico</asp:ListItem>
                                    <asp:ListItem Value="NY">New York</asp:ListItem>
                                    <asp:ListItem Value="NC">North Carolina</asp:ListItem>
                                    <asp:ListItem Value="ND">North Dakota</asp:ListItem>
                                    <asp:ListItem Value="OH">Ohio</asp:ListItem>
                                    <asp:ListItem Value="OK">Oklahoma</asp:ListItem>
                                    <asp:ListItem Value="OR">Oregon</asp:ListItem>
                                    <asp:ListItem Value="PA">Pennsylvania</asp:ListItem>
                                    <asp:ListItem Value="RI">Rhode Island</asp:ListItem>
                                    <asp:ListItem Value="SD">South Carolina</asp:ListItem>
                                    <asp:ListItem Value="SD">South Dakota</asp:ListItem>
                                    <asp:ListItem Value="TN">Tennessee</asp:ListItem>
                                    <asp:ListItem Value="TX">Texas</asp:ListItem>
                                    <asp:ListItem Value="UT">Utah</asp:ListItem>
                                    <asp:ListItem Value="VT">Vermont</asp:ListItem>
                                    <asp:ListItem Value="VA">Virginia</asp:ListItem>
                                    <asp:ListItem Value="WA">Washington</asp:ListItem>
                                    <asp:ListItem Value="WV">West Virginia</asp:ListItem>
                                    <asp:ListItem Value="WI">Wisconsin</asp:ListItem>
                                    <asp:ListItem Value="WY">Wyoming</asp:ListItem>
                                </asp:DropDownList>
                                &nbsp;<asp:RequiredFieldValidator ID="reqBillingState" runat="server" 
                                    ControlToValidate="cmbBillingState" ErrorMessage="Billing State" ForeColor="Red" 
                                    ValidationGroup="Checkout">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <span class="BBFieldCaption ChildSearchFieldCaption">Province/Region:</span><asp:Label ID="lblProvinceRegionRequired1" runat="server" CssClass="BBFieldCaption ChildSearchFieldCaption" Text="*" Visible="false" /></td>
                            <td class="style4">
                                <asp:TextBox ID="txtBillingRegion" runat="server" Enabled="False"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="reqBillingRegion" runat="server" 
                                    ControlToValidate="txtBillingRegion" ErrorMessage="Billing Province/Region" ForeColor="Red" 
                                    ValidationGroup="Checkout" Enabled="false">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <span class="BBFieldCaption ChildSearchFieldCaption">Zip: *</span></td>
                            <td class="style4">
                                <asp:TextBox ID="txtBillingZip" runat="server"></asp:TextBox>
                                &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator13" runat="server" 
                                    ControlToValidate="txtBillingZip" ErrorMessage="Billing Zip" ForeColor="Red" 
                                    ValidationGroup="Checkout">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <span id="Span4" class="BBFieldCaption ChildSearchFieldCaption">Country: *</span></td>
                            <td class="style4">
                                <asp:DropDownList ID="cmbBillingCountry" runat="server" AutoPostBack="True" 
                                    onselectedindexchanged="cmbBillingCountry_SelectedIndexChanged" >
                                    <asp:ListItem Value="">[Select Country]</asp:ListItem>
                                    <asp:ListItem Value="AF">Afghanistan</asp:ListItem>
                                    <asp:ListItem Value="AL">Albania</asp:ListItem>
                                    <asp:ListItem Value="DZ">Algeria</asp:ListItem>
                                    <asp:ListItem Value="AS">American Samoa</asp:ListItem>
                                    <asp:ListItem Value="AD">Andorra</asp:ListItem>
                                    <asp:ListItem Value="AO">Angola</asp:ListItem>
                                    <asp:ListItem Value="AI">Anguilla</asp:ListItem>
                                    <asp:ListItem Value="AQ">Antarctica</asp:ListItem>
                                    <asp:ListItem Value="AG">Antigua And Barbuda</asp:ListItem>
                                    <asp:ListItem Value="AR">Argentina</asp:ListItem>
                                    <asp:ListItem Value="AM">Armenia</asp:ListItem>
                                    <asp:ListItem Value="AW">Aruba</asp:ListItem>
                                    <asp:ListItem Value="AU">Australia</asp:ListItem>
                                    <asp:ListItem Value="AT">Austria</asp:ListItem>
                                    <asp:ListItem Value="AZ">Azerbaijan</asp:ListItem>
                                    <asp:ListItem Value="BS">Bahamas</asp:ListItem>
                                    <asp:ListItem Value="BH">Bahrain</asp:ListItem>
                                    <asp:ListItem Value="BD">Bangladesh</asp:ListItem>
                                    <asp:ListItem Value="BB">Barbados</asp:ListItem>
                                    <asp:ListItem Value="BY">Belarus</asp:ListItem>
                                    <asp:ListItem Value="BE">Belgium</asp:ListItem>
                                    <asp:ListItem Value="BZ">Belize</asp:ListItem>
                                    <asp:ListItem Value="BJ">Benin</asp:ListItem>
                                    <asp:ListItem Value="BM">Bermuda</asp:ListItem>
                                    <asp:ListItem Value="BT">Bhutan</asp:ListItem>
                                    <asp:ListItem Value="BO">Bolivia</asp:ListItem>
                                    <asp:ListItem Value="BA">Bosnia And Herzegowina</asp:ListItem>
                                    <asp:ListItem Value="BW">Botswana</asp:ListItem>
                                    <asp:ListItem Value="BV">Bouvet Island</asp:ListItem>
                                    <asp:ListItem Value="BR">Brazil</asp:ListItem>
                                    <asp:ListItem Value="IO">British Indian Ocean Territory</asp:ListItem>
                                    <asp:ListItem Value="BN">Brunei Darussalam</asp:ListItem>
                                    <asp:ListItem Value="BG">Bulgaria</asp:ListItem>
                                    <asp:ListItem Value="BF">Burkina Faso</asp:ListItem>
                                    <asp:ListItem Value="BI">Burundi</asp:ListItem>
                                    <asp:ListItem Value="KH">Cambodia</asp:ListItem>
                                    <asp:ListItem Value="CM">Cameroon</asp:ListItem>
                                    <asp:ListItem Value="CA">Canada</asp:ListItem>
                                    <asp:ListItem Value="CV">Cape Verde</asp:ListItem>
                                    <asp:ListItem Value="KY">Cayman Islands</asp:ListItem>
                                    <asp:ListItem Value="CF">Central African Republic</asp:ListItem>
                                    <asp:ListItem Value="TD">Chad</asp:ListItem>
                                    <asp:ListItem Value="CL">Chile</asp:ListItem>
                                    <asp:ListItem Value="CN">China</asp:ListItem>
                                    <asp:ListItem Value="CX">Christmas Island</asp:ListItem>
                                    <asp:ListItem Value="CC">Cocos (Keeling) Islands</asp:ListItem>
                                    <asp:ListItem Value="CO">Colombia</asp:ListItem>
                                    <asp:ListItem Value="KM">Comoros</asp:ListItem>
                                    <asp:ListItem Value="CG">Congo</asp:ListItem>
                                    <asp:ListItem Value="CK">Cook Islands</asp:ListItem>
                                    <asp:ListItem Value="CR">Costa Rica</asp:ListItem>
                                    <asp:ListItem Value="CI">Cote D'Ivoire</asp:ListItem>
                                    <asp:ListItem Value="HR">Croatia (Local Name: Hrvatska)</asp:ListItem>
                                    <asp:ListItem Value="CU">Cuba</asp:ListItem>
                                    <asp:ListItem Value="CY">Cyprus</asp:ListItem>
                                    <asp:ListItem Value="CZ">Czech Republic</asp:ListItem>
                                    <asp:ListItem Value="DK">Denmark</asp:ListItem>
                                    <asp:ListItem Value="DJ">Djibouti</asp:ListItem>
                                    <asp:ListItem Value="DM">Dominica</asp:ListItem>
                                    <asp:ListItem Value="DO">Dominican Republic</asp:ListItem>
                                    <asp:ListItem Value="TP">East Timor</asp:ListItem>
                                    <asp:ListItem Value="EC">Ecuador</asp:ListItem>
                                    <asp:ListItem Value="EG">Egypt</asp:ListItem>
                                    <asp:ListItem Value="SV">El Salvador</asp:ListItem>
                                    <asp:ListItem Value="GQ">Equatorial Guinea</asp:ListItem>
                                    <asp:ListItem Value="ER">Eritrea</asp:ListItem>
                                    <asp:ListItem Value="EE">Estonia</asp:ListItem>
                                    <asp:ListItem Value="ET">Ethiopia</asp:ListItem>
                                    <asp:ListItem Value="FK">Falkland Islands (Malvinas)</asp:ListItem>
                                    <asp:ListItem Value="FO">Faroe Islands</asp:ListItem>
                                    <asp:ListItem Value="FJ">Fiji</asp:ListItem>
                                    <asp:ListItem Value="FI">Finland</asp:ListItem>
                                    <asp:ListItem Value="FR">France</asp:ListItem>
                                    <asp:ListItem Value="GF">French Guiana</asp:ListItem>
                                    <asp:ListItem Value="PF">French Polynesia</asp:ListItem>
                                    <asp:ListItem Value="TF">French Southern Territories</asp:ListItem>
                                    <asp:ListItem Value="GA">Gabon</asp:ListItem>
                                    <asp:ListItem Value="GM">Gambia</asp:ListItem>
                                    <asp:ListItem Value="GE">Georgia</asp:ListItem>
                                    <asp:ListItem Value="DE">Germany</asp:ListItem>
                                    <asp:ListItem Value="GH">Ghana</asp:ListItem>
                                    <asp:ListItem Value="GI">Gibraltar</asp:ListItem>
                                    <asp:ListItem Value="GR">Greece</asp:ListItem>
                                    <asp:ListItem Value="GL">Greenland</asp:ListItem>
                                    <asp:ListItem Value="GD">Grenada</asp:ListItem>
                                    <asp:ListItem Value="GP">Guadeloupe</asp:ListItem>
                                    <asp:ListItem Value="GU">Guam</asp:ListItem>
                                    <asp:ListItem Value="GT">Guatemala</asp:ListItem>
                                    <asp:ListItem Value="GN">Guinea</asp:ListItem>
                                    <asp:ListItem Value="GW">Guinea-Bissau</asp:ListItem>
                                    <asp:ListItem Value="GY">Guyana</asp:ListItem>
                                    <asp:ListItem Value="HT">Haiti</asp:ListItem>
                                    <asp:ListItem Value="HM">Heard And Mc Donald Islands</asp:ListItem>
                                    <asp:ListItem Value="VA">Holy See (Vatican City State)</asp:ListItem>
                                    <asp:ListItem Value="HN">Honduras</asp:ListItem>
                                    <asp:ListItem Value="HK">Hong Kong</asp:ListItem>
                                    <asp:ListItem Value="HU">Hungary</asp:ListItem>
                                    <asp:ListItem Value="IS">Icel And</asp:ListItem>
                                    <asp:ListItem Value="IN">India</asp:ListItem>
                                    <asp:ListItem Value="ID">Indonesia</asp:ListItem>
                                    <asp:ListItem Value="IR">Iran (Islamic Republic Of)</asp:ListItem>
                                    <asp:ListItem Value="IQ">Iraq</asp:ListItem>
                                    <asp:ListItem Value="IE">Ireland</asp:ListItem>
                                    <asp:ListItem Value="IL">Israel</asp:ListItem>
                                    <asp:ListItem Value="IT">Italy</asp:ListItem>
                                    <asp:ListItem Value="JM">Jamaica</asp:ListItem>
                                    <asp:ListItem Value="JP">Japan</asp:ListItem>
                                    <asp:ListItem Value="JO">Jordan</asp:ListItem>
                                    <asp:ListItem Value="KZ">Kazakhstan</asp:ListItem>
                                    <asp:ListItem Value="KE">Kenya</asp:ListItem>
                                    <asp:ListItem Value="KI">Kiribati</asp:ListItem>
                                    <asp:ListItem Value="KP">Korea, Dem People'S Republic</asp:ListItem>
                                    <asp:ListItem Value="KR">Korea, Republic Of</asp:ListItem>
                                    <asp:ListItem Value="KW">Kuwait</asp:ListItem>
                                    <asp:ListItem Value="KG">Kyrgyzstan</asp:ListItem>
                                    <asp:ListItem Value="LA">Lao People'S Dem Republic</asp:ListItem>
                                    <asp:ListItem Value="LV">Latvia</asp:ListItem>
                                    <asp:ListItem Value="LB">Lebanon</asp:ListItem>
                                    <asp:ListItem Value="LS">Lesotho</asp:ListItem>
                                    <asp:ListItem Value="LR">Liberia</asp:ListItem>
                                    <asp:ListItem Value="LY">Libyan Arab Jamahiriya</asp:ListItem>
                                    <asp:ListItem Value="LI">Liechtenstein</asp:ListItem>
                                    <asp:ListItem Value="LT">Lithuania</asp:ListItem>
                                    <asp:ListItem Value="LU">Luxembourg</asp:ListItem>
                                    <asp:ListItem Value="MO">Macau</asp:ListItem>
                                    <asp:ListItem Value="MK">Macedonia</asp:ListItem>
                                    <asp:ListItem Value="MG">Madagascar</asp:ListItem>
                                    <asp:ListItem Value="MW">Malawi</asp:ListItem>
                                    <asp:ListItem Value="MY">Malaysia</asp:ListItem>
                                    <asp:ListItem Value="MV">Maldives</asp:ListItem>
                                    <asp:ListItem Value="ML">Mali</asp:ListItem>
                                    <asp:ListItem Value="MT">Malta</asp:ListItem>
                                    <asp:ListItem Value="MH">Marshall Islands</asp:ListItem>
                                    <asp:ListItem Value="MQ">Martinique</asp:ListItem>
                                    <asp:ListItem Value="MR">Mauritania</asp:ListItem>
                                    <asp:ListItem Value="MU">Mauritius</asp:ListItem>
                                    <asp:ListItem Value="YT">Mayotte</asp:ListItem>
                                    <asp:ListItem Value="MX">Mexico</asp:ListItem>
                                    <asp:ListItem Value="FM">Micronesia, Federated States</asp:ListItem>
                                    <asp:ListItem Value="MD">Moldova, Republic Of</asp:ListItem>
                                    <asp:ListItem Value="MC">Monaco</asp:ListItem>
                                    <asp:ListItem Value="MN">Mongolia</asp:ListItem>
                                    <asp:ListItem Value="MS">Montserrat</asp:ListItem>
                                    <asp:ListItem Value="MA">Morocco</asp:ListItem>
                                    <asp:ListItem Value="MZ">Mozambique</asp:ListItem>
                                    <asp:ListItem Value="MM">Myanmar</asp:ListItem>
                                    <asp:ListItem Value="NA">Namibia</asp:ListItem>
                                    <asp:ListItem Value="NR">Nauru</asp:ListItem>
                                    <asp:ListItem Value="NP">Nepal</asp:ListItem>
                                    <asp:ListItem Value="NL">Netherlands</asp:ListItem>
                                    <asp:ListItem Value="AN">Netherlands Ant Illes</asp:ListItem>
                                    <asp:ListItem Value="NC">New Caledonia</asp:ListItem>
                                    <asp:ListItem Value="NZ">New Zealand</asp:ListItem>
                                    <asp:ListItem Value="NI">Nicaragua</asp:ListItem>
                                    <asp:ListItem Value="NE">Niger</asp:ListItem>
                                    <asp:ListItem Value="NG">Nigeria</asp:ListItem>
                                    <asp:ListItem Value="NU">Niue</asp:ListItem>
                                    <asp:ListItem Value="NF">Norfolk Island</asp:ListItem>
                                    <asp:ListItem Value="MP">Northern Mariana Islands</asp:ListItem>
                                    <asp:ListItem Value="NO">Norway</asp:ListItem>
                                    <asp:ListItem Value="OM">Oman</asp:ListItem>
                                    <asp:ListItem Value="PK">Pakistan</asp:ListItem>
                                    <asp:ListItem Value="PW">Palau</asp:ListItem>
                                    <asp:ListItem Value="PA">Panama</asp:ListItem>
                                    <asp:ListItem Value="PG">Papua New Guinea</asp:ListItem>
                                    <asp:ListItem Value="PY">Paraguay</asp:ListItem>
                                    <asp:ListItem Value="PE">Peru</asp:ListItem>
                                    <asp:ListItem Value="PH">Philippines</asp:ListItem>
                                    <asp:ListItem Value="PN">Pitcairn</asp:ListItem>
                                    <asp:ListItem Value="PL">Poland</asp:ListItem>
                                    <asp:ListItem Value="PT">Portugal</asp:ListItem>
                                    <asp:ListItem Value="PR">Puerto Rico</asp:ListItem>
                                    <asp:ListItem Value="QA">Qatar</asp:ListItem>
                                    <asp:ListItem Value="RE">Reunion</asp:ListItem>
                                    <asp:ListItem Value="RO">Romania</asp:ListItem>
                                    <asp:ListItem Value="RU">Russian Federation</asp:ListItem>
                                    <asp:ListItem Value="RW">Rwanda</asp:ListItem>
                                    <asp:ListItem Value="KN">Saint K Itts And Nevis</asp:ListItem>
                                    <asp:ListItem Value="LC">Saint Lucia</asp:ListItem>
                                    <asp:ListItem Value="VC">Saint Vincent, The Grenadines</asp:ListItem>
                                    <asp:ListItem Value="WS">Samoa</asp:ListItem>
                                    <asp:ListItem Value="SM">San Marino</asp:ListItem>
                                    <asp:ListItem Value="ST">Sao Tome And Principe</asp:ListItem>
                                    <asp:ListItem Value="SA">Saudi Arabia</asp:ListItem>
                                    <asp:ListItem Value="SN">Senegal</asp:ListItem>
                                    <asp:ListItem Value="SC">Seychelles</asp:ListItem>
                                    <asp:ListItem Value="SL">Sierra Leone</asp:ListItem>
                                    <asp:ListItem Value="SG">Singapore</asp:ListItem>
                                    <asp:ListItem Value="SK">Slovakia (Slovak Republic)</asp:ListItem>
                                    <asp:ListItem Value="SI">Slovenia</asp:ListItem>
                                    <asp:ListItem Value="SB">Solomon Islands</asp:ListItem>
                                    <asp:ListItem Value="SO">Somalia</asp:ListItem>
                                    <asp:ListItem Value="ZA">South Africa</asp:ListItem>
                                    <asp:ListItem Value="GS">South Georgia , S Sandwich Is.</asp:ListItem>
                                    <asp:ListItem Value="ES">Spain</asp:ListItem>
                                    <asp:ListItem Value="LK">Sri Lanka</asp:ListItem>
                                    <asp:ListItem Value="SH">St. Helena</asp:ListItem>
                                    <asp:ListItem Value="PM">St. Pierre And Miquelon</asp:ListItem>
                                    <asp:ListItem Value="SD">Sudan</asp:ListItem>
                                    <asp:ListItem Value="SR">Suriname</asp:ListItem>
                                    <asp:ListItem Value="SJ">Svalbard, Jan Mayen Islands</asp:ListItem>
                                    <asp:ListItem Value="SZ">Sw Aziland</asp:ListItem>
                                    <asp:ListItem Value="SE">Sweden</asp:ListItem>
                                    <asp:ListItem Value="CH">Switzerland</asp:ListItem>
                                    <asp:ListItem Value="SY">Syrian Arab Republic</asp:ListItem>
                                    <asp:ListItem Value="TW">Taiwan</asp:ListItem>
                                    <asp:ListItem Value="TJ">Tajikistan</asp:ListItem>
                                    <asp:ListItem Value="TZ">Tanzania, United Republic Of</asp:ListItem>
                                    <asp:ListItem Value="TH">Thailand</asp:ListItem>
                                    <asp:ListItem Value="TG">Togo</asp:ListItem>
                                    <asp:ListItem Value="TK">Tokelau</asp:ListItem>
                                    <asp:ListItem Value="TO">Tonga</asp:ListItem>
                                    <asp:ListItem Value="TT">Trinidad And Tobago</asp:ListItem>
                                    <asp:ListItem Value="TN">Tunisia</asp:ListItem>
                                    <asp:ListItem Value="TR">Turkey</asp:ListItem>
                                    <asp:ListItem Value="TM">Turkmenistan</asp:ListItem>
                                    <asp:ListItem Value="TC">Turks And Caicos Islands</asp:ListItem>
                                    <asp:ListItem Value="TV">Tuvalu</asp:ListItem>
                                    <asp:ListItem Value="UG">Uganda</asp:ListItem>
                                    <asp:ListItem Value="UA">Ukraine</asp:ListItem>
                                    <asp:ListItem Value="AE">United Arab Emirates</asp:ListItem>
                                    <asp:ListItem Value="GB">United Kingdom</asp:ListItem>
                                    <asp:ListItem Value="US" Selected="True">United States</asp:ListItem>
                                    <asp:ListItem Value="UM">United States Minor Is.</asp:ListItem>
                                    <asp:ListItem Value="UY">Uruguay</asp:ListItem>
                                    <asp:ListItem Value="UZ">Uzbekistan</asp:ListItem>
                                    <asp:ListItem Value="VU">Vanuatu</asp:ListItem>
                                    <asp:ListItem Value="VE">Venezuela</asp:ListItem>
                                    <asp:ListItem Value="VN">Viet Nam</asp:ListItem>
                                    <asp:ListItem Value="VG">Virgin Islands (British)</asp:ListItem>
                                    <asp:ListItem Value="VI">Virgin Islands (U.S.)</asp:ListItem>
                                    <asp:ListItem Value="WF">Wallis And Futuna Islands</asp:ListItem>
                                    <asp:ListItem Value="EH">Western Sahara</asp:ListItem>
                                    <asp:ListItem Value="YE">Yemen</asp:ListItem>
                                    <asp:ListItem Value="YU">Yugoslavia</asp:ListItem>
                                    <asp:ListItem Value="ZR">Zaire</asp:ListItem>
                                    <asp:ListItem Value="ZM">Zambia</asp:ListItem>
                                    <asp:ListItem Value="ZW">Zimbabwe</asp:ListItem>
                                </asp:DropDownList>
                                &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator14" runat="server" 
                                    ControlToValidate="cmbBillingCountry" ErrorMessage="Billing Country" ForeColor="Red" 
                                    ValidationGroup="Checkout">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>                  
                        </table>
                </td>
            </tr>
            <tr>
                <td>
                    <span class="BBFieldCaption ChildSearchFieldCaptionBold">Please Choose Your Payment
                        Method</span> <span class="NCC_ScholarshipApp_requiredIndicator">&nbsp;*</span>
                    <asp:RadioButtonList ID="radPayment" runat="server" RepeatDirection="Horizontal">
                        <asp:ListItem Value="CC" Selected="True">Credit Card</asp:ListItem>
                        <asp:ListItem Value="Check">Check</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <td>
                    <table id="tblCC">
                        <tr>
                            <td class="NCC_ScholarshipApp_subSectionHeader">Credit Card Information:
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td class="style1" colspan="2">
                                            <span id="PC1689_ctl00_lblCcName" class="BBFieldCaption ChildSearchFieldCaption">Name
                                                    on Card</span><span class="NCC_ScholarshipApp_requiredIndicator">&nbsp;*</span>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtCcName" runat="server" Width="175px" CssClass="BBFieldControlCell ChildSearchFieldControlCell_long"
                                                MaxLength="50" />
                                            <asp:RequiredFieldValidator ID="reqCcName" runat="server" ControlToValidate="txtCcName"
                                                Text="*" ErrorMessage="Credit Card Name" ValidationGroup="Checkout" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style1" colspan="2">
                                            <span id="PC1689_ctl00_lblCcNumber" class="BBFieldCaption ChildSearchFieldCaption">Card
                                                    Number</span><span class="NCC_ScholarshipApp_requiredIndicator">&nbsp;*</span>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtCcNumber" runat="server" Width="150px" CssClass="BBFieldControlCell ChildSearchFieldControlCell_long"
                                                MaxLength="50" />
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator12" runat="server" ControlToValidate="txtCcNumber"
                                                Text="*" ErrorMessage="Credit Card Number" ValidationGroup="Checkout" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style1" colspan="2">
                                            <span id="PC1689_ctl00_lblCcSecurityCode" class="BBFieldCaption ChildSearchFieldCaption">Security Code</span><span class="NCC_ScholarshipApp_requiredIndicator">&nbsp;*</span>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtCcSecurityCode" runat="server" Width="40px" CssClass="BBFieldControlCell ChildSearchFieldControlCell_long"
                                                MaxLength="10" />
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator15" runat="server" ControlToValidate="txtCcSecurityCode"
                                                Text="*" ErrorMessage="Credit Card Security Code" ValidationGroup="Checkout" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style1" colspan="2">
                                            <span id="PC1689_ctl00_lblCcType" class="BBFieldCaption ChildSearchFieldCaption">Credit
                                                    Card Type</span><span class="NCC_ScholarshipApp_requiredIndicator">&nbsp;*</span>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="cmbCcType" runat="server">
                                                <asp:ListItem Value="">-- Select --</asp:ListItem>
                                                <asp:ListItem Value="MasterCard">MasterCard</asp:ListItem>
                                                <asp:ListItem Value="Visa">Visa</asp:ListItem>
                                                <asp:ListItem Value="AmericanExpress">American Express</asp:ListItem>
                                                <asp:ListItem Value="Discover">Discover</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator16" runat="server" ControlToValidate="cmbCcType"
                                                Text="*" ErrorMessage="Credit Card Type" ValidationGroup="Checkout" />
                                                    
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style1" colspan="2">
                                            <span id="PC1689_ctl00_lblCcExpDate" class="BBFieldCaption ChildSearchFieldCaption">Expiration Date</span><span class="NCC_ScholarshipApp_requiredIndicator">&nbsp;*</span>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="cmbCcExpMonth" runat="server">
                                                <asp:ListItem Value="">-- Month --</asp:ListItem>
                                                <asp:ListItem Value="01">01</asp:ListItem>
                                                <asp:ListItem Value="02">02</asp:ListItem>
                                                <asp:ListItem Value="03">03</asp:ListItem>
                                                <asp:ListItem Value="04">04</asp:ListItem>
                                                <asp:ListItem Value="05">05</asp:ListItem>
                                                <asp:ListItem Value="06">06</asp:ListItem>
                                                <asp:ListItem Value="07">07</asp:ListItem>
                                                <asp:ListItem Value="08">08</asp:ListItem>
                                                <asp:ListItem Value="09">09</asp:ListItem>
                                                <asp:ListItem Value="10">10</asp:ListItem>
                                                <asp:ListItem Value="11">11</asp:ListItem>
                                                <asp:ListItem Value="12">12</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator17" runat="server" ControlToValidate="cmbCcExpMonth"
                                                Text="*" ErrorMessage="Credit Card Expiration Month" ValidationGroup="Checkout" />
                                                    
                                                <asp:DropDownList ID="cmbCcExpYear" runat="server">
                                                </asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator18" runat="server" ControlToValidate="cmbCcExpYear"
                                                Text="*" ErrorMessage="Credit Card Expiration Year" ValidationGroup="Checkout" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style1" colspan="2">
                                            <span id="PC1689_ctl00_lblCcRecurrence" class="BBFieldCaption ChildSearchFieldCaption">Select Your Sponsorship Recurrence</span><span class="NCC_ScholarshipApp_requiredIndicator">&nbsp;*</span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td>
                                            <asp:RadioButtonList ID="radCcRecurrence" runat="server">
                                                <asp:ListItem Value="OneTimeGift">One Time Sponsorship Payment</asp:ListItem>
                                                <asp:ListItem Value="3">Monthly</asp:ListItem>
                                                <asp:ListItem Value="2">Quarterly</asp:ListItem>
                                                <asp:ListItem Value="0">Annually</asp:ListItem>
                                            </asp:RadioButtonList>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator19" runat="server" ControlToValidate="radCcRecurrence"
                                                Text="*" ErrorMessage="Payment Recurrence" ValidationGroup="Checkout" />
                                        </td>
                                        <td>
                                            <span id="PC1689_ctl00_reqVldCcRecurrence" style="color: Red; visibility: hidden;">&nbsp;*</span>
                                        </td>
                                        <td></td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                    <table id="tblCheck" style="display: none;">
                        <tr>
                            <td class="NCC_ScholarshipApp_subSectionHeader">Check Payment Information:
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td>Make check payable to:
                                        </td>
                                        <td>Mission of Mercy
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Mail your check to:
                                        </td>
                                        <td>Mission of Mercy
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td>P.O. Box 62600
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td>Colorado Springs, CO 80962
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td><asp:Button ID="btnSubmit" runat="server" Text="Submit Payment" onclick="btnSubmit_Click" ValidationGroup="Checkout" /></td>
            </tr>
        </table>
    </asp:View>
    <asp:View ID="viewThankYou" runat="server">
        <asp:Label id="Label1" runat="server" /><br /><br />
        <asp:Label ID="lblMessage" runat="server" /><br /><br />
        <asp:LinkButton ID="lnkThanksBack" runat="server" Text="Back to My Commitments" 
            onclick="lnkThanksBack_Click" />
    </asp:View>
</asp:MultiView>
