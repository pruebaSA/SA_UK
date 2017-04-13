<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchPanelOne.ascx.cs" Inherits="SalonAddict.Templates.SearchPanelOne" %>
<%@ Register TagPrefix="SA" TagName="CityTowns" Src="~/Modules/DropDownLists/CityTownDropDownList.ascx" %>
<%@ Register TagPrefix="SA" TagName="Localities" Src="~/Modules/DropDownLists/LocalityDropDownList.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="Service" Src="~/Modules/DropDownLists/ServiceTypeDropDownList.ascx" %>
<%@ Register TagPrefix="SA" TagName="Category" Src="~/Modules/DropDownLists/ServiceCategoryDropDownList.ascx" %>

<asp:Panel ID="pnl" runat="server" CssClass="template-searchpanelone" DefaultButton="btn" >
<asp:UpdatePanel ID="ups" runat="server" >
  <ContentTemplate>
    <div class="search-option-one" >
        <asp:RadioButton ID="rbl1" runat="server" GroupName="rbl" Checked="true" onchange="enable_search_bylocation()" />
    </div>
    <asp:Panel ID="pnlSearchCity" runat="server" CssClass="search-city" onclick="enable_search_bylocation()">
        <SA:CityTowns 
            ID="CityTowns" 
            runat="server" 
            Width="190px"
            AutoPostback="true"
            OnSelectedIndexChanged="CityTowns_SelectedIndexChanged" 
            meta:resourceKey="CityTowns" />
    </asp:Panel>
    <asp:Panel ID="pnlSearchLocality" runat="server" CssClass="search-locality" onclick="enable_search_bylocation()" >
        <SA:Localities 
            ID="Localities" 
            runat="server" 
            Width="190px"
            meta:resourceKey="Localities" />
    </asp:Panel>
    <div class="search-option-two" >
        <asp:RadioButton ID="rbl2" runat="server" GroupName="rbl" onchange="enable_search_bykeyword()" />
    </div>
    <asp:Panel ID="pnlKeywords" runat="server" CssClass="search-keyword-disabled" onclick="enable_search_bykeyword()" >
        <asp:TextBox ID="txtKeyword" runat="server" MaxLength="50" autocomplete="off" onfocus="clear_keyword_search();clear_keyword_search();" onkeyUp="suggest_keywords(this)" onblur="suggestion_blur()" ></asp:TextBox>
        <div id="keyword_menu" ></div> 
    </asp:Panel>
    <div class="sdate-label" >
        <%= base.GetLocalResourceObject("lblDate.Text") %>
    </div>
    <div class="sdate" >
        <asp:TextBox ID="txtDate" runat="server" MaxLength="10" Width="85px" ></asp:TextBox>
    </div>
    <div class="sdate-button" >
        <asp:Button ID="btnCalendar" runat="server" SkinID="CalendarButtonMedium" />
        <ajaxToolkit:CalendarExtender ID="ceDateTxt" runat="server" TargetControlID="txtDate" />
        <ajaxToolkit:CalendarExtender ID="ceDateBtn" runat="server" PopupButtonID="btnCalendar" TargetControlID="txtDate" />
    </div>
    <div class="stype-label" >
        <%= base.GetLocalResourceObject("lblService.Text") %>
    </div>
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
  </asp:UpdatePanel>
  <div class="search-button" >
      <asp:Button ID="btn" runat="server" SkinID="BlackButtonSmall" OnClick="btn_Click" meta:resourceKey="btn" />
  </div>
</asp:Panel>

<asp:ScriptManagerProxy ID="smp" runat="server" >
  <Scripts>
     <asp:ScriptReference Path="~/js/SalonAddict.SuggestionMenu.js" />
  </Scripts>
  <Services>
     <asp:ServiceReference Path="~/Services/KeywordSuggestionService.svc" />
  </Services>
</asp:ScriptManagerProxy>

<script type="text/javascript" language="javascript" >
    var _menuID = 'keyword_menu';
    var _txtKeywordID = '<%= txtKeyword.ClientID %>';
    var _suggestion_processing = false;
    
    function suggest_keywords(obj) {
        var value = obj.value;
        var menu = new SalonAddict.SuggestionMenu(_menuID, _txtKeywordID);
        
        if (value.length > 1) {
            if (_suggestion_processing == false) {
                _suggestion_processing = true;
                SalonAddict.Services.KeywordSuggestionService.GetSuggestions(value, callback, null, null);
            }
        }
        else {
            menu.close();
        }
    }
    
    function callback(response) {
        var menu = new SalonAddict.SuggestionMenu(_menuID, _txtKeywordID);
        menu.clear();
        
        if (response.length > 0) {
            menu.renderControls(response);
            menu.show();
        }
        else {
            menu.clear();
            menu.message('<%= base.GetLocalResourceObject("Suggestions.NoResults") %>');
            menu.show();
        }
        _suggestion_processing = false;
    }

    function suggestion_blur() {
        var menu = new SalonAddict.SuggestionMenu(_menuID, _txtKeywordID);
        menu.close();
        var txtKeyword = document.getElementById(_txtKeywordID);
        if (txtKeyword.value == "") {
            reset_keyword_search();
        }
    }

    function enable_search_bylocation() {
        var rb = document.getElementById('<%= rbl1.ClientID %>');
        rb.checked = true;
        var services = document.getElementById('<%= Service.ClientID %>');
        services.selectedIndex = 1;
        $('<%= "#" + pnlSearchCity.ClientID %>').attr('class','search-city');
        $('<%= "#" + pnlSearchLocality.ClientID %>').attr('class', 'search-locality');
        $('<%= "#" + pnlKeywords.ClientID %>').attr('class', 'search-keyword-disabled');
        reset_keyword_search();
    }

    function enable_search_bykeyword() {
        var rb = document.getElementById('<%= rbl2.ClientID %>');
        rb.checked = true;
        var services = document.getElementById('<%= Service.ClientID %>');
        services.selectedIndex = 0;
        $('<%= "#" + pnlSearchCity.ClientID %>').attr('class', 'search-city-disabled');
        $('<%= "#" + pnlSearchLocality.ClientID %>').attr('class', 'search-locality-disabled');
        $('<%= "#" + pnlKeywords.ClientID %>').attr('class', 'search-keyword');
        var city_towns = document.getElementById('<%= CityTowns.ClientID %>');
        var localities = document.getElementById('<%= Localities.ClientID %>');
        city_towns.options[0].selected = true;
        localities.options[0].selected = true;
        localities.options.length = 1;
    }

    function clear_keyword_search() {
        var txtKeyword = document.getElementById(_txtKeywordID);
        var default_text = '<%= base.GetLocalResourceObject("txtKeyword.Text") %>';
        if (txtKeyword.value == default_text) {
            txtKeyword.value = '';
        }
    }
    
    function reset_keyword_search() {
        var txtKeyword = document.getElementById(_txtKeywordID);
        var default_text = '<%= base.GetLocalResourceObject("txtKeyword.Text") %>';
        txtKeyword.value = default_text;
    }
</script>

<script type="text/javascript" language="javascript" >
    enable_search_bylocation();
</script>