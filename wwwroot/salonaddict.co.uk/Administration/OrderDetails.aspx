<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="OrderDetails.aspx.cs" Inherits="SalonAddict.Administration.OrderDetails" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>

<%@ Import Namespace="SalonAddict.Common" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div class="section-header">
    <div class="title">
        <img src="images/ico-sales.png" alt="" />
        View Order details <a href="Orders.aspx" title="Back to orders list">(back to orders list)</a>
    </div>
    <div class="options">
    <% if (Roles.IsUserInRole("OWNER") || Roles.IsUserInRole("VP_SALES"))
       { %>
        <asp:Button ID="DeleteButton" runat="server" Text="Delete" OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="Delete tax provider" OnClientClick="return confirm('Are you sure?')" />
    <% } %>
    </div>
</div>
<ajaxToolkit:TabContainer ID="OrderTabs" runat="server" >
  <ajaxToolkit:TabPanel ID="pnlOrderInfo" runat="server" HeaderText="Order Info" >
    <ContentTemplate>
       <table class="details" cellpadding="0" cellspacing="0" >
          <tr>
             <td class="title" >
                <SA:ToolTipLabel 
                    ID="lblOrderStatus" 
                    runat="server" 
                    Text="Order Status:"
                    ToolTip="The status of this order." 
                    ToolTipImage="~/Administration/images/ico-help.gif" />
             </td>
             <td class="data-item" >
                <b><asp:Literal ID="ltrOrderStatus" runat="server" ></asp:Literal></b>
             </td>
          </tr>
          <tr>
             <td class="title" >
                <SA:ToolTipLabel 
                    ID="lblReferenceNo" 
                    runat="server" 
                    Text="Reference No:"
                    ToolTip="Unique order number." 
                    ToolTipImage="~/Administration/images/ico-help.gif" />
             </td>
             <td class="data-item" >
                <asp:Literal ID="ltrReferenceNo" runat="server" ></asp:Literal>
             </td>
          </tr>
          <tr>
             <td class="title" >
                <SA:ToolTipLabel 
                    ID="lblBusiness" 
                    runat="server" 
                    Text="Business:"
                    ToolTip="The business owner of this order." 
                    ToolTipImage="~/Administration/images/ico-help.gif" />
             </td>
             <td class="data-item" >
                <asp:Literal ID="ltrBusiness" runat="server" ></asp:Literal>
             </td>
          </tr>
             <tr>
                <td class="title" >
                  <SA:ToolTipLabel 
                       ID="lblFirstName" 
                       runat="server" 
                       Text="First name:"
                       ToolTip="The customer's first name." 
                       ToolTipImage="~/Administration/images/ico-help.gif" />
                </td>
                <td class="data-item" >
                    <asp:Literal ID="ltrFirstName" runat="server" ></asp:Literal>
                </td>
             </tr>
             <tr>
                <td class="title" >
                  <SA:ToolTipLabel 
                       ID="lblLastName" 
                       runat="server" 
                       Text="Last name:"
                       ToolTip="The customer's last name." 
                       ToolTipImage="~/Administration/images/ico-help.gif" />
                </td>
                <td class="data-item" >
                    <asp:Literal ID="ltrLastName" runat="server" ></asp:Literal>
                </td>
             </tr>
             <tr>
                <td class="title" >
                  <SA:ToolTipLabel 
                       ID="lblPhone" 
                       runat="server" 
                       Text="Phone:"
                       ToolTip="The customer's phone number." 
                       ToolTipImage="~/Administration/images/ico-help.gif" />
                </td>
                <td class="data-item" >
                    <asp:Literal ID="ltrPhone" runat="server" ></asp:Literal>
                </td>
             </tr>
             <tr>
                <td class="title" >
                  <SA:ToolTipLabel 
                       ID="lblEmail" 
                       runat="server" 
                       Text="Email:"
                       ToolTip="The customer's email." 
                       ToolTipImage="~/Administration/images/ico-help.gif" />
                </td>
                <td class="data-item" >
                    <asp:Literal ID="ltrEmail" runat="server" ></asp:Literal>
                </td>
             </tr>
          <tr>
             <td class="title" >
                <SA:ToolTipLabel 
                    ID="lblPaymentProvider" 
                    runat="server" 
                    Text="Payment Provider:"
                    ToolTip="The payment provider." 
                    ToolTipImage="~/Administration/images/ico-help.gif" />
             </td>
             <td class="data-item" >
                <asp:Literal ID="ltrPaymentProvider" runat="server" ></asp:Literal>
             </td>
         </tr>
         <tr>
            <td class="title" >
              <SA:ToolTipLabel 
                   ID="lblPaymentMethod" 
                   runat="server" 
                   Text="Payment Method:"
                   ToolTip="The payment method." 
                   ToolTipImage="~/Administration/images/ico-help.gif" />
            </td>
            <td class="data-item" >
                <asp:Literal ID="ltrPaymentMethod" runat="server" ></asp:Literal>
            </td>
         </tr>
         <tr>
            <td class="title" >
              <SA:ToolTipLabel 
                   ID="lblCardName" 
                   runat="server" 
                   Text="Card Name:"
                   ToolTip="The cardholder's name." 
                   ToolTipImage="~/Administration/images/ico-help.gif" />
            </td>
            <td class="data-item" >
                <asp:Literal ID="ltrCardName" runat="server" ></asp:Literal>
            </td>
         </tr>
         <tr>
            <td class="title" >
              <SA:ToolTipLabel 
                   ID="lblCardNumber" 
                   runat="server" 
                   Text="Card Number:"
                   ToolTip="The card number." 
                   ToolTipImage="~/Administration/images/ico-help.gif" />
            </td>
            <td class="data-item" >
                <asp:Literal ID="ltrCardNumber" runat="server" ></asp:Literal>
            </td>
         </tr>
         <tr>
             <td class="title" >
                <SA:ToolTipLabel 
                    ID="lblPaymentStatus" 
                    runat="server" 
                    Text="Payment status:"
                    ToolTip="The payment status of the order." 
                    ToolTipImage="~/Administration/images/ico-help.gif" />
             </td>
             <td class="data-item" >
                <asp:Literal ID="ltrPaymentStatus" runat="server" ></asp:Literal>
             </td>
          </tr>
          <tr>
             <td class="title" >
                <SA:ToolTipLabel 
                    ID="lblSubtotalInclTax" 
                    runat="server" 
                    Text="Subtotal:"
                    ToolTip="The subtotal including tax." 
                    ToolTipImage="~/Administration/images/ico-help.gif" />
             </td>
             <td class="data-item" >
                <asp:Literal ID="ltrSubtotalInclTax" runat="server" ></asp:Literal>
             </td>
          </tr>
          <tr>
             <td class="title" >
                <SA:ToolTipLabel 
                    ID="lblDiscount" 
                    runat="server" 
                    Text="Discount:"
                    ToolTip="Discount." 
                    ToolTipImage="~/Administration/images/ico-help.gif" />
             </td>
             <td class="data-item" >
                <asp:Literal ID="ltrDiscount" runat="server" ></asp:Literal>
             </td>
          </tr>
          <tr>
             <td class="title" >
                <SA:ToolTipLabel 
                    ID="lblGiftCard" 
                    runat="server" 
                    Text="Gift card:"
                    ToolTip="Gift card." 
                    ToolTipImage="~/Administration/images/ico-help.gif" />
             </td>
             <td class="data-item" >
                <asp:Literal ID="ltrGiftcard" runat="server" ></asp:Literal>
             </td>
          </tr>
          <tr>
             <td class="title" >
                <SA:ToolTipLabel 
                    ID="lblBalanceAmount" 
                    runat="server" 
                    Text="Balance:"
                    ToolTip="The amount to be paid to the salon." 
                    ToolTipImage="~/Administration/images/ico-help.gif" />
             </td>
             <td class="data-item" >
                <asp:Literal ID="ltrBalance" runat="server" ></asp:Literal>
             </td>
          </tr>
          <tr>
             <td class="title" >
                <SA:ToolTipLabel 
                    ID="lblTotal" 
                    runat="server" 
                    Text="Total:"
                    ToolTip="Total amount paid to SalonAddict." 
                    ToolTipImage="~/Administration/images/ico-help.gif" />
             </td>
             <td class="data-item" >
                <asp:Literal ID="ltrTotal" runat="server" ></asp:Literal>
             </td>
          </tr>
          <tr>
                <td class="title">
                    <SA:ToolTipLabel 
                        ID="lblCreatedOn" 
                        runat="server" 
                        Text="Created on:"
                        ToolTip="Date when customer account was created." 
                        ToolTipImage="~/Administration/images/ico-help.gif" />
                </td>
                <td class="data-item">
                    <asp:Literal ID="ltrCreatedOn" runat="server" ></asp:Literal>
                </td>
           </tr>
           <tr>
                <td class="title">
                    <SA:ToolTipLabel 
                        ID="lblUpdatedOn" 
                        runat="server" 
                        Text="Last Updated:"
                        ToolTip="Date when customer account was last updated." 
                        ToolTipImage="~/Administration/images/ico-help.gif" />
                </td>
                <td class="data-item">
                    <asp:Literal ID="ltrUpdatedOn" runat="server" ></asp:Literal>
                </td>
            </tr> 
       </table>
    </ContentTemplate>
  </ajaxToolkit:TabPanel>
  <ajaxToolkit:TabPanel ID="pnlAppointments" runat="server" HeaderText="Appointment Details" >
     <ContentTemplate>
       <table class="details" cellpadding="0" cellspacing="0" >
         <tr>
             <td class="title" >
                <SA:ToolTipLabel 
                    ID="lblAppointmentDate" 
                    runat="server" 
                    Text="Date:"
                    ToolTip="The appointment date." 
                    ToolTipImage="~/Administration/images/ico-help.gif" />
             </td>
             <td class="data-item" >
                <asp:Literal ID="ltrAppointmentDate" runat="server" ></asp:Literal>
             </td>
          </tr>
          <tr>
             <td class="title" >
                <SA:ToolTipLabel 
                    ID="lblAppointmentTime" 
                    runat="server" 
                    Text="Time:"
                    ToolTip="The appointment time." 
                    ToolTipImage="~/Administration/images/ico-help.gif" />
             </td>
             <td class="data-item" >
                <b><asp:Literal ID="ltrAppointmentTime" runat="server" ></asp:Literal></b>
             </td>
          </tr>
          <tr>
             <td class="title" >
                <SA:ToolTipLabel 
                    ID="lblStaff" 
                    runat="server" 
                    Text="Staff:"
                    ToolTip="The appointment owner." 
                    ToolTipImage="~/Administration/images/ico-help.gif" />
             </td>
             <td class="data-item" >
                <asp:Literal ID="ltrStaff" runat="server" ></asp:Literal>
             </td>
          </tr>
          <tr>
             <td class="title" >
                <SA:ToolTipLabel 
                    ID="lblService" 
                    runat="server" 
                    Text="Service:"
                    ToolTip="The appointment service." 
                    ToolTipImage="~/Administration/images/ico-help.gif" />
             </td>
             <td class="data-item" >
                <asp:Literal ID="ltrService" runat="server" ></asp:Literal>
             </td>
          </tr>
          <tr>
             <td class="title" >
                <SA:ToolTipLabel 
                    ID="lblPriceInclTax" 
                    runat="server" 
                    Text="Service:"
                    ToolTip="The appointment price including tax." 
                    ToolTipImage="~/Administration/images/ico-help.gif" />
             </td>
             <td class="data-item" >
                <asp:Literal ID="ltrPriceInclTax" runat="server" ></asp:Literal>
             </td>
          </tr>
          <tr>
             <td class="title" >
                <SA:ToolTipLabel 
                    ID="lblComments" 
                    runat="server" 
                    Text="Special instructions:"
                    ToolTip="The appointment comments/special instructions." 
                    ToolTipImage="~/Administration/images/ico-help.gif" />
             </td>
             <td class="data-item" >
                <asp:Literal ID="ltrComments" runat="server" ></asp:Literal>
             </td>
          </tr>
       </table>
     </ContentTemplate>
  </ajaxToolkit:TabPanel>
  <ajaxToolkit:TabPanel ID="pnlNotes" runat="server" HeaderText="Notes" >
     <ContentTemplate>
        <asp:UpdatePanel ID="upNotes" runat="server" >
           <ContentTemplate>
                <table style="margin-left:-10px;" cellpadding="0" cellspacing="10" >
                   <tr>
                      <td>
                         <asp:TextBox 
                            ID="txtNewNote" 
                            runat="server" 
                            TextMode="MultiLine" 
                            Height="80px" 
                            EnableViewState="false"
                            Width="600px" >
                         </asp:TextBox>
                      </td>
                   </tr>
                   <tr>
                      <td style="text-align:right">
                         &nbsp; <asp:Button ID="btnAddNote" runat="server" Text="Add New Note" OnClick="btnAddNote_Click" CausesValidation="false" />
                      </td>
                   </tr>
                </table>
                 <asp:GridView 
                    ID="gvNotes" 
                    runat="server" 
                    Width="100%"
                    DataKeyNames="OrderNoteID"
                    OnRowDeleting="gvNotes_RowDeleting"
                    AutoGenerateColumns="False" >
                    <Columns> 
                        <asp:TemplateField HeaderText="Note" >
                            <ItemTemplate>
                                <%# Eval("Note").ToString().HtmlEncode() %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Created on" >
                            <ItemTemplate>
                                <%# Eval("CreatedOn") %>
                            </ItemTemplate>
                            <ItemStyle Width="120px" />
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-Width="50px" >
                            <ItemTemplate>
                               <asp:LinkButton ID="lbDelete" runat="server" Text="Delete" OnClientClick="return confirm('Are you sure?')" CommandName="Delete" CausesValidation="false" ></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                   </Columns>
                </asp:GridView>
            </ContentTemplate>
        </asp:UpdatePanel>
     </ContentTemplate>
  </ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>
</asp:Content>
