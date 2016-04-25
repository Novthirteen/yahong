<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="Facility_FacilityEnvelop_Main" %>
<%@ Register Src="~/Facility/FacilityEnvelop/Reopen.ascx" TagName="Reopen" TagPrefix="uc2" %>
<%@ Register Src="~/Facility/FacilityEnvelop/List.ascx" TagName="List" TagPrefix="uc2" %>
<%@ Register Src="~/Facility/FacilityEnvelop/Search.ascx" TagName="Search" TagPrefix="uc2" %>

<uc2:Search ID="ucSearch" runat="server" Visible="True" />
<uc2:Reopen ID="ucReopen" runat="server" Visible="false" />
<uc2:List ID="ucList" runat="server" Visible="false" />