using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zheng.Mdl.Models
{
    public class Result
    {
        internal Result(bool succeeded, IEnumerable<string> errors, string message = null, object data = null)
        {
            Succeeded = succeeded;
            Errors = errors.ToArray();
            Message = message;
            Data = data;
        }

        public bool Succeeded { get; init; }
        public string[] Errors { get; init; }
        public string Message { get; init; }
        public object Data { get; init; }

        public static Result Success(string message = null, object data = null)
        {
            return new Result(true, Array.Empty<string>(), message, data);
        }

        public static Result Failure(IEnumerable<string> errors, string message = null, object data = null)
        {
            return new Result(false, errors, message, data);
        }
    }
}
