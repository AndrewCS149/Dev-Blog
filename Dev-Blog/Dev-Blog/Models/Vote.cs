﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dev_Blog.Models
{
    public class Vote
    {
        public int PostId { get; set; }
        public string UserId { get; set; }
        public bool HasVoted { get; set; }
    }
}