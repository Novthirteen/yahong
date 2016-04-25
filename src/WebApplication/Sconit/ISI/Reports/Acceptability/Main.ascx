<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="ISI_Reports_Acceptability_Main" %>
<script language="javascript" type="text/javascript" src="Js/Highcharts/js/highcharts.js"></script>
<script language="javascript" type="text/javascript" src="Js/Highcharts/js/modules/exporting.js"></script>
<script language="javascript" type="text/javascript" src="Js/Highcharts/js/themes/grid.js"></script>
<script language="javascript" type="text/javascript">
    var chart;
    function ShowChart() {
        $("#fs").slideDown();
        Sys.Net.WebServiceProxy.invoke('Webservice/CheckupMgrWS.asmx', 'GetCheckups', false,
            { "startDate": $('#<%=tbStartDate.ClientID%>').val(), "endDate": $('#<%=tbEndDate.ClientID%>').val(), "type": $('#<%=ddlType.ClientID%>').val()},
            function OnSucceeded(result, eventArgs) {
                chart = new Highcharts.Chart({
                    chart: {
                        renderTo: 'container'
                    },
                    title: {
                        text: ''
                    },
                    xAxis: {
                        categories: result.categories,
                        labels: {
                            rotation: -45,
                            align: 'right',
                            style: {
                                fontSize: '13px',
                                fontFamily: 'Verdana, sans-serif'
                            }
                        }
                    },
                    yAxis: {
                        title: {
                            text: '人次'
                        },
                        labels: {
                            formatter: function () {
                                return this.value;
                            }
                        }
                    },
                    tooltip: {
                        formatter: function () {
                            var s;
                            if (this.point.name) { // the pie chart
                                s = '' +
                                    this.point.name + ': ' + this.y + '  元';
                            } else {
                                s = '' +
                                    this.series.name + this.x + ': ' + this.y + '人次';
                            }
                            return s;
                        }
                    },

                    labels: {
                        items: [{
                            html: '金额对比',
                            style: {
                                left: '60px',
                                top: '8px',
                                color: 'black'
                            }
                        }]
                    },
                    series: [{
                        type: 'column',
                        name: '增款',
                        data: result.data1,
                        color: '#4572A7'
                    }, {
                        type: 'column',
                        name: '扣款',
                        data: result.data2,
                        color: '#89A54E'
                    }, {
                        type: 'spline',
                        name: '平均数',
                        data: result.data3,
                        marker: {
                            lineWidth: 2,
                            lineColor: Highcharts.getOptions().colors[3],
                            fillColor: 'white'
                        }
                    }, {
                        type: 'pie',
                        name: 'Total consumption',
                        data: [{
                            name: '增款',
                            y: result.p1,
                            color: '#4572A7' // Jane's color
                        }, {
                            name: '扣款',
                            y: result.p2,
                            color: '#89A54E' // John's color
                        }],
                        center: [100, 80],
                        size: 100,
                        showInLegend: false,
                        dataLabels: {
                            enabled: true,
                            color: '#000000',
                            connectorColor: '#000000',
                            formatter: function () {
                                return '<b>' + this.point.name + '</b>: ' + Highcharts.numberFormat(this.percentage, 0) + ' %';
                            }
                        }
                    }]
                });
            },
            function OnFailed(error) {

                alert(error.get_message());
            }
          );
        }
</script>
<fieldset>
    <table class="mtable">
        <tr>
            <td class="td01">
                <asp:Literal ID="lblStartDate" runat="server" Text="${Common.Business.StartDate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbStartDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM'})" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblEndDate" runat="server" Text="${Common.Business.EndDate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbEndDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM'})" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblType" runat="server" Text="${ISI.Checkup.CodeMaster.ISICheckupProjectType}:" />
            </td>
            <td class="td02">
                <asp:DropDownList ID="ddlType" runat="server" DataTextField="Description"
                    DataValueField="Value" />
            </td>
        </tr>
        <tr>
            <td class="td01"></td>
            <td class="td02"></td>
            <td class="td01"></td>
            <td class="t02">
                <div class="buttons">
                    <button type="button" id="btnSearch" class="button2" name="btnSearch" onclick="ShowChart()">
                        ${Common.Button.Search}
                    </button>
                </div>
            </td>
        </tr>
    </table>
</fieldset>
<fieldset id="fs" style="display: none">
    <div id="container" style="min-width: 400px; height: 400px; margin: 0 auto"></div>
</fieldset>
