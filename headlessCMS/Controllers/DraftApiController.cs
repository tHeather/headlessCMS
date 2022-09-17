﻿using headlessCMS.Filters;
using headlessCMS.Models.Services.InsertQuery;
using headlessCMS.Models.Services.SelectQuery;
using headlessCMS.Models.Shared;
using headlessCMS.Services;
using Microsoft.AspNetCore.Mvc;

namespace headlessCMS.Controllers
{
    [ApiController]
    [Route("draft")]
    [AddDraftDbSchemaActionFilter("collectionName")]
    public class DraftApiController : Controller
    {
        private readonly ApiService _apiService;

        public DraftApiController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpPost("get")]
        [AddDraftDbSchemaToSelectQueryActionFilter("queryParameters")]
        public async Task<IActionResult> GetDataAsync(SelectQueryParametersDataCollection queryParameters)
        {
            var data = await _apiService.GetData(queryParameters);
            return Ok(data);
        }

        [HttpPost("{collectionName}/insert")]
        public async Task<IActionResult> InsertDataAsync
            (string collectionName, [FromBody] List<ColumnWithValue> columnsWithValues)
        {
            var insertData  = new InsertQueryParameters() 
            {
                CollectionName = collectionName,
                DataToInsert = columnsWithValues
            };

            var savedDataId = await _apiService.InsertDataAsync(insertData);

            return Ok(savedDataId);
        }

        //[HttpPost("{collectionName}/insert")]
        //public async Task<IActionResult> InsertDataAsync
        //    (string collectionName, [FromBody] List<ColumnWithValue> columnsWithValues)
        //{
        //    var insertData = new InsertQueryParameters()
        //    {
        //        CollectionName = collectionName,
        //        DataToInsert = columnsWithValues
        //    };

        //    var savedDataId = await _apiService.InsertPublishedDataAsync(insertData);

        //    return Ok(savedDataId);
        //}
    }
}
