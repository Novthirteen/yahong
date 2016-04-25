<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="Facility_FacilityAllocate_Main" %>
<%@ Register Src="~/Facility/FacilityAllocate/Edit.ascx" TagName="Edit" TagPrefix="uc2" %>
<%@ Register Src="~/Facility/FacilityAllocate/List.ascx" TagName="List" TagPrefix="uc2" %>
<%@ Register Src="~/Facility/FacilityAllocate/New.ascx" TagName="New" TagPrefix="uc2" %>
<%@ Register Src="~/Facility/FacilityAllocate/Search.ascx" TagName="Search" TagPrefix="uc2" %>

<uc2:Search ID="ucSearch" runat="server" Visible="True" />
<uc2:Edit ID="ucEdit" runat="server" Visible="false" />
<uc2:List ID="ucList" runat="server" Visible="false" />
<uc2:New ID="ucNew" runat="server" Visible="false" />
