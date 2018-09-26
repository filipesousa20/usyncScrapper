using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uSyncScrapper.Models
{
    public class FinalDocument
    {
        public string Body { get; set; }
        public IEnumerable<DocumentType> DocTypes { get; set; }
    }
}
