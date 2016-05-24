<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Edit.ascx.cs" Inherits="ISI_Summary_Edit" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<link rel="stylesheet" type="text/css" href="Js/jquery.ui/jquery-ui.min.css" />
<script language="javascript" type="text/javascript" src="Js/ui.core-min.js"></script>
<script language="javascript" type="text/javascript" src="Js/uploadify/jquery.uploadify.min.js?ver=<%=(new Random()).Next(0, 99999).ToString() %>"></script>
<link rel="stylesheet" type="text/css" href="Js/uploadify/uploadify.css" />
<script language="javascript" type="text/javascript">

    function GetCount()
    {
        $("#<%=((TextBox)this.FV_Summary.FindControl("tbQty")).ClientID%>").val($("input[type='radio'][value!='Poor']:checked").length);
    }

    function TaskUploadify() {
        //$("input[type='file']").uploadify({
        //$("#ctl01_ucList_GV_List_ctl02_uploadify").uploadify({
        $("#uploadify").uploadify({
            'debug': false, //开启调试    
            'auto': true,  //是否自动上传    
            'buttonText': '${ISI.Summary.Upload}',  //按钮上的文字
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
            'formData': { 'TaskCode': '<%=this.SummaryCode%>', 'UserCode': '<%=this.CurrentUser.Code%>', 'UserName': '<%=this.CurrentUser.Name%>', 'ModuleType': '<%= typeof(com.Sconit.ISI.Entity.Summary).FullName%>' },
            'removeTimeout': 1,
            //返回一个错误，选择文件的时候触发    
            'onSelectError': function (file, errorCode, errorMsg) {
                switch (errorCode) {
                    case -100:
                        $("#uploadfileQueue").text("${UploadTheFileNumberIsBeyondSystemOfFile}" + $('#uploadify').uploadify('settings', 'queueSizeLimit'));
                        break;
                    case -110:
                        $("#uploadfileQueue").text(file.name + "${ISI.Summary.Error.FileSize}" + $('#uploadify').uploadify('settings', 'fileSizeLimit'));
                        break;
                    case -120:
                        $("#uploadfileQueue").text(file.name + "${ISI.Summary.Error.FileSizeException}");
                        break;
                    case -130:
                        $("#uploadfileQueue").text(file.name + "${ISI.Summary.Error.FileType}");
                        break;
                }
            },
            //检测FLASH失败调用    
            'onFallback': function () {
                alert("${ISI.Summary.NoFlash}");
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
                $("#uploadfileQueue").append(queueData.uploadsSuccessful + "${ISI.Summary.HasBeenUploadedSuccessfully}").fadeIn(300).delay(2500).fadeOut(300);// 这个是渐渐消失
            }
        });
    }

    $(document).ready(function () {

        TaskUploadify();
        $('textarea').tah({
            moreSpace: 25,   // 输入框底部预留的空白, 默认15, 单位像素
            maxHeight: 260,  // 指定Textarea的最大高度, 默认600, 单位像素
            animateDur: 200  // 调整高度时的动画过渡时间, 默认200, 单位毫秒
        });
    });

</script>

<div id="divFV" runat="server">
    <asp:FormView ID="FV_Summary" runat="server" DataSourceID="ODS_Summary" DefaultMode="Edit"
        Width="100%" DataKeyNames="Code" OnDataBound="FV_Summary_DataBound">
        <EditItemTemplate>
            <fieldset>
                <legend>${ISI.Summary.UpdateSummary}</legend>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblCode" runat="server" Text="${Common.Business.Code}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbCode" runat="server" Text='<%# Bind("Code") %>' ReadOnly="true" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lbStatus" runat="server" Text="${Common.CodeMaster.Status}:" />
                        </td>
                        <td class="td02">
                            <asp:Literal ID="lblStatus" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblUser" runat="server" Text="${ISI.Summary.User}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbUser" runat="server" Text='<%# Bind("User") %>' ReadOnly="true" />

                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlDate" runat="server" Text="${ISI.Summary.Date}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbDate" runat="server" Text='<%# Bind("Date","{0:yyyy-MM}") %>' ReadOnly="true" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblCompany" runat="server" Text="${ISI.Summary.Company}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbCompany" runat="server" Text='<%# Bind("Company") %>' ReadOnly="true" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblDept2" runat="server" Text="${ISI.Summary.Dept2}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbDept2" runat="server" Text='<%# Bind("Dept2") %>' ReadOnly="true" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblCount" runat="server" Text="${ISI.Summary.Count}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbCount" runat="server" Text='<%# Bind("Count") %>' ReadOnly="true" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblQty" runat="server" Text="${ISI.Summary.Qty}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbQty" runat="server" Text='<%# Bind("Qty") %>' ReadOnly="true" CssClass="inputRequired" />
                            ${ISI.Summary.StandardQty}:
                            <asp:Label ForeColor="Blue" ID="lblStandardQty" runat="server" Text='<%# Bind("StandardQty") %>' />
                            <asp:RequiredFieldValidator ID="rfvQty" runat="server" ErrorMessage="${Common.Business.Error.Required}"
                                Display="Dynamic" ControlToValidate="tbQty" ValidationGroup="vgApprove" />
                            <asp:RangeValidator ID="rvQty" runat="server" ControlToValidate="tbQty" ErrorMessage="${Common.Validator.Valid.Number}"
                                Display="Dynamic" Type="Integer" MinimumValue="0" MaximumValue="10000" ValidationGroup="vgApprove" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Label ID="lblDesc" runat="server" Text="${ISI.Summary.Desc}:" />
                        </td>
                        <td class="td02" colspan="3">
                            <asp:TextBox ID="tbDesc" runat="server" Text='<%# Bind("Desc") %>' Height="50" ReadOnly="true"
                                Width="80%" TextMode="MultiLine" onkeypress="javascript:setMaxLength(this,3000);"
                                onpaste="limitPaste(this, 3000)" Font-Size="10" />
                        </td>
                    </tr>
                    <tr id="approveDescTr" runat="server" visible="false">
                        <td class="td01">
                            <asp:Label ID="lblApproveDesc" runat="server" Text="${ISI.Summary.ApproveDesc2}:" />
                        </td>
                        <td class="td02" colspan="3">
                            <asp:TextBox ID="tbApproveDesc" runat="server" Text='<%# Bind("ApproveDesc") %>' Height="50" ReadOnly="true"
                                Width="80%" TextMode="MultiLine" onkeypress="javascript:setMaxLength(this,200);"
                                onpaste="limitPaste(this, 200)" Font-Size="10" ValidationGroup="vgApprove" />
                        </td>
                    </tr>
                    <tr id="ultimatelyDescTr1" runat="server" visible="false">
                        <td class="td01">
                            <asp:Label ID="lblUltimatelyAmount" runat="server" Text="${ISI.Summary.UltimatelyAmount}:" />
                        </td>
                        <td class="td02" colspan="3">
                            <asp:TextBox ID="tbUltimatelyAmount" runat="server" Text='<%# Bind("UltimatelyAmount","{0:0.########}") %>' ReadOnly="true" />${ISI.Checkup.AmountMsg}
                        </td>
                    </tr>
                    <tr id="ultimatelyDescTr2" runat="server" visible="false">
                        <td class="td01">
                            <asp:Label ID="lblUltimatelyDesc" runat="server" Text="${ISI.Summary.UltimatelyDesc}:" />
                        </td>
                        <td class="td02" colspan="3">
                            <asp:TextBox ID="tbUltimatelyDesc" runat="server" Text='<%# Bind("UltimatelyDesc") %>' Height="50" ReadOnly="true"
                                Width="80%" TextMode="MultiLine" onkeypress="javascript:setMaxLength(this,200);"
                                onpaste="limitPaste(this, 200)" Font-Size="10" />
                        </td>
                    </tr>
                </table>
                <div id="divMore" style="display: none">
                    <table class="mtable">
                        <tr>
                            <td class="td01">
                                <asp:Literal ID="ltlDiff" runat="server" Text="${ISI.Summary.Diff}:" />
                            </td>
                            <td class="td02">
                                <asp:TextBox ID="tbDiff" runat="server" Text='<%# Bind("Diff") %>' ReadOnly="true" />
                            </td>
                            <td class="td01">
                                <asp:Literal ID="lblExcellent" runat="server" Text="${ISI.Summary.Excellent}:" />
                            </td>
                            <td class="td02">
                                <asp:TextBox ID="tbExcellent" runat="server" Text='<%# Bind("Excellent") %>' ReadOnly="true" />
                            </td>
                        </tr>
                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblModerate" runat="server" Text="${ISI.Summary.Moderate}:" />
                            </td>
                            <td class="td02">
                                <asp:TextBox ID="tbModerate" runat="server" Text='<%# Bind("Moderate") %>' ReadOnly="true" />
                            </td>
                            <td class="td01">
                                <asp:Literal ID="lblPoor" runat="server" Text="${ISI.Summary.Poor}:" />
                            </td>
                            <td class="td02">
                                <asp:TextBox ID="tbPoor" runat="server" Text='<%# Bind("Poor") %>' ReadOnly="true" />
                            </td>
                        </tr>
                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblCreateDate" runat="server" Text="${Common.Business.CreateDate}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbCreateDate" runat="server" CodeField="CreateDate" />
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
                                <asp:Literal ID="lblSubmitDate" runat="server" Text="${ISI.Summary.SubmitDate}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbSubmitDate" runat="server" CodeField="SubmitDate" />
                            </td>
                            <td class="td01">
                                <asp:Literal ID="lblSubmitUser" runat="server" Text="${ISI.Summary.SubmitUser}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbSubmitUser" runat="server" CodeField="SubmitUserNm" />
                            </td>
                        </tr>
                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblInApproveDate" runat="server" Text="${ISI.Summary.InApproveDate}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbInApproveDate" runat="server" CodeField="InApproveDate" />
                            </td>
                            <td class="td01">
                                <asp:Literal ID="lblInApproveUser" runat="server" Text="${ISI.Summary.InApproveUser}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbInApproveUser" runat="server" CodeField="InApproveUserNm" />
                            </td>
                        </tr>
                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblApproveDate" runat="server" Text="${ISI.Summary.ApproveDate}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbApproveDate" runat="server" CodeField="ApproveDate" />
                            </td>
                            <td class="td01">
                                <asp:Literal ID="lblApproveUser" runat="server" Text="${ISI.Summary.ApproveUser}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbApproveUser" runat="server" CodeField="ApproveUserNm" />
                            </td>
                        </tr>

                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblCloseDate" runat="server" Text="${ISI.Summary.CloseDate}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbCloseDate" runat="server" CodeField="CloseDate" />
                            </td>
                            <td class="td01">
                                <asp:Literal ID="lblCloseUser" runat="server" Text="${ISI.Summary.CloseUser}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbCloseUser" runat="server" CodeField="CloseUserNm" />
                            </td>
                        </tr>
                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblUltimatelyDate" runat="server" Text="${ISI.Summary.UltimatelyDate}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbUltimatelyDate" runat="server" CodeField="UltimatelyDate" />
                            </td>
                            <td class="td01">
                                <asp:Literal ID="lblUltimatelyApproveUser" runat="server" Text="${ISI.Summary.UltimatelyUser}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbUltimatelyApproveUser" runat="server" CodeField="UltimatelyApproveUserNm" />
                            </td>
                        </tr>
                        <tr>
                            <td class="td01">
                                <asp:Literal ID="lblLastModifyDate" runat="server" Text="${Common.Business.LastModifyDate}:" />
                            </td>
                            <td class="td02">
                                <cc1:ReadonlyTextBox ID="tbLastModifyDate" runat="server" CodeField="LastModifyDate" />
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
            </fieldset>
            <div id="uploadfileQueue" name="uploadfileQueue" align="center"></div>
            <table class="mtable">
                <tr>
                    <td class="td01">
                        <input type="file" id="uploadify" name="uploadify" /></td>
                    <td class="td02"></td>
                    <td class="td01">
                        <asp:CheckBox runat="server" Checked='<%# Bind("IsCheckup") %>' ID="cbIsCheckup" Enabled="false" Text="${ISI.Summary.IsCheckup}:" />
                    </td>
                    <td class="td02">
                        <div class="buttons">
                            <cc1:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="${Common.Button.Save}"
                                CssClass="apply" ValidationGroup="vgSave" Visible="false" />
                            <cc1:Button ID="btnSubmit" runat="server" Text="${Common.Button.Submit}"
                                CssClass="apply" ValidationGroup="vgSave" OnClick="btnSubmit_Click" Visible="false" />
                            <cc1:Button ID="btnApprove2" runat="server" Text="${ISI.Summary.Button.Approve2}" Visible="false"
                                CssClass="apply" ValidationGroup="vgApprove" OnClientClick="return confirm('${ISI.Summary.Button.Approve.Confirm}')" OnClick="btnApprove2_Click" />
                            <cc1:Button ID="btnApprove" runat="server" Text="${ISI.Summary.Button.Approve}" Visible="false"
                                CssClass="apply" ValidationGroup="vgApprove" OnClientClick="return confirm('${ISI.Summary.Button.Approve.Confirm}')" OnClick="btnApprove_Click" />
                            <cc1:Button ID="btnClose" runat="server" Text="${Common.Button.Close}" CssClass="button2"
                                OnClick="btnClose_Click" OnClientClick="return confirm('${Common.Button.Close.Confirm}')" />
                            <cc1:Button ID="btnCancel" runat="server" Text="${Common.Button.Cancel}" Visible="false"
                                CssClass="delete" OnClientClick="return confirm('${Common.Button.Cancel.Confirm}')" OnClick="btnCancel_Click" />
                            <asp:Button ID="btnBack" runat="server" Text="${Common.Button.Back}" OnClick="btnBack_Click"
                                CssClass="back" />
                        </div>
                    </td>
                </tr>
            </table>
        </EditItemTemplate>
    </asp:FormView>
</div>
<asp:ObjectDataSource ID="ODS_Summary" runat="server" TypeName="com.Sconit.Web.SummaryMgrProxy"
    DataObjectTypeName="com.Sconit.ISI.Entity.Summary"
    SelectMethod="LoadSummary">
    <SelectParameters>
        <asp:Parameter Name="code" Type="String" />
    </SelectParameters>
</asp:ObjectDataSource>
<div>
    <fieldset>
        <legend runat="server" id="lgd">${ISI.Summary.Detail}</legend>
        <asp:GridView ID="GV_List" runat="server" OnRowDataBound="GV_List_RowDataBound" OnDataBound="GV_List_DataBound"
            AutoGenerateColumns="false" ShowHeader="false">
            <Columns>
                <asp:TemplateField>
                    <ItemTemplate>
                        <table class="mtable">
                            <tr>
                                <td style="text-align: left; width=2%;">
                                    <asp:CheckBox runat="server" ID="cbChecked" Checked="true" Text="<%#Container.DataItemIndex + 1%>" />
                                    <asp:HiddenField ID="hfId" runat="server" Value='<%# Bind("Id") %>' />
                                    <asp:Label ID="lblSeq" Visible="false" runat="server" Text="<%#Container.DataItemIndex + 1%>" />
                                </td>
                                <td style="text-align: right;">
                                    <asp:Label ID="lblSubject" runat="server" Text="${ISI.Summary.Detail.Subject}:" />
                                </td>
                                <td class="td02">
                                    <asp:TextBox ID="tbSubject" runat="server" TabIndex="1" Width="80%" Text='<%# Bind("Subject") %>' />${Common.WordCount.100}
                                </td>
                                <td class="td01">
                                    <asp:Label ID="lblTaskCode" runat="server" Text="${ISI.Summary.Detail.TaskCode}:" />
                                </td>
                                <td>
                                    <asp:TextBox ID="tbTaskCode" runat="server" TabIndex="1" Text='<%# Bind("TaskCode") %>' />
                                </td>
                                <td style="text-align: left; width=10%;"></td>
                                <td style="text-align: right; width=2%; vertical-align: text-bottom;"></td>
                            </tr>
                            <tr>
                                <td style="text-align: right;" colspan="2">
                                    <asp:Label ID="lblConment" runat="server" Text="${ISI.Summary.Detail.Conment}:" />
                                </td>
                                <td class="td02" colspan="4" style="vertical-align: text-bottom;">
                                    <asp:TextBox ID="tbConment" runat="server" Text='<%# Bind("Conment") %>' Height="150"
                                        Width="95%" TextMode="MultiLine" Font-Size="10" CssClass="inputRequired" />

                                </td>
                                <td style="text-align: right; width=2%; vertical-align: bottom;">
                                    <asp:Image ID="descImg" onclick="javascript:scroll(0,0)" ImageUrl="~/Images/ISI/top16.png" runat="server" ToolTip="${Common.Return.Top}" />
                                </td>
                            </tr>
                            <tr id="approveDescTr" runat="server" visible="false">
                                <td style="text-align: right;" colspan="2">
                                    <asp:Label ID="lblApproveDesc" runat="server" Text="${ISI.Summary.ApproveDesc2}:" />
                                </td>
                                <td class="td02" colspan="3">
                                    <asp:TextBox ID="tbApproveDesc" runat="server" Text='<%# Bind("ApproveDesc") %>' Height="50" ReadOnly="true"
                                        Width="95%" TextMode="MultiLine" onkeypress="javascript:setMaxLength(this,200);"
                                        onpaste="limitPaste(this, 200)" Font-Size="10" />
                                </td>
                                <td>
                                    <asp:RadioButtonList ID="rblType" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow" DataTextField="Description" DataValueField="Value">
                                    </asp:RadioButtonList>
                                </td>
                                <td style="text-align: right; width=1%; vertical-align: bottom;">
                                    <div>
                                        <asp:Image ID="approveImg" onclick="javascript:scroll(0,0)" ImageUrl="~/Images/ISI/top16.png" runat="server" ToolTip="${Common.Return.Top}" />
                                    </div>
                                    <div>
                                        <asp:Button ID="btnSave" runat="server" Text="${ISI.Summary.Save}"
                                            CssClass="apply" ValidationGroup="vgSave" OnClick="btnSave_Click" Visible="false" />
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${Common.GridView.Action}">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnDelete" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                            Text="${Common.Button.Delete}" OnClick="lbtnDelete_Click" Visible="false" OnClientClick="return confirm('${Common.Button.Delete.Confirm}')" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </fieldset>
</div>
