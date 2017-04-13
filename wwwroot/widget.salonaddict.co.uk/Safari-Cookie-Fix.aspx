<%@ Page Language="C#" MasterPageFile="~/MasterPages/Root.Master" AutoEventWireup="true" CodeBehind="Safari-Cookie-Fix.aspx.cs" Inherits="IFRAME.Safari_Cookie_FixPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph" runat="server" >
  <p>&nbsp;</p>
  <center>
     <img src="images/ajax-loader.gif" alt="loading..." />
  </center>
  <p>
     <center><%= base.GetLocaleResourceString("ltrMessage.Text") %> <a href="javascript:closeWin();" ><u><%= base.GetLocaleResourceString("hlContinue.Text") %></u></a>.</center>
  </p>
  <script type="text/javascript" language="javascript" >
      function closeWin() {
          try {
              window.opener.location.reload(true);
          }
          catch (e) {
              alert(e.message);
          }

          try {
              window.close();
          }
          catch (e) {
              alert(e.message);
          }
      }

      window.setTimeout(closeWin, 1000);
  </script>
</asp:Content>
