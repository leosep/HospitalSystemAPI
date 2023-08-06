using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalSystem.Domain.Entities
{
    public class PaginatedResult<T>
    {
        public List<T> Data { get; set; }
        public int TotalRecords { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        public PaginatedResult(List<T> data, int totalRecords, int page, int pageSize)
        {
            Data = data;
            TotalRecords = totalRecords;
            Page = page;
            PageSize = pageSize;
        }
    }
}

