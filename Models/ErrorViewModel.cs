using System;

namespace Tabula.ViewModels
{
    public class ErrorViewModel
    {

        public int? StatusCode { get; set; }
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
