<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditMain.ascx.cs" Inherits="Facility_FacilityStock_EditMain" %>

<%@ Register Src="TabNavigator.ascx" TagName="TabNavigator" TagPrefix="uc2" %>
<%@ Register Src="Edit.ascx" TagName="Edit" TagPrefix="uc2" %>
<%@ Register Src="Detail.ascx" TagName="Detail" TagPrefix="uc2" %>
<%@ Register Src="Attachment.ascx" TagName="Attachment" TagPrefix="uc2" %>

    <uc2:TabNavigator ID="ucTabNavigator" runat="server" Visible="true" />
    <uc2:Edit ID="ucEdit" runat="server" Visible="true" />
    <uc2:Detail ID="ucDetail" runat="server" Visible="false" />
    <uc2:Attachment ID="ucAttachment" runat="server" Visible="false" />
</div>
</div> 