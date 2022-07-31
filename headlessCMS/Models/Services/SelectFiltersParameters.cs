﻿namespace headlessCMS.Models.Services
{
    public class SelectFiltersField
    {
        public string CollectionName { get; set; }

        public string FieldName { get; set; }

        public string Operation { get; set; }

        public List<SelectFiltersFilter> Filters { get; set; }
    }
}
