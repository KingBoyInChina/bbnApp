using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.DTOs.CommDto
{
    public class InitDataRefreshDto
    {
        public class InitDataRefreshRequestDto
        {
            public string Type { get; set; } = string.Empty;
        }
        public class InitDataRefreshResponseDto

        {
            public bool Code { get; set; } = false;
            public string Message { get; set; } = string.Empty;
        }
    }
}
