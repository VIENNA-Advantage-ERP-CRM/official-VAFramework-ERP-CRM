using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.Utility;

namespace BaseModel.ModelAD
{
    public interface  IPOValidator
    {
        bool OnSave(Ctx ctx);
        bool AddTranslation(Ctx ctx);
        bool DeleteTranslation(Ctx ctx);
        bool OnDelete(Ctx ctx);

        void UpdatePreferences();
    }
}
