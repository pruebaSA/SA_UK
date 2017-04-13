<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NewsletterSignupTwo.ascx.cs" EnableViewState="false" Inherits="SalonAddict.Templates.NewsletterSignupTwo" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="EmailTextBox" Src="~/Modules/TextBoxes/EmailTextBox.ascx" %>

<div class="template-newslettersignup2" >
<asp:UpdatePanel ID="up" runat="server" >
   <ContentTemplate>
    <asp:MultiView ID="mv" runat="server" ActiveViewIndex="0" >
        <asp:View ID="v1" runat="server" >
           <b><%= base.GetLocalResourceObject("Heading") %></b>
           <div><SA:TextBox ID="txtName" runat="server" MaxLength="50" Width="190px" ValidationGroup="newsletter-two" meta:resourceKey="txtName" /></div>
           <div><SA:EmailTextBox ID="txtEmail" runat="server" Width="190px" ValidationGroup="newsletter-two" meta:resourceKey="txtEmail" /></div>
           <asp:Button ID="btnSubmit" runat="server" CssClass="submit" OnClick="btnSubmit_Click" ValidationGroup="newsletter-two" meta:resourceKey="btnSubmit" />
           <asp:UpdateProgress ID="uprog" runat="server" AssociatedUpdatePanelID="up" DisplayAfter="0"  >
              <ProgressTemplate>
                  <p><b><%= base.GetLocalResourceObject("ProgressMessage") %></b></p>
              </ProgressTemplate>
           </asp:UpdateProgress>
        </asp:View>
        <asp:View ID="v2" runat="server" >
            <b><%= base.GetLocalResourceObject("Heading") %></b>
            <p><%= base.GetLocalResourceObject("Message.Success") %></p>
        </asp:View>
    </asp:MultiView>
   </ContentTemplate>
   <Triggers>
      <asp:AsyncPostBackTrigger ControlID="btnSubmit" />
   </Triggers>
</asp:UpdatePanel>
</div>
<script type="text/javascript" language="javascript" >
    var newslettertwo_beginRequest = function(sender, e) {
        try
        {
           var element = e.get_postBackElement();
           if(element.id == '<%= btnSubmit.ClientID %>')
           {
               element.style.display = 'none';
           }
        }
        catch(e)
        {
        }
    };
    Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(newslettertwo_beginRequest); 
</script>