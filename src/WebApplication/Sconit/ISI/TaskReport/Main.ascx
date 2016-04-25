<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="ISI_TaskReport_Main" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<script type="text/javascript" language="javascript">
    //<![CDATA[
    function GVEmailCheckClick(oCheckbox) {

        if (oCheckbox.checked == true) {
            $(".GVRow span[name='cbIsActive'] input:checkbox").attr("checked", true);
            $(".GVAlternatingRow span[name='cbIsActive'] input:checkbox").attr("checked", true);
        }
        else {
            $(".GVRow span[name='cbIsActive'] input:checkbox").attr("checked", false);
            $(".GVAlternatingRow span[name='cbIsActive'] input:checkbox").attr("checked", false);
        }
    }
    //]]>


</script>
<fieldset>
    <asp:GridView ID="GV_TaskReport" runat="server" AutoGenerateColumns="False"
        DataSourceID="ODS_GV_TaskReport" AllowSorting="false" DataKeyNames="TaskSubTypeCode"
        EmptyDataText="${ISI.TaskReport.NotTaskSubTypeInPermission}" OnRowDataBound="GV_List_RowDataBound"
        OnDataBinding="FV_List_OnDataBinding" OnDataBound="FV_List_DataBound">
        <Columns>
            <asp:TemplateField HeaderText="${ISI.TaskReport.TaskType}">
                <ItemTemplate>
                    <cc1:CodeMstrLabel ID="lblType" runat="server" Code="ISIType" Value='<%# Bind("TaskType") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="TaskSubTypeCode" HeaderText="${ISI.TaskReport.TaskSubTypeCode}"
                SortExpression="TaskSubTypeCode" />
            <asp:BoundField DataField="TaskSubTypeDesc" HeaderText="${ISI.TaskReport.TaskSubTypeDesc}"
                SortExpression="TaskSubTypeDesc" />
            <asp:TemplateField HeaderText="${ISI.TaskReport.IsActive}">
                <HeaderTemplate>
                    <asp:CheckBox ID="CheckAllEmail" onclick="GVEmailCheckClick(this)" runat="server"
                        Text="${ISI.TaskReport.IsActive}" TabIndex="-1" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:HiddenField ID="hfId" runat="server" Value='<%# Bind("Id") %>' />
                    <asp:CheckBox ID="cbIsActive" name="cbIsActive" runat="server" TabIndex="1" Checked='<%# Bind("IsActive") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="ODS_GV_TaskReport" runat="server" TypeName="com.Sconit.Web.TaskReportMgrProxy"
        DataObjectTypeName="com.Sconit.ISI.Entity.UserSubView" SelectMethod="LoadTaskReport"
        OnUpdating="ODS_GV_TaskReport_OnUpdating">
        <SelectParameters>
            <asp:Parameter Name="userCode" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            $('.GV').fixedtableheader();
        });
    </script>
    <table class="mtable">
        <tr>
            <td class="td02">
                <asp:Button ID="btnSave" runat="server" Text="${Common.Button.Save}" OnClick="btnSave_Click"
                    Visible="false" ValidationGroup="vgSave" />
            </td>
            <td class="td02">
            </td>
            <td class="td01">
            </td>
            <td class="td02">
            </td>
        </tr>
    </table>
</fieldset>
