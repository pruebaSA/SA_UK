<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="IFRAME.DefaultPage" %>
<%@ Register TagPrefix="IFRM" TagName="ServiceList" Src="~/Modules/ServiceDropDownList.ascx" %>
<%@ Register TagPrefix="IFRM" TagName="DateTextBox" Src="~/Modules/DateTextBox.ascx" %>
<%@ Register TagPrefix="IFRM" TagName="SalonHours" Src="~/Modules/HomepageSalonHours.ascx" %>

<asp:Content ID="c1" ContentPlaceHolderID="ph1c" runat="server">
<asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" DefaultButton="btnSubmit" >
    <div style="position:relative;" >
       <h1 style="margin-bottom:30px;" ><asp:Literal ID="ltrHeader" runat="server" EnableViewState="false" ></asp:Literal></h1>
       <div style="position:absolute;top:26px;left:0px;font-size:11px;" >
          <asp:Literal ID="ltrAddress" runat="server" EnableViewState="false" ></asp:Literal>
       </div>
    </div>
    <table cellpadding="0" cellspacing="0" width="100%" >
       <tr>
          <td style="width:550px" >
            <table class="details" cellpadding="0" cellspacing="0" >
               <tr>
                  <td class="title" >
                     <%= base.GetLocaleResourceString("ltrService.Text") %> *
                  </td>
                  <td class="data-item" >
                     <IFRM:ServiceList ID="ddlService" runat="server" meta:resourceKey="ddlService" />
                  </td>
               </tr>
               <tr>
                  <td class="title" >
                     <%= base.GetLocaleResourceString("ltrDate.Text") %>
                  </td>
                  <td class="data-item" >
                     <IFRM:DateTextBox ID="txtDate" runat="server" meta:resourceKey="txtDate" />
                  </td>
               </tr>
               <tr><td colspan="2" >&nbsp;</td></tr>
               <tr>
                  <td class="title" ></td>
                  <td class="data-item" >
                     <asp:Button ID="btnSubmit" runat="server" SkinID="SubmitButton" OnClick="btnSubmit_Click" meta:resourceKey="btnSubmit" />
                  </td>
               </tr>
            </table>
          </td>
          <td>
              <IFRM:SalonHours ID="cntrlHours" runat="server" />
          </td>
       </tr>
    </table>
</asp:Panel>
</asp:Content>
