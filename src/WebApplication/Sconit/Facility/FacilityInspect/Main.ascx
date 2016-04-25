<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="Facility_FacilityInspect_Main" %>
<%@ Register Src="~/Facility/FacilityInspect/Finish.ascx" TagName="Finish" TagPrefix="uc2" %>
<%@ Register Src="~/Facility/FacilityInspect/List.ascx" TagName="List" TagPrefix="uc2" %>
<%@ Register Src="~/Facility/FacilityInspect/Search.ascx" TagName="Search" TagPrefix="uc2" %>
<%@ Register Src="~/Facility/FacilityTrans/EditMain.ascx" TagName="Trans" TagPrefix="uc2" %>

<uc2:Search ID="ucSearch" runat="server" Visible="True" />
<uc2:Finish ID="ucFinish" runat="server" Visible="false" />
<uc2:List ID="ucList" runat="server" Visible="false" />
<uc2:Trans  ID="ucTrans" runat="server" Visible ="false" />