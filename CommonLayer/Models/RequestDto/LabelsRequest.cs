using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.RequestDto
{
    public class LabelsRequest
    {
        public string LabelName { get; set; }
        public int UserId { get; set; } 
        public int NoteId { get; set; }
    }
}
