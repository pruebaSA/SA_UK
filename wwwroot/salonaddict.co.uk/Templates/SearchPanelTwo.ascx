<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchPanelTwo.ascx.cs" Inherits="SalonAddict.Templates.SearchPanelTwo" %>
<%@ Register TagPrefix="SA" TagName="CityTowns" Src="~/Modules/DropDownLists/CityTownDropDownList.ascx" %>
<%@ Register TagPrefix="SA" TagName="Localities" Src="~/Modules/DropDownLists/LocalityDropDownList.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="Service" Src="~/Modules/DropDownLists/ServiceTypeDropDownList.ascx" %>
<%@ Register TagPrefix="SA" TagName="Category" Src="~/Modules/DropDownLists/ServiceCategoryDropDownList.ascx" %>

<asp:Panel ID="pnl" runat="server" CssClass="template-searchpaneltwo" DefaultButton="btn" >
 <div class="title" >
    <%= base.GetLocalResourceObject("lblTitle.Text") %>
 </div>
 <asp:UpdatePanel ID="upa" runat="server" >
    <ContentTemplate>
    <div class="search-city" >
       <SA:CityTowns
          ID="CityTowns" 
          runat="server" 
          Width="190px" 
          IsRequired="false" 
          AutoPostback="true"
          meta:resourceKey="CityTowns" 
          OnSelectedIndexChanged="CityTowns_SelectedIndexChanged" />
    </div>
    <div class="search-locality" >
       <SA:Localities 
          ID="Localities" 
          runat="server" 
          Width="190px" 
          IsRequired="false"
          meta:resourceKey="Localities" />
    </div>
    </ContentTemplate>
    <Triggers>
       <asp:AsyncPostBackTrigger ControlID="CityTowns" />
    </Triggers>
  </asp:UpdatePanel>
  <div class="sdate-label" >
     <%= base.GetLocalResourceObject("lblDate.Text") %>
  </div>
  <div class="sdate" >
     <asp:TextBox ID="txtDate" runat="server" MaxLength="10" Width="75px" ></asp:TextBox>
  </div>
  <div class="sdate-button" >
    <asp:Button ID="btnCalendar" runat="server" SkinID="CalendarButtonMedium" />
    <ajaxToolkit:CalendarExtender ID="ceDateTxt" runat="server" TargetControlID="txtDate" />
    <ajaxToolkit:CalendarExtender ID="ceDateBtn" runat="server" PopupButtonID="btnCalendar" TargetControlID="txtDate" />
  </div>
  <div class="stype-label" >
      <%= base.GetLocalResourceObject("lblService.Text") %>
  </div>
  <asp:UpdatePanel ID="ups" runat="server" >
     <ContentTemplate>
      <div class="stype" >
         <SA:Service 
            ID="Service" 
            runat="server" 
            Width="190px"
            IsRequired="false" 
            OnSelectedIndexChanged="Service_SelectedIndexChanged"  
            meta:resourceKey="Service" />
      </div>
      <div class="scat" >
         <SA:Category 
            ID="Category" 
            runat="server" 
            Width="190px"
            IsRequired="false" 
            meta:resourceKey="Category" />
      </div>
     </ContentTemplate>
     <Triggers>
        <asp:AsyncPostBackTrigger ControlID="Service" />
     </Triggers>
  </asp:UpdatePanel>
  <div class="search-button" >
      <asp:Button ID="btn" runat="server" SkinID="BlackButtonSmall" OnClick="btn_Click" meta:resourceKey="btn" />
  </div>
</asp:Panel>