using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MModuleInfo : X_AD_ModuleInfo
    {
        public MModuleInfo(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        public MModuleInfo(Ctx ctx, int id, Trx trxName)
            : base(ctx, id, trxName)
        {
        }

        protected override bool BeforeSave(bool newRecord)
        {
            string prefix = "", sql = "";
            int prefixCount = 0;

            if (GetPrefix() != null)
            {
                prefix = GetPrefix().ToUpper();
            }

            if (prefix != "")
            {
                sql = "SELECT COUNT(prefix) FROM AD_ModuleInfo WHERE UPPER(prefix) = '" + prefix + "'";
                //prefixCount = Convert.ToInt32(DB.ExecuteScalar(Sql));
            }
            if (IsBeta())
            {
                sql += " AND IsBeta='Y' ";
            }
            else
            {
                sql += " AND IsBeta='N' ";
            }


            if (prefix == "VIS_")
            {
                sql += " AND  UPPER(Name) =  '" + GetName().ToUpper() + "'";
            }



            if (base.Get_ColumnIndex("ModuleTechnology") > -1) // sb cloud database
            {
                string mTech = base.Get_Value("ModuleTechnology").ToString();
                sql += " AND ModuleTechnology = " + mTech;
            }




            prefixCount = Convert.ToInt32(DB.ExecuteScalar(sql));

            if (newRecord)
            {
                if (prefixCount > 0)
                {
                    log.SaveError("PrefixNotAvailable", "", false);
                    return false;
                }

            }
            else if (Is_ValueChanged("IsBeta") && prefixCount > 0)
            {
                log.SaveError("PrefixNotAvailable", "", false);
                return false;
            }
            else if (!newRecord && prefixCount >= 1 && !Get_ValueOld("Prefix").Equals(GetPrefix()))
            {
                log.SaveError("PrefixNotAvailable", "", false);
                return false;
            }




            //Check Assembly Name
            string assemblyname = "";
            if (GetAssemblyName() != null)
            {
                assemblyname = GetAssemblyName().ToUpper();

            }

            int asmCount = 0;
            if (assemblyname != "")
            {
                sql = "SELECT COUNT(assemblyname) FROM AD_ModuleInfo WHERE UPPER(assemblyname)='" + assemblyname + "'";

                if (IsBeta())
                {
                    sql += " AND IsBeta='Y' ";
                }
                else
                {
                    sql += " AND IsBeta='N' ";
                }


                if (prefix == "VIS_")
                {
                    sql += " AND  UPPER(Name) =  '" + GetName().ToUpper() + "'";
                }

                if (base.Get_ColumnIndex("ModuleTechnology") > -1) // sb cloud database
                {
                    string mTech = base.Get_Value("ModuleTechnology").ToString();
                    sql += " AND ModuleTechnology = " + mTech;
                }



                asmCount = Convert.ToInt32(DB.ExecuteScalar(sql));


                if (newRecord)
                {
                    if (asmCount > 0)
                    {
                        log.SaveError("PrefixNotAvailable", "", false);
                        return false;
                    }
                }
                else if (Is_ValueChanged("IsBeta") && asmCount > 0)
                {
                    log.SaveError("PrefixNotAvailable", "", false);
                    return false;
                }
                else if (!newRecord && prefixCount >= 1 && !Get_ValueOld("AssemblyName").Equals(GetAssemblyName()))
                {
                    log.SaveError("PrefixNotAvailable", "", false);
                    return false;
                }

                //if ((newRecord && asmCount > 0) || asmCount > 1)
                //{
                //    log.SaveError("AssemblyNameNotAvailable", "", false);
                //    //ShowMessage.Info("AssemblyNameNotAvailable", true, "", null);
                //    return false;
                //}
            }

            return base.BeforeSave(newRecord);
        }
    }
}
