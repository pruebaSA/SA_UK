<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="What-We-Do.aspx.cs" Inherits="SalonAddict.What_We_Do" %>
<%@ Register TagPrefix="SA" TagName="Topic" Src="~/Modules/ContentTopic.ascx" %>
<%@ MasterType VirtualPath="~/MasterPages/OneColumn.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <SA:Topic ID="ContentTopic" runat="server" meta:resourceKey="ContentTopic" />
    <table style="margin:0 auto;" cellpadding="0" cellspacing="0" Width="740px" >
       <tr>
          <td style="padding-right:50px" >
            <div style="position:relative;" >
                <a style="position:absolute;top:140px;left:12px;display:block;width:80px;height:32px;z-index:999" href="#step-1" ></a>
                <img src='<%= Page.ResolveUrl(String.Format("~/App_themes/{0}/images/about_s1.jpg", base.Page.Theme)) %>' alt='<%= base.GetLocalResourceObject("StepOneHeader.Text") %>' />
            </div>
          </td>
          <td style="padding-right:50px" >
            <div style="position:relative;" >
                <a style="position:absolute;top:140px;left:12px;display:block;width:80px;height:32px;z-index:999" href="#step-2" ></a>
                <img src='<%= Page.ResolveUrl(String.Format("~/App_themes/{0}/images/about_s2.jpg", base.Page.Theme)) %>' alt='<%= base.GetLocalResourceObject("StepTwoHeader.Text") %>' />
            </div>
          </td>
          <td>
            <div style="position:relative;" >
                <a style="position:absolute;top:140px;left:12px;display:block;width:80px;height:32px;z-index:999" href="#step-3" ></a>
                <img src='<%= Page.ResolveUrl(String.Format("~/App_themes/{0}/images/about_s3.jpg", base.Page.Theme)) %>' alt='<%= base.GetLocalResourceObject("StepThreeHeader.Text") %>' />
            </div>
          </td>
       </tr>
    </table>
    <p>&nbsp;</p>
    <h1 id="step-1" ><%= base.GetLocalResourceObject("StepOneHeader.Text") %></h1>
    <p>
      <%= base.GetLocalResourceObject("StepOneContent.Text") %>
    </p>
    <div>&nbsp;</div>
    <h1 id="step-2" ><%= base.GetLocalResourceObject("StepTwoHeader.Text") %></h1>
    <p>
       <%= base.GetLocalResourceObject("StepTwoContent.Text") %>
    </p>
    <div>&nbsp;</div>
    <h1 id="step-3" ><%= base.GetLocalResourceObject("StepThreeHeader.Text") %></h1>
    <p>
       <%= base.GetLocalResourceObject("StepThreeContent.Text") %>
    </p>
    <p>
       <%= base.GetLocalResourceObject("StepThreeSubContent.Text") %>
    </p>
    <p>&nbsp;</p>
</asp:Content>

