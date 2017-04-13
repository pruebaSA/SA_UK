<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Footer.ascx.cs" EnableViewState="false" Inherits="SalonAddict.Modules.Footer" %>
<%@ Register TagName="NewsLetterSignup" TagPrefix="SA" Src="~/Templates/NewsletterSignupTwo.ascx" %>

<div class="wrapper-footer" > 
  <div class="footer-center" >
       <div class="column-1" >
          <ul class="quick-links">
             <li><a href='<%= Page.ResolveUrl("~/") %>' ><%= base.GetLocalResourceObject("hlHome.Text") %></a></li>
             <% if (false)
                { %>
             <li><a href='<%= Page.ResolveUrl("~/press.aspx") %>' ><%= base.GetLocalResourceObject("hlPress.Text")%></a></li>
             <% } %>
             <li><a href='<%= Page.ResolveUrl("~/savings.aspx") %>' ><%= base.GetLocalResourceObject("hlSavings.Text") %></a></li>
             <li><a href='<%= Page.ResolveUrl("~/privacy-policy.aspx") %>' ><%= base.GetLocalResourceObject("hlPrivacyPolicy.Text") %></a></li>
             <li><a href='<%= Page.ResolveUrl("~/terms-and-conditions.aspx") %>' ><%= base.GetLocalResourceObject("hlTerms.Text") %></a></li>
             <li><a href='<%= Page.ResolveUrl("~/careers.aspx") %>' ><%= base.GetLocalResourceObject("hlCareers.Text") %></a></li>
             <li><a href='<%= Page.ResolveUrl("~/suggest-salon.aspx") %>' ><%= base.GetLocalResourceObject("hlSuggestSalon.Text") %></a></li>
             <li><a href='<%= Page.ResolveUrl("~/salon-signup.aspx") %>' ><%= base.GetLocalResourceObject("hlSalonSignUp.Text") %></a></li>
             <li><a href='<%= Page.ResolveUrl("~/widget") %>' ><%= base.GetLocalResourceObject("hlWidget.Text") %></a></li>
             <li><a href='<%= Page.ResolveUrl("~/contact-us.aspx") %>' ><%= base.GetLocalResourceObject("hlContactUs.Text") %></a></li>
             <li><a href='<%= Page.ResolveUrl("~/sitemap.aspx") %>' ><%= base.GetLocalResourceObject("hlSiteMap.Text") %></a></li>
          </ul>
       </div>
       <div class="column-2" >
          <div class="additional-links" >
             <b><%= base.GetLocalResourceObject("AdditionalLinks_Heading") %></b>
             <ul>
                <li>
                   <a href="http://www.facebook.com/salonaddict.co.uk" target="_blank" >
                      <img src='<%= Page.ResolveUrl("~/images/fb_ico.png") %>' alt="http://www.facebook.com/salonaddict.co.uk" />
                   </a>
                </li>
                <li>
                   <a href="http://twitter.com/salonaddictuk" target="_blank" >
                      <img src='<%= Page.ResolveUrl("~/images/twitter_ico.png")  %>' alt="http://twitter.com/salonaddictuk" />
                   </a>
                </li>
                <li>
                   <a href="https://plus.google.com/u/0/113478684116892226350/" target="_blank" >
                      <img src='<%= Page.ResolveUrl("~/images/gplus_ico.png")  %>' alt="https://plus.google.com/u/0/113478684116892226350/" />
                   </a>
                </li>
                <li>
                   <a href='<%= Page.ResolveUrl("~/rss") %>' >
                      <img src='<%= Page.ResolveUrl("~/images/rss_ico.png")  %>' alt="http://www.salonaddict.co.uk/rss" />
                   </a>
                </li>
             </ul>
          </div>
       </div>
       <div class="column-3" >
          <div class="charity" >
             <a href="http://www.kiva.org/lender/salonaddictuk" target="_blank" >
                <img src='<%= Page.ResolveUrl("~/images/kiva_ico.png") %>' alt="http://www.kiva.org/lender/salonaddictuk" />
             </a>
             <p>
                <b><%= base.GetLocalResourceObject("Charity_Heading")%></b>
             </p>
             <p>
                <%= base.GetLocalResourceObject("Charity_Info")%>
                (<a href="http://www.kiva.org/lender/salonaddictuk" target="_blank" ><%= base.GetLocalResourceObject("hlMoreInfo.Text")%></a>)
             </p>
          </div>
       </div>
       <div class="column-4" >
           <SA:NewsLetterSignup ID="NewsletterSignup" runat="server" />
           <div class="share-this" >
              <script type="text/javascript" src="http://w.sharethis.com/button/buttons.js"></script>
              <span class='st_facebook_hcount' displayText='Facebook'></span>
              <span class='st_twitter_hcount' displayText='Tweet'></span>
              <script type="text/javascript">stLight.options({ publisher: "21ef45de-d4b2-465b-8161-cff77ab31870" }); </script>
           </div>
       </div>
       <div class="column-5" >
          <div class="info" >
              <b><%= base.GetLocalResourceObject("Info_Heading") %></b>
              <p>
                 <%= base.GetLocalResourceObject("Information_Info") %>
              </p>
              <div><a href="mailto:info@salonaddict.co.uk" >info@salonaddict.co.uk</a></div>
              <div>
                <a id="slogin" href='<%= Page.ResolveUrl("~/salonportal") %>' >
                   <%= base.GetLocalResourceObject("hlSalonLogin.Text") %>
                </a>
              </div>
              <span class="smallprint" >
                 <%= String.Format(base.GetLocalResourceObject("lblCopyright.Text").ToString(), DateTime.Now.Year) %>
              </span>
           </div>
       </div>
   </div>
</div>