using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sconit_CS.SconitWS;

namespace Sconit_CS
{
    public partial class UCLoadMaterial : UCBase
    {


        public UCLoadMaterial(User user, string moduleType)
            : base(user, moduleType)
        {
            InitializeComponent();
           
            this.btnConfirm.Text = "上料";
            this.gvHuList.Columns["CAT"].Visible = true;
            this.gvHuList.Columns["HUE"].Visible = true;
            this.gvHuList.Visible = true;
            this.gvList.Visible = false;
            //this.resolver.Command = BusinessConstants.CS_BIND_VALUE_TRANSFORMERDETAIL;
        }

         
    }
}
