<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DetailEdit.ascx.cs" Inherits="ISI_Bill_DetailEdit" %>
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
<div id="divFV" runat="server">
    <asp:FormView ID="FV_BillDetail" runat="server" DataSourceID="ODS_MouldDetail"
        DefaultMode="Edit" Width="100%" DataKeyNames="Id" OnDataBound="FV_BillDetail_DataBound">
        <EditItemTemplate>
            <fieldset>
                <legend>${PSI.MouldDetail.UpdateMouldDetail}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlPhase" runat="server" Text="${PSI.MouldDetail.Phase}:" />
                        </td>
                        <td class="td02">
                            <cc1:CodeMstrDropDownList ID="ddlPhase" Code="PSIBillDetailPhase" runat="server" />
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

                        <td class="td01"></td>
                        <td class="td02"></td>

                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlBillDate" runat="server" Text="${PSI.MouldDetail.BillDate}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbBillDate" runat="server" Text='<%# Bind("BillDate","{0:yyyy-MM-dd}") %>' onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlPayDate" runat="server" Text="${PSI.MouldDetail.PayDate}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbPayDate" runat="server" Text='<%# Bind("PayDate","{0:yyyy-MM-dd}") %>' onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlBillAmount" runat="server" Text="${PSI.MouldDetail.BillAmount}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbBillAmount" runat="server" Text='<%# Bind("BillAmount","{0:0.########}") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlPayAmount" runat="server" Text="${PSI.MouldDetail.PayAmount}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbPayAmount" runat="server" Text='<%# Bind("PayAmount","{0:0.########}") %>' />
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
                </table>
                <div id="divMore" style="display: none">
                    <table class="mtable">
                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblCreateDate" runat="server" Text="${Common.Business.CreateDate}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbCreateDate" runat="server" CodeField="CreateDate" CodeFieldFormat="{0:yyyy-MM-dd HH:mm}" DescFieldFormat="{0:yyyy-MM-dd HH:mm}" />
                            </td>
                            <td class="td01">
                                <asp:Literal ID="lblCreateUser" runat="server" Text="${Common.Business.CreateUser}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbCreateUser" runat="server" CodeField="CreateUserNm" />
                            </td>
                        </tr>
                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblLastModifyDate" runat="server" Text="${Common.Business.LastModifyDate}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbLastModifyDate" runat="server" CodeField="LastModifyDate" CodeFieldFormat="{0:yyyy-MM-dd HH:mm}" DescFieldFormat="{0:yyyy-MM-dd HH:mm}" />
                            </td>
                            <td class="td01">
                                <asp:Literal ID="lblLastModifyUser" runat="server" Text="${Common.Business.LastModifyUser}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbLastModifyUser" runat="server" CodeField="LastModifyUserNm" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div>
                    <table class="mtable">
                        <tr>
                            <td class="td01">
                                <a type="text/html" onclick="More()" href="#" visible="true" id="more">More... </a>
                            </td>
                            <td class="td02"></td>
                            <td class="td01"></td>
                            <td class="td02"></td>
                        </tr>
                    </table>
                </div>
                <table class="mtable">
                    <tr>
                        <td class="td01"></td>
                        <td class="td02"></td>
                        <td class="td01"></td>
                        <td class="td02">
                            <div class="buttons">
                                <cc1:Button ID="btnSave" runat="server" CommandName="Update" Text="${Common.Button.Save}"
                                    CssClass="apply" ValidationGroup="vgSave" FunctionId="CreatePSIBillDetail" />
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
<asp:ObjectDataSource ID="ODS_MouldDetail" runat="server" TypeName="com.Sconit.Web.MouldDetailMgrProxy"
    DataObjectTypeName="com.Sconit.ISI.Entity.MouldDetail" UpdateMethod="UpdateMouldDetail"
    OnUpdated="ODS_MouldDetail_Updated" SelectMethod="LoadMouldDetail"
    OnUpdating="ODS_MouldDetail_Updating" DeleteMethod="DeleteMouldDetail"
    OnDeleted="ODS_MouldDetail_Deleted" OnDeleting="ODS_MouldDetail_Deleting">
    <SelectParameters>
        <asp:Parameter Name="Id" Type="Int32" />
    </SelectParameters>
    <UpdateParameters>
        <asp:Parameter Name="PayAmount" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="BillAmount" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="PayDate" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="BillDate" Type="DateTime" ConvertEmptyStringToNull="true" />
    </UpdateParameters>
</asp:ObjectDataSource>
