<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="ISI_ResInfo_Attachment_Main" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<fieldset>
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:Literal ID="lblFileName" runat="server" Text="${ISI.Attachment.FileName}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbFileName" runat="server"></asp:TextBox>
            </td>
            <td class="td01">
                <asp:Literal ID="lblCreateUser" runat="server" Text="${ISI.Attachment.CreateUser}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbCreateUser" runat="server" Visible="true" DescField="Name" MustMatch="true"
                    ValueField="Code" ServicePath="UserSubscriptionMgr.service" ServiceMethod="GetCacheAllUser" Width="250" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblStartDate" runat="server" Text="${Common.Business.StartDate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbStartDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblEndDate" runat="server" Text="${Common.Business.EndDate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbEndDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            </td>
        </tr>
        <tr>
            <td class="td01"></td>
            <td class="td02"></td>
            <td class="td01"></td>
            <td class="t02">
                <div class="buttons">
                    <asp:Button ID="btnSearch" runat="server" Text="${Common.Button.Search}" CssClass="query"
                        OnClick="btnSearch_Click" />
                    <asp:Button ID="btnExport" runat="server" Text="${Common.Button.Export}" CssClass="button2"
                        OnClick="btnExport_Click" Visible="false" />
                </div>
            </td>
        </tr>
    </table>
</fieldset>
<fieldset id="fs" runat="server" visible="false">
    <cc1:GridView ID="GV_List_Attachment" runat="server" AutoGenerateColumns="False"
        DataKeyNames="Id" SkinID="GV" AllowMultiColumnSorting="false" AutoLoadStyle="false"
        SeqNo="0" SeqText="No." ShowSeqNo="true" AllowSorting="True" AllowPaging="True"
        PagerID="gp" Width="100%" CellMaxLength="10" OnRowDataBound="GV_List_RowDataBound">
        <Columns>
            <asp:BoundField HeaderText="${ISI.ResSop.WorkShop}" />
            <asp:BoundField HeaderText="${ISI.ResSop.Operate}" />
            <asp:BoundField DataField="CreateUserNm" SortExpression="CreateUser" HeaderStyle-Wrap="false"
                HeaderText="${ISI.TSK.Attachment.CreateUser}" />
            <asp:BoundField DataField="CreateDate" SortExpression="CreateDate" HeaderStyle-Wrap="false"
                HeaderText="${ISI.TSK.Attachment.CreateDate}" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
            <asp:BoundField DataField="Size" HeaderText="${ISI.TSK.Attachment.Size}"
                SortExpression="Size" DataFormatString="{0:0.##}" />
            <asp:TemplateField HeaderText="${Common.GridView.Action}">
                <ItemTemplate>
                    <asp:LinkButton ID="lbtnDownLoad" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                        OnClick="lbtnDownLoad_Click" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </cc1:GridView>
    <cc1:GridPager ID="gp" runat="server" GridViewID="GV_List_Attachment" PageSize="15">
    </cc1:GridPager>
</fieldset>
