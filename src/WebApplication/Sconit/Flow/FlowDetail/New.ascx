﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="New.ascx.cs" Inherits="MasterData_FlowDetail_New" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<div id="floatdiv">
    <asp:FormView ID="FV_FlowDetail" runat="server" DataSourceID="ODS_FlowDetail" DefaultMode="Insert"
        DataKeyNames="Id">
        <InsertItemTemplate>
            <fieldset>
                <legend>${MasterData.Flow.FlowDetail.Basic.Info}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblFlow" runat="server" Text="${MasterData.Flow.FlowDetail.Flow}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbFlow" runat="server" ReadOnly="true" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblSeq" runat="server" Text="${MasterData.Flow.FlowDetail.Sequence}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSeq" runat="server" Text='<%# Bind("Sequence") %>' />
                            <asp:RangeValidator ID="rvSeq" runat="server" ControlToValidate="tbSeq" ErrorMessage="${Common.Validator.Valid.Number}"
                                Display="Dynamic" Type="Integer" MinimumValue="0" MaximumValue="100000" ValidationGroup="vgSaveGroup" />
                            <asp:CustomValidator ID="cvSeqCheck" runat="server" ControlToValidate="tbSeq" ErrorMessage="${MasterData.Flow.FlowDetail.Sequence.Exists}"
                                Display="Dynamic" ValidationGroup="vgSaveGroup" OnServerValidate="checkSeqExists" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblItemCode" runat="server" Text="${MasterData.Flow.FlowDetail.ItemCode}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox id="tbItemCode" runat="server" visible="true" width="250" descfield="Description"
                                valuefield="Code" servicepath="ItemMgr.service" servicemethod="GetCacheAllItem"
                                cssclass="inputRequired" />
                            <asp:RequiredFieldValidator ID="rfvItemCode" runat="server" ControlToValidate="tbItemCode"
                                Display="Dynamic" ErrorMessage="${MasterData.Flow.FlowDetail.ItemCode.Required}"
                                ValidationGroup="vgSaveGroup" />
                            <asp:CustomValidator ID="cvItemCheck" runat="server" ControlToValidate="tbItemCode"
                                Display="Dynamic" ValidationGroup="vgSaveGroup" OnServerValidate="checkItemExists" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblUom" runat="server" Text="${MasterData.Flow.FlowDetail.Uom}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox id="tbUom" runat="server" visible="true" width="250" descfield="Description"
                                serviceparameter="string:#tbItemCode" valuefield="Code" servicepath="UomMgr.service"
                                servicemethod="GetItemUom" cssclass="inputRequired" mustmatch="true" />
                            <asp:RequiredFieldValidator ID="rfvUom" runat="server" ControlToValidate="tbUom"
                                Display="Dynamic" ErrorMessage="${MasterData.Flow.FlowDetail.Uom.Required}" ValidationGroup="vgSaveGroup" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblUC" runat="server" Text="${MasterData.Flow.FlowDetail.UnitCount}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbUC" runat="server" Text='<%# Bind("UnitCount","{0:0.########}") %>'
                                CssClass="inputRequired" />
                            <asp:RequiredFieldValidator ID="rfvUC" runat="server" ErrorMessage="${MasterData.Flow.FlowDetail.UnitCount.Required}"
                                Display="Dynamic" ControlToValidate="tbUC" ValidationGroup="vgSaveGroup" />
                            <asp:RangeValidator ID="rvUC" ControlToValidate="tbUC" runat="server" Display="Dynamic"
                                ErrorMessage="${Common.Validator.Valid.Number}" MaximumValue="999999999" MinimumValue="0.00000001"
                                Type="Double" ValidationGroup="vgSaveGroup" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblHuLotSize" runat="server" Text="${MasterData.Flow.FlowDetail.HuLotSize}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbHuLotSize" runat="server" Text='<%# Bind("HuLotSize") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblStartDate" runat="server" Text="${MasterData.Flow.FlowDetail.StartDate}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbStartDate" runat="server" Text='<%# Bind("StartDate") %>' onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblEndDate" runat="server" Text="${MasterData.Flow.FlowDetail.EndDate}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbEndDate" runat="server" Text='<%# Bind("EndDate") %>' onClick="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblSafeStock" runat="server" Text="${MasterData.Flow.FlowDetail.SafeStock}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSafeStock" runat="server" Text='<%# Bind("SafeStock","{0:0.########}") %>' />
                            <asp:RangeValidator ID="rvSafeStock" ControlToValidate="tbSafeStock" runat="server"
                                Display="Dynamic" ErrorMessage="${Common.Validator.Valid.Number}" MaximumValue="999999999"
                                MinimumValue="-999999999" Type="Double" ValidationGroup="vgSaveGroup" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblMaxStock" runat="server" Text="${MasterData.Flow.FlowDetail.MaxStock}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbMaxStock" runat="server" Text='<%# Bind("MaxStock","{0:0.########}") %>' />
                            <asp:RangeValidator ID="rvMaxStock" ControlToValidate="tbMaxStock" runat="server"
                                Display="Dynamic" ErrorMessage="${Common.Validator.Valid.Number}" MaximumValue="999999999"
                                MinimumValue="-999999999" Type="Double" ValidationGroup="vgSaveGroup" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblMinLotSize" runat="server" Text="${MasterData.Flow.FlowDetail.MinLotSize}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbMinLotSize" runat="server" Text='<%# Bind("MinLotSize","{0:0.########}") %>' />
                            <asp:RangeValidator ID="rvShiftStartStock" ControlToValidate="tbMinLotSize" runat="server"
                                Display="Dynamic" ErrorMessage="${Common.Validator.Valid.Number}" MaximumValue="999999999"
                                MinimumValue="0" Type="Double" ValidationGroup="vgSaveGroup" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblOrderLotSize" runat="server" Text="${MasterData.Flow.FlowDetail.OrderLotSize}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbOrderLotSize" runat="server" Text='<%# Bind("OrderLotSize","{0:0.########}") %>' />
                            <asp:RangeValidator ID="rvOrderLotSize" ControlToValidate="tbOrderLotSize" runat="server"
                                Display="Dynamic" ErrorMessage="${Common.Validator.Valid.Number}" MaximumValue="999999999"
                                MinimumValue="0" Type="Double" ValidationGroup="vgSaveGroup" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblOrderGoodsReceiptLotSize" runat="server" Text="${MasterData.Flow.FlowDetail.GoodsReceiptLotSize}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbOrderGoodsReceiptLotSize" runat="server" Text='<%# Bind("GoodsReceiptLotSize","{0:0.########}") %>' />
                            <asp:RangeValidator ID="rvOrderGoodsReceiptLotSize" ControlToValidate="tbOrderGoodsReceiptLotSize"
                                runat="server" Display="Dynamic" ErrorMessage="${Common.Validator.Valid.Number}"
                                MaximumValue="999999999" MinimumValue="0" Type="Double" ValidationGroup="vgSaveGroup" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblRefItemCode" runat="server" Text="${MasterData.Flow.FlowDetail.RefItemCode}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox id="tbRefItemCode" runat="server" visible="true" width="250" descfield="ReferenceCode"
                                valuefield="ReferenceCode" servicepath="ItemReferenceMgr.service" servicemethod="GetItemReference"
                                text='<%# Bind("ReferenceItemCode")%>' />
                        </td>
                    </tr>
                    <tr id="trBom" runat="server" visible="false">
                        <td class="td01">
                            <asp:Literal ID="lblBom" runat="server" Text="${MasterData.Flow.FlowDetail.Bom}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox id="tbBom" runat="server" visible="true" width="250" descfield="Description"
                                valuefield="Code" servicepath="BomMgr.service" servicemethod="GetAllBom" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblBatchSize" runat="server" Text="${MasterData.Flow.FlowDetail.BatchSize}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbBatchSize" runat="server" Text='<%# Bind("BatchSize","{0:0.########}") %>' />
                            <asp:RangeValidator ID="rvBatchSize" ControlToValidate="tbBatchSize" runat="server"
                                Display="Dynamic" ErrorMessage="${Common.Validator.Valid.Number}" MaximumValue="999999999"
                                MinimumValue="0" Type="Double" ValidationGroup="vgSaveGroup" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblPackageVol" runat="server" Text="${MasterData.Flow.FlowDetail.PackageVolumn}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbPackageVol" runat="server" Text='<%# Bind("PackageVolumn","{0:0.########}") %>' />
                            <asp:RangeValidator ID="rvPackageVol" ControlToValidate="tbPackageVol" runat="server"
                                Display="Dynamic" ErrorMessage="${Common.Validator.Valid.Number}" MaximumValue="999999999"
                                MinimumValue="0" Type="Double" ValidationGroup="vgSaveGroup" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblTimeUnit" runat="server" Text="${MasterData.Flow.FlowDetail.TimeUnit}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbTimeUnit" runat="server" Text='<%# Bind("TimeUnit") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblRoundUpOpt" runat="server" Text="${MasterData.Flow.FlowDetail.RoundUpOpt}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbRoundUpOpt" runat="server" Text='<%# Bind("RoundUpOption","{0:0.########}") %>' />
                            <asp:RangeValidator ID="rvRoundUpOpt" ControlToValidate="tbRoundUpOpt" runat="server"
                                Display="Dynamic" ErrorMessage="${Common.Validator.Valid.Number}" MaximumValue="99"
                                MinimumValue="0" Type="Double" ValidationGroup="vgSaveGroup" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblProjectDescription" runat="server" Text="${MasterData.Flow.FlowDetail.ProjectDescription}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbProjectDescription" runat="server" Text='<%# Bind("ProjectDescription") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblIdMark" runat="server" Text="${MasterData.Flow.FlowDetail.IdMark.Procurement}:"
                                Visible="false" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbIdMark" runat="server" Text='<%# Bind("IdMark") %>' Visible="false" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblBarCodeType" runat="server" Text="${MasterData.Flow.FlowDetail.BarTypeCode}:"
                                Visible="false" />
                        </td>
                        <td class="td02">
                            <cc1:codemstrdropdownlist id="ddlBarCodeType" runat="server" visible="false" code="RMBarCodeType" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblCustomer" runat="server" Text="${MasterData.Flow.FlowDetail.Customer}:"
                                Visible="false" />
                        </td>
                        <td class="td02">
                            <uc3:textbox id="tbCustomer" runat="server" width="250" descfield="Name" valuefield="Code"
                                servicepath="PartyMgr.service" servicemethod="GetFromParty" visible="false" mustmatch="true" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblCustomerItemCode" runat="server" Text="${MasterData.Flow.FlowDetail.CustomerItemCode}:"
                                Visible="false" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbCustomerItemCode" runat="server" Text='<%# Bind("CustomerItemCode") %>'
                                Visible="false" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblFormat" runat="server" Text="${MasterData.Flow.FlowDetail.ExtraDmdSource.Format.Label}:" />
                        </td>
                        <td class="td02" colspan="3">
                            <asp:Literal ID="Tips" runat="server" Text="${MasterData.Flow.FlowDetail.ExtraDmdSource.Format}" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblExtraDmdSource" runat="server" Text="${MasterData.Flow.FlowDetail.ExtraDmdSource}:" />
                        </td>
                        <td class="td02" colspan="3">
                            <asp:TextBox ID="tbExtraDmdSource" runat="server" Text='<%# Bind("ExtraDmdSource") %>'
                                Width="600px"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="revExtraDmdSource" runat="server" Display="Dynamic"
                                ValidationExpression="(\b\w*(\||\b))*" ControlToValidate="tbExtraDmdSource" ErrorMessage="${MasterData.Flow.Strategy.WinTime.Correct.Format}"
                                ValidationGroup="vgSaveGroup"></asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblRemark" runat="server" Text="${MasterData.Flow.FlowDetail.Remark}:" />
                        </td>
                        <td class="td02" colspan="3">
                            <asp:TextBox ID="tbRemark" runat="server" Text='<%# Bind("Remark") %>' TextMode="MultiLine"
                                Width="635" />
                        </td>
                    </tr>
                </table>
            </fieldset>
            <%--采购--%>
            <fieldset id="fdProcurement" runat="server" visible="false">
                <legend>${MasterData.Flow.FlowDetail.Default.Value}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblProcurementLocTo" runat="server" Text="${MasterData.Flow.FlowDetail.LocTo}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox id="tbProcurementLocTo" runat="server" visible="true" width="250" descfield="Name"
                                valuefield="Code" servicepath="LocationMgr.service" servicemethod="GetLocationByUserCode" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblProcurementRejectLocTo" runat="server" Text="${MasterData.Flow.RejectLocationTo}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox id="tbProcurementRejectLocTo" runat="server" visible="true" descfield="Name"
                                valuefield="Code" width="250" servicepath="LocationMgr.service" servicemethod="GetLocation" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblProcurementInspectLocTo" runat="server" Text="${MasterData.Flow.InspectLocationTo}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox id="tbProcurementInspectLocTo" runat="server" visible="true" descfield="Name"
                                valuefield="Code" width="250" servicepath="LocationMgr.service" servicemethod="GetLocation" />
                        </td>
                    </tr>
                </table>
            </fieldset>
            <%--销售--%>
            <fieldset id="fdDistribution" runat="server" visible="false">
                <legend>${MasterData.Flow.FlowDetail.Default.Value}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblDistributionLocFrom" runat="server" Text="${MasterData.Flow.FlowDetail.LocFrom}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox id="tbDistributionLocFrom" runat="server" visible="true" width="250"
                                descfield="Name" valuefield="Code" servicepath="LocationMgr.service" servicemethod="GetLocation" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblDistributionRejectLocFrom" runat="server" Text="${MasterData.Flow.RejectLocationFrom}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox id="tbDistributionRejectLocFrom" runat="server" visible="true" descfield="Name"
                                valuefield="Code" width="250" servicepath="LocationMgr.service" servicemethod="GetLocation" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlNeedRejInspect" runat="server" Text="${MasterData.Inspection.NeedRejInspect}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="cbNeedRejInspect" runat="server" Checked='<%# Bind("NeedRejectInspection") %>' />
                        </td>
                        <td class="td01">
                        </td>
                        <td class="td02">
                        </td>
                    </tr>
                </table>
            </fieldset>
            <%--生产--%>
            <fieldset id="fdProduction" runat="server" visible="false">
                <legend>${MasterData.Flow.FlowDetail.Default.Value}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblProductionLocFrom" runat="server" Text="${MasterData.Flow.FlowDetail.LocFrom.Production}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox id="tbProductionLocFrom" runat="server" visible="true" width="250" descfield="Name"
                                valuefield="Code" servicepath="LocationMgr.service" servicemethod="GetLocationByUserCode" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblProductionLocTo" runat="server" Text="${MasterData.Flow.FlowDetail.LocTo.Production}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox id="tbProductionLocTo" runat="server" visible="true" width="250" descfield="Name"
                                valuefield="Code" servicepath="LocationMgr.service" servicemethod="GetLocationByUserCode" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblInspectLocationFrom" runat="server" Text="${MasterData.Flow.InspectLocationFrom}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox id="tbInspectLocationFrom" runat="server" visible="true" descfield="Name"
                                valuefield="Code" width="250" servicepath="LocationMgr.service" servicemethod="GetLocation" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblInspectLocationTo" runat="server" Text="${MasterData.Flow.InspectLocationTo}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox id="tbInspectLocationTo" runat="server" visible="true" descfield="Name"
                                valuefield="Code" width="250" servicepath="LocationMgr.service" servicemethod="GetLocation" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblRejectLocationFrom" runat="server" Text="${MasterData.Flow.RejectLocationFrom}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox id="tbRejectLocationFrom" runat="server" visible="true" descfield="Name"
                                valuefield="Code" width="250" servicepath="LocationMgr.service" servicemethod="GetLocation" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblRejectLocationTo" runat="server" Text="${MasterData.Flow.RejectLocationTo}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox id="tbRejectLocationTo" runat="server" visible="true" descfield="Name"
                                valuefield="Code" width="250" servicepath="LocationMgr.service" servicemethod="GetLocation" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblRouting" runat="server" Text="${Menu.MasterData.Routing}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox id="tbRouting" runat="server" visible="true" width="250" descfield="Description"
                                valuefield="Code" servicepath="RoutingMgr.service" servicemethod="GetRouting" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblReturnRouting" runat="server" Text="${MasterData.Flow.ReturnRouting}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox id="tbReturnRouting" runat="server" visible="true" width="250" descfield="Description"
                                valuefield="Code" servicepath="RoutingMgr.service" servicemethod="GetRouting" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="Literal1" runat="server" Text="返工检验:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="CheckBox1" runat="server" Checked='<%# Bind("NeedRejectInspection") %>' />
                        </td>
                        <td class="td01">
                        </td>
                        <td class="td02">
                        </td>
                    </tr>
                </table>
            </fieldset>
            <%--移库--%>
            <fieldset id="fdTransfer" runat="server" visible="false">
                <legend>${MasterData.Flow.FlowDetail.Default.Value}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblTransferLocFrom" runat="server" Text="${MasterData.Flow.FlowDetail.LocFrom}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox id="tbTransferLocFrom" runat="server" visible="true" width="250" descfield="Name"
                                valuefield="Code" servicepath="LocationMgr.service" servicemethod="GetLocation" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblTransferLocTo" runat="server" Text="${MasterData.Flow.FlowDetail.LocTo}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox id="tbTransferLocTo" runat="server" visible="true" width="250" descfield="Name"
                                valuefield="Code" servicepath="LocationMgr.service" servicemethod="GetLocation" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblTransferRejectLocFrom" runat="server" Text="${MasterData.Flow.RejectLocationFrom}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox id="tbTransferRejectLocFrom" runat="server" visible="true" descfield="Name"
                                valuefield="Code" width="250" servicepath="LocationMgr.service" servicemethod="GetLocation" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblTransferRejectLocTo" runat="server" Text="${MasterData.Flow.RejectLocationTo}:" />
                        </td>
                        <td class="td02">
                            <uc3:textbox id="tbTransferRejectLocTo" runat="server" visible="true" descfield="Name"
                                valuefield="Code" width="250" servicepath="LocationMgr.service" servicemethod="GetLocation" />
                        </td>
                    </tr>
                </table>
            </fieldset>
            <fieldset id="fdOption" runat="server">
                <legend>${MasterData.Flow.FlowDetail.Control.Option}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblIsAutoCreate" runat="server" Text="${MasterData.Flow.FlowDetail.AutoCreate}:" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="cbIsAutoCreate" runat="server" Checked='<%# Bind("IsAutoCreate") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblNeedInspect" runat="server" Text="${MasterData.Flow.FlowDetail.NeedInspect}:"
                                Visible="false" />
                        </td>
                        <td class="td02">
                            <asp:CheckBox ID="cbNeedInspect" runat="server" Checked='<%# Bind("NeedInspection") %>'
                                Visible="false" />
                        </td>
                    </tr>
                    <tr>
                        <td class="ttd01">
                            <asp:Literal ID="lblBillSettleTerm" runat="server" Text="${MasterData.Flow.FlowDetail.BillSettleTerm}:"
                                Visible="false" />
                        </td>
                        <td class="ttd02">
                            <asp:DropDownList ID="ddlBillSettleTerm" runat="server" DataTextField="Description"
                                DataValueField="Value" Visible="false" />
                        </td>
                        <td class="ttd01">
                        </td>
                        <td class="ttd02">
                        </td>
                    </tr>
                    <tr>
                        <td class="ttd01">
                            <asp:Literal ID="lblOddShipOption" runat="server" Text="${MasterData.Flow.FlowDetail.OddShipOption}:"
                                Visible="false" />
                        </td>
                        <td class="ttd02">
                            <cc1:codemstrdropdownlist id="ddlOddShipOption" code="OddShipOption" runat="server"
                                visible="false">
                            </cc1:codemstrdropdownlist>
                        </td>
                        <td class="ttd01">
                            <asp:Literal ID="lblMRPWeight" runat="server" Text="${MasterData.Flow.FlowDetail.MRPWeight}:" />
                        </td>
                        <td class="ttd02">
                            <asp:TextBox ID="tbMRPWeight" runat="server" Text='<%# Bind("MRPWeight") %>' />
                            <asp:RangeValidator ID="rvMRPWeight" ControlToValidate="tbMRPWeight" runat="server"
                                Display="Dynamic" ErrorMessage="${Common.Validator.Valid.Number}" Type="Integer"
                                MinimumValue="0" MaximumValue="100000" ValidationGroup="vgSaveGroup" />
                        </td>
                    </tr>
                </table>
            </fieldset>
            <div class="tablefooter">
                <asp:Button ID="btnInsert" runat="server" CommandName="Insert" Text="${Common.Button.Save}"
                    CssClass="button2" ValidationGroup="vgSaveGroup" />
                <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                    CssClass="button2" />
            </div>
        </InsertItemTemplate>
    </asp:FormView>
</div>
<asp:ObjectDataSource ID="ODS_FlowDetail" runat="server" TypeName="com.Sconit.Web.FlowDetailMgrProxy"
    DataObjectTypeName="com.Sconit.Entity.MasterData.FlowDetail" InsertMethod="CreateFlowDetail"
    OnInserted="ODS_FlowDetail_Inserted" OnInserting="ODS_FlowDetail_Inserting" SelectMethod="FindFlowDetail">
    <SelectParameters>
        <asp:Parameter Name="id" Type="Int32" />
    </SelectParameters>
    <InsertParameters>
        <asp:Parameter Name="SafeStock" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="MaxStock" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="MinLotSize" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="OrderLotSize" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="GoodsReceiptLotSize" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="BatchSize" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="HuLotSize" Type="Int32" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="PackageVolumn" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="StartDate" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="EndDate" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="Sequence" Type="Int32" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="MRPWeight" Type="Int32" ConvertEmptyStringToNull="true" />
    </InsertParameters>
</asp:ObjectDataSource>
