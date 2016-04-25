using System;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public class ResMatrixLog : ResMatrixLogBase
    {
        #region Non O/R Mapping Properties

        public string WorkShopCodeName
        {
            get
            {
                if (!string.IsNullOrEmpty(this.WorkShopName))
                {
                    return this.WorkShop + "[" + this.WorkShopName + "]";
                }
                else
                {
                    return this.WorkShop;
                }
            }
        }

        public string OperateCodeName
        {
            get
            {
                if (!string.IsNullOrEmpty(this.OperateDesc))
                {
                    return this.Operate + "[" + this.OperateDesc + "]";
                }
                else
                {
                    if (this.Operate != null)
                    {
                        return this.Operate.Value.ToString();
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public string RoleCodeName
        {
            get
            {
                if (!string.IsNullOrEmpty(this.RoleName))
                {
                    return this.Role + "[" + this.RoleName + "]";
                }
                else
                {
                    return this.Role;
                }
            }
        }

        public string UserCodeName
        {
            get
            {
                if (!string.IsNullOrEmpty(this.UserName))
                {
                    return this.UserCode + "[" + this.UserName + "]";
                }
                else
                {
                    return this.UserCode;
                }
            }
        }

        #endregion
    }
}