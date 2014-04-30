using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Archetype.Umbraco.Database
{
    [TableName("Archetype")]
    [PrimaryKey("Id", autoIncrement = false)]
    [ExplicitColumns]
    public class ArchetypeConfiguration
    {
        [Column("Id")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        public Guid Id { get; set; }

        [Column("Configuration")]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        public string Configuration { get; set; }
    }
}
