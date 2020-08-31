using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketSvc.MService
{
    public partial class MarketModuleInfo : Object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged
    {
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ButtonTextField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string InstalledVersionField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ItemTextField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool IsEnabledField;




        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Collections.Generic.List<MarketSvc.MService.MMedia> ModMediaField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Collections.Generic.List<bool> DependentModInstalledField;

        


        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.Generic.List<MarketSvc.MService.MMedia> ModMedia
        {
            get
            {
                return this.ModMediaField;
            }
            set
            {
                if ((object.ReferenceEquals(this.ModMediaField, value) != true))
                {
                    this.ModMediaField = value;
                    this.RaisePropertyChanged("ModMedia");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.Generic.List<bool> DependentModInstalled
        {
            get
            {
                return this.DependentModInstalledField;
            }
            set
            {
                if ((object.ReferenceEquals(this.ModMediaField, value) != true))
                {
                    this.DependentModInstalledField = value;
                    this.RaisePropertyChanged("DependentModInstalled");
                }
            }
        }
      


        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ButtonText
        {
            get
            {
                return this.ButtonTextField;
            }
            set
            {
                if ((object.ReferenceEquals(this.ButtonTextField, value) != true))
                {
                    this.ButtonTextField = value;
                    this.RaisePropertyChanged("ButtonText");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string InstalledVersion
        {
            get
            {
                return this.InstalledVersionField;
            }
            set
            {
                if ((object.ReferenceEquals(this.InstalledVersionField, value) != true))
                {
                    this.InstalledVersionField = value;
                    this.RaisePropertyChanged("InstalledVersion");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ItemText
        {
            get
            {
                return this.ItemTextField;
            }
            set
            {
                if ((object.ReferenceEquals(this.ItemTextField, value) != true))
                {
                    this.ItemTextField = value;
                    this.RaisePropertyChanged("ItemText");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsEnabled
        {
            get
            {
                return this.IsEnabledField;
            }
            set
            {
                if ((this.IsEnabledField.Equals(value) != true))
                {
                    this.IsEnabledField = value;
                    this.RaisePropertyChanged("IsEnabled");
                }
            }
        }

      



    }

    public class MMedia
    {
        public object data;
        public bool isVideo;
        public bool isUrl;
        public string id;
    }


    public partial class ModuleContainer : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged
    {
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool IsMarketExpiredField;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsMarketExpired
        {
            get
            {
                return this.IsMarketExpiredField;
            }
            set
            {
                if ((this.IsMarketExpiredField.Equals(value) != true))
                {
                    this.IsMarketExpiredField = value;
                    this.RaisePropertyChanged("IsMarketExpired");
                }
            }
        }


    }



}
