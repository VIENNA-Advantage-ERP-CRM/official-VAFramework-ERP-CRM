// <script>
/*
=============================================================
WebIntelligence(r) Report Panel
Copyright(c) 2001-2003 Business Objects S.A.
All rights reserved

Use and support of this software is governed by the terms
and conditions of the software license agreement and support
policy of Business Objects S.A. and/or its subsidiaries. 
The Business Objects products and technology are protected
by the US patent number 5,555,403 and 6,247,008

File: labels.js


=============================================================
*/

_default="ดีฟอลต์"
_black="ดำ"
_brown="น้ำตาล"
_oliveGreen="เขียวมะกอก"
_darkGreen="เขียวเข้ม"
_darkTeal="น้ำเงินอมเขียวเข้ม"
_navyBlue="กรมท่า"
_indigo="คราม"
_darkGray="เทาเข้ม"
_darkRed="แดงเข้ม"
_orange="ส้ม"
_darkYellow="เหลืองเข้ม"
_green="เขียว"
_teal="น้ำเงินอมเขียว"
_blue="น้ำเงิน"
_blueGray="น้ำเงินเทา"
_mediumGray="เทาปานกลาง"
_red="แดง"
_lightOrange="ส้มอ่อน"
_lime="เขียวมะนาว"
_seaGreen="เขียวอมน้ำเงิน"
_aqua="สีน้ำทะเล"
_lightBlue="ฟ้าอ่อน"
_violet="ม่วง"
_gray="เทา"
_magenta="แดงม่วง"
_gold="ทอง"
_yellow="เหลือง"
_brightGreen="เขียวสว่าง"
_cyan="น้ำเงินเขียว"
_skyBlue="สีฟ้า"
_plum="สีม่วงแดงอ่อน"
_lightGray="เทาอ่อน"
_pink="ชมพู"
_tan="น้ำตาลแดง"
_lightYellow="เหลืองอ่อน"
_lightGreen="เขียวอ่อน"
_lightTurquoise="เทอร์ควอยซ์อ่อน"
_paleBlue="น้ำเงินจาง"
_lavender="ม่วงลาเวนเดอร์"
_white="ขาว"
_lastUsed="ใช้ครั้งล่าสุด:"
_moreColors="สีเพิ่มเติม..."

_month=new Array

_month[0]="มกราคม"
_month[1]="กุมภาพันธ์"
_month[2]="มีนาคม"
_month[3]="เมษายน"
_month[4]="พฤษภาคม"
_month[5]="มิถุนายน"
_month[6]="กรกฎาคม"
_month[7]="สิงหาคม"
_month[8]="กันยายน"
_month[9]="ตุลาคม"
_month[10]="พฤศจิกายน"
_month[11]="ธันวาคม"

_day=new Array
_day[0]="อา"
_day[1]="จ"
_day[2]="อ"
_day[3]="พ"
_day[4]="พฤ"
_day[5]="ศ"
_day[6]="ส"

_today="วันนี้"

_AM="AM"
_PM="PM"

_closeDialog="ปิดหน้าต่าง"

_lstMoveUpLab="เลื่อนขึ้น"
_lstMoveDownLab="เลื่อนลง"
_lstMoveLeftLab="เลื่อนไปทางซ้าย" 
_lstMoveRightLab="เลื่อนไปทางขวา"
_lstNewNodeLab="เพิ่มฟิลเตอร์ที่ซ้อนกัน"
_lstAndLabel="และ"
_lstOrLabel="หรือ"
_lstSelectedLabel="ที่เลือก"
_lstQuickFilterLab="เพิ่มฟิลเตอร์แบบรวดเร็ว"

_openMenu="คลิกที่นี่เพื่อเข้าถึงตัวเลือกของ {0}"
_openCalendarLab="เปิดปฎิทิน"

_scroll_first_tab="เลื่อนไปที่แท็บแรก"
_scroll_previous_tab="เลื่อนไปที่แท็บก่อนหน้า"
_scroll_next_tab="เลื่อนไปที่แท็บถัดไป"
_scroll_last_tab="เลื่อนไปที่แท็บสุดท้าย"

_expandedLab="ที่ขยาย"
_collapsedLab="ย่อรวม"
_selectedLab="ที่เลือก"

_expandNode="ขยายโหนด %1"
_collapseNode="ย่อรวมโหนด %1"

_checkedPromptLab="ตั้งค่า"
_nocheckedPromptLab="ไม่ตั้งค่า"
_selectionPromptLab="มีค่าเท่ากับ"
_noselectionPromptLab="ไม่มีค่า"

_lovTextFieldLab="พิมพ์ค่าที่นี่"
_lovCalendarLab="พิมพ์วันที่ที่นี่"
_lovPrevChunkLab="ไปที่กลุ่มข้อมูลก่อนหน้า"
_lovNextChunkLab="ไปที่กลุ่มข้อมูลถัดไป"
_lovComboChunkLab="กลุ่มข้อมูล"
_lovRefreshLab="รีเฟรช"
_lovSearchFieldLab="พิมพ์ข้อความที่ต้องการค้นหาที่นี่"
_lovSearchLab="ค้นหา"
_lovNormalLab="ปกติ"
_lovMatchCase="ตัวพิมพ์ตรงกัน"
_lovRefreshValuesLab="รีเฟรชค่า"

_calendarNextMonthLab="ไปที่เดือนถัดไป"
_calendarPrevMonthLab="ไปที่เดือนก่อนหน้า"
_calendarNextYearLab="ไปที่ปีถัดไป"
_calendarPrevYearLab="ไปที่ปีก่อนหน้า"
_calendarSelectionLab="วันที่เลือก"

_menuCheckLab="ที่ตรวจสอบ"
_menuDisableLab="ที่ปิดใช้งาน"
	
_level="ระดับ"
_closeTab="ปิดแท็บ"
_of=" จาก "

_RGBTxtBegin= "RGB("
_RGBTxtEnd= ")"

_helpLab="วิธีใช้"

_waitTitleLab="โปรดรอ"
_cancelButtonLab="ยกเลิก"

_modifiers= new Array
_modifiers[0]="Ctrl+"
_modifiers[1]="Shift+"
_modifiers[2]="Alt+"

_bordersMoreColorsLabel="เส้นขอบเพิ่มเติม..."
_bordersTooltip=new Array
_bordersTooltip[0]="ไม่มีเส้นขอบ"
_bordersTooltip[1]="เส้นขอบซ้าย"
_bordersTooltip[2]="เส้นขอบขวา"
_bordersTooltip[3]="เส้นขอบล่าง"
_bordersTooltip[4]="เส้นขอบล่างแบบหนาปานกลาง"
_bordersTooltip[5]="เส้นขอบล่างแบบหนา"
_bordersTooltip[6]="เส้นขอบบนและล่าง"
_bordersTooltip[7]="เส้นขอบบนและเส้นขอบล่างแบบหนาปานกลาง"
_bordersTooltip[8]="เส้นขอบบนและเส้นขอบล่างแบบหนา"
_bordersTooltip[9]="เส้นขอบทั้งหมด"
_bordersTooltip[10]="เส้นขอบแบบหนาปานกลางทั้งหมด"
_bordersTooltip[11]="เส้นขอบแบบหนาทั้งหมด"