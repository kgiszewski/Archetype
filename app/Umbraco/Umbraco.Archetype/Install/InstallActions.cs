using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using umbraco.interfaces;
using Umbraco.Web;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;
using Archetype.Umbraco.Database;

namespace Archetype.Umbraco.Install
{
    // create/drop Archetype table on package install/uninstall
    public class InstallActions : IPackageAction
    {
        public string Alias()
        {
            return "ArchetypeInstall";
        }

        public bool Execute(string packageName, System.Xml.XmlNode xmlData)
        {
            DatabaseHelper.Install();
            return true;
        }

        public System.Xml.XmlNode SampleXml()
        {
            var element = XElement.Parse(string.Format(@"<Action runat=""install"" undo=""true"" alias=""{0}"" />", Alias()));
            using(var xmlReader = element.CreateReader())
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlReader);
                return xmlDoc;
            }
        }

        public bool Undo(string packageName, System.Xml.XmlNode xmlData)
        {
            DatabaseHelper.Uninstall();
            return true;
        }
    }

}
