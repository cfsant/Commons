namespace Commons.Interfaces
{
    interface IProductionInternalException : IInternalException
    {
        string Detail { get; set; }
    }
}
