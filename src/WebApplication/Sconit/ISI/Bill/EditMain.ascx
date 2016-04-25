<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditMain.ascx.cs" Inherits="ISI_Bill_EditMain" %>
<%@ Register Src="TabNavigator.ascx" TagName="TabNavigator" TagPrefix="uc2" %>
<%@ Register Src="DetailNew.ascx" TagName="DetailNew" TagPrefix="uc2" %>
<%@ Register Src="DetailEdit.ascx" TagName="DetailEdit" TagPrefix="uc2" %>
<%@ Register Src="Edit.ascx" TagName="Edit" TagPrefix="uc2" %>
<%@ Register Src="~/ISI/Attachment/Attachment.ascx" TagName="Attachment" TagPrefix="uc2" %>

<uc2:TabNavigator ID="ucTabNavigator" runat="server" Visible="true" />
<uc2:Edit ID="ucEdit" runat="server" Visible="true" />
<uc2:DetailNew ID="ucDetailNew" runat="server" Visible="false" />
<uc2:DetailEdit ID="ucDetailEdit" runat="server" Visible="false" />
<uc2:Attachment ID="ucAttachment" runat="server" Visible="false" />
</div>
</div> 