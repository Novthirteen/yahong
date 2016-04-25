<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DetailNew.ascx.cs" Inherits="ISI_Bill_DetailNew" %>
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
    <asp:FormView ID="FV_BillDetail" runat="server" DataSourceID="ODS_MouldDetail"
        DefaultMode="Insert" Width="100%" DataKeyNames="Id" OnDataBound="FV_BillDetail_DataBound">
        <InsertItemTemplate>
            <fieldset>
                <legend>${PSI.MouldDetail.NewMouldDetail}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlPhase" runat="server" Text="${PSI.MouldDetail.Phase}:" />
                        </td>
                        <td class="td02">
                            <asp:Literal ID="lblPhase" runat="server" Text="" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlType" runat="server" Text="${PSI.MouldDetail.Type}:" />
                        </td>
                        <td class="td02">
                            <cc1:CodeMstrDropDownList ID="ddlType" Code="PSIBillDetailType" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlInvoice" runat="server" Text="${PSI.MouldDetail.Invoice}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbInvoice" runat="server" Text='<%# Bind("Invoice") %>' />
                        </td>
                        <td class="td01">
                           
                        </td>
                        <td class="td02">
                           
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlBillDate" runat="server" Text="${PSI.MouldDetail.BillDate}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbBillDate" runat="server" Text='<%# Bind("BillDate") %>' onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlPayDate" runat="server" Text="${PSI.MouldDetail.PayDate}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbPayDate" runat="server" Text='<%# Bind("PayDate") %>' onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlBillAmount" runat="server" Text="${PSI.MouldDetail.BillAmount}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbBillAmount" runat="server" Text='<%# Bind("BillAmount","0.###") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlPayAmount" runat="server" Text="${PSI.MouldDetail.PayAmount}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbPayAmount" runat="server" Text='<%# Bind("PayAmount") %>' />
                        </td>
                    </tr>

                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlRemark" runat="server" Text="${PSI.MouldDetail.Remark}:" />
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
                                <cc1:Button ID="btnInsert" runat="server" CommandName="Insert" Text="${Common.Button.Save}"
                                    CssClass="apply" ValidationGroup="vgSave" FunctionId="CreatePSIBillDetail" />
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
<asp:ObjectDataSource ID="ODS_MouldDetail" runat="server" TypeName="com.Sconit.Web.MouldDetailMgrProxy"
    DataObjectTypeName="com.Sconit.ISI.Entity.MouldDetail" InsertMethod="CreateMouldDetail"
    OnInserted="ODS_MouldDetail_Inserted" OnInserting="ODS_MouldDetail_Inserting">
    <InsertParameters>
        <asp:Parameter Name="PayAmount" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="BillAmount" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="PayDate" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="BillDate" Type="DateTime" ConvertEmptyStringToNull="true" />
    </InsertParameters>
</asp:ObjectDataSource>
