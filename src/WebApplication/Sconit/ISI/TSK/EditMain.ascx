<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditMain.ascx.cs" Inherits="ISI_TSK_EditMain" %>
<%@ Register Src="~/ISI/TaskStatus/Main.ascx" TagName="Status" TagPrefix="uc2" %>
<%@ Register Src="RefTask.ascx" TagName="RefTask" TagPrefix="uc2" %>
<%@ Register Src="TabNavigator.ascx" TagName="TabNavigator" TagPrefix="uc2" %>
<%@ Register Src="Edit.ascx" TagName="Edit" TagPrefix="uc2" %>
<%@ Register Src="Detail.ascx" TagName="Detail" TagPrefix="uc2" %>
<%@ Register Src="Process.ascx" TagName="Process" TagPrefix="uc2" %>
<%@ Register Src="Wiki.ascx" TagName="Wiki" TagPrefix="uc2" %>
<%@ Register Src="~/ISI/Attachment/Attachment.ascx" TagName="Attachment" TagPrefix="uc2" %>
<%@ Register Src="ProcessInstance.ascx" TagName="ProcessInstance" TagPrefix="uc2" %>
<%@ Register Src="ResMatrixDet.ascx" TagName="ResMatrixDet" TagPrefix="uc2" %>

<uc2:TabNavigator ID="ucTabNavigator" runat="server" Visible="true" />
<div class="ajax__tab_body">
    <uc2:Edit ID="ucEdit" runat="server" Visible="true" />
    <uc2:ResMatrixDet ID="ucResMatrixDet" runat="server" Visible="false" />
    <uc2:Attachment ID="ucAttachment" runat="server" Visible="false" />
    <uc2:ProcessInstance ID="ucProcessInstance" runat="server" Visible="false" />
    <uc2:Status ID="ucStatus" runat="server" Visible="false" />
    <uc2:RefTask ID="ucRefTask" runat="server" Visible="false" />
    <uc2:Process ID="ucProcess" runat="server" Visible="false" />
    <uc2:Detail ID="ucDetail" runat="server" Visible="false" />
    <uc2:Wiki ID="ucWiki" runat="server" Visible="false" />
</div>
</div> 