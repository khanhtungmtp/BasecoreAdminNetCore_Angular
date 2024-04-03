namespace API.Helpers.Base
{
    public class OperationResult
    {
        public string? Messages { get; set; }
        public bool IsSuccess { get; set; }

        public OperationResult()
        {
        }

        public OperationResult(string messages)
        {
            Messages = messages;
        }

        public OperationResult(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public OperationResult(bool isSuccess, string messages)
        {
            IsSuccess = isSuccess;
            Messages = messages;
        }
    }

    public class OperationResult<T> : OperationResult where T : class
    {
        public T? Data { get; set; }

        public OperationResult()
        {
        }

        public OperationResult(string messages) : base(messages)
        {
        }

        public OperationResult(bool isSuccess) : base(isSuccess)
        {
        }

        public OperationResult(bool isSuccess, string messages) : base(isSuccess, messages)
        {
        }

        public OperationResult(bool isSuccess, T data) : base(isSuccess)
        {
            Data = data;
        }

        public OperationResult(bool isSuccess, string messages, T data) : base(isSuccess, messages)
        {
            Data = data;
        }
    }
}
