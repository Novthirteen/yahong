﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="ISI_EmppRec_List" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<fieldset>
    <div class="GridView">
        <cc1:GridView ID="GV_List" runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
            SkinID="GV" AllowMultiColumnSorting="false" AutoLoadStyle="false" SeqNo="0" SeqText="No."
            ShowSeqNo="true" AllowSorting="true" AllowPaging="True" PagerID="gp" Width="100%"
            CellMaxLength="10" TypeName="com.Sconit.Web.CriteriaMgrProxy" SelectMethod="FindAll"
            SelectCountMethod="FindCount">
            <Columns>
                <asp:TemplateField HeaderText="${ISI.EmppRec.Task}" SortExpression="Task">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "TaskCode")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.EmppRec.MsgID}" SortExpression="MsgID">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "MsgID")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.EmppRec.SeqID}" SortExpression="SeqID">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "SeqID")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.EmppRec.SrcID}" SortExpression="SrcID">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "SrcID")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.EmppRec.DestID}" SortExpression="DestID">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "DestID")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.EmppRec.SubmitDatetime}" SortExpression="SubmitDatetime">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "SubmitDatetime")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.EmppRec.DoneDatetime}" SortExpression="DoneDatetime">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "DoneDatetime")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.EmppRec.Content}" SortExpression="Content">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "Content")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="${ISI.EmppRec.CreateDate}" SortExpression="CreateDate">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "CreateDate")%>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </cc1:GridView>
        <cc1:GridPager ID="gp" runat="server" GridViewID="GV_List" PageSize="10">
        </cc1:GridPager>
    </div>
</fieldset>
