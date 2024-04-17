using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.RequestDto
{
    public  class CollaboratorRequest
    {
        public int UserId { get; set; } // Foreign key from UserEntity
        public int UserNoteId { get; set; } // Foreign key from UserNote
        public string CollaboratorEmail { get; set; } = string.Empty;
    }
}
