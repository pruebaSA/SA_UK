<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Customer-Review.aspx.cs" Inherits="SalonAddict.Customer_Review" %>
<%@ Register TagPrefix="SA" TagName="Rating" Src="~/Modules/Rating.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="Message" Src="~/Modules/Message.ascx" %>
<%@ MasterType VirtualPath="~/MasterPages/OneColumn.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <asp:MultiView ID="mv" runat="server" ActiveViewIndex="0" >
       <asp:View ID="v1" runat="server" >
        <h1><asp:Literal ID="lblHeader1" runat="server" EnableViewState="false" ></asp:Literal></h1>
        <p>
            <%= base.GetLocalResourceObject("lblInformation1.Text") %>
        </p>
        <table style="margin-top:10px;" class="details" cellpadding="0" cellspacing="0" width="950px" >
           <tr>
              <td style="width:400px" >
                <div class="grey-panel" style="height:400px" >
                    <h2><asp:Literal ID="lblSalonRating" runat="server" EnableViewState="false" ></asp:Literal></h2>
                    <table cellpadding="0" cellspacing="0" >
                       <tr>
                          <td class="title" >
                             <%= base.GetLocalResourceObject("lblSalonRating1.Text")%>
                          </td>
                          <td class="data-item" >
                             <SA:Rating ID="RatingB1" runat="server" />
                          </td>
                       </tr>
                       <tr>
                          <td class="title" >
                             <%= base.GetLocalResourceObject("lblSalonRating2.Text")%>
                          </td>
                          <td class="data-item" >
                             <SA:Rating ID="RatingB2" runat="server" />
                          </td>
                       </tr>
                       <tr>
                          <td class="title" >
                             <%= base.GetLocalResourceObject("lblSalonRating3.Text")%>
                          </td>
                          <td class="data-item" >
                             <SA:Rating ID="RatingB3" runat="server" />
                          </td>
                       </tr>
                       <tr>
                          <td class="title" >
                             <%= base.GetLocalResourceObject("lblSalonRating4.Text")%>
                          </td>
                          <td class="data-item" >
                             <SA:Rating ID="RatingB4" runat="server" />
                          </td>
                       </tr>
                       <tr>
                          <td class="title" >
                             <%= base.GetLocalResourceObject("lblSalonRating5.Text")%>
                          </td>
                          <td class="data-item" >
                             <SA:Rating ID="RatingB5" runat="server" />
                          </td>
                       </tr>
                       <tr>
                          <td class="title" >
                             <%= base.GetLocalResourceObject("lblSalonRating6.Text")%>
                          </td>
                          <td class="data-item" >
                             <SA:Rating ID="RatingB6" runat="server" />
                          </td>
                       </tr>
                       <tr>
                          <td class="title" >
                             <%= base.GetLocalResourceObject("lblSalonRating7.Text")%>
                          </td>
                          <td class="data-item" >
                             <SA:Rating ID="RatingB7" runat="server" />
                          </td>
                       </tr>
                    </table>
                </div>
              </td>
              <td style="padding-left:20px" >
                <div class="pink-panel" >
                    <h2><asp:Literal ID="lblSalonReview" runat="server" EnableViewState="false" ></asp:Literal></h2>
                    <table class="details" cellpadding="0" cellspacing="0" >
                       <tr>
                          <td class="data-item" style="padding-bottom:20px;"  >
                              <p>
                                 <%= base.GetLocalResourceObject("lblWritingGuidelines.Text") %>
                              </p>
                              <a style="font-size:10px;color:#999;text-decoration:underline;" href="javascript:void(0)" onclick="$('#ws_1').toggle()" >
                                 <%= base.GetLocalResourceObject("lblStandardsHeader.Text")%>
                              </a>
                              <div id="ws_1" style="display:none;">
                                <%= base.GetLocalResourceObject("lblWritingStandards.Text") %>
                              </div>
                          </td>
                       </tr>
                       <tr>
                          <td class="title" >
                             <%= base.GetLocalResourceObject("lblReviewB.Text") %>
                          </td>
                       </tr>
                       <tr>
                          <td class="data-item" >
                             <SA:TextBox 
                                ID="txtReviewB" 
                                runat="server" 
                                Width="450px" 
                                Height="100px" 
                                TextMode="MultiLine" 
                                IsRequired="false" 
                                MaxLength="500" 
                                OnKeyUp="SalonAddict.Utilities.CharacterCounter(this, '#counter_1')"  />
                             <span id="counter_1" >500</span>
                          </td>
                       </tr>
                       <tr>
                          <td class="data-item" >
                             <asp:CheckBox ID="cbAnonymousB" runat="server" />
                             <%= base.GetLocalResourceObject("cbAnonymousB.Text") %>
                          </td>
                       </tr>
                    </table>
                </div>
              </td>
           </tr>
        </table>
        <div style="padding:10px;text-align:right" >
           <asp:Button ID="btnSubmitB" runat="server" SkinID="BlackButtonLarge" OnClick="btnSubmitB_Click" meta:resourceKey="btnSubmitB" />
        </div>
       </asp:View>
       <asp:View ID="v2" runat="server" >
        <h1><asp:Literal ID="lblHeader2" runat="server" EnableViewState="false" ></asp:Literal></h1>
        <p>
            <%= base.GetLocalResourceObject("lblInformation2.Text") %>
        </p>
        <p>
            <asp:LinkButton ID="lbCancelS" runat="server" OnClick="lbCancelS_Click" CausesValidation="false" Font-Underline="true" Font-Bold="true" ForeColor="Black" ></asp:LinkButton>
        </p>
        <table style="margin-top:10px;" class="details" cellpadding="0" cellspacing="0" width="950px" >
           <tr>
              <td style="width:400px" >
                <div class="grey-panel" style="height:400px;" >
                    <h2><asp:Literal ID="lblStaffRating" runat="server" EnableViewState="false" ></asp:Literal></h2>
                    <table cellpadding="0" cellspacing="0" >
                       <tr>
                          <td class="title" >
                             <%= base.GetLocalResourceObject("lblStaffRating1.Text") %>
                          </td>
                          <td class="data-item" >
                             <SA:Rating ID="RatingS1" runat="server" />
                          </td>
                       </tr>
                       <tr>
                          <td class="title" >
                             <%= base.GetLocalResourceObject("lblStaffRating2.Text") %>
                          </td>
                          <td class="data-item" >
                             <SA:Rating ID="RatingS2" runat="server" />
                          </td>
                       </tr>
                       <tr>
                          <td class="title" >
                             <%= base.GetLocalResourceObject("lblStaffRating3.Text") %>
                          </td>
                          <td class="data-item" >
                             <SA:Rating ID="RatingS3" runat="server" />
                          </td>
                       </tr>
                       <tr>
                          <td class="title" >
                             <%= base.GetLocalResourceObject("lblStaffRating4.Text") %>
                          </td>
                          <td class="data-item" >
                             <SA:Rating ID="RatingS4" runat="server" />
                          </td>
                       </tr>
                       <tr>
                          <td class="title" >
                             <%= base.GetLocalResourceObject("lblStaffRating5.Text") %>
                          </td>
                          <td class="data-item" >
                             <SA:Rating ID="RatingS5" runat="server" />
                          </td>
                       </tr>
                    </table>
                </div>
              </td>
              <td style="padding-left:20px" >
                <div class="pink-panel" >
                    <h2><asp:Literal ID="lblStaffReview" runat="server" EnableViewState="false" ></asp:Literal></h2>
                    <table class="details" cellpadding="0" cellspacing="0" >
                       <tr>
                          <td class="data-item" style="padding-bottom:20px;"  >
                              <p>
                                 <%= base.GetLocalResourceObject("lblWritingGuidelines.Text") %>
                              </p>
                              <a style="font-size:10px;color:#999;text-decoration:underline;" href="javascript:void(0)" onclick="$('#ws_2').toggle()" >
                                 <%= base.GetLocalResourceObject("lblStandardsHeader.Text")%>
                              </a>
                              <div id="ws_2" style="display:none;">
                                <%= base.GetLocalResourceObject("lblWritingStandards.Text") %>
                              </div>
                          </td>
                       </tr>
                       <tr>
                          <td class="title" >
                             <%= base.GetLocalResourceObject("lblReviewS.Text") %>
                          </td>
                       </tr>
                       <tr>
                          <td class="data-item" colspan="2" >
                             <SA:TextBox 
                                ID="txtReviewS" 
                                runat="server" 
                                Width="450px" 
                                Height="100px" 
                                TextMode="MultiLine" 
                                IsRequired="false" 
                                MaxLength="500" 
                                OnKeyUp="SalonAddict.Utilities.CharacterCounter(this, '#counter_2')"  />
                             <span id="#counter_2" >500</span>
                          </td>
                       </tr>
                       <tr>
                          <td class="data-item" >
                             <asp:CheckBox ID="cbAnonynousS" runat="server" />
                             <%= base.GetLocalResourceObject("cbAnonymousS.Text") %>
                          </td>
                       </tr>
                    </table>
                </div>
              </td>
           </tr>
        </table>
        <div style="padding:10px;text-align:right" >
           <asp:Button ID="btnSubmitS" runat="server" SkinID="BlackButtonLarge" OnClick="btnSubmitS_Click" meta:resourceKey="btnSubmitS" />
        </div>
       </asp:View>
       <asp:View ID="v3" runat="server" >
          <h1><%= base.GetLocalResourceObject("lblHeader3.Text") %></h1>
          <SA:Message ID="Message" runat="server" IsError="false" meta:resourceKey="Message" />
       </asp:View>
    </asp:MultiView>
</asp:Content>
