using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace PeerTutoringNetwork.Viewmodels
{
    public class SubjectVM
    {
        public int SubjectId { get; set; }

        public string SubjectName { get; set; }

        public string? Description { get; set; }

        [ValidateNever]
        public string CreatedByUsername { get; set; }

        public int CreatedByUserId { get; set; } 
    }
}
