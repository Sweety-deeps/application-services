using System;
namespace Domain.Models
{
	public class BaseResponseModel<T>
	{
		public BaseResponseModel()
		{
		}

		public bool Status { get; set; }
		public T Data { get; set; }
		public string Message { get; set; }
		public IList<string> Errors { get; set; }
	}
}

