﻿using Model.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Entities
{
    public class UserRole:IEntity
    {
        public int AppUserId { get; set; }
        public int RoleId { get; set; }
    }
}
