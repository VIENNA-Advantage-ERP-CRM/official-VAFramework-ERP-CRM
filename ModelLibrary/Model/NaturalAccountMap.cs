using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Logging;
using System.IO;
using VAdvantage.Model;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using System.Data;

namespace VAdvantage.Model
{
    public class NaturalAccountMap<K, V> : CCache<K, V>
    {
        public NaturalAccountMap(Ctx ctx, Trx trx)
            : base("NaturalAccountMap", 100)
        {
            m_ctx = ctx;
            m_trx = trx;
        }   //  NaturalAccountMap

        /** Context			*/
        private Ctx  m_ctx = null;
        /** Transaction		*/
        private Trx m_trx = null;

        /** Map of Values and Element	*/
        private Dictionary<String, MElementValue> m_valueMap = new Dictionary<String, MElementValue>();
        /**	Logger			*/
        private static VLogger log = VLogger.GetVLogger(typeof(NaturalAccountMap<K, V>).FullName);

        public bool SaveAccounts(int AD_Client_ID, int AD_Org_ID, int C_Element_ID)
        {
            log.Config("");

            foreach (string key in s_base.Keys)
            {
                MElementValue na = (MElementValue)s_base[key];
                na.SetAD_Client_ID(AD_Client_ID);
                na.SetAD_Org_ID(AD_Org_ID);
                na.SetC_Element_ID(C_Element_ID);
                na.SetVIS_DefaultAccount(key);

                if (!na.Save())
                {
                    return false;
                }

            }
            return true;
        }   //  saveAccounts


        public int GetC_ElementValue_ID(String key)
        {
            MElementValue na = null;
            if (s_base.ContainsKey(key))
                na = (MElementValue)s_base[key];
            if (na == null)
                return 0;
            return na.GetC_ElementValue_ID();
        }   //  getC_ElementValue_ID


        public String ParseFile(FileStream file)
        {
            //log.Config(file.Name);
            String line = null;
            try
            {
                //  see FileImport
                FileStream fs = file; // new FileStream("E:\\FrameworkLive\\Framework\\bin\\Debug\\data\\import\\AccountingUS.csv", FileMode.Open);
                StreamReader inn = new StreamReader(fs);
                //	not safe see p108 Network pgm
                String errMsg = "";

                //  read lines
                while ((line = inn.ReadLine()) != null && errMsg.Length == 0)
                    errMsg = ParseLine(line);
                line = "";
                inn.Close();

                //  Error
                if (errMsg.Length != 0)
                    return errMsg;
            }
            catch (Exception ioe)
            {
                String s = ioe.Message;
                if (s == null || s.Length == 0)
                    s = ioe.ToString();
                return "Parse Error: Line=" + line + " - " + s;
            }
            return "";
        }   //  parse


        private static CCache<string, MElementValue> s_base = new CCache<string, MElementValue>("Default_Value", 10);

        /// <summary>
        /// Create Account Entry for Default Accounts only.
        /// </summary>
        /// <param name="line">line with info
        /// <para>
        /// Line format (9 fields)
        /// 1	A   [Account Value]
        /// 2	B   [Account Name]
        /// 3	C   [Description]
        /// 4	D   [Account Type]
        /// 5	E   [Account Sign]
        /// 6	F   [Document Controlled]
        /// 7	G   [Summary Account]
        /// 8	H   [Default_Account]
        /// 9	I   [Parent Value] - ignored
        /// </para>
        /// </param>
        /// <returns>error message or "" if OK</returns>
        public String ParseLine(String line)
        {
            log.Config(line);

            //  Fields with ',' are enclosed in "
            StringBuilder newLine = new StringBuilder();
            StringTokenizer st = new StringTokenizer(line, "\"", false);
            newLine.Append(st.NextToken());         //  first part
            while (st.HasMoreElements())
            {
                String s = st.NextToken();          //  enclosed part
                newLine.Append(s.Replace(',', ' ')); //  remove ',' with space
                if (st.HasMoreTokens())
                    newLine.Append(st.NextToken()); //  unenclosed
            }
            //  add space at the end        - tokenizer does not count empty fields
            newLine.Append(" ");

            //  Parse Line - replace ",," with ", ,"    - tokenizer does not count empty fields
            String pLine = Utility.Util.Replace(newLine.ToString(), ",,", ", ,");
            pLine = Utility.Util.Replace(pLine, ",,", ", ,");
            st = new StringTokenizer(pLine, ",", false);
            //  All fields there ?
            if (st.CountTokens() == 1)
            {
                log.Log(Level.SEVERE, "Ignored: Require ',' as separator - " + pLine);
                return "";
            }
            if (st.CountTokens() < 9)
            {
                log.Log(Level.SEVERE, "Ignored: FieldNumber wrong: " + st.CountTokens() + " - " + pLine);
                return "";
            }

            //  Fill variables
            String Value = null, Name = null, Description = null,
                AccountType = null, AccountSign = null, IsDocControlled = null,
                IsSummary = null, Default_Account = null;
            //
            for (int i = 0; i < 8 && st.HasMoreTokens(); i++)
            {
                String s = st.NextToken().Trim();
                //  Ignore, if is it header line
                if (s.StartsWith("[") && s.EndsWith("]"))
                    return "";
                if (s == null)
                    s = "";
                //
                if (i == 0)			//	A - Value
                    Value = s;
                else if (i == 1)	//	B - Name
                    Name = s;
                else if (i == 2)	//	C - Description
                    Description = s;
                else if (i == 3)	//	D - Type
                    AccountType = s.Length > 0 ? s[0].ToString() : "E";
                else if (i == 4)	//	E - Sign
                    AccountSign = s.Length > 0 ? s[0].ToString() : "N";
                else if (i == 5)	//	F - DocControlled
                    IsDocControlled = s.Length > 0 ? s[0].ToString() : "N";
                else if (i == 6)	//	G - IsSummary
                    IsSummary = s.Length > 0 ? s[0].ToString() : "N";
                else if (i == 7)	//	H - Default_Account
                    Default_Account = s;
            }

            //	Ignore if Value & Name are empty (no error message)
            if ((Value == null || Value.Length == 0) && (Name == null || Name.Length == 0))
                return "";

            //  Default Account may be blank
            if (Default_Account == null || Default_Account.Length == 0)
                //	Default_Account = String.valueOf(s_keyNo++);
                return "";

            //	No Summary Account
            if (IsSummary == null || IsSummary.Length == 0)
                IsSummary = "N";
            if (!IsSummary.Equals("N"))
                return "";

            //  Validation
            if (AccountType == null || AccountType.Length == 0)
                AccountType = "E";

            if (AccountSign == null || AccountSign.Length == 0)
                AccountSign = "N";
            if (IsDocControlled == null || IsDocControlled.Length == 0)
                IsDocControlled = "N";


            //	log.config( "Value=" + Value + ", AcctType=" + AccountType
            //		+ ", Sign=" + AccountSign + ", Doc=" + docControlled
            //		+ ", Summary=" + summary + " - " + Name + " - " + Description);

            try
            {
                //	Try to find - allows to use same natutal account for multiple default accounts 
                MElementValue na = null;
                if (m_valueMap.ContainsKey(Value))
                    na = (MElementValue)m_valueMap[Value];
                if (na == null)
                {
                    //  Create Account - save later
                    na = new MElementValue(m_ctx, Value, Name, Description, AccountType, AccountSign, IsDocControlled.ToUpper().StartsWith("Y"), IsSummary.ToUpper().StartsWith("Y"), m_trx);
                    m_valueMap[Value] = na;
                }

                //  Add to Cache
                s_base.Add(Default_Account.ToUpper(), na);
            }
            catch (Exception e)
            {
                return (e.Message);
            }

            return "";
        }   //  parseLine





        public String ParseFile(FileStream file, int AD_Client_ID, int AD_Org_ID, int C_Element_ID, MTree tree)
        {
            //log.Config(file.Name);
            String line = null;
            try
            {
                //  see FileImport
                FileStream fs = file; // new FileStream("E:\\FrameworkLive\\Framework\\bin\\Debug\\data\\import\\AccountingUS.csv", FileMode.Open);
                StreamReader inn = new StreamReader(fs);
                //	not safe see p108 Network pgm
                String errMsg = "";

                //  read lines
                while ((line = inn.ReadLine()) != null && errMsg.Length == 0)
                    errMsg = ParseAndSaveLine(line, AD_Client_ID, AD_Org_ID, C_Element_ID,tree);
                line = "";
                inn.Close();

                //  Error
                if (errMsg.Length != 0)
                    return errMsg;
            }
            catch (Exception ioe)
            {
                String s = ioe.Message;
                if (s == null || s.Length == 0)
                    s = ioe.ToString();
                return "Parse Error: Line=" + line + " - " + s;
            }
            return "";
        }   //  parse


        public String ParseAndSaveLine(String line, int AD_Client_ID, int AD_Org_ID, int C_Element_ID,MTree tree)
        {
            log.Config(line);

            //  Fields with ',' are enclosed in "
            StringBuilder newLine = new StringBuilder();
            StringTokenizer st = new StringTokenizer(line, "\"", false);
            newLine.Append(st.NextToken());         //  first part
            while (st.HasMoreElements())
            {
                String s = st.NextToken();          //  enclosed part
                newLine.Append(s.Replace(',', ' ')); //  remove ',' with space
                if (st.HasMoreTokens())
                    newLine.Append(st.NextToken()); //  unenclosed
            }
            //  add space at the end        - tokenizer does not count empty fields
            newLine.Append(" ");

            //  Parse Line - replace ",," with ", ,"    - tokenizer does not count empty fields
            String pLine = Utility.Util.Replace(newLine.ToString(), ",,", ", ,");
            pLine = Utility.Util.Replace(pLine, ",,", ", ,");
            st = new StringTokenizer(pLine, ",", false);
            //  All fields there ?
            if (st.CountTokens() == 1)
            {
                log.Log(Level.SEVERE, "Ignored: Require ',' as separator - " + pLine);
                return "";
            }
            if (st.CountTokens() < 9)
            {
                log.Log(Level.SEVERE, "Ignored: FieldNumber wrong: " + st.CountTokens() + " - " + pLine);
                return "";
            }

            //  Fill variables
            String Value = null, Name = null, Description = null,
                AccountType = null, AccountSign = null, IsDocControlled = null,
                IsSummary = null, Default_Account = null;
            int accountParent = -1;
            //
            for (int i = 0; i < 9 && st.HasMoreTokens(); i++)
            {
                String s = st.NextToken().Trim();
                //  Ignore, if is it header line
                if (s.StartsWith("[") && s.EndsWith("]"))
                    return "";
                if (s == null)
                    s = "";
                //
                if (i == 0)			//	A - Value
                    Value = s;
                else if (i == 1)	//	B - Name
                    Name = s;
                else if (i == 2)	//	C - Description
                    Description = s;
                else if (i == 3)	//	D - Type
                    AccountType = s.Length > 0 ? s[0].ToString() : "E";
                else if (i == 4)	//	E - Sign
                    AccountSign = s.Length > 0 ? s[0].ToString() : "N";
                else if (i == 5)	//	F - DocControlled
                    IsDocControlled = s.Length > 0 ? s[0].ToString() : "N";
                else if (i == 6)	//	G - IsSummary
                    IsSummary = s.Length > 0 ? s[0].ToString() : "N";
                else if (i == 7)	//	H - Default_Account
                    Default_Account = s;
                else if (i == 8)
                    accountParent = Util.GetValueOfInt(s);
            }

            //	Ignore if Value & Name are empty (no error message)
            if ((Value == null || Value.Length == 0) && (Name == null || Name.Length == 0))
                return "";
            ////////////////////
            //Commented By Lakhwinder
            ////  Default Account may be blank
            //if (Default_Account == null || Default_Account.Length == 0)
            //    //	Default_Account = String.valueOf(s_keyNo++);
            //    return "";

            ////	No Summary Account
            //if (IsSummary == null || IsSummary.Length == 0)
            //    IsSummary = "N";
            //if (!IsSummary.Equals("N"))
            //    return "";

            ////  Validation
            //if (AccountType == null || AccountType.Length == 0)
            //    AccountType = "E";

            //if (AccountSign == null || AccountSign.Length == 0)
            //    AccountSign = "N";
            //if (IsDocControlled == null || IsDocControlled.Length == 0)
            //    IsDocControlled = "N";
            //////////////////////

            //	log.config( "Value=" + Value + ", AcctType=" + AccountType
            //		+ ", Sign=" + AccountSign + ", Doc=" + docControlled
            //		+ ", Summary=" + summary + " - " + Name + " - " + Description);

            try
            {
                //	Try to find - allows to use same natutal account for multiple default accounts 
                MElementValue na = null;
                if (m_valueMap.ContainsKey(Value))
                    na = (MElementValue)m_valueMap[Value];
                if (na == null)
                {
                    //  Create Account - save later
                    na = new MElementValue(m_ctx, Value, Name, Description, AccountType, AccountSign, IsDocControlled.ToUpper().StartsWith("Y"), IsSummary.ToUpper().StartsWith("Y"), m_trx);
                    int refElementID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT C_ElementValue_ID FROM C_ElementValue
                                                                                    WHERE IsActive='Y' AND AD_Client_ID=" + na.GetAD_Client_ID() + " AND Value='" + accountParent + @"'
                                                                                    AND C_Element_ID=" + C_Element_ID, null, m_trx));
                    na.SetRef_C_ElementValue_ID(refElementID);
                    m_valueMap[Value] = na;
                    na.SetAD_Client_ID(AD_Client_ID);
                    na.SetAD_Org_ID(AD_Org_ID);
                    na.SetC_Element_ID(C_Element_ID);
                    na.SetVIS_DefaultAccount(Default_Account);
                    if (!na.Save(m_trx))
                    {
                        return "Acct Element Values NOT inserted";
                        //m_info.Append(Msg.Translate(m_lang, "C_ElementValue_ID")).Append(" # ").Append(m_nap.Count).Append("\n");
                    }
                    VAdvantage.Model.MTreeNode mNode = VAdvantage.Model.MTreeNode.Get(tree, na.Get_ID());
                    if (mNode == null)
                    {
                        mNode = new VAdvantage.Model.MTreeNode(tree, na.Get_ID());
                    }
                    ((VAdvantage.Model.PO)mNode).Set_Value("Parent_ID", refElementID);
                    if (!mNode.Save(m_trx))
                    {
                        return "Acct Element Values NOT inserted";
                    }
                }

                if (!(Default_Account == null || Default_Account.Length == 0))
                {
                    //  Add to Cache
                    s_base.Add(Default_Account.ToUpper(), na);
                }


            }
            catch (Exception e)
            {
                return (e.Message);
            }

            return "";
        }   // 
    }
}
