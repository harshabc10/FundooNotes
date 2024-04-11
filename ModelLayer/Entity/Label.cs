using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Entity
{
    public class Label
    {
        public int LabelId { get; set; }
        public string LabelName { get; set; }
        public int UserId { get; set; } // Foreign key
        public int NoteId { get; set; } // Foreign key

        // Other properties and relationships as needed
    }
}
