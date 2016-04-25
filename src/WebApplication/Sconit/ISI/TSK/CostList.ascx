<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CostList.ascx.cs" Inherits="ISI_TSK_CostList" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<fieldset>
    <legend>${WFS.Cost.List}</legend>
    <asp:GridView ID="GV_List" runat="server" OnRowDataBound="GV_List_RowDataBound" OnDataBound="GV_List_DataBound"
        AutoGenerateColumns="false" ShowHeader="false">
        <Columns>
            <asp:TemplateField HeaderStyle-Wrap="false">
                <ItemTemplate>
                    <table class="mtable">
                        
                        <tr runat="server" id="tr1" visible="false">
                            <td style="text-align: right; width: 10%">
                                <asp:Label ID="Label12" runat="server" Text="${WFS.Cost.Item}:" /></td>
                            <td style="width: 15%">
                                <asp:Label ID="lblItem" runat="server" Text='<%# Bind("Item") %>' Visible="false" />
                                <asp:TextBox ID="tbItem" CssClass="inputRequired" runat="server" Width="120" Text='<%# Bind("Item") %>' Visible="false" />
                                <asp:RequiredFieldValidator ID="rfvItem" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                                    Display="Dynamic" ControlToValidate="tbItem" ValidationGroup='vgAdd' />
                            </td>
                            <td style="text-align: right; width: 10%">
                                <asp:Label ID="Label14" runat="server" Text="${WFS.Cost.Uom}:" /></td>
                            <td style="width: 15%">
                                <asp:Label ID="lblUom" runat="server" Text='<%# Bind("Uom") %>' Visible="false" />
                                <asp:TextBox ID="tbUom" CssClass="inputRequired" runat="server" Width="120" Text='<%# Bind("Uom") %>' Visible="false" />
                                <asp:RequiredFieldValidator ID="rfvUom" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                                    Display="Dynamic" ControlToValidate="tbUom" ValidationGroup='vgAdd' />
                            </td>
                            <td style="text-align: right; width: 10%">
                                <asp:Label ID="Label16" runat="server" Text="${WFS.Cost.Qty}:" /></td>
                            <td style="width: 15%">
                                <asp:Label ID="lblQty" runat="server" Text='<%# Bind("Qty","{0:0.########}") %>' Visible="false" />
                                <asp:TextBox ID="tbQty" runat="server" Width="120" CssClass="inputRequired" Text='<%# Bind("Qty","{0:0.########}") %>' Visible="false" />
                                <asp:RequiredFieldValidator ID="rfvQty" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                                    Display="Dynamic" ControlToValidate="tbQty" ValidationGroup='vgAdd' />
                                <asp:RangeValidator ID="rvQty" runat="server" ControlToValidate="tbQty"
                                    Display="Dynamic" ErrorMessage="${Common.Validator.Valid.Number}" ValidationGroup='vgAdd'
                                    Type="Double" MaximumValue="99999999" MinimumValue="-99999999" />
                            </td>
                            <td style="text-align: right; width: 10%"></td>
                            <td style="width: 15%"></td>
                        </tr>
                        <tr runat="server" id="tr2" visible="false">
                            <td style="text-align: right; width: 10%">
                                <asp:Label ID="ltlStartDate" runat="server" Text="${WFS.Cost.StartDate}:" /></td>
                            <td style="width: 15%">
                                <asp:Label ID="lblStartDate" runat="server" Text='<%# Bind("StartDate","{0:yyyy-MM-dd HH:mm}") %>' Visible="false" />
                                <asp:TextBox ID="tbStartDate" CssClass="inputRequired" runat="server" onClick="WdatePicker({startDate:'%y-%M-%d 08:00:00',dateFmt:'yyyy-MM-dd HH:mm'})" Width="120" Text='<%# Bind("StartDate","{0:yyyy-MM-dd HH:mm}") %>' Visible="false" />
                                <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                                    Display="Dynamic" ControlToValidate="tbStartDate" ValidationGroup='vgAdd' />
                            </td>
                            <td style="text-align: right; width: 10%">
                                <asp:Label ID="ltlEndDate" runat="server" Text="${WFS.Cost.EndDate}:" /></td>
                            <td style="width: 15%">
                                <asp:Label ID="lblEndDate" runat="server" Text='<%# Bind("EndDate","{0:yyyy-MM-dd HH:mm}") %>' Visible="false" />
                                <asp:TextBox ID="tbEndDate" CssClass="inputRequired" runat="server" onClick="WdatePicker({startDate:'%y-%M-%d 16:30:00',dateFmt:'yyyy-MM-dd HH:mm'})" Width="120" Text='<%# Bind("EndDate","{0:yyyy-MM-dd HH:mm}") %>' Visible="false" />
                                <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                                    Display="Dynamic" ControlToValidate="tbEndDate" ValidationGroup='vgAdd' />
                            </td>
                            <td style="text-align: right; width: 10%">
                                <asp:Label ID="ltlStartAddr" runat="server" Text="${WFS.Cost.StartAddr}:" /></td>
                            <td style="width: 15%">
                                <asp:Label ID="lblStartAddr" runat="server" Text='<%# Bind("StartAddr") %>' Visible="false" />
                                <asp:TextBox ID="tbStartAddr" runat="server" CssClass="inputRequired" Width="120" Text='<%# Bind("StartAddr") %>' Visible="false" />
                                <asp:RequiredFieldValidator ID="rfvStartAddr" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                                    Display="Dynamic" ControlToValidate="tbStartAddr" ValidationGroup='vgAdd' />
                            </td>
                            <td style="text-align: right; width: 10%">
                                <asp:Label ID="ltlEndAddr" runat="server" Text="${WFS.Cost.EndAddr}:" /></td>
                            <td style="width: 15%">
                                <asp:Label ID="lblEndAddr" runat="server" Text='<%# Bind("EndAddr") %>' Visible="false" />
                                <asp:TextBox ID="tbEndAddr" CssClass="inputRequired" runat="server" Width="120" Text='<%# Bind("EndAddr") %>' Visible="false" />
                                <asp:RequiredFieldValidator ID="rfvEndAddr" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                                    Display="Dynamic" ControlToValidate="tbEndAddr" ValidationGroup='vgAdd' />
                            </td>
                        </tr>
                        <tr runat="server" id="tr3" visible="false">
                            <td style="text-align: right; width: 10%">
                                <asp:Label ID="Label3" runat="server" Text="${WFS.Cost.Allowance}:" /></td>
                            <td style="width: 15%">
                                <asp:Label ID="lblAllowance" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "Allowance","{0:N}")%>' />
                                <asp:TextBox ID="tbAllowance" runat="server" Width="120" Text='<%# Bind("Allowance","{0:0.########}") %>' Visible="false" />
                                <asp:RangeValidator ID="rvAllowance" runat="server" ControlToValidate="tbAllowance"
                                    Display="Dynamic" ErrorMessage="${Common.Validator.Valid.Number}" ValidationGroup='vgAdd'
                                    Type="Double" MaximumValue="99999999" MinimumValue="-99999999" />
                            </td>
                            <td style="text-align: right; width: 10%">
                                <asp:Label ID="Label6" runat="server" Text="${WFS.Cost.Fare}:" /></td>
                            <td style="width: 15%">
                                <asp:Label ID="lblFare" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "Fare","{0:N}")%>' />
                                <asp:TextBox ID="tbFare" runat="server" Width="120" Text='<%# Bind("Fare","{0:0.########}") %>' Visible="false" />
                                <asp:RangeValidator ID="rvFare" runat="server" ControlToValidate="tbFare"
                                    Display="Dynamic" ErrorMessage="${Common.Validator.Valid.Number}" ValidationGroup='vgAdd'
                                    Type="Double" MaximumValue="99999999" MinimumValue="-99999999" />
                            </td>
                            <td style="text-align: right; width: 10%">
                                <asp:Label ID="Label8" runat="server" Text="${WFS.Cost.Quarterage}:" /></td>
                            <td style="width: 15%">
                                <asp:Label ID="lblQuarterage" Visible="false" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Quarterage","{0:N}")%>' />
                                <asp:TextBox ID="tbQuarterage" runat="server" Width="120" Text='<%# Bind("Quarterage","{0:0.########}") %>' Visible="false" />
                                <asp:RangeValidator ID="rvQuarterage" runat="server" ControlToValidate="tbQuarterage"
                                    Display="Dynamic" ErrorMessage="${Common.Validator.Valid.Number}" ValidationGroup='vgAdd'
                                    Type="Double" MaximumValue="99999999" MinimumValue="-99999999" />
                            </td>
                            <td style="text-align: right; width: 10%">
                                <asp:Label ID="Label10" runat="server" Text="${WFS.Cost.Haulage}:" /></td>
                            <td style="width: 15%">
                                <asp:Label ID="lblHaulage" Visible="false" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Haulage","{0:N}")%>' />
                                <asp:TextBox ID="tbHaulage" runat="server" Width="120" Text='<%# Bind("Haulage","{0:0.########}") %>' Visible="false" />
                                <asp:RangeValidator ID="rvHaulage" runat="server" ControlToValidate="tbHaulage"
                                    Display="Dynamic" ErrorMessage="${Common.Validator.Valid.Number}" ValidationGroup='vgAdd'
                                    Type="Double" MaximumValue="99999999" MinimumValue="-99999999" />
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: right; width: 10%">
                                <asp:Label ID="Label5" runat="server" Text="${WFS.Cost.NoTaxAmount}:" /></td>
                            <td style="width: 15%">
                                <asp:Label ID="lblNoTaxAmount" Visible="false" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "NoTaxAmount","{0:N}")%>' />
                                <asp:TextBox ID="tbNoTaxAmount" CssClass="inputRequired" runat="server" Width="120" Text='<%# Bind("NoTaxAmount","{0:0.########}") %>' Visible="false" />
                                <asp:RequiredFieldValidator ID="rfvNoTaxAmount" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                                    Display="Dynamic" ControlToValidate="tbNoTaxAmount" ValidationGroup='vgAdd' />
                                <asp:RangeValidator ID="rvNoTaxAmount" runat="server" ControlToValidate="tbNoTaxAmount"
                                    Display="Dynamic" ErrorMessage="${Common.Validator.Valid.Number}" ValidationGroup='vgAdd'
                                    Type="Double" MaximumValue="99999999" MinimumValue="-99999999" />
                            </td>
                            <td style="text-align: right; width: 10%">
                                <asp:Label ID="Label7" runat="server" Text="${WFS.Cost.Taxes}:" /></td>
                            <td style="width: 15%">
                                <asp:Label ID="lblTaxes" Visible="false" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Taxes","{0:N}")%>' />
                                <asp:TextBox ID="tbTaxes" runat="server" Width="120" Text='<%# Bind("Taxes","{0:0.########}") %>' Visible="false" />
                                <asp:RangeValidator ID="rvTaxes" runat="server" ControlToValidate="tbTaxes"
                                    Display="Dynamic" ErrorMessage="${Common.Validator.Valid.Number}" ValidationGroup='vgAdd'
                                    Type="Double" MaximumValue="99999999" MinimumValue="-99999999" />
                            </td>
                            <td style="text-align: right; width: 10%">
                                <asp:Label ID="Label9" runat="server" Text="${WFS.Cost.TotalAmount}:" />
                            </td>
                            <td style="width: 15%">
                                <asp:Label ID="lblTotalAmount" Visible="false" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "TotalAmount","{0:N}")%>' />
                                <asp:TextBox ID="tbTotalAmount" runat="server" Width="120" Text='<%# Bind("TotalAmount","{0:0.########}") %>' Visible="false" ReadOnly="true" /></td>
                            <td style="text-align: right; width: 10%">
                                <asp:Label ID="ltlExtNo" runat="server" Text="${WFS.Cost.ExtNo}:" />
                            </td>
                            <td style="width: 15%">
                                <asp:Label ID="lblExtNo" Visible="false" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ExtNo")%>' />
                                <asp:TextBox ID="tbExtNo" runat="server" Text='<%# Bind("ExtNo") %>' Visible="false" /></td>
                        </tr>
                        <tr>
                            <td style="text-align: right; width: 10%">
                                <asp:Label ID="ltlTaskSubType" runat="server" Text="${WFS.Cost.TaskSubType}:" />
                            </td>
                            <td style="width: 15%">
                                <asp:HiddenField ID="hfId" runat="server" Value='<%# Bind("Id") %>' />
                                <asp:HiddenField ID="hfTaskSubType" runat="server" Value='<%# Bind("TaskSubType") %>' />
                                <asp:Label ID="lblTaskSubType" runat="server" Text='<%# Bind("TaskSubTypeName") %>' Visible="false" />
                                <uc3:textbox ID="tbTaskSubType" runat="server" DescField="Name" MustMatch="true"
                                    ValueField="Code" ServicePath="TaskSubTypeMgr.service" ServiceMethod="GetCostCenter" Width="260" Visible="false"
                                    ServiceParameter="bool:true" />
                            </td>
                            <td style="text-align: right; width: 10%">
                                <asp:Label ID="ltlAccount1" runat="server" Text="${WFS.Cost.Account1}:" />
                            </td>
                            <td style="width: 15%">
                                <asp:Label ID="lblAccount1" runat="server" Text='<%# Bind("Account1Name") %>' Visible="false" />
                                <uc3:textbox ID="tbAccount1" runat="server" DescField="Account1Desc" MustMatch="true"
                                    ValueField="Account1" ServicePath="BudgetDetMgr.service" ServiceMethod="GetAccount1" Width="260" Visible="false" />
                            </td>
                            <td style="text-align: right; width: 10%">
                                <asp:Label ID="ltlAccount2" runat="server" Text="${WFS.Cost.Account2}:" />
                            </td>
                            <td style="width: 15%">
                                <asp:Label ID="lblAccount2" runat="server" Text='<%# Bind("Account2Name") %>' Visible="false" />
                                <uc3:textbox ID="tbAccount2" runat="server" DescField="Account2Desc" MustMatch="true"
                                    ValueField="Account2" ServicePath="BudgetDetMgr.service" ServiceMethod="GetAccount2" Width="260" Visible="false" />
                            </td>
                            <td style="text-align: right; width: 10%">
                                <asp:Label ID="Label11" runat="server" Text="${WFS.Cost.UserCode}:" /></td>
                            <td style="width: 15%">
                                <asp:Label ID="lblUserCode" runat="server" Text='<%# Bind("User") %>' Visible="false" />
                                <uc3:textbox ID="tbUserCode" runat="server" DescField="Name" MustMatch="true"
                                    ValueField="Code" ServicePath="UserMgr.service" ServiceMethod="GetAllUser" Width="260" />
                            </td>
                        </tr>
                        
                        <tr>
                            <td style="text-align: right; width: 10%">
                                <asp:Label ID="ltlDesc1" runat="server" Text="${WFS.Cost.Desc1}:" />
                            </td>
                            <td colspan="3">
                                <asp:Label ID="lblDesc1" runat="server" Text='<%# Bind("Desc1") %>' Visible="false" />
                                <asp:TextBox ID="tbDesc1" TextMode="MultiLine" Font-Size="10" Height="30" runat="server" Width="100%" Text='<%# Bind("Desc1") %>' Visible="false" onkeypress="javascript:setMaxLength(this,150);" onpaste="limitPaste(this, 150)" />
                            </td>
                            <td style="text-align: right; width: 10%">
                                <asp:Literal ID="ltlVehicle" runat="server" Text="${WFS.Cost.Vehicle}:" />
                            </td>
                            <td style="width: 15%">
                                <cc1:CodeMstrLabel ID="lblVehicle" Code="VehicleType" runat="server" Value='<%#Bind("Vehicle") %>' Visible="false" />
                                <cc1:CodeMstrDropDownList ID="tbVehicle" Code="VehicleType" runat="server" IncludeBlankOption="false" Visible="false" />
                            </td>
                            <td style="text-align: right; width: 10%">
                                <div>
                                    <asp:Literal ID="ltlLastModifyUser" runat="server" Visible="false" Text="${Common.Business.LastModifyUser}:" />
                                </div>
                                <div>
                                    <asp:Literal ID="ltlLastModifyDate" runat="server" Text="${Common.Business.LastModifyDate}:" Visible="false" />
                                </div>
                            </td>
                            <td style="width: 15%">
                                <div>
                                    <asp:Label ID="lblLastModifyUser" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "LastModifyUserNm")%>' />
                                </div>
                                <div>
                                    <asp:Label ID="lblLastModifyDate" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "LastModifyDate","{0:yyyy-MM-dd HH:mm}")%>' />
                                </div>
                            </td>
                        </tr>
                    </table>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="${Common.GridView.Action}" HeaderStyle-Wrap="false">
                <ItemTemplate>
                    <div>
                        <asp:LinkButton ID="lbtnAdd" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                            Text="${Common.Button.New}" OnClick="lbtnAdd_Click"
                            ValidationGroup='vgAdd' TabIndex="1">
                        </asp:LinkButton>
                    </div>
                    <div>
                        <asp:LinkButton ID="lbtnDelete" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                            Text="${Common.Button.Delete}" OnClick="lbtnDelete_Click" OnClientClick="return confirm('${Common.Button.Delete.Confirm}')">
                        </asp:LinkButton>
                    </div>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            $('.GV').fixedtableheader();
        });
    </script>
</fieldset>
