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

_default="По умолчанию"
_black="Черный"
_brown="Коричневый"
_oliveGreen="Оливково-зеленый"
_darkGreen="Темно-зеленый"
_darkTeal="Темно-бирюзовый"
_navyBlue="Темно-синий"
_indigo="Индиго"
_darkGray="Темно-серый"
_darkRed="Темно-красный"
_orange="Оранжевый"
_darkYellow="Темно-желтый"
_green="Зеленый"
_teal="Бирюзовый"
_blue="Синий"
_blueGray="Серо-синий"
_mediumGray="Средне-серый"
_red="Красный"
_lightOrange="Светло-оранжевый"
_lime="Известковый"
_seaGreen="Морская волна"
_aqua="Аквамарин"
_lightBlue="Светло-синий"
_violet="Фиолетовый"
_gray="Серый"
_magenta="Пурпурный"
_gold="Золотой"
_yellow="Желтый"
_brightGreen="Ярко-зеленый"
_cyan="Голубой"
_skyBlue="Небесно-голубой"
_plum="Темно-фиолетовый"
_lightGray="Светло-серый"
_pink="Розовый"
_tan="Желто-коричневый"
_lightYellow="Светло-желтый"
_lightGreen="Светло-зеленый"
_lightTurquoise="Светло-бирюзовый"
_paleBlue="Бледно-голубой"
_lavender="Бледно-лиловый"
_white="Белый"
_lastUsed="Последний используемый:"
_moreColors="Дополнительные цвета..."

_month=new Array

_month[0]="Январь"
_month[1]="Февраль"
_month[2]="Март"
_month[3]="Апрель"
_month[4]="Май"
_month[5]="Июнь"
_month[6]="Июль"
_month[7]="Август"
_month[8]="Сентябрь"
_month[9]="Октябрь"
_month[10]="Ноябрь"
_month[11]="Декабрь"

_day=new Array
_day[0]="В"
_day[1]="П"
_day[2]="В"
_day[3]="С"
_day[4]="Ч"
_day[5]="П"
_day[6]="С"

_today="Сегодня"

_AM="AM"
_PM="PM"

_closeDialog="Закрыть окно"

_lstMoveUpLab="Переместить вверх"
_lstMoveDownLab="Переместить вниз"
_lstMoveLeftLab="Переместить влево" 
_lstMoveRightLab="Переместить вправо"
_lstNewNodeLab="Добавить вложенный фильтр"
_lstAndLabel="и"
_lstOrLabel="ИЛИ"
_lstSelectedLabel="Выбор выполнен"
_lstQuickFilterLab="Добавить быстрый фильтр"

_openMenu="Для получения доступа к параметрам {0} щелкните здесь"
_openCalendarLab="Открыть календарь"

_scroll_first_tab="Прокрутить до первой вкладки"
_scroll_previous_tab="Прокрутить до предыдущей вкладки"
_scroll_next_tab="Прокрутить до следующей вкладки"
_scroll_last_tab="Прокрутить до последней вкладки"

_expandedLab="Развернуто"
_collapsedLab="Свернуто"
_selectedLab="Выбор выполнен"

_expandNode="Развернуть узел %1"
_collapseNode="Свернуть узел %1"

_checkedPromptLab="Задано"
_nocheckedPromptLab="Не задано"
_selectionPromptLab="значения, равные"
_noselectionPromptLab="Отсутствуют значения"

_lovTextFieldLab="Введите значения здесь"
_lovCalendarLab="Введите дату здесь"
_lovPrevChunkLab="Перейти к предыдущему блоку"
_lovNextChunkLab="Перейти к следующему блоку"
_lovComboChunkLab="Блок"
_lovRefreshLab="Обновить на сервере"
_lovSearchFieldLab="Введите искомый текст здесь"
_lovSearchLab="Поиск"
_lovNormalLab="Обычный"
_lovMatchCase="Учитывать регистр"
_lovRefreshValuesLab="Обновить значения"

_calendarNextMonthLab="Перейти к следующему месяцу"
_calendarPrevMonthLab="Перейти к предыдущему месяцу"
_calendarNextYearLab="Перейти к следующему году"
_calendarPrevYearLab="Перейти к предыдущему году"
_calendarSelectionLab="Выбранный день"

_menuCheckLab="Отмечен"
_menuDisableLab="Отключен"
	
_level="Уровень"
_closeTab="Закрыть вкладку"
_of="из"

_RGBTxtBegin= "RGB("
_RGBTxtEnd= ")"

_helpLab="Справка"

_waitTitleLab="Подождите"
_cancelButtonLab="Отмена"

_modifiers= new Array
_modifiers[0]="Ctrl+"
_modifiers[1]="Shift+"
_modifiers[2]="Alt+"

_bordersMoreColorsLabel="Дополнительные границы..."
_bordersTooltip=new Array
_bordersTooltip[0]="Без границы"
_bordersTooltip[1]="Левая граница"
_bordersTooltip[2]="Правая граница"
_bordersTooltip[3]="Нижняя граница"
_bordersTooltip[4]="Нижняя граница средней толщины"
_bordersTooltip[5]="Толстая нижняя граница"
_bordersTooltip[6]="Верхняя и нижняя граница"
_bordersTooltip[7]="Верхняя и нижняя граница средней толщины"
_bordersTooltip[8]="Верхняя и толстая нижняя граница"
_bordersTooltip[9]="Все границы"
_bordersTooltip[10]="Все границы средней толщины"
_bordersTooltip[11]="Все толстые границы"