/* Copyright (c) Business Objects 2006. All rights reserved. */

var L_bobj_crv_MainReport = "Informe principal";
// Viewer Toolbar tooltips
var L_bobj_crv_FirstPage = "Ir a primera p\u00E1gina";
var L_bobj_crv_PrevPage = "Ir a p\u00E1gina anterior";
var L_bobj_crv_NextPage = "Ir a p\u00E1gina siguiente";
var L_bobj_crv_LastPage = "Ir a \u00FAltima p\u00E1gina";
var L_bobj_crv_ParamPanel = "Panel de par\u00E1metros";
var L_bobj_crv_Parameters = "Par\u00E1metros";
var L_bobj_crv_GroupTree = "\u00C1rbol de grupos";
var L_bobj_crv_DrillUp = "Sintetizar";
var L_bobj_crv_Refresh = "Actualizar informe";
var L_bobj_crv_Zoom = "Zoom";
var L_bobj_crv_PageNav = "Exploraci\u00F3n de p\u00E1ginas";
var L_bobj_crv_SelectPage = "Ir a p\u00E1gina";
var L_bobj_crv_SearchText = "Buscar texto";
var L_bobj_crv_Export = "Exportar este informe";
var L_bobj_crv_Print = "Imprimir este informe";
var L_bobj_crv_TabList = "Lista de fichas";
var L_bobj_crv_Close = "Cerrar";
var L_bobj_crv_Logo=  "Logotipo de Business Objects";
var L_bobj_crv_FileMenu = "Men\u00FA de archivo";

var L_bobj_crv_File = "Archivo";

var L_bobj_crv_Show = "Mostrar";
var L_bobj_crv_Hide = "Ocultar";

var L_bobj_crv_Find = "Buscar...";
var L_bobj_crv_of = "%1 de %2"; // Example: Page "1 of 3"

var L_bobj_crv_submitBtnLbl = "Exportar";
var L_bobj_crv_ActiveXPrintDialogTitle = "Imprimir";
var L_bobj_crv_PDFPrintDialogTitle = "Imprimir en PDF";
var L_bobj_crv_PrintRangeLbl = "Intervalo de p\u00E1ginas:";
var L_bobj_crv_PrintAllLbl = "Todas las p\u00E1ginas";
var L_bobj_crv_PrintPagesLbl = "Seleccionar p\u00E1ginas";
var L_bobj_crv_PrintFromLbl = "Desde:";
var L_bobj_crv_PrintToLbl = "Hasta:";
var L_bobj_crv_PrintInfoTitle = "Imprimir en PDF:";
var L_bobj_crv_PrintInfo1 = 'El visor debe exportar a PDF para imprimir. Seleccione la opci\u00F3n Imprimir de la aplicaci\u00F3n de lectura de PDF cuando el documento est\u00E9 abierto.';
var L_bobj_crv_PrintInfo2 = 'Nota: debe tener instalado un lector PDF para imprimir. (ej. Adobe Reader)';
var L_bobj_crv_PrintPageRangeError = "Especifique un intervalo de p\u00E1ginas v\u00E1lido.";

var L_bobj_crv_ExportBtnLbl = "Exportar";
var L_bobj_crv_ExportDialogTitle = "Exportar";
var L_bobj_crv_ExportFormatLbl = "Formato de archivo:";
var L_bobj_crv_ExportInfoTitle = "Para exportar:";

var L_bobj_crv_ParamsApply = "Aplicar";
var L_bobj_crv_ParamsAdvDlg = "Editar el valor del par\u00E1metro";
var L_bobj_crv_ParamsDeleteTooltip = "Eliminar el valor del par\u00E1metro";
var L_bobj_crv_ParamsAddValue = "Haga clic para agregar...";
var L_bobj_crv_ParamsApplyTip = "Bot\u00F3n Aplicar (habilitado)";
var L_bobj_crv_ParamsApplyDisabledTip = "Bot\u00F3n Aplicar (deshabilitado)";
var L_bobj_crv_ParamsDlgTitle = "Especificar valores";
var L_bobj_crv_ParamsCalBtn = "Bot\u00F3n Calendario";
var L_bobj_crv_Reset= "Restablecer";
var L_bobj_crv_ResetTip = "Bot\u00F3n Restablecer (habilitado)";
var L_bobj_crv_ResetDisabledTip = "Bot\u00F3n Restablecer (deshabilitado)";
var L_bobj_crv_ParamsDirtyTip = "El valor del par\u00E1metro ha cambiado. Haga clic en el bot\u00F3n Aplicar para aplicar los cambios.";
var L_bobj_crv_ParamsDataTip = "Este es un par\u00E1metro de obtenci\u00F3n de datos";
var L_bobj_crv_ParamsMaxNumDefaultValues = "Haga clic aqu\u00ED para ver m\u00E1s elementos...";
var L_bobj_crv_paramsOpenAdvance = "Bot\u00F3n de petici\u00F3n avanzada de \'%1\'";

var L_bobj_crv_ParamsInvalidTitle = "El valor del par\u00E1metro no es v\u00E1lido";
var L_bobj_crv_ParamsTooLong = "El valor del par\u00E1metro no puede tener m\u00E1s de %1 caracteres";
var L_bobj_crv_ParamsTooShort = "El valor del par\u00E1metro debe tener al menos %1 caracteres";
var L_bobj_crv_ParamsBadNumber = "Este par\u00E1metro es de tipo \"N\u00FAmero\" y s\u00F3lo puede contener un s\u00EDmbolo de signo negativo, d\u00EDgitos (\"0-9\"), s\u00EDmbolos de agrupaci\u00F3n de d\u00EDgitos o un s\u00EDmbolo decimal.";
var L_bobj_crv_ParamsBadCurrency = "Este par\u00E1metro es de tipo \"Moneda\" y s\u00F3lo puede contener un s\u00EDmbolo de signo negativo, d\u00EDgitos (\"0-9\"), s\u00EDmbolos de agrupaci\u00F3n de d\u00EDgitos o un s\u00EDmbolo decimal. ";
var L_bobj_crv_ParamsBadDate = "Este par\u00E1metro es de tipo \"Fecha\" y el formato correcto es \"%1\", donde \"aaaa\" es el a\u00F1o con cuatro d\u00EDgitos, \"mm\" es el mes (por ejemplo, enero = 1) y \"dd\" es el d\u00EDa del mes.";
var L_bobj_crv_ParamsBadTime = "Este par\u00E1metro es de tipo \"Hora\" y el formato correcto es \"hh:mm:ss\", donde \"hh\" son las horas en formato de 24 horas, \"mm\" son los minutos y \"ss\" son los segundos.";
var L_bobj_crv_ParamsBadDateTime = "Este par\u00E1metro es de tipo \"FechaHora\" y el formato correcto es \"%1 hh:mm:ss\". \"aaaa\" es el a\u00F1o con cuatro d\u00EDgitos, \"mm\" es el mes (por ejemplo, enero = 1), \"dd\" es el d\u00EDa del mes, \"hh\" son las horas en formato de 24 horas, \"mm\" son los minutos y \"ss\" son los segundos.";
var L_bobj_crv_ParamsMinTooltip = "Especifique un valor de %1 igual o superior a %2.";
var L_bobj_crv_ParamsMaxTooltip = "Especifique un valor de %1 igual o inferior a %2.";
var L_bobj_crv_ParamsMinAndMaxTooltip = "Especifique un valor de %1 comprendido entre %2 y %3.";
var L_bobj_crv_ParamsStringMinOrMaxTooltip = "La longitud de %1 para este campo es %2.";
var L_bobj_crv_ParamsStringMinAndMaxTooltip = "La longitud del valor debe estar comprendida entre %1 y %2 caracteres.";
var L_bobj_crv_ParamsYearToken = "aaaa";
var L_bobj_crv_ParamsMonthToken = "mm";
var L_bobj_crv_ParamsDayToken = "dd";
var L_bobj_crv_ParamsReadOnly = "Este par\u00E1metro es de tipo \"S\u00F3lo lectura\".";
var L_bobj_crv_ParamsNoValue = "Ning\u00FAn valor";
var L_bobj_crv_ParamsDuplicateValue = "No se permiten valores duplicados.";
var L_bobj_crv_ParamsEnterOptional = "Introducir %1 (opcional)";
var L_bobj_crv_ParamsNoneSelected= "(Ninguna selecci\u00F3n)";
var L_bobj_crv_ParamsClearValues= "Borrar valores";
var L_bobj_crv_ParamsMoreValues= "%1 valores m\u00E1s...";
var L_bobj_crv_ParamsMoreValue= "%1 valor m\u00E1s...";
var L_bobj_crv_Error = "Error";
var L_bobj_crv_OK = "Aceptar";
var L_bobj_crv_Cancel = "Cancelar";
var L_bobj_crv_showDetails = "Mostrar detalles";
var L_bobj_crv_hideDetails = "Ocultar detalles";
var L_bobj_crv_RequestError = "No se puede procesar la solicitud";
var L_bobj_crv_ServletMissing = "El visor no puede conectar al CrystalReportViewerServlet que gestiona las solicitudes as\u00EDncronas.\nCompruebe que el Servlet y su asignaci\u00F3n se han declarado correctamente en el archivo web.xml de la aplicaci\u00F3n.";
var L_bobj_crv_FlashRequired = "Este contenido requiere Adobe Flash Player 9 o superior. {0}Haga clic aqu\u00ED para instalarlo";
var L_bobj_crv_ReadOnlyInPanel= "No se puede editar este par\u00E1metro en el panel. Abra el di\u00E1logo de petici\u00F3n avanzada para modificar este valor";

var L_bobj_crv_Tree_Drilldown_Node = "Profundizar nodo %1";

var L_bobj_crv_ReportProcessingMessage = "Espere mientras el documento se procesa.";
var L_bobj_crv_PrintControlProcessingMessage = "Espere mientras se carga el control de impresi\u00F3n de Crystal Reports.";

var L_bobj_crv_SundayShort = "D";
var L_bobj_crv_MondayShort = "L";
var L_bobj_crv_TuesdayShort = "M";
var L_bobj_crv_WednesdayShort = "X";
var L_bobj_crv_ThursdayShort = "J";
var L_bobj_crv_FridayShort = "V";
var L_bobj_crv_SaturdayShort = "S";

var L_bobj_crv_Minimum = "m\u00EDnimo";
var L_bobj_crv_Maximum = "m\u00E1ximo";

var L_bobj_crv_Date = "Fecha";
var L_bobj_crv_Time = "Hora";
var L_bobj_crv_DateTime = "FechaHora";
var L_bobj_crv_Boolean = "Booleano";
var L_bobj_crv_Number = "N\u00FAmero";
var L_bobj_crv_Text = "Texto";

var L_bobj_crv_InteractiveParam_NoAjax = "El explorador Web que usa no est\u00E1 configurado para mostrar el panel de par\u00E1metros.";
var L_bobj_crv_AdvancedDialog_NoAjax= "El visor no puede abrir un di\u00E1logo de petici\u00F3n avanzada.";

var L_bobj_crv_EnableAjax= "P\u00F3ngase en contacto con el administrador para habilitar las solicitudes as\u00EDncronas.";

var L_bobj_crv_LastRefreshed = "\u00DAltima actualizaci\u00F3n";

var L_bobj_crv_Collapse = "Contraer";

var L_bobj_crv_CatalystTip = "Recursos en l\u00EDnea";
