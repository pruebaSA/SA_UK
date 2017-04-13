<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NewsletterSignupOne.ascx.cs" EnableViewState="false" Inherits="SalonAddict.Templates.NewsletterSignupOne" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="EmailTextBox" Src="~/Modules/TextBoxes/EmailTextBox.ascx" %>

<div class="template-newslettersignup1" >
<asp:UpdatePanel ID="up" runat="server"  >
    <ContentTemplate>
    <asp:MultiView ID="mv" runat="server" ActiveViewIndex="0" >
       <asp:View ID="v1" runat="server" >
           <ul>
              <li><b><%= base.GetLocalResourceObject("Heading") %></b></li>
              <li>
                 <SA:TextBox ID="txtName" runat="server" MaxLength="50" Width="120px" ValidationGroup="newsletter-one" meta:resourceKey="txtName" />
              </li>
              <li>
                 <SA:EmailTextBox ID="txtEmail" runat="server" Width="120px" ValidationGroup="newsletter-one" meta:resourceKey="txtEmail" />
              </li>
              <li>
                 <asp:Button ID="btnSubmit" runat="server" CssClass="submit" OnClick="btnSubmit_Click" ValidationGroup="newsletter-one" meta:resourceKey="btnSubmit" />
              </li>
              <li>
                 <asp:UpdateProgress ID="uprog" runat="server" AssociatedUpdatePanelID="up" DisplayAfter="0"  >
                    <ProgressTemplate>
                        <b><%= base.GetLocalResourceObject("ProgressMessage") %></b>
                    </ProgressTemplate>
                 </asp:UpdateProgress>
              </li>
           </ul>
       </asp:View>
       <asp:View ID="v2" runat="server" >
          <ul>
             <li><b><%= base.GetLocalResourceObject("Message.Success") %></b></li>
          </ul>
       </asp:View>
    </asp:MultiView>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="btnSubmit" />
    </Triggers>
</asp:UpdatePanel>
</div>
<script type="text/javascript" language="javascript" >
    var newsletterone_beginRequest = function(sender, e) {
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
    Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(newsletterone_beginRequest); 
</script>