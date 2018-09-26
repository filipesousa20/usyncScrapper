﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uSyncScrapper.Models
{
    public class DocumentType
    {
        public string Description { get; set; }
        public IEnumerable<DocumentTypeProperty> Properties { get; set; }

    }
}