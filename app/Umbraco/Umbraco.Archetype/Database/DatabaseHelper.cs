using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace Archetype.Umbraco.Database
{
    internal class DatabaseHelper
    {
        public static UmbracoDatabase UmbracoDatabase
        {
            get
            {
                return ApplicationContext.Current.DatabaseContext.Database;
            }
        }

        public static ArchetypeConfiguration Get(Guid guid)
        {
            return UmbracoDatabase.FirstOrDefault<ArchetypeConfiguration>(string.Format("WHERE Id = '{0}'", guid));
        }

        public static void Add(ArchetypeConfiguration configuration)
        {
            UmbracoDatabase.Insert(configuration);
        }

        public static void Update(ArchetypeConfiguration configuration)
        {
            UmbracoDatabase.Update(configuration);
        }

        public static void Install()
        {
            if(UmbracoDatabase.TableExist("Archetype") == false)
            {
                UmbracoDatabase.CreateTable<ArchetypeConfiguration>();
            }
        }

        public static void Uninstall()
        {
            if(UmbracoDatabase.TableExist("Archetype"))
            {
                UmbracoDatabase.DropTable<ArchetypeConfiguration>();
            }
        }
    }
}
