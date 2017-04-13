<%@ Page Language="C#" MasterPageFile="~/MasterPages/Root.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SalonAddict.Widget.DefaultPage" %>
<%@ MasterType VirtualPath="~/MasterPages/Root.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph" runat="server">
<link href="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/themes/base/jquery-ui.css" rel="stylesheet" type="text/css"/>
<script src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/jquery-ui.min.js"></script>

<style type="text/css" >
.master-root-wrapper { margin-top:5px;padding-top:15px;background:url('images/widget-bg.png') repeat-x top;}
.widget-banner { padding-top:25px;padding-left:20px;width:920px;height:55px;background:url('images/widget-banner.png') no-repeat;line-height:30px;font-size:30px;color:#fff;font-family:Arial;font-weight:bold; }
.widget-page { margin-bottom:8px;padding-top:15px;background:url('images/widget-bg-light.png') no-repeat bottom; }
.widget-slides { position:relative;width:615px;height:412px;}
#widget-s1 { position:absolute;top:5px;left:20px;display:block; }
#widget-s1 a.trial { display:block;position:absolute;bottom:65px;left:45px;width:200px;height:35px;}
#widget-s2 { position:absolute;top:5px;left:20px;display:none; }
#widget-s3 { position:absolute;top:5px;left:20px;display:none; }
#widget-s4 { position:absolute;top:5px;left:20px;display:none; }
.widget-footer { position:relative;height:188px;background:url('images/widget-footer.png') no-repeat right; }
.master-root { margin-bottom:0px; padding-bottom:0px; }
.widget-resources { }
.widget-resources a { color:#333;text-decoration:none; }
.widget-resources a:hover { text-decoration:underline; }
.widget-contact { position:absolute;top:0px;left:355px;}
.widget-contact a { color:#333;text-decoration:none; }
.widget-contact a:hover { text-decoration:underline; }
</style>
<div class="widget-banner" >
  <%= base.GetLocalResourceObject("ltrBanner.Text") %>
</div>
<div class="widget-page" >
    <table cellpadding="0" cellspacing="0" >
      <tr>
         <td style="padding-top:5px" >
             <img src="images/widget-header.png" alt='<%= base.GetLocalResourceObject("ltrHeader.AlternativeText") %>' />
             <table style="font-family:Arial;font-size:13px;line-height:16px;" cellpadding="6" cellspacing="0" width="325px" >
                <tr>
                   <td style="vertical-align:top;"><img src="images/widget-check.png" alt="" /></td>
                   <td><%= base.GetLocalResourceObject("bullet1") %></td>
                </tr>
                <tr>
                   <td style="vertical-align:top;"><img src="images/widget-check.png" alt="" /></td>
                   <td><%= base.GetLocalResourceObject("bullet2") %></td>
                </tr>
                <tr>
                   <td style="vertical-align:top;"><img src="images/widget-check.png" alt="" /></td>
                   <td><%= base.GetLocalResourceObject("bullet3") %></td>
                </tr>
                <tr>
                   <td style="vertical-align:top;"><img src="images/widget-check.png" alt="" /></td>
                   <td><%= base.GetLocalResourceObject("bullet4") %></td>
                </tr>
             </table>
         </td>
         <td>
           <div class="widget-slides" >
               <div id="widget-s1" >
                   <img src="images/widget-slide1.png" alt="" />
                   <a class="trial" href="javascript:void(0)" onclick="openForm('trial_form');" ></a>
               </div>
               <div id="widget-s2" >
                   <img src="images/widget-slide2.png" alt="" />
               </div>
               <div id="widget-s3" >
                   <img src="images/widget-slide3.png" alt="" />
               </div>
               <div id="widget-s4" >
                   <img src="images/widget-slide4.png" alt="" />
               </div>
           </div>
         </td>
      </tr>
    </table>
</div>
<div class="widget-footer" >
    <div class="widget-resources" >
       <img src="images/widget-resources.png" alt="" />
       <table cellpadding="0" cellspacing="0" >
          <tr>
             <td><img src="images/widget-bullet.png" alt="" /></td>
             <td>
                <a href="javascript:void(0)" onclick="openForm('resource_software_form');" ><%= base.GetLocalResourceObject("hlResourceSoftware.Text") %></a>
             </td>
          </tr>
          <tr><td colspan="2" ><div style="height:5px;" ></div></td></tr>
          <tr>
             <td><img src="images/widget-bullet.png" alt="" /></td>
             <td>
                <a href="javascript:void(0)" onclick="openForm('resource_renew_form');" ><%= base.GetLocalResourceObject("hlResourceRenew.Text") %></a>
             </td>
          </tr>
          <tr><td colspan="2" ><div style="height:5px;" ></div></td></tr>
          <tr>
             <td><img src="images/widget-bullet.png" alt="" /></td>
             <td>
                <a href="javascript:void(0)" onclick="openForm('resource_terms_form');" ><%= base.GetLocalResourceObject("hlResourceTerms.Text") %></a>
             </td>
          </tr>
       </table>
    </div>
    <div class="widget-contact" >
        <img src="images/widget-contact.png" alt="" />
       <div>
           <table cellpadding="0" cellspacing="0" >
              <tr>
                 <td colspan="2" ><b style="font-size:13px" ><%= base.GetLocalResourceObject("ltrContactSalesHeader.Text") %></b></td>
              </tr>
              <tr><td colspan="2" ><div style="height:10px;" ></div></td></tr>
              <tr>
                <td><img src="images/widget-phone-ico.png" alt="" /> &nbsp;</td>
                <td>
                    +44 (0) 20 8123 4322
                </td>
              </tr>
              <tr><td colspan="2" ><div style="height:8px;" ></div></td></tr>
              <tr>
                <td><img src="images/widget-email-ico.png" alt="" /></td>
                <td>
                    <a href="javascript:void(0)" onclick="openForm('contact_sales_form');" ><%= base.GetLocalResourceObject("hlContactSalesEmail.Text") %></a>
                </td>
              </tr>
              <tr><td colspan="2" ><div style="height:8px;" ></div></td></tr>
              <tr>
                <td><img src="images/widget-call-ico.png" alt="" /></td>
                <td>
                    <a href="javascript:void(0)" onclick="openForm('contact_call_form');" ><%= base.GetLocalResourceObject("hlContactSalesCall.Text") %></a>
                </td>
              </tr>
           </table>
       </div>
    </div>
</div>
<script type="text/javascript" language="javascript" >
    var _numberOfSlides = 4;

    function getSlideIndex(x) {
        x = x % 5;
        if (x == 0) {
            x = 1;
        }
        return x;
    }

    function showSlide(x) {
        var current = getSlideIndex(x);
        var next = getSlideIndex(current + 1);

        var s1 = "#widget-s" + current;
        var s2 = "#widget-s" + next;

        $(s1).css({ "z-index": _numberOfSlides }); 
        $(s2).css({ "z-index": _numberOfSlides - 1 });
        
        $(s2).show();
        $(s1).fadeOut(3000, function() {
            setTimeout(function() { showSlide(next) }, 3000)
        });
    }
</script>

<script type="text/javascript" language="javascript" >
    function openForm(form) {
        $(".ui-dialog-content").dialog("close");
        
        if (form == "trial_form") {
            $('#trial_form').dialog({ height: 320, width: 600 });
        }
        else if (form == "resource_software_form") {
            $('#resource_software_form').dialog({ height: 350, width: 600 });
        }
        else if (form == "resource_renew_form") {
            $('#resource_renew_form').dialog({ height: 350, width: 520 });
        }
        else if (form == "resource_terms_form") {
            $('#resource_terms_form').dialog({ height: 450, width: 600 });
        }
        else if (form == "contact_sales_form") {
            $('#contact_sales_form').dialog({ height: 350, width: 600 });
        }
        else if (form == "contact_call_form") {
            $('#contact_call_form').dialog({ height: 290, width: 600 });
        }
    }
</script>
<div id="trial_form" style="display:none;" title='<%= base.GetLocalResourceObject("hlRequestTrial.Text") %>'>
   <iframe src="Forms/RequestTrialForm.aspx" height="260px" width="580px" frameborder="0" scrolling="auto" ></iframe>
</div>
<div id="resource_software_form" style="display:none;" title='<%= base.GetLocalResourceObject("hlResourceSoftware.Text") %>' >
   <iframe src="Forms/ResourceSoftwareForm.aspx" height="300px" width="580px" frameborder="0" scrolling="auto" ></iframe>
</div>
<div id="resource_renew_form" style="display:none;" title='<%= base.GetLocalResourceObject("hlResourceRenew.Text") %>' >
   <iframe src="Forms/ResourceRenewForm.aspx" height="300px" width="500px" frameborder="0" scrolling="auto" ></iframe>
</div>
<div id="resource_terms_form" style="display:none;" title='<%= base.GetLocalResourceObject("hlResourceTerms.Text") %>' >
   <iframe src="Forms/ResourceTermsForm.aspx" height="400px" width="580px" frameborder="0" scrolling="auto" ></iframe>
</div>
<div id="contact_sales_form" style="display:none;" title='<%= base.GetLocalResourceObject("hlContactSalesEmail.Text") %>' >
   <iframe src="Forms/ContactSalesEmailForm.aspx" height="300px" width="580px" frameborder="0" scrolling="auto" ></iframe>
</div>
<div id="contact_call_form" style="display:none;" title='<%= base.GetLocalResourceObject("hlContactSalesCall.Text") %>' >
   <iframe src="Forms/ContactSalesCallForm.aspx" height="240px" width="580px" frameborder="0" scrolling="auto" ></iframe>
</div>

<script type="text/javascript" language="javascript" >
    $(document).ready(function() {
        setTimeout(function() { showSlide(1) }, 3000);
    });
</script>
</asp:Content>
