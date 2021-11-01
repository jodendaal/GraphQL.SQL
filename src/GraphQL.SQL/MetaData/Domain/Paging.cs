using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQL.SQL
{
    public class Paging
    {
        public Paging()
        {
        }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalRecords { get; set; }
    }
}
