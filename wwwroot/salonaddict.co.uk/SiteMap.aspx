<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="SiteMap.aspx.cs" Inherits="SalonAddict.SiteMapPage" %>
<%@ MasterType VirtualPath="~/MasterPages/OneColumn.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server" >
   <style type="text/css" >
       .sitemap-links { }
       .sitemap-links ul { margin:0px; margin-bottom:15px; padding:0px; list-style:none; }
       .sitemap-links ul li { line-height:18px; }
       .sitemap-links ul li b { font-size:13px; }
       .sitemap-links ul li a { text-decoration: none; color: #76797c; }
       .sitemap-links ul li a:hover { text-decoration: underline; }
       .sitemap-topic { font-weight:bold; font-size:17px; color:#333; padding-bottom:6px; border-bottom: 1px solid #d5d5d5; }
       .sitemap-subtopic { font-weight:bold; font-size:12px; color:#000; padding-bottom:15px; }
       #sitemap-wrapper { position:relative; width:100%; height:2050px; }
       #sitemap-salons { position:absolute; top:0px; left:0px; width:470px; height:600px; }
       #sitemap-hairdressers { position:absolute; top:40px; left:0px; }
       #sitemap-hairdressers-continued { position:absolute; top:122px; left:0px; }
       #sitemap-beauty-salons { position:absolute; top:40px; left:220px; }
       #sitemap-nail-salons { position:absolute; top:205px; left:220px; }
       #sitemap-waxing-salons { position:absolute; top:310px; left:220px; }
       #sitemap-waxing-salons-continued { position:absolute; top:460px; left:220px; }
       #sitemap-salonaddict-info { position:absolute; top:0px; left:515px; }
       #sitemap-info-details { position:absolute; top:70px; left:0px; }
       #sitemap-support { position:absolute; top:0px; left:690px; }
       #sitemap-support-details { position:absolute; top:70px; left:0px; }
       #sitemap-salon-info { position:absolute; top:0px; left:800px; width:150px; }
       #sitemap-salon-info-details { position:absolute; top:70px; left:0px; }
   </style>
   
   <div id="sitemap-wrapper" >
      <div id="sitemap-salons" >
          <div class="sitemap-topic" ><%= base.GetLocalResourceObject("Topic.Salons") %></div>
          <div id="sitemap-hairdressers" >
             <div class="sitemap-subtopic" ><%= base.GetLocalResourceObject("Topic.Hairdressers") %></div>
             <div class="sitemap-links" >
                 <ul>
                     <li><a href="/search.aspx?service=hairdressing&city=london&country=united-kingdom">Hairdressers London</a></li>
                     <li><a href="/search.aspx?service=hairdressing&city=london&country=united-kingdom&order=most_booked">Best Hairdressers London</a></li>
                  </ul>
             </div>
          </div>
          <div id="sitemap-hairdressers-continued" >
              <div class="sitemap-links" >
                <ul>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=wandsworth&country=united-kingdom" >Hairdressers Balham</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=barking-and-dagenham&country=united-kingdom" >Hairdressers Barking</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=wandsworth&country=united-kingdom" >Hairdressers Battersea</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=bromley&country=united-kingdom">Hairdressers Beckenham</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=tower-hamlets&country=united-kingdom" >Hairdressers Bethnal Green</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=bexley&country=united-kingdom" >Hairdressers Bexleyheath</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=camden&country=united-kingdom" >Hairdressers Bloomsbury</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=brent&country=united-kingdom" >Hairdressers Brent</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=lambeth&country=united-kingdom" >Hairdressers Brixton</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=bromley&country=united-kingdom">Hairdressers Bromley</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=camden&country=united-kingdom" >Hairdressers Camden</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=lewisham&country=united-kingdom" >Hairdressers Catford</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=kensington-and-chelsea&country=united-kingdom" >Hairdressers Chelsea</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=waltham-forest&country=united-kingdom" >Hairdressers Chingford</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=city-of-london&country=united-kingdom">Hairdressers City of London</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=city-of-westminster&country=united-kingdom" >Hairdressers City of Westminster</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=lambeth&country=united-kingdom" >Hairdressers Clapham</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=barnet&country=united-kingdom">Hairdressers Colindale</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=camden&country=united-kingdom" >Hairdressers Covent Garden</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=brent&country=united-kingdom" >Hairdressers Cricklewood</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=croydon&country=united-kingdom" >Hairdressers Croydon</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=barking-and-dagenham&country=united-kingdom" >Hairdressers Dagenham</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=hackney&country=united-kingdom" >Hairdressing Dalston</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=ealing&country=united-kingdom" >Hairdressing Ealing</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=croydon&country=united-kingdom" >Hairdressers East Croydon</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=newham&country=united-kingdom" >Hairdressers Eastham</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=enfield&country=united-kingdom" >Hairdressing Edmonton</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=enfield&country=united-kingdom" >Hairdressing Enfield</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=havering&country=united-kingdom" >Hairdressing Essex</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=barnet&country=united-kingdom" >Hairdressers Finchley</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=newham&country=united-kingdom" >Hairdressers Forest Gate</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=hammersmith-and-fulham&country=united-kingdom" >Hairdressing Fulham</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=greenwich&country=united-kingdom" >Hairdressing Greenwich</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=hackney&country=united-kingdom" >Hairdressing Hackney</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=hammersmith-and-fulham&country=united-kingdom" >Hairdressing Hammersmith</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=ealing&country=united-kingdom" >Hairdressing Hanwell</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=camden&country=united-kingdom" >Hairdressers Hampstead</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=haringey&country=united-kingdom" >Hairdressing Harringay</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=harrow&country=united-kingdom" >Hairdressing Harrow</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=hillingdon&country=united-kingdom" >Hairdressing Hayes</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=barnet&country=united-kingdom">Hairdressers Hendon</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=haringey&country=united-kingdom" >Hairdressing Highgate</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=hillingdon&country=united-kingdom" >Hairdressing Hillingdon</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=camden&country=united-kingdom" >Hairdressers Holborn</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=hackney&country=united-kingdom" >Hairdressing Homerton</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=haringey&country=united-kingdom" >Hairdressing Hornsey</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=hounslow&country=united-kingdom" >Hairdressing Hounslow</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=redbridge&country=united-kingdom" >Hairdressers Ilford</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=islington&country=united-kingdom" >Hairdressing Islington</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=kensington-and-chelsea&country=united-kingdom" >Hairdressers Kensington</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=bromley&country=united-kingdom">Hairdressers Kent</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=brent&country=united-kingdom" >Hairdressers Kilburn</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=brent&country=united-kingdom" >Hairdressers Kingsbury</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=kingston-upon-thames&country=united-kingdom" >Hairdressers Kingston upon Thames</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=kensington-and-chelsea&country=united-kingdom" >Hairdressers Knightsbridge</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=lewisham&country=united-kingdom" >Hairdressers Lewisham</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=waltham-forest&country=united-kingdom" >Hairdressers Leyton</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=city-of-westminster&country=united-kingdom" >Hairdressers Marylebone</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=city-of-westminster&country=united-kingdom" >Hairdressers Mayfair</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=merton&country=united-kingdom" >Hairdressers Merton</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=brent&country=united-kingdom" >Hairdressers Middlesex</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=lewisham&country=united-kingdom" >Hairdressers New Cross</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=barnet&country=united-kingdom" >Hairdressers North Finchley</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=harrow&country=united-kingdom" >Hairdressing North Harrow</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=kensington-and-chelsea&country=united-kingdom" >Hairdressers North Kensington</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=lambeth&country=united-kingdom" >Hairdressers Norwood</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=kensington-and-chelsea&country=united-kingdom" >Hairdressers Notting Hill</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=bromley&country=united-kingdom">Hairdressers Orpington</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=southwark&country=united-kingdom" >Hairdressers Peckham</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=harrow&country=united-kingdom" >Hairdressing Pinner</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=greenwich&country=united-kingdom" >Hairdressing Plumstead</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=camden&country=united-kingdom" >Hairdressers Primrose Hill</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=richmond-upon-thames&country=united-kingdom" >Hairdressers Richmond upon Thames</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=southwark&country=united-kingdom" >Hairdressers Rotherhithe</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=hammersmith-and-fulham&country=united-kingdom" >Hairdressing Shepherd's Bush</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=kensington-and-chelsea&country=united-kingdom" >Hairdressers South Kensington</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=redbridge&country=united-kingdom" >Hairdressers South Woodford</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=wandsworth&country=united-kingdom" >Hairdressers Southfields</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=southwark&country=united-kingdom" >Hairdressers Southwark</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=city-of-westminster&country=united-kingdom" >Hairdressers St. John's Wood</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=newham&country=united-kingdom" >Hairdressers Stratford</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=sutton&country=united-kingdom" >Hairdressers Sutton</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=croydon&country=united-kingdom" >Hairdressers Thornton Heath</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=haringey&country=united-kingdom" >Hairdressing Tottenham</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=tower-hamlets&country=united-kingdom" >Hairdressers Tower Hamlets</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=city-of-westminster&country=united-kingdom" >Hairdressers Victoria</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=croydon&country=united-kingdom" >Hairdressers Waddon</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=wandsworth&country=united-kingdom" >Hairdressers Wandsworth</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=sutton&country=united-kingdom" >Hairdressers Wallington</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=waltham-forest&country=united-kingdom" >Hairdressers Waltham</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=waltham-forest&country=united-kingdom" >Hairdressers Walthamstow</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=southwark&country=united-kingdom" >Hairdressers Walworth</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=bexley&country=united-kingdom" >Hairdressers Welling</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=brent&country=united-kingdom" >Hairdressers Wembley</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=croydon&country=united-kingdom" >Hairdressers West Croydon</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=camden&country=united-kingdom" >Hairdressers West Hampstead</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=hammersmith-and-fulham&country=united-kingdom" >Hairdressing West Kensington</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=bromley&country=united-kingdom">Hairdressers West Wickham</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=city-of-westminster&country=united-kingdom" >Hairdressers Westminster</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=barnet&country=united-kingdom" >Hairdressers Whetstone</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=tower-hamlets&country=united-kingdom" >Hairdressers Whitechapel</a></li>
                    <li><a href="/search.aspx?service=hairdressing&city=london&area=merton&country=united-kingdom" >Hairdressers Wimbledon</a></li>
                </ul>
              </div>
          </div>
          <div id="sitemap-beauty-salons" >
              <div class="sitemap-subtopic" ><%= base.GetLocalResourceObject("Topic.BeautySalons") %></div>
              <div class="sitemap-links" >
                  <ul>
                     <li><a href="/search.aspx?service=tanning+spray&city=london&country=united-kingdom">Spray Tan London</a></li>
                     <li><a href="/search.aspx?service=massage&city=london&country=united-kingdom">Massage London</a></li>
                     <li><a href="/search.aspx?service=face&city=london&country=united-kingdom">Facial London</a></li>
                     <li><a href="/search.aspx?service=nails+manicure&city=london&country=united-kingdom">Manicure London</a></li>
                     <li><a href="/search.aspx?service=nails+pedicure&city=london&country=united-kingdom">Pedicure London</a></li>
                  </ul>
              </div>
          </div>
          <div id="sitemap-nail-salons" >
               <div class="sitemap-subtopic" ><%= base.GetLocalResourceObject("Topic.NailSalons") %></div>
               <div class="sitemap-links" >
                  <ul>
                      <li><a href="/search.aspx?service=nails+3-week-polish&city=london&country=united-kingdom">Shellac Nails London</a></li>
                      <li><a href="/search.aspx?service=nails+gelacrylic-nails&city=london&country=united-kingdom">Gel Nails London</a></li>
                  </ul>
               </div>
          </div>
          <div id="sitemap-waxing-salons" >
             <div class="sitemap-subtopic" ><%= base.GetLocalResourceObject("Topic.WaxingSalons") %></div>
             <div class="sitemap-links" >
                <ul>
                   <li><a href="/search.aspx?service=waxing&city=london&country=united-kingdom">Waxing Salons London</a></li>
                   <li><a href="/search.aspx?service=waxing+bikini&city=london&country=united-kingdom">Bikini Wax London</a></li>
                   <li><a href="/search.aspx?service=waxing+bikini&city=london&country=united-kingdom">Extended Bikini Wax London</a></li>
                   <li><a href="/search.aspx?service=waxing+bikini&city=london&country=united-kingdom">Brazilian Wax London</a></li>
                   <li><a href="/search.aspx?service=waxing+bikini&city=london&country=united-kingdom">Hollywood Wax London</a></li>
                </ul>
             </div>
          </div>
          <div id="sitemap-waxing-salons-continued" >
             <div class="sitemap-links" >
               <ul>
                  <li><a href="/search.aspx?service=nails&city=london&country=united-kingdom&order=most_reviewed">Best Nail Salons London</a></li>
                  <li><a href="/search.aspx?service=waxing&city=london&country=united-kingdom&order=most_reviewed">Best Wax Salons London</a></li>
               </ul>
             </div>
          </div>
      </div>
     <div id="sitemap-salonaddict-info" >
         <div class="sitemap-topic" ><%= base.GetLocalResourceObject("Topic.SalonAddictInformation") %></div>
         <div id="sitemap-info-details" >
             <div class="sitemap-links" >
               <ul>
                   <li><a href="/what-we-do.aspx" >About Us</a></li>
                   <li><a href="/promotions.aspx" >Recommendations</a></li>
                   <li><a href="/suggest-salon.aspx" >Suggest a Salon</a></li>
                   <li><a href="http://blog.salonaddict.co.uk" >Blog</a></li>
                   <li><a href="/savings.aspx" >Pricing & Discounts</a></li>
                   <% if (false)
                      { %>
                   <li><a href="/press.aspx" >Press</a></li>
                   <% } %>
                   <li><a href="http://www.salonaddict.ie" >Ireland</a></li>
               </ul>
             </div>
         </div>
     </div>
     <div id="sitemap-support" >
         <div class="sitemap-topic" ><%= base.GetLocalResourceObject("Topic.Suppport") %></div>
         <div id="sitemap-support-details" >
             <div class="sitemap-links" >
                <ul>
                   <li><a href="/contact-us.aspx" >Contact Us</a></li>
                   <li><a href="/terms-and-conditions.aspx" >Terms & Conditions</a></li>
                   <li><a href="/privacy-policy.aspx" >Privacy Policy</a></li>
                </ul>
             </div>
         </div>
     </div>
     <div id="sitemap-salon-info" >
         <div class="sitemap-topic" ><%= base.GetLocalResourceObject("Topic.SalonInformation") %></div>
         <div id="sitemap-salon-info-details" >
             <div class="sitemap-links" >
                  <ul>
                      <li><a href="/salonportal/" >Salon Portal Login</a></li>
                      <li><a href="/salon-signup.aspx" >Salon Sign Up</a></li>
                  </ul>
             </div>
         </div>
     </div>
   </div>
</asp:Content>
