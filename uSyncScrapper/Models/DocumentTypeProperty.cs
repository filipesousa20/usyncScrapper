using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uSyncScrapper.Models
{
    public class DocumentTypeProperty
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public string Tab { get; set; }
        public int Order { get; set; }
        public string Type { get; set; }
        public string Key { get; set; }
        public int MaxItems { get; set; }
    }
}
