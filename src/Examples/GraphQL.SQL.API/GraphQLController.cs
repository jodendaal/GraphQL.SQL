using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL.DataLoader;
using GraphQL.NewtonsoftJson;
using Microsoft.AspNetCore.Mvc;

namespace GraphQL.SQL.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class GraphQLController : ControllerBase
    {
        private readonly SqlSchema _schema;
        private readonly IServiceProvider _serviceProvider;
        public GraphQLController(SqlSchema schema, IServiceProvider serviceProvider)
        {
            _schema = schema;
            _serviceProvider = serviceProvider;
        }

        [HttpPost]
        public async Task<string> Post([FromBody] GraphQLQuery query)
        {
            var documentCache = _serviceProvider.GetService(typeof(DataLoaderDocumentListener));
            var json = await _schema.ExecuteAsync(_ =>
            {
                _.Query = query.Query;
                _.Inputs = query.Variables?.ToInputs();
                _.OperationName = query.OperationName;
                _.RequestServices = _serviceProvider;
                _.Listeners.Add(documentCache as DataLoaderDocumentListener);
            });

            return json;
        }
    }

    public class GraphQLQuery
    {
        public string OperationName { get; set; }
        public string NamedQuery { get; set; }
        public string Query { get; set; }
        public Dictionary<string, object> Variables { get; set; }
    }
}
