﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServer.Models
{
    public class UpVoteModel
    {
        public int PostModelId { get; set; }
        public string UserName { get; set; }
    }
}