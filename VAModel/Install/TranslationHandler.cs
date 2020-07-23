using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Logging;
using VAdvantage.Login;
using VAdvantage.DataBase;
using System.Xml;
using VAdvantage.Install;
namespace VAdvantage.Install
{
    public class TranslationHandler
    {
        public TranslationHandler(int AD_Client_ID)
        {
            _AD_Client_ID = AD_Client_ID;
        }

        /**	Client							*/
        private int _AD_Client_ID = -1;
        /** Language						*/
        private String _AD_Language = null;
        /** Is Base Language				*/
        private bool _isBaseLanguage = false;
        /** Table							*/
        private String _TableName = null;
        /** Update SQL						*/
        private String _updateSQL = null;
        /** Current ID						*/
        private String _curID = null;
        /** Translated Flag					*/
        private String _trl = null;
        /** Current ColumnName				*/
        private String _curColumnName = null;
        /** Current Value					*/
        private StringBuilder _curValue = null;
        /**	SQL								*/
        private StringBuilder _sql = null;

        private DateTime _time = new DateTime();//  new DateTime((System.currentTimeMillis());

        private int _updateCount = 0;
        private int _wordCount = 0;

        private static VLogger log = VLogger.GetVLogger(typeof(TranslationHandler).FullName);//.class);

        private bool _ByExportID = true;

        /** Current ID						*/
        private String _curExportID = null;

        internal void SetByExportD(bool check)
        {
            _ByExportID = check;
        }



        /// <summary>
        /// Receive notification of the start of an element.    
        /// </summary>
        /// <param name="uri">namespace</param>
        /// <param name="localName">simple name</param>
        /// <param name="qName">qualified name</param>
        /// <param name="attributes">attributes</param>
        public void StartElement(String uri, String localName, String qName, List<string> attributes)
        {
            if (qName.Equals(Translation.XML_TAG) || qName.Equals(Translation.XML_TAG_Vienna) || qName.Equals(Translation.XML_TAG_Adam))
            {
                _AD_Language = attributes[0]; //attributes[Translation.XML_ATTRIBUTE_LANGUAGE].Value;
                _isBaseLanguage = Language.IsBaseLanguage(_AD_Language);
                _TableName = attributes[1];  //attributes[Translation.XML_ATTRIBUTE_TABLE].Value;
                _updateSQL = "UPDATE " + _TableName;
                if (!_isBaseLanguage)
                    _updateSQL += "_Trl";
                _updateSQL += " SET ";
                log.Fine("AD_Language=" + _AD_Language + ", Base=" + _isBaseLanguage + ", TableName=" + _TableName);
            }
            else if (qName.Equals(Translation.XML_ROW_TAG))
            {
                _curID = attributes[0];// Translation.XML_ROW_ATTRIBUTE_ID;
                _trl = attributes[1];// Translation.XML_ROW_ATTRIBUTE_TRANSLATED;

                if (_ByExportID || attributes.Count > 2)
                {
                    _curExportID = attributes[2];
                }
                else
                {
                    _curExportID = null;
                }

                //	log.finest( "ID=" + m_curID);
                _sql = new StringBuilder();
            }
            else if (qName.Equals(Translation.XML_VALUE_TAG))
            {
                _curColumnName = attributes[0];// Translation.XML_VALUE_ATTRIBUTE_COLUMN;      
            }
            else
            {
                log.Severe("UNKNOWN TAG: " + qName);
                
            }
            _curValue = new StringBuilder();
        }	//	startElement

        /// <summary>
        /// Receive notification of character data inside an element.	 
        /// </summary>
        /// <param name="ch">buffer</param>
        /// <param name="start">start</param>
        /// <param name="length">lenth</param>
        public void Characters(char[] ch)//, int start, int length)		
        {
            _curValue.Append(ch);//, start, length);	
        }	//	characters

        /// <summary>
        /// Receive notification of the end of an element.	
        /// </summary>
        /// <param name="uri">name space</param>
        /// <param name="localName">name</param>
        /// <param name="qName">qualified name</param>
        public void EndElement(String uri, String localName, String qName)
        {
            if (qName.Equals(Translation.XML_TAG))
            {
            }
            else if (qName.Equals(Translation.XML_ROW_TAG))
            {
                _time = DateTime.Now;
                //	Set section
                if (_sql.Length > 0)
                    _sql.Append(",");
                _sql.Append("Updated=").Append(DataBase.DB.TO_DATE(_time, false));
                if (!_isBaseLanguage)
                {
                    if (_trl != null
                        && ("Y".Equals(_trl) || "N".Equals(_trl)))
                        _sql.Append(",IsTranslated='").Append(_trl).Append("'");
                    else
                        _sql.Append(",IsTranslated='Y'");
                }
                //	Where section
                _sql.Append(" WHERE ");
                if (!_ByExportID || _curExportID == null)
                {
                    _sql.Append(_TableName).Append("_ID=").Append(_curID);
                }
                else
                {
                    string trlTable = _TableName;
                    if (!_isBaseLanguage)
                    {
                        trlTable = _TableName + "_Trl";
                    }

                    _sql.Append(_TableName).Append("_ID=").Append("(").Append(" SELECT ").Append(_TableName).Append("_ID ")
                        .Append(" FROM ").Append(_TableName).Append(" WHERE Export_ID = '").Append(_curExportID).Append("' )");
                }

                if (!_isBaseLanguage)
                    _sql.Append(" AND AD_Language='").Append(_AD_Language).Append("'");
                if (_AD_Client_ID >= 0)
                    _sql.Append(" AND AD_Client_ID=").Append(_AD_Client_ID);
                //	Update section
                _sql.Insert(0, _updateSQL);

                //	Execute
                int no = DataBase.DB.ExecuteQuery(_sql.ToString(), null, null);
                if (no == 1)
                {
                    if (VLogMgt.IsLevelFinest())
                        log.Fine(_sql.ToString());
                    _updateCount++;
                }
                else if (no == 0)
                    log.Warning("Not Found - " + _sql.ToString());
                else
                    log.Severe("Update Rows=" + no + " (Should be 1) - " + _sql.ToString());
            }
            else if (qName.Equals(Translation.XML_VALUE_TAG))
            {
                if (_sql.Length > 0)
                    _sql.Append(",");
                _sql.Append(_curColumnName).Append("=").Append(DataBase.DB.TO_STRING(_curValue.ToString()));
                _wordCount += Utility.Util.CountWords(_curValue.ToString());
            }
        }	//	endElementtran


        /// <summary>
        /// Get Number of updates	 
        /// </summary>
        /// <returns>update count</returns>
        public int GetUpdateCount()
        {
            return _updateCount;
        }
        public int GetWordCount()
        {
            return _wordCount;
        }

        public bool IsBaseLanguage()
        {
            return _isBaseLanguage;
        }

        public string GetTableName()
        {
            return _TableName;
        }
    }
}
