using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.Print
{
    public interface CharacterIterator : ICloneable
    {
        char First();
        char Last();
        char Current();
        char Next();
        char Previous();
        char SetIndex(int position);
        int GetBeginIndex();
        int GetEndIndex();
        int GetIndex();
        new Object Clone();
    }

    public interface AttributedCharacterIterator : CharacterIterator
    {
        Object GetAttribute(TextAttribute textAtt);
        //int GetRunStart();
        //int GetRunStart(Attributes attribute);
        int GetRunLimit();
        string GetText();
        System.Drawing.Font GetFont();
        System.Drawing.Color GetColor();
        //int GetRunLimit(Attributes attribute);
    }
}
