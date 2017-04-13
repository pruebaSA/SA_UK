<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QueuedMessagesReport.ascx.cs" Inherits="SalonAddict.Administration.Modules.QueuedMessagesReport" %>

<table cellpadding="0" cellspacing="0" class="details" >
    <tr>
        <td>
           <img src="images/ico-attention.jpg" />
        </td>
        <td class="data-item">
            <b>Queued Messages</b>&nbsp;&nbsp; - <i><asp:Literal ID="ltrCount" runat="server" EnableViewState="false" ></asp:Literal> message(s) failed to send</i>
        </td>
    </tr>
</table>
<div style="height:200px; overflow:auto;" >
<asp:GridView 
    ID="gv" 
    runat="server" 
    Width="400px"
    AutoGenerateColumns="False" >
    <Columns>
        <asp:TemplateField HeaderText="Recipient" >
            <ItemTemplate>
                <%# Eval("To") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Created On">
            <ItemTemplate>
               <%# Eval("CreatedOn")%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <a href="<%# (((bool)Eval("IsSMS"))? "SMSQueueDetails.aspx?QueuedSMSID=" : "MessageQueueDetails.aspx?QueuedEmailID=") + Eval("QueuedID") %>" title="Requeue message">
                    manage
                </a>
            </ItemTemplate>
            <ItemStyle Width="60px" />
        </asp:TemplateField>
    </Columns>
</asp:GridView>
</div>