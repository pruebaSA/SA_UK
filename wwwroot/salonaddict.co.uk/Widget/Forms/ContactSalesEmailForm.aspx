<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ContactSalesEmailForm.aspx.cs" Inherits="SalonAddict.Widget.Forms.ContactSalesEmailFormPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>SalonAddict</title>
    <style type="text/css" >
        .form-wrapper { padding:20px; }
        input[type="text"], textarea { width:180px;text-align:left;padding:3px 5px 3px 5px;color: #42382c;font-size:14px;font-family:Arial, Hevetica, Sans-Serif;border:solid 1px #cecece;}
    </style>
</head>
<body>
    <div class="form-wrapper" >
    <form id="form1" runat="server">
    <asp:ScriptManager ID="sm" runat="server" ></asp:ScriptManager>
    <asp:MultiView ID="mv" runat="server" ActiveViewIndex="0" >
        <asp:View ID="v0" runat="server" >
            <table style="margin-left:-6px;" class="details" cellpadding="0" cellspacing="0" >
               <tr>
                  <td class="title" >Your Name:</td>
                  <td class="data-item" >
                    <asp:TextBox ID="txtFullName" runat="server" MaxLength="100" ></asp:TextBox>
                    <asp:RequiredFieldValidator ID="valFullName" runat="server" ControlToValidate="txtFullName" Display="None" ErrorMessage="Name is a required field." ></asp:RequiredFieldValidator>
                    <ajaxToolkit:ValidatorCalloutExtender ID="valFullnameEx" runat="Server" TargetControlID="valFullName" EnableViewState="false" />
                    <asp:RegularExpressionValidator ID="valFullNameRegex1" runat="server" ControlToValidate="txtFullName" Display="None" ValidationExpression="[^0-9]+" ErrorMessage="Numeric characters are not allowed."></asp:RegularExpressionValidator>
                    <ajaxToolkit:ValidatorCalloutExtender ID="valFullNameRegex1Ex" runat="server" TargetControlID="valFullNameRegex1" EnableViewState="false" />
                    <asp:RegularExpressionValidator ID="valFullNameRegex2" runat="server" ControlToValidate="txtFullName" Display="None" ValidationExpression=".{2,100}" ErrorMessage="Name is too short."></asp:RegularExpressionValidator>
                    <ajaxToolkit:ValidatorCalloutExtender ID="valFullNameRegex2Ex" runat="server" TargetControlID="valFullNameRegex2" EnableViewState="false" />
                  </td>
               </tr>
               <tr>
                  <td class="title" >Salon Name:</td>
                  <td class="data-item" >
                    <asp:TextBox ID="txtSalonName" runat="server" MaxLength="100" ></asp:TextBox>
                    <asp:RequiredFieldValidator ID="valSalonName" runat="server" ControlToValidate="txtSalonName" Display="None" ErrorMessage="Salon is a required field." ></asp:RequiredFieldValidator>
                    <ajaxToolkit:ValidatorCalloutExtender ID="valSalonNameEx" runat="Server" TargetControlID="valSalonName" EnableViewState="false" />
                    <asp:RegularExpressionValidator ID="valSalonNameRegex" runat="server" ControlToValidate="txtSalonName" Display="None" ValidationExpression=".{3,100}" ErrorMessage="Name is too short."></asp:RegularExpressionValidator>
                    <ajaxToolkit:ValidatorCalloutExtender ID="valSalonNameRegexEx" runat="server" TargetControlID="valSalonNameRegex" EnableViewState="false" />
                  </td>
               </tr>
               <tr>
                  <td class="title" >Email Address:</td>
                  <td class="data-item" >
                      <asp:TextBox ID="txtEmail" runat="server" SkinID="TextBox" MaxLength="50" ></asp:TextBox>
                      <asp:RequiredFieldValidator ID="valEmail" runat="server" ControlToValidate="txtEmail" Display="None" ErrorMessage="Email is a required field." ></asp:RequiredFieldValidator>
                      <ajaxToolkit:ValidatorCalloutExtender ID="valEmailEx" runat="server" TargetControlID="valEmail" EnableViewState="false" />
                      <asp:RegularExpressionValidator ID="valEmailRegEx" runat="server" ControlToValidate="txtEmail" Display="None" ValidationExpression=".+@.+\..+" ErrorMessage="Email is invalid." ></asp:RegularExpressionValidator>
                      <ajaxToolkit:ValidatorCalloutExtender ID="valEmailRegExEx" runat="server" TargetControlID="valEmailRegEx" EnableViewState="false" />
                  </td>
               </tr>
               <tr>
                  <td class="title" ></td>
                  <td class="data-item" >
                      <asp:TextBox ID="txtMessage" runat="server" TextMode="MultiLine" Text="Please type your message here." Width="400px" Height="80px" onfocus="message_focus(this)" onblur="message_blur(this)" ></asp:TextBox>
                      <script type="text/javascript" language="javascript" >
                          function message_focus(obj) {
                              if (obj.value == "Please type your message here.") {
                                  obj.value = "";
                              }
                          }

                          function message_blur(obj) {
                              if (obj.value == "") {
                                  obj.value = "Please type your message here.";
                              }
                          }
                      </script>
                  </td>
               </tr>
               <tr>
                  <td class="title" ></td>
                  <td class="data-item" >
                     <asp:Button ID="btnSubmit" runat="server" SkinID="BlackButtonSmall" Text="Submit" OnClick="btnSubmit_Click" />
                  </td>
               </tr>
            </table>
       </asp:View>
       <asp:View ID="v2" runat="server" >
          <p style="font-size:14px;line-height:20px;" >
             We have received your online inquiry and forwarded your information to one of our representatives who will be in touch with you shortly.
          </p>
       </asp:View>
    </asp:MultiView>
    </form>
  </div>
</body>
</html>