<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditMain.ascx.cs" Inherits="Facility_MouldMaster_EditMain" %>

<%@ Register Src="TabNavigator.ascx" TagName="TabNavigator" TagPrefix="uc2" %>
<%@ Register Src="Edit.ascx" TagName="Edit" TagPrefix="uc2" %>
<%@ Register Src="../FacilityMaintainPlan/Main.ascx" TagName="Maintain" TagPrefix="uc2" %>
<%@ Register Src="Transaction.ascx" TagName="Trans" TagPrefix="uc2" %>
<%@ Register Src="Attachment.ascx" TagName="Attachment" TagPrefix="uc2" %>
<%@ Register Src="Template.ascx" TagName="Template" TagPrefix="uc2" %>


    <uc2:TabNavigator ID="ucTabNavigator" runat="server" Visible="true" />
    <uc2:Edit ID="ucEdit" runat="server" Visible="true" />
    <uc2:Maintain ID="ucMaintain" runat="server" Visible="false" />
    <uc2:Trans ID="ucTrans" runat="server" Visible="false" />
    <uc2:Attachment ID="ucAttachment" runat="server" Visible="false" />
    <uc2:Template ID="ucTemplate" runat="server" Visible="false" />
</div>
</div> 