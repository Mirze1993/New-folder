﻿using BLCafe.Interface;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLCafe.ConcreateRepository
{
    public class PlayGameRepository:CRUD<PlayGame>, IPlayGameRepository
    {
    }
}
