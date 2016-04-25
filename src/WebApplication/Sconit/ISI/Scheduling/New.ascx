<%@ Control Language="C#" AutoEventWireup="true" CodeFile="New.ascx.cs" Inherits="ISI_Scheduling_New" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<div id="divFV" runat="server">
    <asp:FormView ID="FV_Scheduling" runat="server" DataSourceID="ODS_Scheduling" DefaultMode="Insert"
        Width="100%" DataKeyNames="Code" OnDataBound="FV_Scheduling_DataBound">
        <InsertItemTemplate>
            <fieldset>
                <legend>${ISI.Scheduling.NewScheduling}</legend>
                <table class="mtable">
                    <div id="dayOfWeekShiftDv" runat="server">
                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblDayOfWeek" runat="server" Text="${ISI.Scheduling.Week}:" />
                            </td>
                            <td class="td02">
                                <asp:DropDownList ID="ddlDayOfWeek" runat="server" Text='<%# Bind("DayOfWeek") %>'>
                                    <asp:ListItem Text="" Value=""></asp:ListItem>
                                    <asp:ListItem Text="${Common.Week.Monday}" Value="Monday"></asp:ListItem>
                                    <asp:ListItem Text="${Common.Week.Tuesday}" Value="Tuesday"></asp:ListItem>
                                    <asp:ListItem Text="${Common.Week.Wednesday}" Value="Wednesday"></asp:ListItem>
                                    <asp:ListItem Text="${Common.Week.Thursday}" Value="Thursday"></asp:ListItem>
                                    <asp:ListItem Text="${Common.Week.Friday}" Value="Friday"></asp:ListItem>
                                    <asp:ListItem Text="${Common.Week.Saturday}" Value="Saturday"></asp:ListItem>
                                    <asp:ListItem Text="${Common.Week.Sunday}" Value="Sunday"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:CustomValidator ID="cvWeek" runat="server" ControlToValidate="ddlDayOfWeek"
                                    ErrorMessage="${ISI.Scheduling.WarningMessage.Error1}" Display="Dynamic" ValidationGroup="vgSave"
                                    OnServerValidate="Save_ServerValidate" />
                            </td>
                            <td class="td01">
                                <asp:Literal ID="lblShift" runat="server" Text="${ISI.Scheduling.Shift}:" />
                            </td>
                            <td class="td02">
                                <uc3:textbox ID="tbShift" runat="server" Visible="true" DescField="ShiftName" MustMatch="true"
                                    ValueField="Code" ServicePath="SchedulingMgr.service" ServiceMethod="GetShift"
                                    Width="200" ServiceParameter="string:#ddlDayOfWeek" />
                                <asp:CustomValidator ID="cvShift" runat="server" ControlToValidate="tbShift" ErrorMessage="${ISI.Scheduling.WarningMessage.Error2}"
                                    Display="Dynamic" ValidationGroup="vgSave" OnServerValidate="Save_ServerValidate" />
                            </td>
                        </tr>
                    </div>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlDesc" runat="server" Text="${Common.Business.Description}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbDesc" runat="server" Text='<%# Bind("Desc") %>' CssClass="inputRequired"
                                Width="80%" onkeypress="javascript:setMaxLength(this,25);" onpaste="limitPaste(this, 25)" />
                            <asp:RequiredFieldValidator ID="rfvDesc" runat="server" ErrorMessage="${Common.String.Empty}"
                                Display="Dynamic" ControlToValidate="tbDesc" ValidationGroup="vgSave" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblTaskSubType" runat="server" Text="${ISI.Scheduling.TaskSubType}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbTaskSubType" runat="server" Visible="true" DescField="Name" MustMatch="true"
                                ValueField="Code" ServicePath="TaskSubTypeMgr.service" ServiceMethod="GetTaskSubType"
                                Width="260" CssClass="inputRequired" />
                            <asp:RequiredFieldValidator ID="rfvTaskSubType" runat="server" ErrorMessage="${Common.String.Empty}"
                                Display="Dynamic" ControlToValidate="tbTaskSubType" ValidationGroup="vgSave" />
                            <asp:CustomValidator ID="cvTaskSubType" runat="server" ControlToValidate="tbTaskSubType"
                                ErrorMessage="" Display="Dynamic" ValidationGroup="vgSave" OnServerValidate="Save_ServerValidate" />
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
                    <div id="isSpecialDv" runat="server">
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
                                <asp:Button ID="btnInsert" runat="server" CommandName="Insert" Text="${Common.Button.Save}"
                                    CssClass="apply" ValidationGroup="vgSave" />
                                <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                                    CssClass="back" />
                            </div>
                        </td>
                    </tr>
                </table>
            </fieldset>
        </InsertItemTemplate>
    </asp:FormView>
</div>
<asp:ObjectDataSource ID="ODS_Scheduling" runat="server" TypeName="com.Sconit.Web.SchedulingMgrProxy"
    DataObjectTypeName="com.Sconit.ISI.Entity.Scheduling" InsertMethod="CreateScheduling"
    OnInserted="ODS_Scheduling_Inserted" OnInserting="ODS_Scheduling_Inserting">
</asp:ObjectDataSource>
