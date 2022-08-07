﻿namespace headlessCMS.Models.Services.Select
{
    public class SelectFiltersField
    {
        public string CollectionName { get; set; }

        public string FieldName { get; set; }

        public string Operation { get; set; }

        public List<SelectQueryFiltersFilter> Filters { get; set; }
    }
}