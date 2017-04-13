<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Salons.aspx.cs" Inherits="IFRAME.Admin.SalonsPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
<asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
        <IFRM:Menu ID="cntrlMenu" runat="server" SelectedItem="Salons" />
        <div class="horizontal-line" ></div>
        <table style="margin:-20px;margin-bottom:0px;" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../App_Themes/" + base.Theme + "/images/overview-salons.png" %>' alt="salons" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
              <td style="vertical-align:middle" >
                  <asp:Button ID="btnAdd" runat="server" SkinID="SubmitButton" OnClick="btnAdd_Click" meta:resourceKey="btnAdd" />
              </td>
           </tr>
        </table>
        <table style="margin:-10px;margin-bottom:15px;" cellpadding="0" cellspacing="10" >
           <tr>
              <td><a href='<%= IFRMHelper.GetURL("salons.aspx", "s=A") %>' ><u><%= (this.SearchTerm == "A") ? "<strong>A</strong>" : "A"%></u></a></td>
              <td><a href='<%= IFRMHelper.GetURL("salons.aspx", "s=B") %>' ><u><%= (this.SearchTerm == "B") ? "<strong>B</strong>" : "B"%></u></a></td>
              <td><a href='<%= IFRMHelper.GetURL("salons.aspx", "s=C") %>' ><u><%= (this.SearchTerm == "C") ? "<strong>C</strong>" : "C"%></u></a></td>
              <td><a href='<%= IFRMHelper.GetURL("salons.aspx", "s=D") %>' ><u><%= (this.SearchTerm == "D") ? "<strong>D</strong>" : "D"%></u></a></td>
              <td><a href='<%= IFRMHelper.GetURL("salons.aspx", "s=E") %>' ><u><%= (this.SearchTerm == "E") ? "<strong>E</strong>" : "E"%></u></a></td>
              <td><a href='<%= IFRMHelper.GetURL("salons.aspx", "s=F") %>' ><u><%= (this.SearchTerm == "F") ? "<strong>F</strong>" : "F"%></u></a></td>
              <td><a href='<%= IFRMHelper.GetURL("salons.aspx", "s=G") %>' ><u><%= (this.SearchTerm == "G") ? "<strong>G</strong>" : "G"%></u></a></td>
              <td><a href='<%= IFRMHelper.GetURL("salons.aspx", "s=H") %>' ><u><%= (this.SearchTerm == "H") ? "<strong>H</strong>" : "H"%></u></a></td>
              <td><a href='<%= IFRMHelper.GetURL("salons.aspx", "s=I") %>' ><u><%= (this.SearchTerm == "I") ? "<strong>I</strong>" : "I"%></u></a></td>
              <td><a href='<%= IFRMHelper.GetURL("salons.aspx", "s=J") %>' ><u><%= (this.SearchTerm == "J") ? "<strong>J</strong>" : "J"%></u></a></td>
              <td><a href='<%= IFRMHelper.GetURL("salons.aspx", "s=K") %>' ><u><%= (this.SearchTerm == "K") ? "<strong>K</strong>" : "K"%></u></a></td>
              <td><a href='<%= IFRMHelper.GetURL("salons.aspx", "s=L") %>' ><u><%= (this.SearchTerm == "L") ? "<strong>L</strong>" : "L"%></u></a></td>
              <td><a href='<%= IFRMHelper.GetURL("salons.aspx", "s=M") %>' ><u><%= (this.SearchTerm == "M") ? "<strong>M</strong>" : "M"%></u></a></td>
              <td><a href='<%= IFRMHelper.GetURL("salons.aspx", "s=N") %>' ><u><%= (this.SearchTerm == "N") ? "<strong>N</strong>" : "N"%></u></a></td>
              <td><a href='<%= IFRMHelper.GetURL("salons.aspx", "s=O") %>' ><u><%= (this.SearchTerm == "O") ? "<strong>O</strong>" : "O"%></u></a></td>
              <td><a href='<%= IFRMHelper.GetURL("salons.aspx", "s=P") %>' ><u><%= (this.SearchTerm == "P") ? "<strong>P</strong>" : "P"%></u></a></td>
              <td><a href='<%= IFRMHelper.GetURL("salons.aspx", "s=Q") %>' ><u><%= (this.SearchTerm == "Q") ? "<strong>Q</strong>" : "Q"%></u></a></td>
              <td><a href='<%= IFRMHelper.GetURL("salons.aspx", "s=R") %>' ><u><%= (this.SearchTerm == "R") ? "<strong>R</strong>" : "R"%></u></a></td>
              <td><a href='<%= IFRMHelper.GetURL("salons.aspx", "s=S") %>' ><u><%= (this.SearchTerm == "S") ? "<strong>S</strong>" : "S"%></u></a></td>
              <td><a href='<%= IFRMHelper.GetURL("salons.aspx", "s=T") %>' ><u><%= (this.SearchTerm == "T") ? "<strong>T</strong>" : "T"%></u></a></td>
              <td><a href='<%= IFRMHelper.GetURL("salons.aspx", "s=U") %>' ><u><%= (this.SearchTerm == "U") ? "<strong>U</strong>" : "U"%></u></a></td>
              <td><a href='<%= IFRMHelper.GetURL("salons.aspx", "s=V") %>' ><u><%= (this.SearchTerm == "V") ? "<strong>V</strong>" : "V"%></u></a></td>
              <td><a href='<%= IFRMHelper.GetURL("salons.aspx", "s=W") %>' ><u><%= (this.SearchTerm == "W") ? "<strong>W</strong>" : "W"%></u></a></td>
              <td><a href='<%= IFRMHelper.GetURL("salons.aspx", "s=X") %>' ><u><%= (this.SearchTerm == "X") ? "<strong>X</strong>" : "X"%></u></a></td>
              <td><a href='<%= IFRMHelper.GetURL("salons.aspx", "s=Y") %>' ><u><%= (this.SearchTerm == "Y") ? "<strong>Y</strong>" : "Y"%></u></a></td>
              <td><a href='<%= IFRMHelper.GetURL("salons.aspx", "s=Z") %>' ><u><%= (this.SearchTerm == "Z") ? "<strong>Z</strong>" : "Z"%></u></a></td>
              <td><a href='<%= IFRMHelper.GetURL("salons.aspx", "s=0") %>' ><u><%= (this.SearchTerm == "0") ? "<strong>0</strong>" : "0"%></u></a></td>
              <td><a href='<%= IFRMHelper.GetURL("salons.aspx", "s=1") %>' ><u><%= (this.SearchTerm == "1") ? "<strong>1</strong>" : "1"%></u></a></td>
              <td><a href='<%= IFRMHelper.GetURL("salons.aspx", "s=2") %>' ><u><%= (this.SearchTerm == "2") ? "<strong>2</strong>" : "2"%></u></a></td>
              <td><a href='<%= IFRMHelper.GetURL("salons.aspx", "s=3") %>' ><u><%= (this.SearchTerm == "3") ? "<strong>3</strong>" : "3"%></u></a></td>
              <td><a href='<%= IFRMHelper.GetURL("salons.aspx", "s=4") %>' ><u><%= (this.SearchTerm == "4") ? "<strong>4</strong>" : "4"%></u></a></td>
              <td><a href='<%= IFRMHelper.GetURL("salons.aspx", "s=5") %>' ><u><%= (this.SearchTerm == "5") ? "<strong>5</strong>" : "5"%></u></a></td>
              <td><a href='<%= IFRMHelper.GetURL("salons.aspx", "s=6") %>' ><u><%= (this.SearchTerm == "6") ? "<strong>6</strong>" : "6"%></u></a></td>
              <td><a href='<%= IFRMHelper.GetURL("salons.aspx", "s=7") %>' ><u><%= (this.SearchTerm == "7") ? "<strong>7</strong>" : "7"%></u></a></td>
              <td><a href='<%= IFRMHelper.GetURL("salons.aspx", "s=8") %>' ><u><%= (this.SearchTerm == "8") ? "<strong>8</strong>" : "8"%></u></a></td>
              <td><a href='<%= IFRMHelper.GetURL("salons.aspx", "s=9") %>' ><u><%= (this.SearchTerm == "9") ? "<strong>9</strong>" : "9"%></u></a></td>
              <% if(false)
                 { %>
              <td><a href='<%= IFRMHelper.GetURL("salons.aspx", "s=") %>' ><u><%= (this.SearchTerm == "") ? "<strong>" + base.GetLocaleResourceString("hlAll.Text") + "</strong>" : base.GetLocaleResourceString("hlAll.Text") %></u></a></td>
              <% } %>
           </tr>
        </table>
        <asp:GridView 
            ID="gv" 
            runat="server" 
            AutoGenerateColumns="False" 
            DataKeyNames="SalonId"
            OnRowCreated="gv_RowCreated"
            OnRowEditing="gv_RowEditing" >
           <Columns>
              <asp:TemplateField>
                 <ItemTemplate>
                      <%# Eval("Name") %>
                 </ItemTemplate>
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                      <%# Eval("AddressLine1") + "<div>" + Eval("AddressLine3") + "<div>" %>
                 </ItemTemplate>
                 <ItemStyle Width="200px" />
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                      <%# Eval("PhoneNumber")%>
                 </ItemTemplate>
                 <ItemStyle Width="100px" />
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                      <asp:Panel ID="pnlDescriptionMini" runat="server" >
                          <center style="cursor:pointer;" ><u><%# String.IsNullOrEmpty(Eval("ShortDescription").ToString())? String.Empty : "<strong>...</strong>" %></u></center>
                      </asp:Panel>
                      <asp:Panel ID="pnlDescription" runat="server" >
                          <%# Eval("ShortDescription") %>
                      </asp:Panel>
                      <ajaxToolkit:BalloonPopupExtender 
                        ID="pnlPopout" 
                        runat="server" 
                        BalloonPopupControlID="pnlDescription" 
                        BalloonSize="Large" 
                        BalloonStyle="Rectangle"
                        CacheDynamicResults="false"
                        DisplayOnClick="true"
                        DisplayOnMouseOver="false"
                        DisplayOnFocus="false"
                        Position="TopLeft"
                        TargetControlID="pnlDescriptionMini"
                        OffsetX="0"
                        OffsetY="0"
                        UseShadow="true" />
                 </ItemTemplate>
                 <ItemStyle Width="75px" />
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                    <center>
                        <asp:ImageButton ID="ibEdit" runat="server" SkinID="GridEditImageButton" CommandName="Edit" meta:resourceKey="ibEdit" />
                    </center>
                 </ItemTemplate>
                 <ItemStyle Width="30px" />
              </asp:TemplateField>
           </Columns>
        </asp:GridView>
        <IFRM:IFRMPager ID="cntrlPager" runat="server" PageSize="10" CssClass="pager" Visible="false" meta:resourceKey="Pager" ></IFRM:IFRMPager>
    </asp:Panel>
</asp:Content>
