using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Utility;
using VAdvantage.Classes;
using VAdvantage.Logging;
using System.Data;
using VAdvantage.Model;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class ScheduleUtil
    {
        /**
	 *	Constructor
	 *  @param ctx context
	 */
        public ScheduleUtil(Ctx ctx)
        {
            _ctx = ctx;
        }	//	MSchedule

        private Ctx _ctx;
        private int _S_Resource_ID;
        private bool _isAvailable = true;
        private int _S_ResourceType_ID = 0;
        private int _C_UOM_ID = 0;

        private DateTime? _startDate = null;
        private DateTime? _endDate = null;

        /**	Resource Type Name			*/
        private String _typeName = null;
        /**	Resource Type Start Time	*/
        private DateTime? _slotStartTime = null;
        /** Resource Type End Time		*/
        private DateTime? _slotEndTime = null;

        /**	Time Slots						*/
        private MAssignmentSlot[] _timeSlots = null;


        /**	Begin Timestamp		1/1/1970			*/
        public static DateTime EARLIEST = new DateTime(1970, 1, 1);
        /**	End Timestamp		12/31/2070			*/
        public static DateTime LATEST = new DateTime(2070, 12, 31);

        /**	Logger			*/
        private static VLogger log = VLogger.GetVLogger(typeof(ScheduleUtil).FullName);


        /**************************************************************************
         * 	Get Assignments for timeframe.
         *  <pre>
         * 		- Resource is Active and Available
         * 		- Resource UnAvailability
         * 		- NonBusinessDay
         * 		- ResourceType Available
         *  </pre>
         *  @param S_Resource_ID resource
         *  @param start_Date start date
         *  @param end_Date optional end date, need to provide qty to calculate it
         *  @param qty optional qty in ResourceType UOM - ignored, if end date is not null
         *  @param getAll if true return all errors
         *	@param trxName transaction
         *  @return Array of existing Assigments or null - if free
         */
        //@SuppressWarnings("unchecked")
        public MAssignmentSlot[] GetAssignmentSlots(int S_Resource_ID,
            DateTime? start_Date, DateTime? end_Date,
            Decimal? qty, bool getAll, Trx trxName)
	{
		log.Config(start_Date.ToString());
		if (_S_Resource_ID != S_Resource_ID)
			GetBaseInfo (S_Resource_ID);
		//
		List<MAssignmentSlot> list = new List<MAssignmentSlot>();
		MAssignmentSlot ma = null;

		if (!_isAvailable)
		{
			ma = new MAssignmentSlot (EARLIEST, LATEST,
				Msg.GetMsg (_ctx, "ResourceNotAvailable"), "", MAssignmentSlot.STATUS_NotAvailable);
			if (!getAll)
				return new MAssignmentSlot[] {ma};
			list.Add(ma);
		}

		_startDate = start_Date;
		_endDate = end_Date;
		if (_endDate == null)
			_endDate = MUOMConversion.GetEndDate(_ctx, _startDate, _C_UOM_ID, qty);
		log.Fine( "- EndDate=" + _endDate);


		//	Resource Unavailability -------------------------------------------
	//	log.fine( "- Unavailability -");
		String sql = "SELECT Description, DateFrom, DateTo "
		  + "FROM S_ResourceUnavailable "
		  + "WHERE S_Resource_ID=@1"					//	#1
		  + " AND DateTo >= @2"						//	#2	start
		  + " AND DateFrom <= @3"					//	#3	end
		  + " AND IsActive='Y'";

        IDataReader dr = null; 
        System.Data.SqlClient.SqlParameter[] param = null;
		try
		{
	//		log.fine( sql, "ID=" + S_Resource_ID + ", Start=" + m_startDate + ", End=" + m_endDate);
            param = new System.Data.SqlClient.SqlParameter[3];
            param[0] = new System.Data.SqlClient.SqlParameter("@1",_S_Resource_ID);
            param[1] = new System.Data.SqlClient.SqlParameter("@2",_startDate);
            param[2] = new System.Data.SqlClient.SqlParameter("@3",_endDate);
			dr = DataBase.DB.ExecuteReader(sql,param,trxName);

            while (dr.Read())
			{
				ma = new MAssignmentSlot (TimeUtil.GetDay(dr.GetDateTime(1)),
					TimeUtil.GetNextDay(dr.GetDateTime(2)),	//	user entered date need to convert to not including end time
					Msg.GetMsg (_ctx, "ResourceUnAvailable"), dr.GetString(1),
					MAssignmentSlot.STATUS_UnAvailable);
			//	log.fine( "- Unavailable", ma);
				if (getAll)
					CreateDaySlot (list, ma);
				else
					list.Add(ma);
			}
			dr.Close();
            dr = null;
            param = null;
		}
		catch (Exception e)
		{
            if(dr!=null)
            {
                dr.Close();
            }
            dr = null;
            param = null;
			log.Log(Level.SEVERE, sql, e);
			ma = new MAssignmentSlot (EARLIEST, LATEST,
				Msg.GetMsg (_ctx, "ResourceUnAvailable"), e.ToString(),
				MAssignmentSlot.STATUS_UnAvailable);
		}
		if (ma != null && !getAll)
			return new MAssignmentSlot[] {ma};


		//	NonBusinessDay ----------------------------------------------------
	//	log.fine( "- NonBusinessDay -");
		//	"WHERE TRUNC(Date1) BETWEEN TRUNC(?) AND TRUNC(?)"   causes
		//	ORA-00932: inconsistent datatypes: expected NUMBER got TIMESTAMP
		sql = MRole.GetDefault(_ctx, false).AddAccessSQL (
			"SELECT Name, Date1 FROM C_NonBusinessDay "
			+ "WHERE TRUNC(Date1,'DD') BETWEEN @1 AND @2",
			"C_NonBusinessDay", false, false);	// not qualified - RO
		try
		{
			 DateTime? startDay = TimeUtil.GetDay(_startDate);
			 DateTime? endDay = TimeUtil.GetDay(_endDate);
	//		log.fine( sql, "Start=" + startDay + ", End=" + endDay);
            param = new System.Data.SqlClient.SqlParameter[2];
            param[0] = new System.Data.SqlClient.SqlParameter("@1",startDay);
            param[1] = new System.Data.SqlClient.SqlParameter("@2",endDay);
			 
            dr = DataBase.DB.ExecuteReader(sql,param,trxName);
			while (dr.Read())
			{
				ma = new MAssignmentSlot (TimeUtil.GetDay(dr.GetDateTime(1)),
					TimeUtil.GetNextDay(dr.GetDateTime(1)),	//	user entered date need to convert to not including end time
					Msg.GetMsg(_ctx, "NonBusinessDay"), dr.GetString(0),
					MAssignmentSlot.STATUS_NonBusinessDay);
				log.Finer("- NonBusinessDay " + ma);
				list.Add(ma);
			}
			dr.Close();
            dr = null;
			param = null;
		}
		catch (Exception e)
		{
            if(dr!=null)
            {
                dr.Close();
                dr = null;
            }
            param = null;

			log.Log(Level.SEVERE, sql, e);
			ma = new MAssignmentSlot (EARLIEST, LATEST,
				Msg.GetMsg(_ctx, "NonBusinessDay"), e.ToString(),
				MAssignmentSlot.STATUS_NonBusinessDay);
		}
		if (ma != null && !getAll)
			return new MAssignmentSlot[] {ma};


		//	ResourceType Available --------------------------------------------
	//	log.fine( "- ResourceTypeAvailability -");
		sql = "SELECT Name, IsTimeSlot,TimeSlotStart,TimeSlotEnd, "	//	1..4
			+ "IsDateSlot,OnMonday,OnTuesday,OnWednesday,"			//	5..8
			+ "OnThursday,OnFriday,OnSaturday,OnSunday "			//	9..12
			+ "FROM S_ResourceType "
			+ "WHERE S_ResourceType_ID=@1";
		try
		{
            param  = new System.Data.SqlClient.SqlParameter[1];
            param[0] =new System.Data.SqlClient.SqlParameter("@1",_S_ResourceType_ID);
			
            dr = DataBase.DB.ExecuteReader(sql,param,trxName);
            
            if (dr.Read())
			{
				_typeName = dr.GetString(0);
				//	TimeSlot
				if ("Y".Equals(dr.GetString(1)))
				{
					_slotStartTime = TimeUtil.GetDayTime (_startDate, dr.GetDateTime(2));
					_slotEndTime = TimeUtil.GetDayTime (_endDate, dr.GetDateTime(3));
					if (TimeUtil.InRange(_startDate, _endDate, _slotStartTime, _slotEndTime))
					{
						ma = new MAssignmentSlot (_slotStartTime, _slotEndTime,
							Msg.GetMsg(_ctx, "ResourceNotInSlotTime"), _typeName,
							MAssignmentSlot.STATUS_NotInSlotTime);
						if (getAll)
							CreateTimeSlot (list,
								dr.GetDateTime(2), dr.GetDateTime(3));
					}
				}	//	TimeSlot

				//	DaySlot
				if ("Y".Equals(dr.GetString(4)))
				{
					if (TimeUtil.InRange(_startDate, _endDate,
						"Y".Equals(dr.GetString(5)), "Y".Equals(dr.GetString(6)), 				//	Mo..Tu
						"Y".Equals(dr.GetString(7)), "Y".Equals(dr.GetString(8)), "Y".Equals(dr.GetString(9)),	//  We..Fr
						"Y".Equals(dr.GetString(10)), "Y".Equals(dr.GetString(11))))
					{
						ma = new MAssignmentSlot (_startDate, _endDate,
							Msg.GetMsg(_ctx, "ResourceNotInSlotDay"), _typeName,
							MAssignmentSlot.STATUS_NotInSlotDay);
						if (getAll)
							CreateDaySlot (list,
								"Y".Equals(dr.GetString(5)), "Y".Equals(dr.GetString(6)), 		//	Mo..Tu
								"Y".Equals(dr.GetString(7)), "Y".Equals(dr.GetString(8)), "Y".Equals(dr.GetString(9)),	//  We..Fr
								"Y".Equals(dr.GetString(10)), "Y".Equals(dr.GetString(11)));
					}
				}	//	DaySlot

			}
			dr.Close();
            dr = null;
            param = null;
		}
		catch (Exception e)
		{
            if(dr!=null)
            {
                dr.Close();
                dr= null;
            }
            param = null;

			log.Log(Level.SEVERE, sql, e);
			ma = new MAssignmentSlot (EARLIEST, LATEST,
				Msg.GetMsg(_ctx, "ResourceNotInSlotDay"), e.ToString(),
				MAssignmentSlot.STATUS_NonBusinessDay);
		}
		if (ma != null && !getAll)
			return new MAssignmentSlot[] {ma};

		//	Assignments -------------------------------------------------------
		sql = "SELECT S_ResourceAssignment_ID "
			+ "FROM S_ResourceAssignment "
			+ "WHERE S_Resource_ID=@1"					//	#1
			+ " AND AssignDateTo >= @2"					//	#2	start
			+ " AND AssignDateFrom <= @3"				//	#3	end
			+ " AND IsActive='Y'";
		try
		{
            param = new System.Data.SqlClient.SqlParameter[3];
            param[0] = new System.Data.SqlClient.SqlParameter("@1",_S_Resource_ID);
            param[1] = new System.Data.SqlClient.SqlParameter("@2",_startDate);
            param[2] = new System.Data.SqlClient.SqlParameter("@3",_endDate);
			
            dr =DataBase.DB.ExecuteReader(sql,param,trxName);
			while (dr.Read())
			{
				MResourceAssignment mAssignment = 
					new MResourceAssignment(_ctx, Utility.Util.GetValueOfInt(dr[0]), trxName);
				ma = new MAssignmentSlot (mAssignment);
				if (!getAll)
					break;
				list.Add(ma);
			}
			dr.Close();
            dr = null;
		}
		catch (Exception e)
		{
			log.Log(Level.SEVERE, sql, e);
			ma = new MAssignmentSlot (EARLIEST, LATEST,
				Msg.Translate(_ctx, "S_R"), e.ToString(),
				MAssignmentSlot.STATUS_NotConfirmed);
		}
		if (ma != null && !getAll)
			return new MAssignmentSlot[] {ma};

		/*********************************************************************/

		//	fill m_timeSlots (required for layout)
		CreateTimeSlots();

		//	Clean list - date range
		List<MAssignmentSlot> clean = new List<MAssignmentSlot>(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			MAssignmentSlot mas = (MAssignmentSlot)list[i];
			if ((mas.GetStartTime().Equals(_startDate) || mas.GetStartTime() > _startDate)
					&& (mas.GetEndTime().Equals(_endDate)) || mas.GetEndTime()< _endDate)
				clean.Add(mas);
		}
		//	Delete Unavailability TimeSlots when all day assigments exist
		MAssignmentSlot[] sorted = new MAssignmentSlot[clean.Count];
		sorted =  clean.ToArray();
		Array.Sort(sorted);	//	sorted by start/end date
		list.Clear();	//	used as day list
		clean.Clear();	//	cleaned days
		DateTime? sortedDay = null;
		for (int i = 0; i < sorted.Length; i++)
		{
			if (sortedDay == null)
				sortedDay = TimeUtil.GetDay(sorted[i].GetStartTime());
			if (sortedDay.Equals(TimeUtil.GetDay(sorted[i].GetStartTime())))
				list.Add(sorted[i]);
			else
			{
				//	process info list -> clean
				LayoutSlots (list, clean);
				//	prepare next
				list.Clear();
				list.Add(sorted[i]);
				sortedDay = TimeUtil.GetDay(sorted[i].GetStartTime());
			}
		}
		//	process info list -> clean
		LayoutSlots (list, clean);

		//	Return
		MAssignmentSlot[] retValue = new MAssignmentSlot[clean.Count];
		retValue =   clean.ToArray();
		Array.Sort(retValue);	//	sorted by start/end date
		return retValue;
	}	//	getAssignmentSlots

        /**
         * 	Copy valid Slots of a day from list to clear and layout
         * 	@param list list with slos of the day
         * 	@param clean list with only valid slots
         */
        //@SuppressWarnings("unchecked")
        private void LayoutSlots(List<MAssignmentSlot> list, List<MAssignmentSlot> clean)
        {
            int size = list.Count;
            //	System.out.println("Start List=" + size + ", Clean=" + clean.size());
            if (size == 0)
                return;
            else if (size == 1)
            {
                MAssignmentSlot mas = (MAssignmentSlot)list[0];
                LayoutY(mas);
                clean.Add(mas);
                return;
            }

            //	Delete Unavailability TimeSlots when all day assigments exist
            bool allDay = false;
            for (int i = 0; !allDay && i < size; i++)
            {
                MAssignmentSlot mas = (MAssignmentSlot)list[i];
                if (mas.GetStatus() == MAssignmentSlot.STATUS_NotAvailable
                    || mas.GetStatus() == MAssignmentSlot.STATUS_UnAvailable
                    || mas.GetStatus() == MAssignmentSlot.STATUS_NonBusinessDay
                    || mas.GetStatus() == MAssignmentSlot.STATUS_NotInSlotDay)
                    allDay = true;

            }
            if (allDay)
            {
                //	delete Time Slot
                for (int i = 0; i < list.Count; i++)
                {
                    MAssignmentSlot mas = (MAssignmentSlot)list[i];
                    if (mas.GetStatus() == MAssignmentSlot.STATUS_NotInSlotTime)
                        list.RemoveAt(i--);
                }
            }

            //	Copy & Y layout remaining
            for (int i = 0; i < list.Count; i++)
            {
                MAssignmentSlot mas = (MAssignmentSlot)list[i];
                LayoutY(mas);
                clean.Add(mas);
            }

            //	X layout
            int maxYslots = _timeSlots.Length;
            int[] xSlots = new int[maxYslots];		//	number of parallel slots
            for (int i = 0; i < list.Count; i++)
            {
                MAssignmentSlot mas = (MAssignmentSlot)list[i];
                for (int y = mas.GetYStart(); y < mas.GetYEnd(); y++)
                    xSlots[y]++;
            }
            //	Max parallel X Slots
            int maxXslots = 0;
            for (int y = 0; y < xSlots.Length; y++)
            {
                if (xSlots[y] > maxXslots)
                    maxXslots = xSlots[y];
            }
            //	Only one column
            if (maxXslots < 2)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    MAssignmentSlot mas = (MAssignmentSlot)list[i];
                    mas.SetX(0, 1);
                }
                return;
            }

            ////	Create xy Matrix
            //Array [][] arr = new Array();

            //Array[][] matrix =  new Array  [maxXslots][maxYslots];
            ////	Populate Matrix first column
            //for (int y = 0; y < maxYslots; y++)
            //{
            //    ArrayList<Object> xyList = new ArrayList<Object>();
            //    matrix[0][y] = xyList;
            //    //	see if one assignment fits into slot
            //    for (int i = 0; i < list.size(); i++)
            //    {
            //        MAssignmentSlot mas = (MAssignmentSlot)list.get(i);
            //        if (y >= mas.getYStart() && y <= mas.getYEnd())
            //            xyList.add(mas);
            //    }
            //    //	initiate right columns
            //    for (int x = 1; x < maxXslots; x++)
            //        matrix[x][y] = new ArrayList<Object>();
            //}	//	for all y slots

            ///**
            // * 	(AB)()	->	(B)(A)	->	(B)(A)
            // *  (BC)()	->	(BC)()	->	(B)(C)
            // * 	- if the row above is empty, move the first one right
            // *  - else	- check col_1..x above and move any content if the same
            // *  		- if size > 0
            // * 				- if the element is is not the same as above,
            // * 					move to the first empty column on the right
            // */
            ////	if in one column cell, there is more than one, move it to the right
            //for (int y = 0; y < maxYslots; y++)
            //{
            //    //	if an element is the same as the line above, move it there
            //    if (y > 0 && matrix[0][y].size() > 0)
            //    {
            //        for (int x = 1; x < maxXslots; x++)
            //        {
            //            if (matrix[x][y-1].size() > 0)	//	above slot is not empty
            //            {
            //                Object above = matrix[x][y-1].get(0);
            //                for (int i = 0; i < matrix[x][y].size(); i++)
            //                {
            //                    if (above.equals(matrix[0][y].get(i)))	//	same - move it
            //                    {
            //                        matrix[x][y].add(matrix[0][y].get(i));
            //                        matrix[0][y].remove(i--);
            //                    }
            //                }
            //            }
            //        }
            //    }	//	if an element is the same as the line above, move it there

            //    //	we need to move items to the right
            //    if (matrix[0][y].size() > 1)
            //    {
            //        Object above = null;
            //        if (y > 0 && matrix[0][y-1].size() > 0)
            //            above = matrix[0][y-1].get(0);
            //        //
            //        for (int i = 0; matrix[0][y].size() > 1; i++)
            //        {
            //            Object move = matrix[0][y].get(i);
            //            if (!move.equals(above))	//	we can move it
            //            {
            //                for (int x = 1; move != null && x < maxXslots; x++)
            //                {
            //                    if (matrix[x][y].size() == 0)	//	found an empty slot
            //                    {
            //                        matrix[x][y].add(move);
            //                        matrix[0][y].remove(i--);
            //                        move = null;
            //                    }
            //                }
            //            }
            //        }
            //    }	//	we need to move items to the right
            //}	//	 for all y slots

            ////	go through the matrix and assign the X position
            //for (int y = 0; y < maxYslots; y++)
            //{
            //    for (int x = 0; x < maxXslots; x++)
            //    {
            //        if (matrix[x][y].size() > 0)
            //        {
            //            MAssignmentSlot mas = (MAssignmentSlot)matrix[x][y].get(0);
            //            mas.setX(x, xSlots[y]);
            //        }
            //    }
            //}
            //	clean up
            //matrix = null;
        }	//	layoutSlots

        /**
         * 	Layout Y axis
         * 	@param mas assignment slot
         */
        private void LayoutY(MAssignmentSlot mas)
        {
            int timeSlotStart = GetTimeSlotIndex(mas.GetStartTime(), false);
            int timeSlotEnd = GetTimeSlotIndex(mas.GetEndTime(), true);
            if (TimeUtil.IsAllDay(mas.GetStartTime(), mas.GetEndTime()))
                timeSlotEnd = _timeSlots.Length - 1;
            //
            mas.SetY(timeSlotStart, timeSlotEnd);
        }	//	layoutY

        /**
         * 	Return the Time Slot index for the time.
         *  Based on start time and not including end time
         * 	@param time time (day is ignored)
         *  @param endTime if true, the end time is included
         * 	@return slot index
         */
        private int GetTimeSlotIndex(DateTime? time, bool endTime)
        {
            //	Just one slot
            if (_timeSlots.Length <= 1)
                return 0;
            //	search for it
            for (int i = 0; i < _timeSlots.Length; i++)
            {
                if (_timeSlots[i].InSlot(time, endTime))
                    return i;
            }
            log.Log(Level.SEVERE, "MSchedule.getTimeSlotIndex - did not find Slot for " + time + " end=" + endTime);
            return 0;
        }


        /**
         * 	Get Basic Info
         *  @param S_Resource_ID resource
         */
        private void GetBaseInfo(int S_Resource_ID)
        {
            //	Resource is Active and Available
            String sql = MRole.GetDefault(_ctx, false).AddAccessSQL(
                "SELECT r.IsActive,r.IsAvailable,NULL,"	//	r.IsSingleAssignment,"
                + "r.S_ResourceType_ID,rt.C_UOM_ID "
                + "FROM S_Resource r, S_ResourceType rt "
                + "WHERE r.S_Resource_ID=" + S_Resource_ID + " "
                + " AND r.S_ResourceType_ID=rt.S_ResourceType_ID",
                "r", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);
            //
            IDataReader dr = null;
           // System.Data.SqlClient.SqlParameter param = null;
            try
            {
                dr = DataBase.DB.ExecuteReader(sql);
                if (dr.Read())
                {
                    if (!"Y".Equals(Utility.Util.GetValueOfString(dr[0])))					//	Active
                        _isAvailable = false;
                    if (_isAvailable && !"Y".Equals(Utility.Util.GetValueOfString(dr[1])))	//	Available
                        _isAvailable = false;
                    //
                    _S_ResourceType_ID = Utility.Util.GetValueOfInt(dr[3]);
                    _C_UOM_ID = Utility.Util.GetValueOfInt(dr[4]);
                    //	log.fine( "- Resource_ID=" + m_S_ResourceType_ID + ",IsAvailable=" + m_isAvailable);
                }
                else
                    _isAvailable = false;
                dr.Close();
                dr = null;
                
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
                log.Log(Level.SEVERE, sql, e);
                _isAvailable = false;
            }
            _S_Resource_ID = S_Resource_ID;
        }

        /**
         * 	Create Unavailable Timeslots.
         *  For every day from startDay..endDay create unavailable slots
         *  for 00:00..startTime and endTime..24:00
         *  @param list list to add time slots to
         *  @param startTime start time in day
         *  @param endTime end time in day
         */
        private void CreateTimeSlot(List<MAssignmentSlot> list,
            DateTime? startTime, DateTime? endTime)
        {
           // log.fine("MSchedule.createTimeSlot");
            //DateTime startTime =new DateTime(0,0,0,startTime_1.Value.Hour,startTime_1.Value.Minute,startTime_1.Value.Second,0);
            //DateTime endTime = new DateTime(0,0,0,endTime_1.Value.Hour,endTime_1.Value.Minute,endTime_1.Value.Second,0);

            DateTime cal = new DateTime(_startDate.Value.Ticks);

            DateTime calEnd = new DateTime(_endDate.Value.Ticks);

            while (cal < calEnd)
            {
            //    //	00:00..startTime
               //cal.TimeOfDay.
                cal = new DateTime(cal.Year,cal.Month,cal.Day);
                //cal.set(Calendar.HOUR_OF_DAY, 0);
            //    cal.set(Calendar.MINUTE, 0);
            //    cal.set(Calendar.SECOND, 0);
            //    cal.set(Calendar.MILLISECOND, 0);
                DateTime start = new DateTime(cal.Ticks);
            //    //
            //    GregorianCalendar cal_1 = new GregorianCalendar(Language.getLoginLanguage().getLocale());
               DateTime cal_1 = new DateTime(startTime.Value.Ticks);
            //    cal_1.setTimeInMillis(startTime.getTime());
            //    cal.set(Calendar.HOUR_OF_DAY, cal_1.get(Calendar.HOUR_OF_DAY));
            //    cal.set(Calendar.MINUTE, cal_1.get(Calendar.MINUTE));
            //    cal.set(Calendar.SECOND, cal_1.get(Calendar.SECOND));
                cal = new DateTime(cal.Year,cal.Month,cal.Day,cal_1.Hour,cal_1.Minute,cal_1.Second,0);
                DateTime end = new DateTime(cal.Ticks);
            //    //
                MAssignmentSlot ma = new MAssignmentSlot(start, end,
                    Msg.GetMsg(_ctx, "ResourceNotInSlotTime"), "",
                    MAssignmentSlot.STATUS_NotInSlotTime);
                list.Add(ma);

            //    //	endTime .. 00:00 next day
                cal_1 = new DateTime(endTime.Value.Ticks);
            //    cal_1.setTimeInMillis(endTime.getTime());
            //    cal.set(Calendar.HOUR_OF_DAY, cal_1.get(Calendar.HOUR_OF_DAY));
            //    cal.set(Calendar.MINUTE, cal_1.get(Calendar.MINUTE));
            //    cal.set(Calendar.SECOND, cal_1.get(Calendar.SECOND));
                cal = new DateTime(cal.Year,cal.Month,cal.Day,cal_1.Hour,cal_1.Minute,cal_1.Second,0);
                start = new DateTime(cal.Ticks);
            //    //
            //    cal.set(Calendar.HOUR_OF_DAY, 0);
            //    cal.set(Calendar.MINUTE, 0);
            //    cal.set(Calendar.SECOND, 0);
            //    cal.add(Calendar.DAY_OF_YEAR, 1);
                cal = new DateTime(cal.Year,cal.Month,cal.Day);
                cal = cal.AddDays(1);
                end = new  DateTime(cal.Ticks);
            //    //
                ma = new MAssignmentSlot(start, end,
                    Msg.GetMsg(_ctx, "ResourceNotInSlotTime"), "",
                    MAssignmentSlot.STATUS_NotInSlotTime);
                list.Add(ma);
            }	//	createTimeSlot
        }

        /**
         * 	Create Unavailable Dayslots.
         *  For every day from startDay..endDay create unavailable slots
         *  @param list list to add Day slots to
         *  @param OnMonday true if OK to have appointments (i.e. blocked if false)
         *  @param OnTuesday true if OK
         *  @param OnWednesday true if OK
         *  @param OnThursday true if OK
         *  @param OnFriday true if OK
         *  @param OnSaturday true if OK
         *  @param OnSunday true if OK
         */
        private void CreateDaySlot(List<MAssignmentSlot> list,
            bool OnMonday, bool OnTuesday, bool OnWednesday,
            bool OnThursday, bool OnFriday, bool OnSaturday, bool OnSunday)
        {
            ////	log.fine( "MSchedule.createDaySlot");
            //    GregorianCalendar cal = new GregorianCalendar(Language.getLoginLanguage().getLocale());
            DateTime cal = new DateTime(_startDate.Value.Ticks);
            //    cal.setTimeInMillis(m_startDate.getTime());
            //    //	End Date for Comparison
            //    GregorianCalendar calEnd = new GregorianCalendar(Language.getLoginLanguage().getLocale());
            DateTime calEnd = new DateTime(_endDate.Value.Ticks);
            //    calEnd.setTimeInMillis(m_endDate.getTime());

                while (cal < calEnd)
                {
                    DayOfWeek weekday = cal.DayOfWeek; //cal.get(Calendar.DAY_OF_WEEK);
                    if ((!OnSaturday && weekday == DayOfWeek.Saturday)
                        || (!OnSunday && weekday == DayOfWeek.Sunday)
                        || (!OnMonday && weekday == DayOfWeek.Monday)
                        || (!OnTuesday && weekday == DayOfWeek.Tuesday)
                        || (!OnWednesday && weekday == DayOfWeek.Wednesday)
                        || (!OnThursday && weekday == DayOfWeek.Thursday)
                        || (!OnFriday && weekday == DayOfWeek.Friday))
                    {
            //            //	00:00..00:00 next day
            //            cal.set(Calendar.HOUR_OF_DAY, 0);
            //            cal.set(Calendar.MINUTE, 0);
            //            cal.set(Calendar.SECOND, 0);
            //            cal.set(Calendar.MILLISECOND, 0);
                        DateTime start = new DateTime(cal.Year, cal.Month, cal.Day, 0, 0, 0, 0);
            //            Timestamp start = new Timestamp (cal.getTimeInMillis());
                        cal =  cal.AddDays(1); 
                        DateTime end = new DateTime(cal.Ticks);

                        MAssignmentSlot	ma = new MAssignmentSlot (start, end,
                            Msg.GetMsg(_ctx, "ResourceNotInSlotDay"), "",
                            MAssignmentSlot.STATUS_NotInSlotDay);
                        list.Add(ma);
                   }
                    else	//	next day
                        cal = cal.AddDays(1);//  .add(Calendar.DAY_OF_YEAR, 1);
                }
        }	//	createDaySlot

        /**
         * 	Create a day slot for range
         * 	@param list list
         * 	@param ma assignment
         */
        private void CreateDaySlot(List<MAssignmentSlot> list, MAssignmentSlot ma)
        {
            //	log.fine( "MSchedule.createDaySlot", ma);
            //
            //Timestamp start = ma.getStartTime();
            //GregorianCalendar calStart = new GregorianCalendar();
            //calStart.setTime(start);
            //calStart.set(Calendar.HOUR_OF_DAY, 0);
            //calStart.set(Calendar.MINUTE, 0);
            //calStart.set(Calendar.SECOND, 0);
            //calStart.set(Calendar.MILLISECOND, 0);
            //Timestamp end = ma.getEndTime();
            //GregorianCalendar calEnd = new GregorianCalendar();
            //calEnd.setTime(end);
            //calEnd.set(Calendar.HOUR_OF_DAY, 0);
            //calEnd.set(Calendar.MINUTE, 0);
            //calEnd.set(Calendar.SECOND, 0);
            //calEnd.set(Calendar.MILLISECOND, 0);
            ////
            //while (calStart.before(calEnd))
            //{
            //    Timestamp xStart = new Timestamp(calStart.getTimeInMillis());
            //    calStart.add(Calendar.DAY_OF_YEAR, 1);
            //    Timestamp xEnd = new Timestamp(calStart.getTimeInMillis());
            //    MAssignmentSlot myMa = new MAssignmentSlot (xStart, xEnd,
            //        ma.getName(), ma.getDescription(), ma.getStatus());
            //    list.add(myMa);
            //}
        }	//	createDaySlot

        /*************************************************************************/

        /**
         * 	Get Day Time Slots for Date
         *  @return "heading" or null
         */
        public MAssignmentSlot[] GetDayTimeSlots()
        {
            return _timeSlots;
        }	//	getDayTimeSlots

        /**
         * 	Create Time Slots
         */
        private void CreateTimeSlots()
        {
            //	development error
            if (_typeName == null)
                throw new ArgumentNullException("ResourceTypeName not set");

            List<MAssignmentSlot> list = new List<MAssignmentSlot>();
            MUOM.Get(_ctx, _C_UOM_ID);
            int minutes = MUOMConversion.ConvertToMinutes(_ctx, _C_UOM_ID, Env.ONE);
            log.Config("Minutes=" + minutes);
            //
            if (minutes > 0 && minutes < 60 * 24)
            {
                //	Set Start Time
                //            GregorianCalendar cal = new GregorianCalendar();
                //            cal.setTime(m_startDate);
                //            cal.set(Calendar.HOUR_OF_DAY, 0);
                //            cal.set(Calendar.MINUTE, 0);
                //            cal.set(Calendar.SECOND, 0);
                //            cal.set(Calendar.MILLISECOND, 0);
                DateTime cal = new DateTime(_startDate.Value.Year, _startDate.Value.Month, _startDate.Value.Day);
                //            //	we have slots - create first
                            if (_slotStartTime != null)
                            {
                                long start = cal.Ticks;// cal.getTimeInMillis();
                                cal = TimeUtil.GetDayTime(_startDate, _slotStartTime);	//	set to start time
                //                cal.set(Calendar.SECOND, 0);
                //                cal.set(Calendar.MILLISECOND, 0);
                                list.Add(new MAssignmentSlot(start, cal.Ticks));
                            }
                //            //	Set End Time
                //            GregorianCalendar calEnd = new GregorianCalendar();
                            DateTime calEnd; 
                            if (_slotEndTime != null)
                            {
                                calEnd =  TimeUtil.GetDayTime(_startDate, _slotEndTime);
                                //calEnd.set(Calendar.SECOND, 0);
                //               // calEnd.set(Calendar.MILLISECOND, 0);
                            }
                            else	//	No Slot - all day
                            {
                //                calEnd.setTime(m_startDate);
                  //              calEnd.set(Calendar.HOUR_OF_DAY, 0);
                    //            calEnd.set(Calendar.MINUTE, 0);
                      //          calEnd.set(Calendar.SECOND, 0);
                        //        calEnd.set(Calendar.MILLISECOND, 0);
                                calEnd = new DateTime(_startDate.Value.Year,_startDate.Value.Month,_startDate.Value.Day,0,0,0,0);
                                calEnd  =  calEnd.AddDays(1);
                            }
                ////System.out.println("Start=" + new Timestamp(cal.getTimeInMillis()));
                ////System.out.println("Endt=" + new Timestamp(calEnd.getTimeInMillis()));

                //            //	Set end Slot Time
                //            GregorianCalendar calEndSlot = new GregorianCalendar();
                            DateTime calEndSlot = new DateTime(cal.Ticks);
                //            calEndSlot.setTime(cal.getTime());
                            calEndSlot =  calEndSlot.AddMinutes(minutes);

                            while (cal < calEnd)
                            {
                                list.Add(new MAssignmentSlot(cal.Ticks, calEndSlot.Ticks));
                //                //	Next Slot
                                cal = cal.AddMinutes(minutes);
                                calEndSlot =  calEndSlot.AddMinutes(minutes);
                            }
                //            //	create last slot
                            //calEndSlot.setTime(cal.getTime());
                            calEndSlot = new DateTime(cal.Year, cal.Month, cal.Day, 0, 0, 0, 0);
                //            calEndSlot.set(Calendar.HOUR_OF_DAY, 0);
                //            calEndSlot.set(Calendar.MINUTE, 0);
                //            calEndSlot.set(Calendar.SECOND, 0);
                //            calEndSlot.set(Calendar.MILLISECOND, 0);
                //            calEndSlot.add(Calendar.DAY_OF_YEAR, 1);	//	00:00 next day
                            calEndSlot = calEndSlot.AddDays(1);
                            list.Add(new MAssignmentSlot(cal.Ticks, calEndSlot.Ticks));
            }

            else	//	Day, ....
            {
                list.Add(new MAssignmentSlot(TimeUtil.GetDay(_startDate), TimeUtil.GetNextDay(_startDate)));
            }

            //
            _timeSlots = new MAssignmentSlot[list.Count];
           _timeSlots =  list.ToArray();
        }	//	createTimeSlots

        /*************************************************************************/

        /**
         * 	Get Resource ID. Set by getAssignmentSlots
         * 	@return current resource
         */
        public int GetS_Resource_ID()
        {
            return _S_Resource_ID;
        }	//	getS_Resource_ID

        /**
         * 	Return Start Date. Set by getAssignmentSlots
         * 	@return start date
         */
        public DateTime? GetStartDate()
        {
            return _startDate;
        }	//	getStartDate

        /**
         * 	Return End Date. Set by getAssignmentSlots
         * 	@return end date
         */
        public DateTime? GetEndDate()
        {
            return _endDate;
        }	//	getEndDate
    }
}