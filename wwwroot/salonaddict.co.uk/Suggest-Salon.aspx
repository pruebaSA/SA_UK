<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Suggest-Salon.aspx.cs" Inherits="SalonAddict.Suggest_Salon" %>
<%@ Register TagPrefix="SA" TagName="Topic" Src="~/Modules/ContentTopic.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="Message" Src="~/Modules/Message.ascx" %>
<%@ Register TagPrefix="SA" TagName="EmailTextBox" Src="~/Modules/TextBoxes/EmailTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="CountryDropDownList" Src="~/Modules/DropDownLists/CountryDropDownList.ascx" %>
<%@ MasterType VirtualPath="~/MasterPages/OneColumn.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <SA:Topic ID="ContentTopic" runat="server" meta:resourceKey="ContentTopic" />
    <SA:Message ID="lblMessage" runat="server" Auto="false" />
    <table cellpadding="0" cellspacing="0" width="950px" >
        <tr>
           <td>
            <div style="height:200px;" class="grey-panel" >
                <h2><%= base.GetLocalResourceObject("lblHeaderCompany.Text") %></h2>
                <table class="details" cellpadding="0" cellspacing="0" >
                   <tr>
                      <td class="title" ><%= base.GetLocalResourceObject("lblCompany.Text") %></td>
                      <td class="data-item" >
                         <SA:TextBox ID="txtCompany" runat="server" MaxLength="100" Width="200px" EnableViewState="false" meta:resourceKey="txtCompany" />
                      </td>
                   </tr>
                   <tr>
                      <td class="title" ><%= base.GetLocalResourceObject("lblAddress1.Text") %></td>
                      <td class="data-item" >
                         <SA:TextBox ID="txtAddress1" runat="server" MaxLength="100" Width="200px" EnableViewState="false" meta:resourceKey="txtAddress1" />
                      </td>
                   </tr>
                   <tr>
                      <td class="title" ><%= base.GetLocalResourceObject("lblAddress2.Text") %></td>
                      <td class="data-item" >
                         <SA:TextBox ID="txtAddress2" runat="server" IsRequired="false" MaxLength="100" Width="200px" EnableViewState="false"  />
                      </td>
                   </tr>
                   <tr>
                      <td class="title" ><%= base.GetLocalResourceObject("lblCountry.Text") %></td>
                      <td class="data-item" >
                         <SA:CountryDropDownList ID="Country" runat="server" AutoPostback="false" Width="130px" meta:resourceKey="Country" />
                      </td>
                   </tr>
                </table>
            </div>
           </td>
           <td class="data-item" style="padding-left:20px;">
            <div style="height:200px;" class="pink-panel" >
                <h2><%= base.GetLocalResourceObject("lblHeaderPersonal.Text").ToString() %></h2>
                <table class="details" cellpadding="0" cellspacing="0" >
                   <tr>
                      <td class="title" ><%= base.GetLocalResourceObject("lblFirstName.Text") %></td>
                      <td class="title" ><%= base.GetLocalResourceObject("lblSurname.Text") %></td>
                   </tr>
                   <tr>
                      <td class="data-item" >
                         <SA:TextBox ID="txtFirstName" runat="server" MaxLength="50" Width="150px" meta:resourceKey="txtFirstName" />
                      </td>
                      <td class="data-item" >
                         <SA:TextBox ID="txtSurname" runat="server" MaxLength="50" Width="150px" meta:resourceKey="txtSurname" />
                      </td>
                   </tr>
                   <tr>
                      <td class="title" colspan="2" >
                          <%= base.GetLocalResourceObject("lblEmail.Text") %>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" colspan="2" >
                          <SA:EmailTextBox ID="txtEmail" runat="server" MaxLength="100" Width="324px" />
                      </td>
                   </tr>
                   <tr>
                      <td class="title" colspan="2" >
                         <asp:CheckBox ID="cbSubscribe" runat="server" Checked="true" EnableViewState="false" />
                         <asp:Label ID="lblSubscribe" runat="server" EnableViewState="false" Font-Size="10px" meta:resourceKey="lblSubscribe" ></asp:Label>
                      </td>
                   </tr>
                </table>
                <div style="text-align:right;" >
                   <asp:Button ID="btnSubmit" runat="server" SkinID="BlackButtonSmall" OnClick="btnSubmit_Click" meta:resourceKey="btnSubmit" />
                </div>
            </div>
           </td>
        </tr>
    </table>
</asp:Content>
