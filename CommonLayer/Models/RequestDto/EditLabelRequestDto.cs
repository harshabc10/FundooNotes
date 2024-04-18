using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.RequestDto
{
    public class EditLabelRequestDto
    {
        public string LabelName { get; set; }

        [Required]
        public int NoteId { get; set; }
    }

}
