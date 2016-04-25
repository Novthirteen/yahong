<%@ Control Language="C#" AutoEventWireup="true" CodeFile="New.ascx.cs" Inherits="ISI_Bill_New" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<script language="javascript" type="text/javascript">
    $(document).ready(function () {
        $('textarea').tah({
            moreSpace: 25,   // 输入框底部预留的空白, 默认15, 单位像素
            maxHeight: 260,  // 指定Textarea的最大高度, 默认600, 单位像素
            animateDur: 200  // 调整高度时的动画过渡时间, 默认200, 单位毫秒
        });
    });
</script>
<div id="divFV">
    <fieldset>
        <legend runat="server" id="lgd"></legend>
        <asp:FormView ID="FV_Bill" runat="server" DataSourceID="ODS_Mould"
            DefaultMode="Insert" Width="100%" DataKeyNames="Code" OnDataBound="FV_Bill_DataBound">
            <InsertItemTemplate>

                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlPrjCode" runat="server" Text="${PSI.Bill.PrjCode}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbPrjCode" runat="server" Visible="true" Width="250" DescField="Desc" CssClass="inputRequired"
                                ValueField="Code" ServicePath="TaskSubTypeMgr.service" ServiceMethod="GetProjectTaskSubType" Text='<%# Bind("PrjCode") %>' />
                            <asp:RequiredFieldValidator ID="rfvPrjCode" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                                Display="Dynamic" ControlToValidate="tbPrjCode" ValidationGroup="vgSave" />
                            <asp:CustomValidator ID="cvPrjCode" runat="server" ControlToValidate="tbPrjCode"
                                ErrorMessage="${PSI.Bill.SOContractNo.Exists}" Display="Dynamic"
                                ValidationGroup="vgSave" OnServerValidate="checkCodeExists" />
                        </td>
                        <td class="td01"></td>
                        <td class="td02"></td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlDesc1" runat="server" Text="${Common.Business.Description}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbDesc1" runat="server" Text='<%# Bind("Desc1") %>' CssClass="inputRequired" Width="260" />
                            <asp:RequiredFieldValidator ID="rfvDesc1" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                                Display="Dynamic" ControlToValidate="tbDesc1" ValidationGroup="vgSave" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlQS" runat="server" Text="${PSI.Bill.QS}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbQS" runat="server" Text='<%# Bind("QS") %>' />
                            <asp:RangeValidator ID="rvQS" runat="server" ControlToValidate="tbQS" ErrorMessage="${Common.Validator.Valid.Number}"
                                Display="Dynamic" Type="Double" MinimumValue="0" MaximumValue="99999999" ValidationGroup="vgSave" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlQty" runat="server" Text="${PSI.Bill.Qty}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbQty" runat="server" Text='<%# Bind("Qty") %>' CssClass="inputRequired" />
                            <asp:RequiredFieldValidator ID="rfvQty" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                                Display="Dynamic" ControlToValidate="tbQty" ValidationGroup="vgSave" />
                            <asp:RangeValidator ID="rvQty" runat="server" ControlToValidate="tbQty" ErrorMessage="${Common.Validator.Valid.Number}"
                                Display="Dynamic" Type="Integer" MinimumValue="0" MaximumValue="100000" ValidationGroup="vgSave" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblMouldUser" runat="server" Text="${PSI.Bill.MouldUser}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbMouldUser" runat="server" Visible="true" DescField="Name" MustMatch="true"
                                ValueField="Code" ServicePath="UserSubscriptionMgr.service" ServiceMethod="GetCacheAllUser" Width="250" Text='<%# Bind("MouldUser") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlSOUser" runat="server" Text="${PSI.Bill.SOUser}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbSOUser" runat="server" Visible="true" DescField="Name" MustMatch="true"
                                ValueField="Code" ServicePath="UserSubscriptionMgr.service" ServiceMethod="GetCacheAllUser" Width="250" Text='<%# Bind("SOUser") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlPOUser" runat="server" Text="${PSI.Bill.POUser}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbPOUser" runat="server" Visible="true" DescField="Name" MustMatch="true"
                                ValueField="Code" ServicePath="UserSubscriptionMgr.service" ServiceMethod="GetCacheAllUser" Width="250" Text='<%# Bind("POUser") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlCustomer" runat="server" Text="${PSI.Bill.Customer}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbCustomer" runat="server" Visible="true" Width="250" DescField="Name"
                                ValueField="Code" ServicePath="CustomerMgr.service" ServiceMethod="GetAllCustomer" Text='<%# Bind("Customer") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlSupplier" runat="server" Text="${PSI.Bill.Supplier}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox ID="tbSupplier" runat="server" Visible="true" Width="250" DescField="Name"
                                ValueField="Code" ServicePath="SupplierMgr.service" ServiceMethod="GetAllSupplier"
                                Text='<%# Bind("Supplier") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlSOContractNo" runat="server" Text="${PSI.Bill.SOContractNo}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSOContractNo" runat="server" Text='<%# Bind("SOContractNo") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlSupplierContractNo" runat="server" Text="${PSI.Bill.SupplierContractNo}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSupplierContractNo" runat="server" Text='<%# Bind("SupplierContractNo") %>' />
                            <asp:CustomValidator ID="cvSupplierContractNo" runat="server" ControlToValidate="tbSupplierContractNo"
                                ErrorMessage="${PSI.Bill.SupplierContractNo.Exists}" Display="Dynamic"
                                ValidationGroup="vgSave" OnServerValidate="checkSupplierContractNoExists" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlSOAmount" runat="server" Text="${PSI.Bill.SOAmount}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSOAmount" runat="server" Text='<%# Bind("SOAmount","{0:0.########}") %>' />
                            <asp:RangeValidator ID="rvSOAmount" runat="server" ControlToValidate="tbSOAmount" ErrorMessage="${Common.Validator.Valid.Number}"
                                Display="Dynamic" Type="Double" MinimumValue="0" MaximumValue="99999999" ValidationGroup="vgSave" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlSupplierAmount" runat="server" Text="${PSI.Bill.SupplierAmount}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSupplierAmount" runat="server" Text='<%# Bind("SupplierAmount","{0:0.########}") %>' />
                            <asp:RangeValidator ID="rvSupplierAmount" runat="server" ControlToValidate="tbSupplierAmount" ErrorMessage="${Common.Validator.Valid.Number}"
                                Display="Dynamic" Type="Double" MinimumValue="0" MaximumValue="99999999" ValidationGroup="vgSave" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlSOAmount1" runat="server" Text="${PSI.Bill.SOAmount1}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSOAmount1" runat="server" Text='<%# Bind("SOAmount1","{0:0.########}") %>' />
                            <asp:RangeValidator ID="rvSOAmount1" runat="server" ControlToValidate="tbSOAmount1" ErrorMessage="${Common.Validator.Valid.Number}"
                                Display="Dynamic" Type="Double" MinimumValue="0" MaximumValue="99999999" ValidationGroup="vgSave" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlSupplierAmount1" runat="server" Text="${PSI.Bill.SupplierAmount1}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSupplierAmount1" runat="server" Text='<%# Bind("SupplierAmount1","{0:0.########}") %>' />
                            <asp:RangeValidator ID="rvSupplierAmount1" runat="server" ControlToValidate="tbSupplierAmount1" ErrorMessage="${Common.Validator.Valid.Number}"
                                Display="Dynamic" Type="Double" MinimumValue="0" MaximumValue="99999999" ValidationGroup="vgSave" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblSOBillDate1" runat="server" Text="${PSI.Bill.SOBillDate1}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSOBillDate1" runat="server" Text='<%# Bind("SOBillDate1","{0:yyyy-MM-dd}") %>' onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblSupplierBillDate1" runat="server" Text="${PSI.Bill.SupplierBillDate1}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSupplierBillDate1" runat="server" Text='<%# Bind("SupplierBillDate1","{0:yyyy-MM-dd}") %>' onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblSOPayDate1" runat="server" Text="${PSI.Bill.SOPayDate1}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSOPayDate1" runat="server" Text='<%# Bind("SOPayDate1","{0:yyyy-MM-dd}") %>' onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblSupplierPayDate1" runat="server" Text="${PSI.Bill.SupplierPayDate1}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSupplierPayDate1" runat="server" Text='<%# Bind("SupplierPayDate1","{0:yyyy-MM-dd}") %>' onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlSOAmount2" runat="server" Text="${PSI.Bill.SOAmount2}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSOAmount2" runat="server" Text='<%# Bind("SOAmount2","{0:0.########}") %>' />
                            <asp:RangeValidator ID="rvSOAmount2" runat="server" ControlToValidate="tbSOAmount2" ErrorMessage="${Common.Validator.Valid.Number}"
                                Display="Dynamic" Type="Double" MinimumValue="0" MaximumValue="99999999" ValidationGroup="vgSave" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlSupplierAmount2" runat="server" Text="${PSI.Bill.SupplierAmount2}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSupplierAmount2" runat="server" Text='<%# Bind("SupplierAmount2","{0:0.########}") %>' />
                            <asp:RangeValidator ID="rvSupplierAmount2" runat="server" ControlToValidate="tbSupplierAmount2" ErrorMessage="${Common.Validator.Valid.Number}"
                                Display="Dynamic" Type="Double" MinimumValue="0" MaximumValue="99999999" ValidationGroup="vgSave" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblSOBillDate2" runat="server" Text="${PSI.Bill.SOBillDate2}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSOBillDate2" runat="server" Text='<%# Bind("SOBillDate2","{0:yyyy-MM-dd}") %>' onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblSupplierBillDate2" runat="server" Text="${PSI.Bill.SupplierBillDate2}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSupplierBillDate2" runat="server" Text='<%# Bind("SupplierBillDate2","{0:yyyy-MM-dd}") %>' onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblSOPayDate2" runat="server" Text="${PSI.Bill.SOPayDate2}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSOPayDate2" runat="server" Text='<%# Bind("SOPayDate2","{0:yyyy-MM-dd}") %>' onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblSupplierPayDate2" runat="server" Text="${PSI.Bill.SupplierPayDate2}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSupplierPayDate2" runat="server" Text='<%# Bind("SupplierPayDate2","{0:yyyy-MM-dd}") %>' onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
                        </td>
                    </tr>





                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlSOAmount4" runat="server" Text="${PSI.Bill.SOAmount4}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSOAmount4" runat="server" Text='<%# Bind("SOAmount4","{0:0.########}") %>' />
                            <asp:RangeValidator ID="rvSOAmount4" runat="server" ControlToValidate="tbSOAmount4" ErrorMessage="${Common.Validator.Valid.Number}"
                                Display="Dynamic" Type="Double" MinimumValue="0" MaximumValue="99999999" ValidationGroup="vgSave" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlSupplierAmount4" runat="server" Text="${PSI.Bill.SupplierAmount4}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSupplierAmount4" runat="server" Text='<%# Bind("SupplierAmount4","{0:0.########}") %>' />
                            <asp:RangeValidator ID="rvSupplierAmount4" runat="server" ControlToValidate="tbSupplierAmount4" ErrorMessage="${Common.Validator.Valid.Number}"
                                Display="Dynamic" Type="Double" MinimumValue="0" MaximumValue="99999999" ValidationGroup="vgSave" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblSOBillDate4" runat="server" Text="${PSI.Bill.SOBillDate4}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSOBillDate4" runat="server" Text='<%# Bind("SOBillDate4","{0:yyyy-MM-dd}") %>' onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblSupplierBillDate4" runat="server" Text="${PSI.Bill.SupplierBillDate4}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSupplierBillDate4" runat="server" Text='<%# Bind("SupplierBillDate4","{0:yyyy-MM-dd}") %>' onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblSOPayDate4" runat="server" Text="${PSI.Bill.SOPayDate4}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSOPayDate4" runat="server" Text='<%# Bind("SOPayDate4","{0:yyyy-MM-dd}") %>' onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblSupplierPayDate4" runat="server" Text="${PSI.Bill.SupplierPayDate4}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSupplierPayDate4" runat="server" Text='<%# Bind("SupplierPayDate4","{0:yyyy-MM-dd}") %>' onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
                        </td>
                    </tr>



                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlSOAmount3" runat="server" Text="${PSI.Bill.SOAmount3}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSOAmount3" runat="server" Text='<%# Bind("SOAmount3","{0:0.########}") %>' />
                            <asp:RangeValidator ID="rvSOAmount3" runat="server" ControlToValidate="tbSOAmount3" ErrorMessage="${Common.Validator.Valid.Number}"
                                Display="Dynamic" Type="Double" MinimumValue="0" MaximumValue="99999999" ValidationGroup="vgSave" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlSupplierAmount3" runat="server" Text="${PSI.Bill.SupplierAmount3}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSupplierAmount3" runat="server" Text='<%# Bind("SupplierAmount3","{0:0.########}") %>' />
                            <asp:RangeValidator ID="rvSupplierAmount3" runat="server" ControlToValidate="tbSupplierAmount3" ErrorMessage="${Common.Validator.Valid.Number}"
                                Display="Dynamic" Type="Double" MinimumValue="0" MaximumValue="99999999" ValidationGroup="vgSave" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblSOBillDate3" runat="server" Text="${PSI.Bill.SOBillDate3}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSOBillDate3" runat="server" Text='<%# Bind("SOBillDate3","{0:yyyy-MM-dd}") %>' onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />

                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblSupplierBillDate3" runat="server" Text="${PSI.Bill.SupplierBillDate3}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSupplierBillDate3" runat="server" Text='<%# Bind("SupplierBillDate3","{0:yyyy-MM-dd}") %>' onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblSOPayDate3" runat="server" Text="${PSI.Bill.SOPayDate3}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSOPayDate3" runat="server" Text='<%# Bind("SOPayDate3","{0:yyyy-MM-dd}") %>' onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />

                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblSupplierPayDate3" runat="server" Text="${PSI.Bill.SupplierPayDate3}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSupplierPayDate3" runat="server" Text='<%# Bind("SupplierPayDate3","{0:yyyy-MM-dd}") %>' onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlRemark" runat="server" Text="${PSI.Bill.Remark}:" />
                        </td>
                        <td class="td02" colspan="3">
                            <asp:TextBox ID="tbRemark" runat="server" Text='<%# Bind("Remark") %>' TextMode="MultiLine"
                                Height="50" Width="75%" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01"></td>
                        <td class="td02"></td>
                        <td class="td01"></td>
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

            </InsertItemTemplate>
        </asp:FormView>
    </fieldset>
</div>
<asp:ObjectDataSource ID="ODS_Mould" runat="server" TypeName="com.Sconit.Web.MouldMgrProxy"
    DataObjectTypeName="com.Sconit.ISI.Entity.Mould" InsertMethod="CreateMould"
    OnInserted="ODS_Bill_Inserted" OnInserting="ODS_Bill_Inserting">
    <InsertParameters>
        <asp:Parameter Name="SOAmount" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SOAmount1" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SOAmount2" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SOAmount3" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SOBillDate1" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SOBillDate2" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SOBillDate3" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SOPayDate1" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SOPayDate2" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SOPayDate3" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SupplierAmount" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SupplierAmount1" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SupplierAmount2" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SupplierAmount3" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SupplierBillDate1" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SupplierBillDate2" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SupplierBillDate3" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SupplierPayDate1" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SupplierPayDate2" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SupplierPayDate3" Type="DateTime" ConvertEmptyStringToNull="true" />

        <asp:Parameter Name="SOAmount4" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SOBillDate4" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SOPayDate4" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SupplierAmount4" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SupplierBillDate4" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SupplierPayDate4" Type="DateTime" ConvertEmptyStringToNull="true" />

        <asp:Parameter Name="PrjCode" Type="String" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="QS" Type="String" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="Remark" Type="String" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="Customer" Type="String" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="MouldUser" Type="String" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SOUser" Type="String" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="Supplier" Type="String" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="POUser" Type="String" ConvertEmptyStringToNull="true" />
    </InsertParameters>
</asp:ObjectDataSource>
