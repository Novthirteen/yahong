<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ListAttachment.ascx.cs" Inherits="Facility_FacilityMaintainPlanDetail_ListAttachment" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<div id="floatdiv">
<fieldset>
    <asp:GridView ID="GV_List_Attachment" runat="server" OnRowDataBound="GV_List_RowDataBound"
        AutoGenerateColumns="false">
        <Columns>
            <asp:BoundField DataField="CreateUserNm" SortExpression="CreateUser" HeaderStyle-Wrap="false"
                HeaderText="${ISI.TSK.Attachment.CreateUser}" />
            <asp:BoundField DataField="CreateDate" SortExpression="CreateDate" HeaderStyle-Wrap="false"
                HeaderText="${ISI.TSK.Attachment.CreateDate}" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
            <asp:BoundField DataField="Size" HeaderText="${ISI.TSK.Attachment.Size}"
                    SortExpression="Size" DataFormatString="{0:0.##}" />
            <asp:TemplateField HeaderText="${Common.GridView.Action}">
                <ItemTemplate>
                    <asp:LinkButton ID="lbtnDownLoad" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                        OnClick="lbtnDownLoad_Click" />&nbsp;
                  
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</fieldset>
<div class="buttons">
    <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
        CssClass="back" />
</div>
</div>