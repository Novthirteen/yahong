<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Edit.ascx.cs" Inherits="ISI_Scheduling_Edit" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc2" %>
<div id="divFV" runat="server">
    <asp:FormView ID="FV_Scheduling" runat="server" DataSourceID="ODS_Scheduling" DefaultMode="Edit"
        Width="100%" DataKeyNames="Id" OnDataBound="FV_Scheduling_DataBound">
        <EditItemTemplate>
            <fieldset>
                <legend>${ISI.Scheduling.UpdateScheduling}</legend>
                <table class="mtable">
                    <div id="dayOfWeekShiftDv" runat="server">
                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblDayOfWeek" runat="server" Text="${ISI.Scheduling.Week}:" />
                            </td>
                            <td class="td02">
                                <asp:DropDownList ID="ddlDayOfWeek" runat="server" Text='<%# Bind("DayOfWeek") %>'
                                    Enabled="false">
                                    <asp:ListItem Text="" Value=""></asp:ListItem>
                                    <asp:ListItem Text="${Common.Week.Monday}" Value="Monday"></asp:ListItem>
                                    <asp:ListItem Text="${Common.Week.Tuesday}" Value="Tuesday"></asp:ListItem>
                                    <asp:ListItem Text="${Common.Week.Wednesday}" Value="Wednesday"></asp:ListItem>
                                    <asp:ListItem Text="${Common.Week.Thursday}" Value="Thursday"></asp:ListItem>
                                    <asp:ListItem Text="${Common.Week.Friday}" Value="Friday"></asp:ListItem>
                                    <asp:ListItem Text="${Common.Week.Saturday}" Value="Saturday"></asp:ListItem>
                                    <asp:ListItem Text="${Common.Week.Sunday}" Value="Sunday"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td class="td01">
                                <asp:Literal ID="lblShift" runat="server" Text="${ISI.Scheduling.Shift}:" />
                            </td>
                            <td class="td02">
                                <asp:TextBox ID="tbShift" runat="server" Text='<%# Bind("Shift") %>' ReadOnly="true"
                                    onfocus="this.blur();" />
                            </td>
                        </tr>
                    </div>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlDesc" runat="server" Text="${Common.Business.Description}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbDesc" runat="server" Text='<%# Bind("Desc") %>' CssClass="inputRequired"
                                onkeypress="javascript:setMaxLength(this,25);" onpaste="limitPaste(this, 25)" />
                            <asp:RequiredFieldValidator ID="rfvDesc" runat="server" ErrorMessage="${Common.String.Empty}"
                                Display="Dynamic" ControlToValidate="tbDesc" ValidationGroup="vgSave" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblTaskSubType" runat="server" Text="${ISI.Scheduling.TaskSubType}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbTaskSubType" runat="server" ReadOnly="true" onfocus="this.blur();" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlStartUser" runat="server" Text="${ISI.Scheduling.StartUser}:" />
                        </td>
                        <td class="td02" colspan="3">
                            <asp:Literal ID="ltlStartUserFormat" runat="server" Text="${ISI.User.Format}" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                        </td>
                        <td class="td02" colspan="3">
                            <asp:TextBox ID="tbStartUser" runat="server" Text='<%# Bind("StartUser") %>' Width="600"
                                CssClass="inputRequired" onkeypress="javascript:setMaxLength(this,255);" onpaste="limitPaste(this, 255)" />
                            <asp:RequiredFieldValidator ID="rfvStartUser" runat="server" ErrorMessage="${Common.String.Empty}"
                                Display="Dynamic" ControlToValidate="tbStartUser" ValidationGroup="vgSave" />
                            <asp:RegularExpressionValidator ID="revStartUser" runat="server" Display="Dynamic"
                                ValidationExpression="^([a-zA-Z0-9]+[,]+[a-zA-Z0-9]+|[a-zA-Z0-9]*)*$" ControlToValidate="tbStartUser"
                                ErrorMessage="${ISI.Error.NoVerifiedThrough}" ValidationGroup="vgSave"></asp:RegularExpressionValidator>
                            <asp:CustomValidator ID="cvStartUser" runat="server" ControlToValidate="tbStartUser"
                                Display="Dynamic" ValidationGroup="vgSave" OnServerValidate="Save_ServerValidate" />
                        </td>
                    </tr>
                    <div id="isSpecialdv" runat="server">
                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblStartDate" runat="server" Text="${Common.Business.StartTime}:" />
                            </td>
                            <td class="td02">
                                <asp:TextBox ID="tbStartDate" runat="server" Text='<%# Bind("StartDate") %>' CssClass="inputRequired"
                                    onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})" />
                                <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" ErrorMessage="${Common.String.Empty}"
                                    Display="Dynamic" ControlToValidate="tbStartDate" ValidationGroup="vgSave" />
                                <asp:CustomValidator ID="cvStartDate" runat="server" ControlToValidate="tbStartDate"
                                    ErrorMessage="${ISI.Scheduling.WarningMessage.StartTimeInvalid}" Display="Dynamic"
                                    ValidationGroup="vgSave" OnServerValidate="Save_ServerValidate" />
                            </td>
                            <td class="td01">
                                <asp:Literal ID="lblEndDate" runat="server" Text="${Common.Business.EndTime}:" />
                            </td>
                            <td class="td02">
                                <asp:TextBox ID="tbEndDate" runat="server" Text='<%# Bind("EndDate") %>' CssClass="inputRequired"
                                    onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})" />
                                <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" ErrorMessage="${Common.String.Empty}"
                                    Display="Dynamic" ControlToValidate="tbEndDate" ValidationGroup="vgSave" />
                                <asp:CustomValidator ID="cvEndDate" runat="server" ControlToValidate="tbEndDate"
                                    ErrorMessage="${ISI.Scheduling.WarningMessage.EndTimeInvalid}" Display="Dynamic"
                                    ValidationGroup="vgSave" OnServerValidate="Save_ServerValidate" />
                            </td>
                        </tr>
                    </div>
                    <tr>
                        <td class="td01">
                        </td>
                        <td class="td02">
                        </td>
                        <td class="td01">
                        </td>
                        <td class="td02">
                            <div class="buttons">
                                <asp:Button ID="btnSave" runat="server" CommandName="Update" Text="${Common.Button.Save}"
                                    CssClass="apply" ValidationGroup="vgSave" />
                                <asp:Button ID="btnDelete" runat="server" CommandName="Delete" Text="${Common.Button.Delete}"
                                    CssClass="delete" OnClientClick="return confirm('${Common.Button.Delete.Confirm}')" />
                                <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                                    CssClass="back" />
                            </div>
                        </td>
                    </tr>
                </table>
            </fieldset>
        </EditItemTemplate>
    </asp:FormView>
</div>
<asp:ObjectDataSource ID="ODS_Scheduling" runat="server" TypeName="com.Sconit.Web.SchedulingMgrProxy"
    DataObjectTypeName="com.Sconit.ISI.Entity.Scheduling" UpdateMethod="UpdateScheduling"
    OnUpdated="ODS_Scheduling_Updated" SelectMethod="LoadScheduling" OnUpdating="ODS_Scheduling_Updating"
    DeleteMethod="DeleteScheduling" OnDeleting="ODS_Scheduling_Deleting" OnDeleted="ODS_Scheduling_Deleted">
    <SelectParameters>
        <asp:Parameter Name="id" Type="Int32" />
    </SelectParameters>
    <DeleteParameters>
        <asp:Parameter Name="id" Type="Int32" />
    </DeleteParameters>
</asp:ObjectDataSource>
