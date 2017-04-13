<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BusinessDetails.ascx.cs" Inherits="SalonAddict.Administration.Modules.BusinessDetails" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Administration/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="NumericTextBox" Src="~/Administration/Modules/TextBoxes/NumericTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="DecimalTextBox" Src="~/Administration/Modules/TextBoxes/DecimalTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="EmailTextBox" Src="~/Administration/Modules/TextBoxes/EmailTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="CityTownDropDownList" Src="~/Administration/Modules/DropDownLists/CityTownDropDownList.ascx" %>
<%@ Register TagPrefix="SA" TagName="CountryDropDownList" Src="~/Administration/Modules/DropDownLists/CountryDropDownList.ascx" %>
<%@ Register TagPrefix="SA" TagName="LanguageDropDownList" Src="~/Administration/Modules/DropDownLists/LanguageDropDownList.ascx" %>
<%@ Register TagPrefix="SA" TagName="StateProvinceDropDownList" Src="~/Administration/Modules/DropDownLists/StateProvinceDropDownList.ascx" %>
<%@ Register TagPrefix="SA" TagName="TaxProviderDropDownList" Src="~/Administration/Modules/DropDownLists/TaxProviderDropDownList.ascx" %>

<%@ Import Namespace="SalonAddict.BusinessAccess.Implementation" %>
<%@ Import Namespace="SalonAddict.BusinessAccess.Configuration" %>

<% if(this.Action == ActionType.Create)
   { %>
<div class="section-header">
    <div class="title">
        <img src="images/ico-customers.png" alt="" />
        Add a new business <a href="Businesses.aspx" title="Back to business list">(back to business list)</a>
    </div>
    <div class="options">
        <asp:Button ID="AddButton" runat="server" Text="Add" OnClick="AddButton_Click" ToolTip="Add business" />
    </div>
</div>
<% } 
   else
   { %>
<div class="section-header">
    <div class="title">
        <img src="images/ico-customers.png" alt="" />
        Edit business details <a href="Businesses.aspx" title="Back to business list">(back to business list)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" ToolTip="Save business changes" />
        <% if (Roles.IsUserInRole("OWNER") || Roles.IsUserInRole("VP_SALES") || Roles.IsUserInRole("SALES_MGR"))
           { %>
        <asp:Button ID="DeleteButton" runat="server" Text="Delete" OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="Delete business" OnClientClick="return confirm('Are you sure?')" />
        <% } %>
    </div>
</div>
<% } %>
<ajaxToolkit:TabContainer runat="server" ID="BusinessTabs" ActiveTabIndex="0" >
    <ajaxToolkit:TabPanel runat="server" ID="pnlBusinessInfo" HeaderText="Business Info" >
        <ContentTemplate>
            <table class="details">
                 <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblName" 
                            runat="server" 
                            Text="Name:"
                            IsRequired="true"
                            ToolTip="The business's name." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:TextBox 
                            ID="txtName" 
                            runat="server" 
                            MaxLength="100"
                            ErrorMessage="Name is required" />
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblCode" 
                            runat="server" 
                            Text="Code:"
                            IsRequired="true"
                            ToolTip="The business's identifier / code." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:UpdatePanel ID="up" runat="server" >
                           <ContentTemplate>
                            <table class="details" cellpadding="0" cellspacing="0" >
                               <tr>
                                  <td>
                                     <SA:TextBox 
                                        ID="txtCode" 
                                        runat="server" 
                                        MaxLength="100"
                                        ErrorMessage="Code is required" />
                                  </td>
                                  <td class="data-item">
                                     <asp:Button ID="btnCheckUsernameAvailability" runat="server" Text="Check Availability" CausesValidation="false" OnClick="btnCheckCodeAvailability_Click" />
                                  </td>
                                  <td class="data-item" > 
                                      <nowrap><asp:Label ID="lblAvailability" runat="server" Font-Bold="true" ></asp:Label></nowrap>
                                  </td>
                               </tr>
                            </table>
                           </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                 <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblPhoneNumber" 
                            runat="server" 
                            Text="Phone number:"
                            ToolTip="The customer's phone number (optional)." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:TextBox ID="txtPhoneNumber" runat="server" MaxLength="50" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblEmail" 
                            runat="server" 
                            Text="Email address:"
                            ToolTip="The bussiness's email address." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:EmailTextBox ID="txtEmail" runat="server" IsRequired="false" />
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblFaxNumber" 
                            runat="server" 
                            Text="Fax number:"
                            ToolTip="The business's fax number (optional)." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:TextBox ID="txtFaxNumber" runat="server" MaxLength="50" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblWebsite" 
                            runat="server" 
                            Text="Website:"
                            ToolTip="The business's website (optional)." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:TextBox ID="txtWebsite" runat="server" MaxLength="500" ></asp:TextBox>
                    </td>
                </tr>
                <% if(this.Action == ActionType.Edit)
                   { %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblRatingSum" 
                            runat="server" 
                            Text="Rating sum:"
                            ToolTip="The business's rating sum." 
                            IsRequired="true"
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:NumericTextBox 
                            ID="txtRatingSum" 
                            runat="server" 
                            RequiredErrorMessage="Rating sum is required"
                            MinimumValue="0" 
                            MaximumValue="999999" 
                            Value="0"
                            RangeErrorMessage="The value must be from 0 to 999999">
                        </SA:NumericTextBox>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblTotalRatingVotes" 
                            runat="server" 
                            Text="Total rating votes:"
                            IsRequired="true"
                            ToolTip="The business's total number of raing votes." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:NumericTextBox 
                            ID="txtTotalRatingVotes" 
                            runat="server" 
                            MinimumValue="0" 
                            MaximumValue="999999999"
                            Value="0"
                            RequiredErrorMessage="Total rating votes is required" 
                            RangeErrorMessage="Value must be greater than 0" >
                        </SA:NumericTextBox>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblTotalReviews" 
                            runat="server" 
                            Text="Total reviews:"
                            IsRequired="true"
                            ToolTip="The number of business reviews." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:NumericTextBox 
                            ID="txtTotalReviews" 
                            runat="server" 
                            MinimumValue="0" 
                            MaximumValue="999999999"
                            Value="0"
                            RequiredErrorMessage="Total reviews is required" 
                            RangeErrorMessage="Value must be greater than 0" >
                        </SA:NumericTextBox>
                    </td>
                </tr>
                <% } %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblAllowEmailNotifications" 
                            runat="server" 
                            Text="Allow email notifications:"
                            ToolTip="Determines whether the associated accounts can be notified by email." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:CheckBox ID="cbAllowEmailNotifications" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblAllowSMSNotifications" 
                            runat="server" 
                            Text="Allow SMS notifications:"
                            ToolTip="Determines whether the associated accounts can be notified by SMS." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:CheckBox ID="cbAllowSMSNotifications" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblIsGuest" 
                            runat="server" 
                            Text="Is Guest:"
                            ToolTip="Determines whether the account is a guest account on the website." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:CheckBox ID="cbIsGuest" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblPublished" 
                            runat="server" 
                            Text="Published:"
                            ToolTip="Determines whether the account is available on the website." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:CheckBox ID="cbPublished" runat="server" />
                    </td>
                </tr>
                <% if (this.Action == ActionType.Edit)
                   { %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblCreatedOn" 
                            runat="server" 
                            Text="Registration Date:"
                            ToolTip="Date when business account was created." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:Literal ID="ltrCreatedOn" runat="server" ></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblUpdatedOn" 
                            runat="server" 
                            Text="Last Updated:"
                            ToolTip="Date when business account was last updated." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:Literal ID="ltrUpdatedOn" runat="server" ></asp:Literal>
                    </td>
                </tr> 
                <% } %>
             </table>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlAddress" HeaderText="Business Billing Address">
        <ContentTemplate>
            <div style="position:relative; height:380px;" >
            <div id="map_canvas" style="position:absolute;height:330px;width:500px;top:5px;right:10px;border:solid 1px #ccc;" ></div>
            <asp:UpdatePanel ID="pnlAddress_Ajax" runat="server" >
                <ContentTemplate>
                    <table class="details">
                        <tr>
                            <td class="title">
                                <SA:ToolTipLabel 
                                    ID="lblAddressLine1" 
                                    runat="server" 
                                    Text="Address:"
                                    ToolTip="The business's address (optional)." 
                                    IsRequired="true"
                                    ToolTipImage="~/Administration/images/ico-help.gif" />
                            </td>
                            <td class="data-item">
                                <SA:TextBox ID="txtAddressLine1"    
                                        runat="server"
                                        MaxLength="100"
                                        ErrorMessage="Address line 1 is a required field." />
                            </td>
                        </tr>
                        <tr>
                            <td class="title">
                                <SA:ToolTipLabel 
                                    ID="lblAddressLine2" 
                                    runat="server" 
                                    Text="Address continued:"
                                    ToolTip="The business's address continued (optional)." 
                                    ToolTipImage="~/Administration/images/ico-help.gif" />
                            </td>
                            <td class="data-item">
                                <asp:TextBox ID="txtAddressLine2" runat="server" MaxLength="100" ></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="title">
                                <SA:ToolTipLabel 
                                    ID="lblCityTown" 
                                    runat="server" 
                                    Text="City/Town:"
                                    ToolTip="The business's city/town." 
                                    IsRequired="true"
                                    ToolTipImage="~/Administration/images/ico-help.gif" />
                            </td>
                            <td class="data-item">
                                <SA:CityTownDropDownList 
                                    ID="ddlCityTown"
                                    runat="server" 
                                    DefaultText="Choose a City/Town"
                                    ErrorMessage="City/Town is a required field."
                                    AutoPostback="true" />
                            </td>
                        </tr>
                        <tr>
                            <td class="title">
                                <SA:ToolTipLabel 
                                    ID="lblZipPostalCode" 
                                    runat="server" 
                                    Text="Zip/Postal code:"
                                    ToolTip="The customer's zip/postal code (optional)." 
                                    ToolTipImage="~/Administration/images/ico-help.gif" />
                            </td>
                            <td class="data-item">
                                <SA:TextBox ID="txtZipPostalCode"    
                                        runat="server"
                                        MaxLength="20"
                                        ErrorMessage="Zip/Postal code is a required field." />
                            </td>
                        </tr>
                        <tr>
                            <td class="title">
                                <SA:ToolTipLabel 
                                    ID="lblLongitude" 
                                    runat="server" 
                                    Text="Longitude:"
                                    ToolTip="The business's longitude." 
                                    ToolTipImage="~/Administration/images/ico-help.gif" />
                            </td>
                            <td class="data-item">
                                <asp:TextBox ID="txtLongitude" runat="server" MaxLength="10" ></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="title">
                                <SA:ToolTipLabel 
                                    ID="lblLatitude" 
                                    runat="server" 
                                    Text="Latitude:"
                                    ToolTip="The business's latitude." 
                                    ToolTipImage="~/Administration/images/ico-help.gif" />
                            </td>
                            <td class="data-item">
                                <asp:TextBox ID="txtLatitude" runat="server" MaxLength="10" ></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                           <td></td>
                           <td class="data-item" >
                              <p>
                                <b>Auto populate geocodes using google maps API!</b>
                              </p>
                              <asp:Button ID="btnGeoCode" runat="server" OnClientClick="getGeoCodes();return false;" Text="Longitude/Latitude" />
                           </td>
                        </tr>
                     </table>
                </ContentTemplate>
            </asp:UpdatePanel>
            </div>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel ID="pnlLocalities" runat="server" HeaderText="Localities" >
      <ContentTemplate>
           <p>
              <% if (this.Action == ActionType.Edit)
                 { %>
              <a href='<%= "BusinessLocalities.aspx?BusinessGUID=" + this.BusinessGUID %>'>click here to manage localities</a>
              <% } %>
           </p>
           <asp:ListView 
                ID="lvLocalities" 
                runat="server" 
                GroupItemCount="1"
                GroupPlaceholderID="GroupPlaceHolderID" 
                ItemPlaceholderID="ItemPlaceHolderID" >
                <LayoutTemplate>
                     <ol>
                        <asp:PlaceHolder ID="GroupPlaceHolderID" runat="server" ></asp:PlaceHolder>
                     </ol>
                </LayoutTemplate>
                <GroupTemplate>
                     <asp:PlaceHolder ID="ItemPlaceHolderID" runat="server" ></asp:PlaceHolder>
                </GroupTemplate>
                <ItemTemplate>
                    <li><%# GetLocalityFullName((SalonAddict.BusinessAccess.ModelClasses.LocalityBusinessMapping)Container.DataItem)%></li>
                </ItemTemplate>
           </asp:ListView>
      </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel ID="pnlDirections" runat="server" HeaderText="Directions" >
       <ContentTemplate>
           <style type="text/css" >
              .instructions { padding-top:25px; padding-bottom:25px; width:700px; }
           </style>
           <p>
              <% if (this.Action == ActionType.Edit)
                 { %>
              <a href='<%= "BusinessDirections.aspx?BusinessGUID=" + this.BusinessGUID %>'>click here to manage directions</a>
              <% } %>
           </p>
           <asp:ListView 
                ID="lvDirections" 
                runat="server" 
                GroupItemCount="1"
                GroupPlaceholderID="GroupPlaceHolderID" 
                ItemPlaceholderID="ItemPlaceHolderID" >
                <LayoutTemplate>
                     <asp:PlaceHolder ID="GroupPlaceHolderID" runat="server" ></asp:PlaceHolder>
                </LayoutTemplate>
                <GroupTemplate>
                     <asp:PlaceHolder ID="ItemPlaceHolderID" runat="server" ></asp:PlaceHolder>
                </GroupTemplate>
                <ItemTemplate>
                    <div class="instructions" ><%# Eval("Instructions")%></div>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <div class="instructions" style="border-top:solid 1px #e7e7e7" ><%# Eval("Instructions")%></div>
                </AlternatingItemTemplate>
           </asp:ListView>
       </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlSEO" HeaderText="SEO">
        <ContentTemplate>
            <table class="details">
               <tr>
                    <td class="title" style="vertical-align:top;">
                        <SA:ToolTipLabel 
                            ID="lblPicture" 
                            runat="server" 
                            Text="Picture:"
                            ToolTip="The business's picture." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:Image ID="imgPicture" runat="server" /><br />
                        <p><asp:FileUpload ID="fuPicture" runat="server" /></p>
                        <asp:Label ID="lblPictureError" runat="server" EnableViewState="false" CssClass="message-error" Text="only files of type *.(jpg|jpeg|png|gif) can be uploaded" Visible="false" ></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblShortDescription" 
                            runat="server" 
                            Text="Description:"
                            ToolTip="The business's short description approx. 400 characters (optional)." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:TextBox ID="txtShortDescription" runat="server" Width="600px" MaxLength="400" ></asp:TextBox>
                    </td>
                </tr>
             </table>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlNotes" HeaderText="Notes" >
       <ContentTemplate>
            <asp:UpdatePanel ID="upNotes" runat="server" >
               <ContentTemplate>
                     <asp:GridView 
                        ID="gvNotes" 
                        runat="server" 
                        Width="100%"
                        DataKeyNames="BusinessNoteID"
                        OnRowDeleting="gvNotes_RowDeleting"
                        AutoGenerateColumns="False" >
                        <Columns> 
                            <asp:TemplateField HeaderText="Note" >
                                <ItemTemplate>
                                    <%# Server.HtmlEncode(Eval("Note").ToString())  %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="50px" >
                                <ItemTemplate>
                                   <asp:LinkButton ID="lbDelete" runat="server" Text="Delete" OnClientClick="return confirm('Are you sure?')" CommandName="Delete" CausesValidation="false" ></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                       </Columns>
                    </asp:GridView>
                    <br /><hr /><br />
                    <table style="margin-left:-10px;" cellpadding="0" cellspacing="10" >
                       <tr>
                          <td>
                             <asp:TextBox 
                                ID="txtNewNote" 
                                runat="server" 
                                TextMode="MultiLine" 
                                Height="80px" 
                                EnableViewState="false"
                                Width="600px" >
                             </asp:TextBox>
                          </td>
                       </tr>
                       <tr>
                          <td style="text-align:right">
                             &nbsp; <asp:Button ID="btnAddNote" runat="server" Text="Add New Note" OnClick="btnAddNote_Click" CausesValidation="false" />
                          </td>
                       </tr>
                    </table>
               </ContentTemplate>
            </asp:UpdatePanel>
       </ContentTemplate>
    </ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>

<%= SalonAddict.BusinessAccess.Implementation.ConfigurationManager.GetSettingValue("Common.GoogleMapsScript")%>
<script type="text/javascript" language="javascript" >
    var _txtAddressLine1 = document.getElementById('<%= txtAddressLine1.ClientID %>');
    var _ddlCityTown = document.getElementById('<%= ddlCityTown.ClientID %>');
    var _txtZipPostalCode = document.getElementById('<%= txtZipPostalCode.ClientID %>');
   

    function getAddress()
    {
        var address = new Array();
        if(_txtAddressLine1.value != "")
        {
            address.push(_txtAddressLine1.value);
            if (_ddlCityTown.options[_ddlCityTown.selectedIndex].value != "")
            {
                address.push(_ddlCityTown.options[_ddlCityTown.selectedIndex].text);
            }
            if(_txtZipPostalCode.value != "")
            {
                address.push(_txtZipPostalCode.value);
            }
        }
        return address.join(", ");
    }
    
    function getGeoCodes()
    {
        var address = getAddress();
        if(address == null || address == "")
        {
            return;
        }
        try
        {
            displayMap(address);
        }
        catch(e)
        {
           alert(e.message);
        }
    }
    
	function displayMap(address) 
	{
	   var map = new GMap2(document.getElementById("map_canvas"));
       map.addControl(new GSmallMapControl());
       map.addControl(new GMapTypeControl());
       var geocoder = new GClientGeocoder();
       if (geocoder) 
       {
         geocoder.getLatLng(
           address,
           function(point) {
		     document.getElementById('<%= txtLatitude.ClientID %>').value = point.lat().toFixed(5);
	         document.getElementById('<%= txtLongitude.ClientID %>').value = point.lng().toFixed(5);
		     map.clearOverlays()
			 map.setCenter(point, 13);
             var marker = new GMarker(point, {draggable: true});  
		     map.addOverlay(marker);

		     GEvent.addListener(marker, "dragend", function() {
                var pt = marker.getPoint();
	            map.panTo(pt);
                document.getElementById('<%= txtLatitude.ClientID %>').value = pt.lat().toFixed(5);
	            document.getElementById('<%= txtLongitude.ClientID %>').value = pt.lng().toFixed(5);
             });


	        GEvent.addListener(map, "moveend", function() {
		      map.clearOverlays();
              var center = map.getCenter();
		      var marker = new GMarker(center, {draggable: true});
		      map.addOverlay(marker);
		      document.getElementById('<%= txtLatitude.ClientID %>').value = center.lat().toFixed(5);
	          document.getElementById('<%= txtLongitude.ClientID %>').value = center.lng().toFixed(5);

	          GEvent.addListener(marker, "dragend", function() {
                var pt = marker.getPoint();
	            map.panTo(pt);
                document.getElementById('<%= txtLatitude.ClientID %>').value = pt.lat().toFixed(5);
	            document.getElementById('<%= txtLongitude.ClientID %>').value = pt.lng().toFixed(5);
              });
            });
          });
        }
      }
</script>