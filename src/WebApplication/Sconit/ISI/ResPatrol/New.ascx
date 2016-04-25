<%@ Control Language="C#" AutoEventWireup="true" CodeFile="New.ascx.cs" Inherits="Modules_ISI_ResPatrol_New" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<asp:FormView ID="FV_ResPatrol" runat="server" DataSourceID="ODS_ResPatrol" DefaultMode="Insert"
    DataKeyNames="Id">
    <InsertItemTemplate>
        <fieldset>
            <legend>${Common.New}</legend>
            <table class="mtable">
                <tr>
                    <td class="td01">${ISI.ResPatrol.WorkShop}:</td>
                    <td class="td02">
                        <uc3:textbox ID="tbWorkShop" runat="server" DescField="Name" CssClass="inputRequired"
                            ValueField="Code" ServicePath="ResWokShopMgr.service" ServiceMethod="GetAllResWokShop"
                            MustMatch="true" />
                        <asp:RequiredFieldValidator ID="rfvWorkShop" runat="server" ControlToValidate="tbWorkShop"
                            Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
                    </td>
                    <td class="td01">${ISI.ResPatrol.Role}:</td>
                    <td class="td02">
                        <uc3:textbox ID="tbRole" runat="server" DescField="Name"
                            ValueField="Code" ServicePath="ResRoleMgr.service" ServiceMethod="GetAllResRole" CssClass="inputRequired"
                            MustMatch="true" />
                        <asp:RequiredFieldValidator ID="rfvRole" runat="server" ControlToValidate="tbRole"
                            Display="Dynamic" ErrorMessage="${Common.Error.NotNull}" ValidationGroup="vgSave" />
                    </td>
                </tr>
                <%--<tr>
                    <td class="td01">
                        <asp:Literal ID="WinTime" runat="server" Text="${MasterData.Flow.Strategy.WinTime}:" />
                    </td>
                    <td class="td02" colspan="3">
                        <asp:Literal ID="Tips" runat="server" Text="${MasterData.Flow.Strategy.WinTime.Format}" />
                    </td>
                </tr>--%>
                <tr>
                    <td class="td01">${ISI.ResPatrol.WinTime1}:</td>
                    <td class="td02" colspan="3">
                        <asp:TextBox ID="tbWinTime1" runat="server" Text='<%# Bind("WinTime1") %>' Width="500" />
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" Display="Dynamic" ValidationExpression="(\b(20|21|22|23|[0-1]+\d):[0-5]+\d(\||\b))*"
                            ControlToValidate="tbWinTime1" ErrorMessage="${MasterData.Flow.Strategy.WinTime.Correct.Format}"
                            ValidationGroup="vgSave"></asp:RegularExpressionValidator>
                    </td>
                </tr>
                <tr>
                    <td class="td01">${ISI.ResPatrol.WinTime2}:</td>
                    <td class="td02" colspan="3">
                        <asp:TextBox ID="tbWinTime2" runat="server" Text='<%# Bind("WinTime2") %>' Width="500" />
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" Display="Dynamic" ValidationExpression="(\b(20|21|22|23|[0-1]+\d):[0-5]+\d(\||\b))*"
                            ControlToValidate="tbWinTime2" ErrorMessage="${MasterData.Flow.Strategy.WinTime.Correct.Format}"
                            ValidationGroup="vgSave"></asp:RegularExpressionValidator>
                    </td>
                </tr>
                <tr>
                    <td class="td01">${ISI.ResPatrol.WinTime3}:</td>
                    <td class="td02" colspan="3">
                        <asp:TextBox ID="tbWinTime3" runat="server" Text='<%# Bind("WinTime3") %>' Width="500" />
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" Display="Dynamic" ValidationExpression="(\b(20|21|22|23|[0-1]+\d):[0-5]+\d(\||\b))*"
                            ControlToValidate="tbWinTime3" ErrorMessage="${MasterData.Flow.Strategy.WinTime.Correct.Format}"
                            ValidationGroup="vgSave"></asp:RegularExpressionValidator>
                    </td>
                </tr>
                <tr>
                    <td class="td01">${ISI.ResPatrol.WinTime4}:</td>
                    <td class="td02" colspan="3">
                        <asp:TextBox ID="tbWinTime4" runat="server" Text='<%# Bind("WinTime4") %>' Width="500" />
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" Display="Dynamic" ValidationExpression="(\b(20|21|22|23|[0-1]+\d):[0-5]+\d(\||\b))*"
                            ControlToValidate="tbWinTime4" ErrorMessage="${MasterData.Flow.Strategy.WinTime.Correct.Format}"
                            ValidationGroup="vgSave"></asp:RegularExpressionValidator>
                    </td>
                </tr>
                <tr>
                    <td class="td01">${ISI.ResPatrol.WinTime5}:</td>
                    <td class="td02" colspan="3">
                        <asp:TextBox ID="tbWinTime5" runat="server" Text='<%# Bind("WinTime5") %>' Width="500" />
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator5" runat="server" Display="Dynamic" ValidationExpression="(\b(20|21|22|23|[0-1]+\d):[0-5]+\d(\||\b))*"
                            ControlToValidate="tbWinTime5" ErrorMessage="${MasterData.Flow.Strategy.WinTime.Correct.Format}"
                            ValidationGroup="vgSave"></asp:RegularExpressionValidator>
                    </td>
                </tr>
                <tr>
                    <td class="td01">${ISI.ResPatrol.WinTime6}:</td>
                    <td class="td02" colspan="3">
                        <asp:TextBox ID="tbWinTime6" runat="server" Text='<%# Bind("WinTime6") %>' Width="500" />
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator6" runat="server" Display="Dynamic" ValidationExpression="(\b(20|21|22|23|[0-1]+\d):[0-5]+\d(\||\b))*"
                            ControlToValidate="tbWinTime6" ErrorMessage="${MasterData.Flow.Strategy.WinTime.Correct.Format}"
                            ValidationGroup="vgSave"></asp:RegularExpressionValidator>
                    </td>
                </tr>
                <tr>
                    <td class="td01">${ISI.ResPatrol.WinTime7}:</td>
                    <td class="td02" colspan="3">
                        <asp:TextBox ID="tbWinTime7" runat="server" Text='<%# Bind("WinTime7") %>' Width="500" />
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator7" runat="server" Display="Dynamic" ValidationExpression="(\b(20|21|22|23|[0-1]+\d):[0-5]+\d(\||\b))*"
                            ControlToValidate="tbWinTime7" ErrorMessage="${MasterData.Flow.Strategy.WinTime.Correct.Format}"
                            ValidationGroup="vgSave"></asp:RegularExpressionValidator>
                    </td>
                </tr>
                <tr style="display: none">
                    <td class="td01">${ISI.ResPatrol.NextOrderTime}:</td>
                    <td class="td02">
                        <asp:TextBox ID="tbNextOrderTime" runat="server" Text='<%# Bind("NextOrderTime") %>' />
                    </td>
                    <td class="td01">${ISI.ResPatrol.NextWinTime}:</td>
                    <td class="td02">
                        <asp:TextBox ID="tbNextWinTime" runat="server" Text='<%# Bind("NextWinTime") %>' />
                    </td>
                </tr>
                <tr>
                    <td class="td01">${ISI.ResPatrol.LeadTime}:</td>
                    <td class="td02">
                        <asp:TextBox ID="tbLeadTime" runat="server" Text='<%# Bind("LeadTime") %>' />
                        <asp:RangeValidator ID="rvLeadTime" runat="server" ControlToValidate="tbLeadTime" ErrorMessage="${Common.Validator.Valid.Number}"
                            Display="Dynamic" Type="Double" MinimumValue="0.5" MaximumValue="720" ValidationGroup="vgSave" />
                    </td>
                </tr>
            </table>
        </fieldset>
        <div class="tablefooter">
            <asp:Button ID="btnInsert" runat="server" CommandName="Insert" Text="${Common.Button.Save}"
                CssClass="button2" ValidationGroup="vgSave" />
            <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                CssClass="button2" />
        </div>
    </InsertItemTemplate>
</asp:FormView>

<asp:ObjectDataSource ID="ODS_ResPatrol" runat="server" TypeName="com.Sconit.Web.ResPatrolMgrProxy"
    DataObjectTypeName="com.Sconit.ISI.Entity.ResPatrol" InsertMethod="CreateResPatrol"
    OnInserted="ODS_ResPatrol_Inserted" OnInserting="ODS_ResPatrol_Inserting" SelectMethod="LoadResPatrol">
    <SelectParameters>
        <asp:Parameter Name="Id" Type="Int32" />
    </SelectParameters>
    <InsertParameters>
        <asp:Parameter Name="NextOrderTime" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="NextWinTime" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="WeekInterval" Type="Int32" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="LeadTime" Type="Double" ConvertEmptyStringToNull="true" />
    </InsertParameters>
</asp:ObjectDataSource>
