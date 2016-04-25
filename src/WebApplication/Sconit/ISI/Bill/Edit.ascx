<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Edit.ascx.cs" Inherits="ISI_Bill_Edit" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<link rel="stylesheet" type="text/css" href="Js/jquery.ui/jquery-ui.min.css" />
<script language="javascript" type="text/javascript" src="Js/ui.core-min.js"></script>
<script language="javascript" type="text/javascript" src="Js/uploadify/jquery.uploadify.min.js?ver=<%=(new Random()).Next(0, 99999).ToString() %>"></script>
<link rel="stylesheet" type="text/css" href="Js/uploadify/uploadify.css" />
<style type="text/css">
    .link {
        text-decoration: underline;
        color: blue;
        cursor: pointer;
    }
</style>
<script language="javascript" type="text/javascript">

    function TaskUploadify() {
        //$("input[type='file']").uploadify({
        //$("#ctl01_ucList_GV_List_ctl02_uploadify").uploadify({
        $("#uploadify").uploadify({
            'debug': false, //开启调试    
            'auto': true,  //是否自动上传    
            'buttonText': '${ISI.Status.SelectFiles}',  //按钮上的文字
            'swf': "/JS/uploadify/uploadify.swf",//flash      
            'queueID': 'uploadfileQueue',  //文件选择后的容器ID  
            'uploader': '/ISI/Attachment/FileHandler.ashx',  //后台action  
            'width': '50',  //按钮宽度  
            'height': '20',  //按钮高度  
            'multi': true,  //是否支持多文件上传，默认就是true  
            'fileObjName': 'fileData',//后台接收的参数，如：使用struts2上传时，后台有get,set方法的接收参数  
            'fileTypeDesc': '<%=this.FileExtensions%>',//  可选择文件类型说明  
            'fileTypeExts': '<%=this.FileExtensions%>',  //允许上传文件的类型
            'fileSizeLimit': <%=this.ContentLength%> * 1024 * 1024 ,  //文件上传的最大大小
            'queueSizeLimit': 50,
            'formData': { 'TaskCode': '<%=this.MouldCode%>', 'UserCode': '<%=this.CurrentUser.Code%>', 'UserName': '<%=this.CurrentUser.Name%>', 'PSIType': '<%= typeof(com.Sconit.ISI.Entity.Mould).FullName%>' },
            'removeTimeout': 1,
            //返回一个错误，选择文件的时候触发    
            'onSelectError': function (file, errorCode, errorMsg) {
                switch (errorCode) {
                    case -100:
                        $("#uploadfileQueue").text("${ISI.TSK.UploadTheFileNumberIsBeyondSystemOfFile}" + $('#uploadify').uploadify('settings', 'queueSizeLimit'));
                        break;
                    case -110:
                        $("#uploadfileQueue").text(file.name + "${ISI.TSK.Error.FileSize}" + $('#uploadify').uploadify('settings', 'fileSizeLimit'));
                        break;
                    case -120:
                        $("#uploadfileQueue").text(file.name + "${ISI.TSK.Error.FileSizeException}");
                        break;
                    case -130:
                        $("#uploadfileQueue").text(file.name + "${ISI.TSK.Error.FileType}");
                        break;
                }
            },
            //检测FLASH失败调用    
            'onFallback': function () {
                alert("${ISI.TSK.NoFlash}");
            },
            'onSelect': function (file) {
                //$("#uploadfileQueue").append("正在上传...");
                //$("#uploadfileQueue").show();
            },
            //上传到服务器，服务器返回相应信息到data里    
            'onUploadSuccess': function (file, data, response) {
                //var json = eval("(" + data + ")");
                //如需上传后生成预览，可在此操作 
                //$("#uploadfileQueue").append('<br>' +  ' - 索引: ' + file.index + ' - 文件名: ' + file.name + ' 上传成功');
            },
            'onQueueComplete': function (queueData) { //队列里所有的文件处理完成后调用 
                $("#uploadfileQueue").text('');
                $("#uploadfileQueue").append(queueData.uploadsSuccessful + "${ISI.TSK.HasBeenUploadedSuccessfully}").fadeIn(300).delay(2500).fadeOut(300);// 这个是渐渐消失
            }
        });
        $("#lblFile").hide();
        $("#uploadify").fadeIn(100);
    }

    $(document).ready(function () {

        //TaskUploadify();
        $('textarea').tah({
            moreSpace: 25,   // 输入框底部预留的空白, 默认15, 单位像素
            maxHeight: 260,  // 指定Textarea的最大高度, 默认600, 单位像素
            animateDur: 200  // 调整高度时的动画过渡时间, 默认200, 单位毫秒
        });
    });

</script>
<div>
    <fieldset>
        <legend runat="server" id="lgd"></legend>
        <asp:FormView ID="FV_Bill" runat="server" DataSourceID="ODS_Mould"
            DefaultMode="Edit" Width="100%" DataKeyNames="Code" OnDataBound="FV_Bill_DataBound">
            <EditItemTemplate>

                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlCode" runat="server" Text="${PSI.Bill.Code}:" />
                        </td>
                        <td class="td02">
                            <asp:Label ID="lblCode" runat="server" Text='<%# Bind("Code") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblStatus" runat="server" Text="${PSI.Bill.Status}:" />
                        </td>
                        <td class="td02">
                            <asp:Label ID="tbStatusDesc" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlPrjCode" runat="server" Text="${PSI.Bill.PrjCode}:" />
                        </td>
                        <td class="td02">
                            <asp:Label ID="lblPrjCode" runat="server" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlFCID" runat="server" Text="${PSI.Bill.FCID}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbFCID" runat="server" Text='<%# Bind("FCID") %>' ReadOnly="true" />
                        </td>
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
                                ValueField="Code" ServicePath="UserSubscriptionMgr.service" ServiceMethod="GetCacheAllUser" Width="250" Text='<%# Bind("POUser") %>' />

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
                        <td colspan="4">
                            <hr />
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
                            <asp:Literal ID="lblPOAmount" runat="server" Text="${PSI.Bill.POAmount}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbPOAmount" runat="server" Text='<%# Bind("POAmount","{0:0.########}") %>' ReadOnly="true" />
                        </td>
                        <td class="td01"></td>
                        <td class="td02"></td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlSOBilledAmount" runat="server" Text="${PSI.Bill.SOBilledAmount}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSOBilledAmount" ReadOnly="true" runat="server" Text='<%# Bind("SOBilledAmount","{0:0.########}") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlSupplierBilledAmount" runat="server" Text="${PSI.Bill.SupplierBilledAmount}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSupplierBilledAmount" ReadOnly="true" runat="server" Text='<%# Bind("SupplierBilledAmount","{0:0.########}") %>' />

                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblPOBilledAmount" runat="server" Text="${PSI.Bill.POBilledAmount}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbPOBilledAmount" runat="server" Text='<%# Bind("POBilledAmount","{0:0.########}") %>' ReadOnly="true" />
                        </td>
                        <td class="td01"></td>
                        <td class="td02"></td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlSOPayAmount" runat="server" Text="${PSI.Bill.SOPayAmount}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSOPayAmount" ReadOnly="true" runat="server" Text='<%# Bind("SOPayAmount","{0:0.########}") %>' />

                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlSupplierPayAmount" runat="server" Text="${PSI.Bill.SupplierPayAmount}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSupplierPayAmount" ReadOnly="true" runat="server" Text='<%# Bind("SupplierPayAmount","{0:0.########}") %>' />

                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblPOPayAmount" runat="server" Text="${PSI.Bill.POPayAmount}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbPOPayAmount" runat="server" Text='<%# Bind("POPayAmount","{0:0.########}") %>' ReadOnly="true" />
                        </td>
                        <td class="td01"></td>
                        <td class="td02"></td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <hr />
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
                            <asp:Literal ID="lblPOAmount1" runat="server" Text="${PSI.Bill.POAmount1}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbPOAmount1" runat="server" Text='<%# Bind("POAmount1","{0:0.########}") %>' ReadOnly="true" />
                        </td>
                        <td class="td01"></td>
                        <td class="td02"></td>
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
                            <asp:Literal ID="ltlSOBilledAmount1" runat="server" Text="${PSI.Bill.SOBilledAmount1}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSOBilledAmount1" ReadOnly="true" runat="server" Text='<%# Bind("SOBilledAmount1","{0:0.########}") %>' />

                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlSupplierBilledAmount1" runat="server" Text="${PSI.Bill.SupplierBilledAmount1}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSupplierBilledAmount1" ReadOnly="true" runat="server" Text='<%# Bind("SupplierBilledAmount1","{0:0.########}") %>' />

                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblPOBilledAmount1" runat="server" Text="${PSI.Bill.POBilledAmount1}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbPOBilledAmount1" runat="server" Text='<%# Bind("POBilledAmount1","{0:0.########}") %>' ReadOnly="true" />
                        </td>
                        <td class="td01"></td>
                        <td class="td02"></td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlSOPayAmount1" runat="server" Text="${PSI.Bill.SOPayAmount1}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSOPayAmount1" ReadOnly="true" runat="server" Text='<%# Bind("SOPayAmount1","{0:0.########}") %>' />

                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlSupplierPayAmount1" runat="server" Text="${PSI.Bill.SupplierPayAmount1}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSupplierPayAmount1" ReadOnly="true" runat="server" Text='<%# Bind("SupplierPayAmount1","{0:0.########}") %>' />

                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblPOPayAmount1" runat="server" Text="${PSI.Bill.POPayAmount1}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbPOPayAmount1" runat="server" Text='<%# Bind("POPayAmount1","{0:0.########}") %>' ReadOnly="true" />
                        </td>
                        <td class="td01"></td>
                        <td class="td02">
                            <cc1:Button ID="btnAmount1" runat="server" Text="${PSI.Bill.Button.Amount1}"
                                CssClass="apply" OnClick="btnMouldDetail_Click" FunctionId="CreatePSIBillDetail" CommandArgument="1" /></td>
                    </tr>

                    <tr>
                        <td colspan="4">
                            <hr />
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
                            <asp:Literal ID="lblPOAmount2" runat="server" Text="${PSI.Bill.POAmount2}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbPOAmount2" runat="server" Text='<%# Bind("POAmount2","{0:0.########}") %>' ReadOnly="true" />
                        </td>
                        <td class="td01"></td>
                        <td class="td02"></td>
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
                            <asp:Literal ID="ltlSOBilledAmount2" runat="server" Text="${PSI.Bill.SOBilledAmount2}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSOBilledAmount2" ReadOnly="true" runat="server" Text='<%# Bind("SOBilledAmount2","{0:0.########}") %>' />

                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlSupplierBilledAmount2" runat="server" Text="${PSI.Bill.SupplierBilledAmount2}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSupplierBilledAmount2" ReadOnly="true" runat="server" Text='<%# Bind("SupplierBilledAmount2","{0:0.########}") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblPOBilledAmount2" runat="server" Text="${PSI.Bill.POBilledAmount2}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbPOBilledAmount2" runat="server" Text='<%# Bind("POBilledAmount2","{0:0.########}") %>' ReadOnly="true" />
                        </td>
                        <td class="td01"></td>
                        <td class="td02"></td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlSOPayAmount2" runat="server" Text="${PSI.Bill.SOPayAmount2}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSOPayAmount2" ReadOnly="true" runat="server" Text='<%# Bind("SOPayAmount2","{0:0.########}") %>' />

                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlSupplierPayAmount2" runat="server" Text="${PSI.Bill.SupplierPayAmount2}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSupplierPayAmount2" ReadOnly="true" runat="server" Text='<%# Bind("SupplierPayAmount2","{0:0.########}") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblPOPayAmount2" runat="server" Text="${PSI.Bill.POPayAmount2}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbPOPayAmount2" runat="server" Text='<%# Bind("POPayAmount2","{0:0.########}") %>' ReadOnly="true" />
                        </td>
                        <td class="td01"></td>
                        <td class="td02">
                            <cc1:Button ID="btnAmount2" runat="server" Text="${PSI.Bill.Button.Amount2}"
                                CssClass="apply" OnClick="btnMouldDetail_Click" FunctionId="CreatePSIBillDetail" CommandArgument="2" /></td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <hr />
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
                            <asp:Literal ID="lblPOAmount4" runat="server" Text="${PSI.Bill.POAmount4}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbPOAmount4" runat="server" Text='<%# Bind("POAmount4","{0:0.########}") %>' ReadOnly="true" />
                        </td>
                        <td class="td01"></td>
                        <td class="td02"></td>
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
                            <asp:Literal ID="ltlSOBilledAmount4" runat="server" Text="${PSI.Bill.SOBilledAmount4}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSOBilledAmount4" ReadOnly="true" runat="server" Text='<%# Bind("SOBilledAmount4","{0:0.########}") %>' />

                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlSupplierBilledAmount4" runat="server" Text="${PSI.Bill.SupplierBilledAmount4}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSupplierBilledAmount4" ReadOnly="true" runat="server" Text='<%# Bind("SupplierBilledAmount4","{0:0.########}") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblPOBilledAmount4" runat="server" Text="${PSI.Bill.POBilledAmount4}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbPOBilledAmount4" runat="server" Text='<%# Bind("POBilledAmount4","{0:0.########}") %>' ReadOnly="true" />
                        </td>
                        <td class="td01"></td>
                        <td class="td02"></td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlSOPayAmount4" runat="server" Text="${PSI.Bill.SOPayAmount4}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSOPayAmount4" ReadOnly="true" runat="server" Text='<%# Bind("SOPayAmount4","{0:0.########}") %>' />

                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlSupplierPayAmount4" runat="server" Text="${PSI.Bill.SupplierPayAmount4}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSupplierPayAmount4" ReadOnly="true" runat="server" Text='<%# Bind("SupplierPayAmount4","{0:0.########}") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblPOPayAmount4" runat="server" Text="${PSI.Bill.POPayAmount4}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbPOPayAmount4" runat="server" Text='<%# Bind("POPayAmount4","{0:0.########}") %>' ReadOnly="true" />
                        </td>
                        <td class="td01"></td>
                        <td class="td02">
                            <cc1:Button ID="btnAmount4" runat="server" Text="${PSI.Bill.Button.Amount4}"
                                CssClass="apply" OnClick="btnMouldDetail_Click" FunctionId="CreatePSIBillDetail" CommandArgument="4" /></td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <hr />
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
                            <asp:Literal ID="ltlPOAmount3" runat="server" Text="${PSI.Bill.POAmount3}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSupplierAmount3" runat="server" Text='<%# Bind("SupplierAmount3","{0:0.########}") %>' />
                            <asp:RangeValidator ID="rvSupplierAmount3" runat="server" ControlToValidate="tbSupplierAmount3" ErrorMessage="${Common.Validator.Valid.Number}"
                                Display="Dynamic" Type="Double" MinimumValue="0" MaximumValue="99999999" ValidationGroup="vgSave" />

                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblPOAmount3" runat="server" Text="${PSI.Bill.POAmount3}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbPOAmount3" runat="server" Text='<%# Bind("POAmount3","{0:0.########}") %>' ReadOnly="true" />
                        </td>
                        <td class="td01"></td>
                        <td class="td02"></td>
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
                            <asp:Literal ID="ltlSOBilledAmount3" runat="server" Text="${PSI.Bill.SOBilledAmount3}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSOBilledAmount3" ReadOnly="true" runat="server" Text='<%# Bind("SOBilledAmount3","{0:0.########}") %>' />

                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlSupplierBilledAmount3" runat="server" Text="${PSI.Bill.SupplierBilledAmount3}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSupplierBilledAmount3" ReadOnly="true" runat="server" Text='<%# Bind("SupplierBilledAmount3","{0:0.########}") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblPOBilledAmount3" runat="server" Text="${PSI.Bill.POBilledAmount3}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbPOBilledAmount3" runat="server" Text='<%# Bind("POBilledAmount3","{0:0.########}") %>' ReadOnly="true" />
                        </td>
                        <td class="td01"></td>
                        <td class="td02"></td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlSOPayAmount3" runat="server" Text="${PSI.Bill.SOPayAmount3}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSOPayAmount3" ReadOnly="true" runat="server" Text='<%# Bind("SOPayAmount3","{0:0.########}") %>' />

                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlSupplierPayAmount3" runat="server" Text="${PSI.Bill.SupplierPayAmount3}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSupplierPayAmount3" ReadOnly="true" runat="server" Text='<%# Bind("SupplierPayAmount3","{0:0.########}") %>' />

                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblPOPayAmount3" runat="server" Text="${PSI.Bill.POPayAmount3}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbPOPayAmount3" runat="server" Text='<%# Bind("POPayAmount3","{0:0.########}") %>' ReadOnly="true" />
                        </td>
                        <td class="td01"></td>
                        <td class="td02">
                            <cc1:Button ID="btnAmount3" runat="server" Text="${PSI.Bill.Button.Amount3}"
                                CssClass="apply" OnClick="btnMouldDetail_Click" FunctionId="CreatePSIBillDetail" CommandArgument="3" /></td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <hr />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblRemark" runat="server" Text="${PSI.Bill.Remark}:" />
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
                                <asp:Literal ID="lblSOCompleteDate" runat="server" Text="${PSI.Bill.SOCompleteDate}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbSOCompleteDate" runat="server" CodeField="SOCompleteDate" CodeFieldFormat="{0:yyyy-MM-dd HH:mm}" DescFieldFormat="{0:yyyy-MM-dd HH:mm}" />
                            </td>
                            <td class="td01">
                                <asp:Literal ID="lblSOCompleteUser" runat="server" Text="${PSI.Bill.SOCompleteUser}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbSOCompleteUser" runat="server" CodeField="SOCompleteUserNm" />
                            </td>
                        </tr>
                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblPOCompleteDate" runat="server" Text="${PSI.Bill.POCompleteDate}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbPOCompleteDate" runat="server" CodeField="POCompleteDate" CodeFieldFormat="{0:yyyy-MM-dd HH:mm}" DescFieldFormat="{0:yyyy-MM-dd HH:mm}" />
                            </td>
                            <td class="td01">
                                <asp:Literal ID="lblPOCompleteUser" runat="server" Text="${PSI.Bill.POCompleteUser}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbPOCompleteUser" runat="server" CodeField="POCompleteUserNm" />
                            </td>
                        </tr>
                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblCloseDate" runat="server" Text="${PSI.Bill.CloseDate}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbCloseDate" runat="server" CodeField="CloseDate" CodeFieldFormat="{0:yyyy-MM-dd HH:mm}" DescFieldFormat="{0:yyyy-MM-dd HH:mm}" />
                            </td>
                            <td class="td01">
                                <asp:Literal ID="lblCloseUser" runat="server" Text="${PSI.Bill.CloseUser}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbCloseUser" runat="server" CodeField="CloseUserNm" />
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
                <div id="uploadfileQueue" name="uploadfileQueue" align="center"></div>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <span class="link" id="lblFile" name="lblFile" onclick="TaskUploadify()">${ISI.Status.Upload}</span>
                            <input type="file" id="uploadify" name="uploadify" style="display: none;" />
                        </td>
                        <td class="td02"></td>
                        <td class="td01"></td>
                        <td class="td02">
                            <div class="buttons">
                                <cc1:Button ID="btnSave" runat="server" CommandName="Update" Text="${Common.Button.Save}"
                                    CssClass="apply" ValidationGroup="vgSave" FunctionId="SavePSIBill" />
                                <cc1:Button ID="btnCopy" runat="server" Text="${Common.Button.Copy}"
                                    OnClick="btnCopy_Click" ValidationGroup="vgSave" FunctionId="SavePSIBill" />
                                <cc1:Button ID="btnDelete" runat="server" Text="${Common.Button.Delete}" CssClass="apply"
                                    OnClick="btnDelete_Click" FunctionId="DeletePSIBill" />
                                <cc1:Button ID="btnPOComplete" runat="server" Text="${PSI.Bill.Button.POComplete}"
                                    CssClass="apply" OnClick="btnPOComplete_Click" FunctionId="POComplete" />
                                <cc1:Button ID="btnSOComplete" runat="server" Text="${PSI.Bill.Button.SOComplete}"
                                    CssClass="apply" OnClick="btnSOComplete_Click" FunctionId="SOComplete" />
                                <cc1:Button ID="btnClose" runat="server" Text="${Common.Button.Close}" CssClass="apply"
                                    OnClick="btnClose_Click" FunctionId="ClosePSIBill" />
                                <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                                    CssClass="back" />
                            </div>
                        </td>
                    </tr>
                </table>

            </EditItemTemplate>
        </asp:FormView>
    </fieldset>
</div>
<asp:ObjectDataSource ID="ODS_Mould" runat="server" TypeName="com.Sconit.Web.MouldMgrProxy"
    DataObjectTypeName="com.Sconit.ISI.Entity.Mould" UpdateMethod="UpdateMould"
    OnUpdated="ODS_Bill_Updated" SelectMethod="LoadMould"
    OnUpdating="ODS_Bill_Updating" DeleteMethod="DeleteMould"
    OnDeleted="ODS_Bill_Deleted" OnDeleting="ODS_Bill_Deleting">
    <SelectParameters>
        <asp:Parameter Name="Code" Type="String" />
    </SelectParameters>
    <UpdateParameters>
        <asp:Parameter Name="SOAmount" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SOAmount1" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SOAmount2" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SOAmount4" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SOAmount3" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SOPayAmount" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SOPayAmount1" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SOPayAmount2" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SOPayAmount4" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SOPayAmount3" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SOBilledAmount" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SOBilledAmount1" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SOBilledAmount2" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SOBilledAmount4" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SOBilledAmount3" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SOBillDate1" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SOBillDate2" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SOBillDate4" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SOBillDate3" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SOPayDate1" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SOPayDate2" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SOPayDate4" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SOPayDate3" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SupplierAmount" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SupplierAmount1" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SupplierAmount2" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SupplierAmount4" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SupplierAmount3" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SupplierPayAmount" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SupplierPayAmount1" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SupplierPayAmount2" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SupplierPayAmount4" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SupplierPayAmount3" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SupplierBilledAmount" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SupplierBilledAmount1" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SupplierBilledAmount2" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SupplierBilledAmount4" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SupplierBilledAmount3" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SupplierBillDate1" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SupplierBillDate2" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SupplierBillDate4" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SupplierBillDate3" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SupplierPayDate1" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SupplierPayDate2" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SupplierPayDate4" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SupplierPayDate3" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="POAmount" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="POAmount1" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="POAmount2" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="POAmount4" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="POAmount3" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="POPayAmount" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="POPayAmount1" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="POPayAmount2" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="POPayAmount4" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="POPayAmount3" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="POBilledAmount" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="POBilledAmount1" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="POBilledAmount2" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="POBilledAmount4" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="POBilledAmount3" Type="Decimal" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="PrjCode" Type="String" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="QS" Type="String" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="Remark" Type="String" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="Customer" Type="String" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="SOUser" Type="String" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="Supplier" Type="String" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="POUser" Type="String" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="MouldUser" Type="String" ConvertEmptyStringToNull="true" />
    </UpdateParameters>
</asp:ObjectDataSource>
<div runat="server" id="mouldDetailDiv">
    <fieldset>
        <legend>${Common.ListFormat.Detail}</legend>
        <asp:GridView ID="GV_List" runat="server" OnRowDataBound="GV_List_RowDataBound" OnDataBound="GV_List_DataBound" DataKeyNames="Id"
            AutoGenerateColumns="false" DefaultSortDirection="Descending" ShowSeqNo="true">
            <Columns>
                <asp:TemplateField HeaderText="${PSI.MouldDetail.Type}" SortExpression="Type">
                    <ItemTemplate>
                        <asp:Label ID="lblType" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Phase" HeaderText="${PSI.MouldDetail.Phase}"
                    SortExpression="Phase" />
                <asp:BoundField DataField="Invoice" HeaderText="${PSI.MouldDetail.Invoice}"
                    SortExpression="Invoice" />
                <asp:BoundField DataField="BillDate" HeaderText="${PSI.MouldDetail.BillDate}"
                    SortExpression="BillDate" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="BillAmount" HeaderText="${PSI.MouldDetail.BillAmount}"
                    SortExpression="BillAmount" DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="PayDate" HeaderText="${PSI.MouldDetail.PayDate}"
                    SortExpression="PayDate" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="PayAmount" HeaderText="${PSI.MouldDetail.PayAmount}"
                    SortExpression="PayAmount" DataFormatString="{0:0.###}" />
                <asp:BoundField DataField="CreateDate" HeaderText="${Common.Business.CreateDate}"
                    SortExpression="CreateDate" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                <asp:BoundField DataField="CreateUserNm" HeaderText="${Common.Business.CreateUser}"
                    SortExpression="CreateUser" />
                <asp:BoundField DataField="Remark" HeaderText="${PSI.MouldDetail.Remark}"
                    SortExpression="Remark" />
                <asp:TemplateField HeaderText="${Common.GridView.Action}">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnEdit" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                            Text="${Common.Button.Edit}" OnClick="lbtnEdit_Click" />
                        <cc1:LinkButton ID="lbtnDelete" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                            Text="${Common.Button.Delete}" OnClick="lbtnDelete_Click" FunctionId="CreateMouldDetail" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </fieldset>
</div>
