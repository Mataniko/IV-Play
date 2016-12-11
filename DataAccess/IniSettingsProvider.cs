using IVPlay.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Specialized;

namespace IVPlay.DataAccess
{
    class IniSettingsProvider : SettingsProvider, IApplicationSettingsProvider
    {

       private Dictionary<string, object> _settings;       

       public IniSettingsProvider()
        {
            _settings = new Dictionary<string, object>();

            if (File.Exists(Resources.Configuration_File_Name))
            {
                var lines = File.ReadAllLines(Resources.Configuration_File_Name);

                foreach (var item in lines)
                {
                    var split = item.Split('=');
                    _settings.Add(split[0], split[1]);
                }
            }
        }
        public override string ApplicationName
        {
            get
            {
                return "IVPlay";
            }

            set
            {
                
            }
        }
        public override string Name
        {
            get { return "IniSettingsProvider"; }
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(Name, config);
        }
        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection)
        {            
            SettingsPropertyValueCollection values = new SettingsPropertyValueCollection();

            foreach (SettingsProperty property in collection)
            {
                values.Add(new SettingsPropertyValue(property)
                {
                    SerializedValue = GetValue(property)
                });
            }

            return values;
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
        {
            foreach (SettingsPropertyValue propertyValue in collection)
                SetValue(propertyValue);

            try
            {
                File.WriteAllLines(Resources.Configuration_File_Name, ConvertDictionaryToArray(_settings));
            }
            catch (Exception)
            {
                // Don't save anything
            }
        }

        private void SetValue(SettingsPropertyValue propertyValue)
        {
            if (_settings.ContainsKey(propertyValue.Name))
                _settings[propertyValue.Name] = propertyValue.SerializedValue.ToString();
            else
                _settings.Add(propertyValue.Name, propertyValue.SerializedValue.ToString());
        }

        private string GetValue(SettingsProperty property)
        {
            if (_settings.ContainsKey(property.Name))
                return _settings[property.Name].ToString();
            else
                return property.DefaultValue != null ? property.DefaultValue.ToString() : string.Empty;
        }

        private string[] ConvertDictionaryToArray(Dictionary<string, object> settings)
        {
            var values = new List<string>();

            foreach (var kvp in settings)
            {
                values.Add(string.Format("{0}={1}", kvp.Key, kvp.Value.ToString()));
            }

            return values.ToArray();
        }

        public SettingsPropertyValue GetPreviousVersion(SettingsContext context, SettingsProperty property)
        {
            return new SettingsPropertyValue(property);
        }

        public void Reset(SettingsContext context)
        {
            _settings = new Dictionary<string, object>();
        }

        public void Upgrade(SettingsContext context, SettingsPropertyCollection properties)
        {
            
        }
    }
}
