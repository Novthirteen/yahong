<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="ISI_OrgChart_View" %>

<head>
    <meta charset="utf-8" />
    <title>Spacetree - Mindmap Demo</title>
    <script src="js/utils.js" type="text/javascript"></script>
</head>
<body>
    <div id="gridlist" class="wrapper" style="width: 100%; height: 100%; margin-top: 15px;"></div>
    <div id="infovis">
        <asp:HiddenField ID="hfa" runat="server" Value='' />
    </div>

    <script type="text/javascript">

        (function () {

            var data = $.parseJSON($('#ctl01_hfa').val());
            this.initGraph = function (data) {
                self.data = data;
                var wrapper = $("div.wrapper").empty();
                var treeData = utils.toTreeData(data, "Code", "ParentCode", "children");

                var tb = renderTreeGraph(treeData);
                tb.appendTo(wrapper);

                //绑定事件
                $(wrapper).find(".td-node").click(function () {
                    $(".td-node").css({ "background-color": "#f6f6ff", "color": "" });
                    $(this).css({ "background-color": "#faffbe", "color": "#FF0000" });
                    self.selectNode = $(this).data("node");
                }).dblclick(self.editClick);
                if (self.selectNode) {
                    $("#td" + self.selectNode.OrganizeCode).css({ "background-color": "#faffbe", "color": "#FF0000" });
                }
            }

            this.initGraph(data);

        })();

        function renderTreeGraph(treeData) {
            //生成图形
            var tb = $('<table class="tb-node" cellspacing="0" cellpadding="0" align="center" border="0" style="border-width:0px;border-collapse:collapse;margin:0 auto;vertical-align:top"></table>');
            var tr = $('<tr></tr>');
            for (var i in treeData) {
                if (i > 0) $('<td>&nbsp;</td>').appendTo(tr);
                $('<td style="vertical-align:top;text-align:center;"></td>').append(createChild(treeData[i])).appendTo(tr);
            }
            tr.appendTo(tb);
            return tb;
        }

        //递归生成机构树图形
        function createChild(node, ischild) {
            var length = (node.children || []).length;
            var colspan = length * 2 - 1;
            if (length == 0)
                colspan = 1;

            var fnTrVert = function () {
                var tr1 = $('<tr class="tr-vline"><td align="center" valign="top" colspan="' + colspan + '"><img class="img-v" src="Images/tree/Tree_Vert.gif" border="0"></td></tr>');
                return tr1;
            };
            //1.创建容器
            var tb = $('<table class="tb-node" cellspacing="0" cellpadding="0" align="center" border="0" style="border-width:0px;border-collapse:collapse;margin:0 auto;vertical-align:top"></table>');
            //var tb = $('<table style="BORDER-BOTTOM: 1px solid; BORDER-LEFT: 1px solid; BORDER-TOP: 1px solid; BORDER-RIGHT: 1px solid" border="1" cellspacing="0" cellpadding="2" align="center"></table>');

            //2.如果本节点是子节点，添加竖线在节点上面
            if (ischild) {
                fnTrVert().appendTo(tb);
            }

            // 3.添加本节点到图表
            var tr3 = utils.functionComment(function () {/*
<tr class="tr-node"><td align="center" valign="top" colspan="{0}">
	<table align="center" style="border:solid 2px" border="1" cellpadding="2" cellspacing="0">
	    <tr>
	        <td class="td-node" id='td{3}' data-node='{2}' title='{4}' align="center" valign="top" style="background-color:#f6f6ff;cursor:pointer;padding:2px;">{1}</td>
	    </tr>
	</table>
</td></tr> */
            });
            tr3 = utils.formatString(tr3, colspan, node.Name, JSON.stringify(node), node.Code, node.ToolTips);
            $(tr3).appendTo(tb);

            // 4.增加上下级的连接线
            if (length > 1) {
                //增加本级连接下级的首节点竖线，在节点下方
                fnTrVert().appendTo(tb);

                //增加本级连接下级的中间横线
                var tr4 = utils.functionComment(function () {/*
<tr class="tr-hline">
    <td colspan="1">
        <table width="100%" height="100%" border="0" cellpadding="0" cellspacing="0">
            <tbody>
                <tr>
                    <td width="50%" style="background:url(Images/tree/Tree_Empty.gif)"></td>
                    <td width="3px" height="3px" ><img src="Images/tree/Tree_Dot.gif" border="0"></td>
                    <td width="50%" style="background:url(Images/tree/Tree_Dot.gif)"></td>
                </tr>
            </tbody>
        </table>
    </td>
    <td style="background:url(Images/tree/Tree_Dot.gif)" colspan="{0}"></td>
    <td colspan="1">
        <table width="100%" height="100%" border="0" cellpadding="0" cellspacing="0">
            <tbody>
                <tr>
                    <td width="50%" style="background:url(Images/tree/Tree_Dot.gif)"></td>
                    <td width="3px" height="3px" ><img src="Images/tree/Tree_Dot.gif" border="0"></td>
                    <td width="50%" style="background:url(Images/tree/Tree_Empty.gif)"></td>
                </tr>
            </tbody>
        </table>
    </td>
</tr>*/});
                tr4 = utils.formatString(tr4, colspan - 2);
                $(tr4).appendTo(tb);
            }

            //5.递归增加下级所有子节点到图表
            if (length > 0) {
                var tr5 = $('<tr></tr>');

                for (var i in node.children) {
                    if (i > 0) {
                        $('<td>&nbsp;</td>').appendTo(tr5);
                    }
                    $('<td style="vertical-align:top;text-align:center;"></td>').append(createChild(node.children[i], true)).appendTo(tr5);
                }

                tr5.appendTo(tb);
            }

            return tb;
        }
    </script>
</body>
