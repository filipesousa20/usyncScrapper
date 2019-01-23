using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uSyncScrapper.Models
{
    public class TabGroupedProperties
    {
        public string Tab { get; set; }
        public IEnumerable<DocumentTypeProperty> Properties { get; set; }
    }
}
