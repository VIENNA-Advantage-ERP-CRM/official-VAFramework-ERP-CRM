using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.Controller
{
    /// <summary>
    /// This class contains properties of Header panel items (Grid Layout Items)
    /// </summary>
    public class HeaderPanelItemsVO
    {

        public int AD_GridLayoutItems_ID = 0;

        /// <summary>
        /// Align Header Items to Top, Bottom or center.
        /// By Default is center
        /// </summary>
        public string AlignItems = "T";

        /// <summary>
        /// Justify Header Items to left, right or center.
        /// By Default is center
        /// </summary>
        public string JustifyItems = "L";

        /// <summary>
        /// Item start from this Column
        /// </summary>
        public int StartColumn = 0;

        /// <summary>
        /// No. of columns item cover
        /// </summary>
        public int ColumnSpan = 0;

        /// <summary>
        /// Item start from this Row
        /// </summary>
        public int StartRow = 0;

        /// <summary>
        /// No. of rows item cover
        /// </summary>
        public int RowSpan = 0;

        /// <summary>
        /// If more than one item to be included in one cell, then flow of items in that cell.
        /// It can be Column-wise or Row-wise
        /// By Default is Column-Wise
        /// </summary>
        public string Flow = "C";

        /// <summary>
        /// Sequence No of items
        /// </summary>
        public float SeqNo = 0;

        public string BackgroundColor = "";

        public string FontColor = "";

        public string FontSize = "";

        public string Padding = "";

        public bool IsDynamic = false;

        public string ColSql = "";

        public bool HideFieldIcon = false;

        public bool HideFieldText = false;

        public string FieldValueStyle = "";

    }

    public class HeaderPanelGrid
    {
        /****   Header Back Color   ***/
        public string HeaderBackColor = "";

        /****   Header Name   ***/
        public string HeaderName = "";

        /****   Header Total Column   ***/
        public int HeaderTotalColumn = 0;

        /****   Header Total Row   ***/
        public int HeaderTotalRow = 0;

        /*** Header Padding  *******/
        public string HeaderPadding = "";

        /******   Grid Layout ID   *****/
        public int AD_GridLayout_ID = 0;

        /****   Header Items   ***/
        public Dictionary<int, object> HeaderItems = null;
    }
}
