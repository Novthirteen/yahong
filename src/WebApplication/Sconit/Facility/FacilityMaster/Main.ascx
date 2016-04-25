<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="Facility_FacilityMaster_Main" %>
<%@ Register Src="~/Facility/FacilityMaster/EditMain.ascx" TagName="Edit" TagPrefix="uc2" %>
<%@ Register Src="~/Facility/FacilityMaster/List.ascx" TagName="List" TagPrefix="uc2" %>
<%@ Register Src="~/Facility/FacilityMaster/New.ascx" TagName="New" TagPrefix="uc2" %>
<%@ Register Src="~/Facility/FacilityMaster/Search.ascx" TagName="Search" TagPrefix="uc2" %>
<%@ Register Src="~/Facility/FacilityMaster/Import.ascx" TagName="Import" TagPrefix="uc2" %>

<uc2:Search ID="ucSearch" runat="server" Visible="True" />
<uc2:Edit ID="ucEdit" runat="server" Visible="false" />
<uc2:List ID="ucList" runat="server" Visible="false" />
<uc2:New ID="ucNew" runat="server" Visible="false" />
<uc2:Import ID="ucImport" runat="server" Visible="false" />
