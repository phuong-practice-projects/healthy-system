using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthy.Application.Common.Models
{
    public class CreateBodyRecordRequest
    {
        public decimal Weight { get; set; }
        public decimal? BodyFatPercentage { get; set; }
        public DateTime RecordDate { get; set; }
        public string? Notes { get; set; }
    }
}
