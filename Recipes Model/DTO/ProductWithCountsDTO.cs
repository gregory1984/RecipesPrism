﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recipes_Model.DTO
{
    public class ProductWithCountsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MountsCount { get; set; }
        public int RecipesCount { get; set; }
    }
}
