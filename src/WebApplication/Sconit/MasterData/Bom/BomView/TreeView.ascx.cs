using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using com.Sconit.Web;
using System.Collections.Generic;
using com.Sconit.Entity.MasterData;
using com.Sconit.Service.Ext.MasterData;
using Whidsoft.WebControls;
using com.Sconit.Control;
using com.Sconit.Entity;
using System.Reflection;
using NHibernate.Expression;

public partial class MasterData_Bom_BomView_TreeView : ListModuleBase
{
    private Bom bom;
    private Item item;
    private IList<BomDetail> bomDetailList;

    protected void Page_Load(object sender, System.EventArgs e)
    {
    }
    public override void UpdateView()
    {
    }

    public void ShowTreeView(object sender)
    {
        string itemCode = ((object[])sender)[0].ToString();
        string date = ((object[])sender)[1].ToString();
        string viewType = ((object[])sender)[2].ToString();
        DateTime effDate = DateTime.Now;

        item = TheItemMgr.LoadItem(itemCode);
        if (item == null || item.Bom == null)
        {
            bom = TheBomMgr.LoadBom(itemCode);
        }
        else
        {
            bom = item.Bom;
        }
        if (bom == null)
        {
            ShowErrorMessage("MasterData.BomDetail.ErrorMessage.BomNotExist");
            this.fld.Visible = false;
            return;
        }
        else
        {
            this.fld.Visible = true;
        }
        try
        {
            effDate = Convert.ToDateTime(date);
        }
        catch (Exception)
        {
            ShowWarningMessage("MasterData.BomView.WarningMessage.DateInvalid");
            return;
        }

        bomDetailList = TheBomDetailMgr.GetTreeBomDetail(bom.Code, effDate);

        if (bomDetailList != null && bomDetailList.Count > 0)
        {
            MyOrgNode RootNode = new MyOrgNode();
            GenChildOrgNode(RootNode);
            OrgChartBomTreeView.Node = RootNode;
        }
    }

    private void GenChildOrgNode(MyOrgNode RootNode)
    {
        decimal BaseQty = 1;
        RootNode.Code = bom.Code;
        RootNode.Name = bom.Description;
        RootNode.Memo1 = BaseQty.ToString() + " " + bom.Uom.Code;
        createChildNode(RootNode, bom.Code, BaseQty);
    }

    private void createChildNode(MyOrgNode parentnode, string compCode, decimal BaseQty)
    {
        foreach (BomDetail bomDetail in bomDetailList)
        {
            if (bomDetail.Bom.Code.ToLower() == compCode.ToLower())
            {
                bomDetail.CalculatedQty = bomDetail.CalculatedQty * BaseQty;
                MyOrgNode subOrgNode = new MyOrgNode();
                subOrgNode.Code = bomDetail.Item.Code;
                subOrgNode.Name = bomDetail.Item.Description;
                subOrgNode.Memo1 = bomDetail.CalculatedQty.ToString("0.########") + " " + bomDetail.Uom.Code;
                subOrgNode.Memo2 = bomDetail.Item.Type;
                parentnode.Nodes.Add(subOrgNode);
                createChildNode(subOrgNode, bomDetail.Item.Code, bomDetail.CalculatedQty);
            }
        }
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        return;
        List<BomDetail> bomDetailList = new List<BomDetail>();
        //Item a = new Item();
        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(Item));
        selectCriteria.Add(Expression.Like("ItemCategory.Code", "3", MatchMode.Start));
        IList<Item> items = TheCriteriaMgr.FindAll<Item>(selectCriteria);
        foreach (Item item in items)
        {
            IList<BomDetail> newBomDetails = TheBomDetailMgr.GetTreeBomDetail(item.Code, DateTime.Now);
            if (newBomDetails != null)
            {
                bomDetailList.AddRange(newBomDetails.ToList());
            }
        }

        Dictionary<string, string> columnInfo = new Dictionary<string, string>();
        columnInfo.Add("BomCode", "父物料");
        columnInfo.Add("BomDesc", "父物料描述");
        columnInfo.Add("ItemCode", "子物料");
        columnInfo.Add("ItemDesc", "子物料描述");
        columnInfo.Add("RateQty", "用量");
        columnInfo.Add("UomCode", "单位");
        ExExcel(bomDetailList, "bomdet.xls", columnInfo);

    }
    /// <summary> 
    /// 将一组对象导出成EXCEL 
    /// </summary> 
    /// <typeparam name="T">要导出对象的类型</typeparam> 
    /// <param name="objList">一组对象</param> 
    /// <param name="FileName">导出后的文件名</param> 
    /// <param name="columnInfo">列名信息</param> 
    private void ExExcel<T>(List<T> objList, string FileName, Dictionary<string, string> columnInfo)
    {

        if (columnInfo.Count == 0) { return; }
        if (objList.Count == 0) { return; }
        //生成EXCEL的HTML 
        string excelStr = "";

        Type myType = objList[0].GetType();
        //根据反射从传递进来的属性名信息得到要显示的属性 
        List<PropertyInfo> myPro = new List<PropertyInfo>();
        foreach (string cName in columnInfo.Keys)
        {
            PropertyInfo p = myType.GetProperty(cName);
            if (p != null)
            {
                myPro.Add(p);
                excelStr += columnInfo[cName] + "\t";
            }
        }
        //如果没有找到可用的属性则结束 
        if (myPro.Count == 0) { return; }
        excelStr += "\n";

        foreach (T obj in objList)
        {
            foreach (PropertyInfo p in myPro)
            {
                excelStr += p.GetValue(obj, null) + "\t";
            }
            excelStr += "\n";
        }

        //输出EXCEL 
        HttpResponse rs = System.Web.HttpContext.Current.Response;
        rs.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
        rs.AppendHeader("Content-Disposition", "attachment;filename=" + FileName);
        rs.ContentType = "application/ms-excel";
        rs.Write(excelStr);
        rs.End();
    }


}
