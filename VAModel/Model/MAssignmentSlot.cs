using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using VAdvantage.Login;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MAssignmentSlot : IComparable
    {
        /**
	 *	Comparator Constructor
	 */
        public MAssignmentSlot():this(null, null, null, null, STATUS_TimeSlot)
        {
        }	//	MAssignmentSlot

        /**
         *	Timeslot Constructor
         *  @param startTime start time
         *  @param endTime end time
         */
        public MAssignmentSlot(DateTime? startTime, DateTime? endTime):this(startTime, endTime, null, null, STATUS_TimeSlot)
        {
            
            SetDisplay(DISPLAY_TIME_FROM);
        }	//	MAssignmentSlot

        /**
         *	Timeslot Constructor
         *  @param startTime start time
         *  @param endTime end time
         */
        public MAssignmentSlot(long startTime, long endTime):this(new DateTime(startTime), new DateTime(endTime), null, null, STATUS_TimeSlot)
        {
            
            SetDisplay(DISPLAY_TIME_FROM);
        }	//	MAssignmentSlot

        /**
         *	Non Assignment Constructor
         *  @param startTime start time
         *  @param endTime end time
         *  @param name name
         *  @param description description
         *  @param status status
         */
        public MAssignmentSlot(DateTime? startTime, DateTime? endTime,
            String name, String description, int status)
        {
            SetStartTime(startTime);
            SetEndTime(endTime);
            SetName(name);
            SetDescription(description);
            SetStatus(status);
            //
            //	log.fine( toString());
        }	//	MAssignmentSlot

        /**
         *	Assignment Constructor
         *  @param assignment MAssignment
         */
        public MAssignmentSlot(MResourceAssignment assignment)
        {
            SetStatus(assignment.IsConfirmed() ? STATUS_Confirmed : STATUS_NotConfirmed);
            SetMAssignment(assignment);
            //	log.fine( toString());
        }	//	MAssignmentSlot


        /** Not Available Code				*/
        public const int STATUS_NotAvailable = 0;
        /** Not Available Code				*/
        public const int STATUS_UnAvailable = 11;
        /** Not Available Code				*/
        public const int STATUS_NonBusinessDay = 12;
        /** Not Available Code				*/
        public const int STATUS_NotInSlotDay = 21;
        /** Not Available Code				*/
        public const int STATUS_NotInSlotTime = 22;
        /** Assignment Code					*/
        public const int STATUS_NotConfirmed = 101;
        /** Assignment Code					*/
        public const int STATUS_Confirmed = 102;

        /** Assignment Code					*/
        public const int STATUS_TimeSlot = 100000;

        /**	Start Time						*/
        private DateTime? _startTime;
        /**	End Time						*/
        private DateTime? _endTime;
        /** Name							*/
        private String _name;
        /** Description						*/
        private String _description;
        /**	Status							*/
        private int _status = STATUS_NotAvailable;
        /** Y position						*/
        private int _yStart = 0;
        private int _yEnd = 0;
        private int _xPos = 0;
        private int _xMax = 1;

        /**	The assignment					*/
        private MResourceAssignment _mAssignment;

        /**	Language used for formatting			*/
        private Login.Language _language = Login.Language.GetLoginLanguage();

        /** toString displays everything			*/
        public const int DISPLAY_ALL = 0;

        /** toString displays formatted time from	*/
        public const int DISPLAY_TIME_FROM = 1;
        /** toString displays formatted time from-to		*/
        public const int DISPLAY_TIME_FROM_TO = 1;
        /** toString displays formatted day time from-to	*/
        public const int DISPLAY_DATETIME_FROM_TO = 1;
        /** toString displays name					*/
        public const int DISPLAY_NAME = 1;
        /** toString displays name and optional description	*/
        public const int DISPLAY_NAME_DESCRIPTION = 1;
        /** toString displays formatted all info	*/
        public const int DISPLAY_FULL = 1;

        /**	DisplayMode								*/
        private int _displayMode = DISPLAY_FULL;

        /*************************************************************************/

        /**
         * 	Set Status
         * 	@param status STATUS_..
         */
        public void SetStatus(int status)
        {
            _status = status;
        }	//	setStatus

        /**
         * 	Get Status
         * 	@return STATUS_..
         */
        public int GetStatus()
        {
            return _status;
        }	//	getStatus

        /**
         * 	Is the Slot an Assignment?
         * 	@return true if slot is an assignment
         */
        public bool IsAssignment()
        {
            return (_status == STATUS_NotConfirmed || _status == STATUS_Confirmed);
        }	//	isAssignment

        /**
         * 	Get Color for Status
         *  @param background true if background - or foreground
         * 	@return Color
         */
        public System.Drawing.Color GetColor(bool background)
        {
            //	Not found, Inactive, not available
            if (_status == STATUS_NotAvailable)
                return background ? Color.Gray : Color.Magenta;

            //	Holiday
            else if (_status == STATUS_UnAvailable)
                return background ? Color.Gray : Color.Pink;

            //	Vacation
            else if (_status == STATUS_NonBusinessDay)
                return background ? Color.LightGray : Color.Red;

            //	Out of normal hours
            else if (_status == STATUS_NotInSlotDay || _status == STATUS_NotInSlotTime)
                return background ? Color.LightGray : Color.Black;

            //	Assigned
            else if (_status == STATUS_NotConfirmed)
                return background ? Color.Blue : Color.White;

            //	Confirmed
            else if (_status == STATUS_Confirmed)
                return background ? Color.Blue : Color.Black;

            //	Unknown
            return background ? Color.Black : Color.White;
        }	//	getColor

        /*************************************************************************/

        /**
         * 	Get Start time
         * 	@return start time
         */
        public DateTime? GetStartTime()
        {
            return _startTime;
        }

        /**
         * 	Set Start time
         *  @param startTime start time, if null use current time
         */
        public void SetStartTime(DateTime? startTime)
        {
            if (startTime == null)
                _startTime = new DateTime(Classes.CommonFunctions.CurrentTimeMillis());//  System.currentTimeMillis());
            else
                _startTime = startTime;
        }	//	setStartTime

        /**
         * 	Get End time
         * 	@return end time
         */
        public DateTime? GetEndTime()
        {
            return _endTime;
        }

        /**
         *  Set End time
         *  @param endTime end time, if null use start time
         */
        public void SetEndTime(DateTime? endTime)
        {
            if (endTime == null)
                _endTime = _startTime;
            else
                _endTime = endTime;
        }

        /*************************************************************************/

        /**
         * 	Set Assignment
         * 	@param assignment MAssignment
         */
        public void SetMAssignment(MResourceAssignment assignment)
        {
            if (assignment == null)
                return;
            if (!IsAssignment())
                throw new ArgumentException("Assignment Slot not an Assignment");
            //
            _mAssignment = assignment;
            SetStartTime(_mAssignment.GetAssignDateFrom());
            SetEndTime(_mAssignment.GetAssignDateTo());
            SetName(_mAssignment.GetName());
            SetDescription(_mAssignment.GetDescription());
            SetStatus(_mAssignment.IsConfirmed() ? STATUS_Confirmed : STATUS_NotConfirmed);
        }	//	setMAssignment

        /**
         * 	Get Assugnment
         * 	@return assignment
         */
        public MResourceAssignment GetMAssignment()
        {
            return _mAssignment;
        }	//	getAssignment

        /**
         *  Set Name
         * 	@param name name
         */
        public void SetName(String name)
        {
            if (name == null)
                _name = "";
            else
                _name = name;
        }	//	setName

        /**
         * 	Get Name
         *  @return name
         */
        public String GetName()
        {
            return _name;
        }	//	getName

        /**
         * 	Set Description
         * 	@param description description
         */
        public void SetDescription(String description)
        {
            if (description == null)
                _description = "";
            else
                _description = description;
        }	//	setDescription

        /**
         * 	Get Description
         *  @return description
         */
        public String GetDescription()
        {
            return _description;
        }	//	getDescription

        /*************************************************************************/

        /**
         * 	Set Y position
         * 	@param yStart zero based Y start index
         * 	@param yEnd zero based Y end index
         */
        public void SetY(int yStart, int yEnd)
        {
            _yStart = yStart;
            _yEnd = yEnd;
        }	//	setY

        /**
         * 	Get Y start position
         * 	@return zero based Y start index
         */
        public int GetYStart()
        {
            return _yStart;
        }	//	getYStart

        /**
         * 	Get Y end position
         * 	@return zero based Y end index
         */
        public int GetYEnd()
        {
            return _yEnd;
        }	//	setYEnd

        /**
         * 	Set X position
         * 	@param xPos zero based X position index
         * 	@param xMax number of parallel columns
         */
        public void SetX(int xPos, int xMax)
        {
            _xPos = xPos;
            if (xMax > _xMax)
                _xMax = xMax;
        }	//	setX

        /**
         * 	Get X position
         * 	@return zero based X position index
         */
        public int GetXPos()
        {
            return _xPos;
        }	//	setXPos

        /**
         * 	Get X columns
         * 	@return number of parallel columns
         */
        public int GetXMax()
        {
            return _xMax;
        }	//	setXMax

        /*************************************************************************/

        /**
         * 	Set Language
         * 	@param language language
         */
        public void SetLanguage(Language language)
        {
            _language = language;
        }	//	setLanguage

        /**
         * 	Set Display Mode of toString()
         * 	@param displayMode DISPLAY_
         */
        public void SetDisplay(int displayMode)
        {
            _displayMode = displayMode;
        }	//	setDisplay


        /**
         * 	String representation
         *  @return info
         */
        public override String ToString()
        {
            if (_displayMode == DISPLAY_TIME_FROM)
                return GetInfoTimeFrom();
            else if (_displayMode == DISPLAY_TIME_FROM_TO)
                return GetInfoTimeFromTo();
            else if (_displayMode == DISPLAY_DATETIME_FROM_TO)
                return GetInfoDateTimeFromTo();
            else if (_displayMode == DISPLAY_NAME)
                return _name;
            else if (_displayMode == DISPLAY_NAME_DESCRIPTION)
                return GetInfoNameDescription();
            else if (_displayMode == DISPLAY_FULL)
                return GetInfo();

            //	DISPLAY_ALL
            StringBuilder sb = new StringBuilder("MAssignmentSlot[");
            sb.Append(_startTime).Append("-").Append(_endTime)
                .Append("-Status=").Append(_status).Append(",Name=")
                .Append(_name).Append(",").Append(_description).Append("]");
            return sb.ToString();
        }	//	toString

        /**
         * 	Get Info with Time From
         *  @return info 00:00
         */
        public String GetInfoTimeFrom()
        {
            return ""; // _language. .GetTimeFormat(). .format(m_startTime);
        }	//	getInfoTimeFrom

        /**
         * 	Get Info with Time From-To
         *  @return info 00:00 - 01:00
         */
        public String GetInfoTimeFromTo()
	{
		StringBuilder sb = new StringBuilder();
		//sb.Append(_language.GetTimeFormat().format(m_startTime))
        sb.Append(_startTime.ToString())
			.Append(" - ")
            .Append(_endTime.ToString());
			//.Append(_language.getTimeFormat().format(m_endTime));
		return sb.ToString();
	}	//	getInfoTimeFromTo

        /**
         * 	Get Info with Date & Time From-To
         *  @return info 12/12/01 00:00 - 01:00 or 12/12/01 00:00 - 12/13/01 01:00
         */
        public String GetInfoDateTimeFromTo()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(_startTime.ToString())
                //sb.Append(_language.getDateTimeFormat().format(m_startTime))
                .Append(" - ");
            if (TimeUtil.IsSameDay(_startTime, _endTime))
            {
                //   sb.Append(m_language.getTimeFormat().format(m_endTime));
                sb.Append(_endTime.ToString());
            }
            else
            {
                //m_language.getDateTimeFormat().format(m_endTime);
                sb.Append(_endTime.ToString());
            }
            return sb.ToString();
        }

        /**
         * 	Get Info with Name and optional Description
         * 	@return Name (Description)
         */
        public String GetInfoNameDescription()
        {
            StringBuilder sb = new StringBuilder(_name);
            if (_description.Length > 0)
                sb.Append(" (").Append(_description).Append(")");
            return sb.ToString();
        }	//	getInfoNameDescription

        /**
         *	Get Info with Date, Time From-To Name Description
         * 	@return 12/12/01 00:00 - 01:00: Name (Description)
         */
        public String GetInfo()
        {
            StringBuilder sb = new StringBuilder(GetInfoDateTimeFromTo());
            sb.Append(": ").Append(_name);
            if (_description.Length > 0)
                sb.Append(" (").Append(_description).Append(")");
            return sb.ToString();
        }	//	getInfo

        /*************************************************************************/

        /**
         * 	Returns true if time is between start and end Time.
         *  Date part is ignored.
         *  <pre>
         *  Example:
         *  - Slots: 0:00-9:00 - 9:00-10:00 - 10:00-11:00 - ...
         *  - inSlot (9:00, false) -> 1		//	start time
         *  - inSlot (10:00, true) -> 1		//	end time
         *  </pre>
         * 	@param time time of the day
         *  @param endTime if true, the end time is included
         * 	@return true if within slot
         */
        public bool InSlot(DateTime? time, bool endTime)
        {
            //	Compare	--
            //GregorianCalendar cal = new GregorianCalendar();
            //cal.setTime(time);
            //cal.set(Calendar.YEAR, 1970);
            //cal.set(Calendar.DAY_OF_YEAR, 1);
            ////	handle -00:00 (end time)
            //if (endTime && cal.get(Calendar.HOUR_OF_DAY) == 0 && cal.get(Calendar.MINUTE) == 0)
            //{
            //    cal.set(Calendar.HOUR_OF_DAY, 23);
            //    cal.set(Calendar.MINUTE, 59);
            //}
            //Time compare = new Time (cal.getTimeInMillis());
            ////	Start Time --
            //cal.setTime(m_startTime);
            //cal.set(Calendar.YEAR, 1970);
            //cal.set(Calendar.DAY_OF_YEAR, 1);
            //Time start = new Time (cal.getTimeInMillis());
            ////	End time --
            //cal.setTime(m_endTime);
            //cal.set(Calendar.YEAR, 1970);
            //cal.set(Calendar.DAY_OF_YEAR, 1);
            //if (cal.get(Calendar.HOUR_OF_DAY) == 0 && cal.get(Calendar.MINUTE) == 0)
            //{
            //    cal.set(Calendar.HOUR_OF_DAY, 23);
            //    cal.set(Calendar.MINUTE, 59);
            //}
            //Time end = new Time (cal.getTimeInMillis());

            ////	before start			x |---|
            //if (compare.before(start))
            //{
            ////	System.out.println("InSlot-false Compare=" + compare + " before start " + start);
            //    return false;
            //}
            ////	after end				|---| x
            //if (compare.after(end))
            //{
            ////	System.out.println("InSlot-false Compare=" + compare + " after end " + end);
            //    return false;
            //}

            ////	start					x---|
            //if (!endTime && compare.equals(start))
            //{
            ////	System.out.println("InSlot-true Compare=" + compare + " = Start=" + start);
            //    return true;
            //}

            ////
            ////	end						|---x
            //if (endTime && compare.equals(end))
            //{
            ////	System.out.println("InSlot-true Compare=" + compare + " = End=" + end);
            //    return true;
            //}
            ////	between start/end		|-x-|
            //if (compare.before(end))
            //{
            ////	System.out.println("InSlot-true Compare=" + compare + " before end " + end);
            //    return true;
            //}
            return false;
        }	//	inSlot

        /*************************************************************************/

        /**
         * Compares its two arguments for order.  Returns a negative integer,
         * zero, or a positive integer as the first argument is less than, equal
         * to, or greater than the second.
         *
         * @param obj the first object to be compared.
         * @param o2 the second object to be compared.
         * @return a negative integer, zero, or a positive integer as the
         * 	       first argument is less than, equal to, or greater than the
         *	       second.
         * @throws ClassCastException if the arguments' types prevent them from
         * 	       being compared by this Comparator.
         */
        public int CompareTo(Object obj)
        {
            MAssignmentSlot slot = (MAssignmentSlot)obj;

            //	Start Date
            int result = GetStartTime().Value.CompareTo(slot.GetStartTime());
            if (result != 0)
                return result;
            //	Status
            result = slot.GetStatus() - GetStatus();
            if (result != 0)
                return result;
            //	End Date
            result = GetEndTime().Value.CompareTo(slot.GetEndTime());
            if (result != 0)
                return result;
            //	Name
            result = GetName().CompareTo(slot.GetName());
            if (result != 0)
                return result;
            //	Description
            return GetDescription().CompareTo(slot.GetDescription());
        }	//	compare

        /**
         * Indicates whether some other object is &quot;equal to&quot; this
         * Comparator.
         * @param   obj   the reference object with which to compare.
         * @return  <code>true</code> only if the specified object is also
         *		a comparator and it imposes the same ordering as this
         *		comparator.
         * @see     java.lang.Object#equals(java.lang.Object)
         * @see java.lang.Object#hashCode()
         */
        public override bool Equals(Object obj)
        {
            if (obj is MAssignmentSlot)
            {
                MAssignmentSlot cmp = (MAssignmentSlot)obj;
                if (_startTime.Equals(cmp.GetStartTime())
                    && _endTime.Equals(cmp.GetEndTime())
                    && _status == cmp.GetStatus()
                    && _name.Equals(cmp.GetName())
                    && _description.Equals(cmp.GetDescription()))
                    return true;
            }
            return false;
        }	//	equals

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /**
         * 	HashCode of MAssignmentSlot
         * 	@return has code
         */
        public int HashCode()
        {
            return _startTime.GetHashCode() + _endTime.GetHashCode() + _status
                + _name.GetHashCode() + _description.GetHashCode();
        }	//	hashCode
    }
}
