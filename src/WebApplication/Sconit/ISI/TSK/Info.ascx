<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Info.ascx.cs" Inherits="ISI_TSK_Info" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<div>
    <fieldset>
        <legend>${Common.Business.BasicInfo}</legend>
        <asp:FormView ID="FV_TSKView" runat="server" DefaultMode="ReadOnly" DataSourceID="ODS_TaskMstr"
            DataKeyNames="Code" OnDataBound="FV_ISI_DataBound">
            <ItemTemplate>
                <table class="mtable">
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblCode" runat="server" Text="${ISI.TSK.Code}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbCode" runat="server" Text='<%# Bind("Code") %>' onfocus="this.blur();"
                                ReadOnly="true" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lbStatus" runat="server" Text="${Common.CodeMaster.Status}:" />
                        </td>
                        <td class="td02">
                            <asp:Literal ID="lblStatus" runat="server"  />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblSubject" runat="server" Text="${ISI.TSK.Subject}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbSubject" runat="server" Text='<%# Bind("Subject") %>' Width="80%"
                                onfocus="this.blur();" ReadOnly="true" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="ltlTaskSubType" runat="server" Text="${ISI.TSK.TaskSubType}:" />
                        </td>
                        <td class="td02">
                            <cc1:ReadonlyTextBox ID="rtbTaskSubType" runat="server" CodeField="TaskSubType.Code"
                                DescField="TaskSubType.Desc" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblFailureMode" runat="server" Text="${ISI.TSK.FailureMode}:" />
                        </td>
                        <td class="td02">
                            <cc1:ReadonlyTextBox ID="rtbFailureMode" runat="server" CodeField="FailureMode.Code"
                                DescField="FailureMode.Desc" />
                            <cc1:CodeMstrLabel ID="ddlPhase" Code="ISIPhase" runat="server" Value='<%# Bind("Phase") %>' />
                            <asp:TextBox ID="tbSeq" runat="server" Visible="False" Text='<%# Bind("Seq") %>' ReadOnly="true"></asp:TextBox>
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblBackYards" runat="server" Text="${ISI.TSK.BackYards}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="tbBackYards" runat="server" Text='<%# Bind("BackYards") %>' onfocus="this.blur();"
                                ReadOnly="true" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblFlag" runat="server" Text="${Common.CodeMaster.Flag}:" />
                        </td>
                        <td class="td02">
                            <cc1:CodeMstrLabel ID="cmlFlag" Code="ISIFlag" runat="server" Value='<%# Bind("Flag") %>' />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblColor" runat="server" Text="${Common.CodeMaster.Color}:" />
                        </td>
                        <td class="td02">
                            <cc1:CodeMstrLabel ID="cmlColor" Code="ISIColor" runat="server" Value='<%# Bind("Color") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="lblTaskAddress" runat="server" Text="${ISI.TSK.TaskAddress}:" />
                        </td>
                        <td class="td02">
                            <asp:TextBox ID="rtbTaskAddress" runat="server" Text='<%# Bind("TaskAddress") %>'
                                ReadOnly="true" />
                        </td>
                        <td class="td01">
                            <asp:Literal ID="lblPriority" runat="server" Text="${ISI.TSK.Priority}:" />
                        </td>
                        <td class="td02">
                            <cc1:CodeMstrDropDownList ID="ddlPriority" Code="ISIPriority" runat="server" Enabled="false"
                                IncludeBlankOption="false" />
                        </td>
                    </tr>
                    <tr>
                        <td class="td01">
                            <asp:Literal ID="ltlDesc1" runat="server" Text="${ISI.TSK.Desc1}:" />
                        </td>
                        <td class="td02" colspan="3">
                            <asp:TextBox ID="tbDesc1" runat="server" Text='<%# Bind("Desc1") %>' Height="60"
                                Width="77%" TextMode="MultiLine" ReadOnly="true" Font-Size="10" />
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
        </asp:FormView>
    </fieldset>
</div>
<asp:ObjectDataSource ID="ODS_TaskMstr" runat="server" TypeName="com.Sconit.Web.TaskMstrMgrProxy"
    DataObjectTypeName="com.Sconit.Entity.MasterData.TaskMstr" SelectMethod="LoadTaskMstr">
    <SelectParameters>
        <asp:Parameter Name="code" Type="String" />
    </SelectParameters>
</asp:ObjectDataSource>
