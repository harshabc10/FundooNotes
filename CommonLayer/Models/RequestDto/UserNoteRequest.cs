﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.RequestDto
{
    public class UserNoteRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public string ImagePaths { get; set; }
        public DateTime? Reminder { get; set; }
        public bool IsArchive { get; set; }
        public bool IsPinned { get; set; }
        public bool IsTrash { get; set; }
        //public int UserId { get; set; }
    }
}
