namespace BusinessLogic.ProductServices
{
    public enum ProductDeleteResult
    {
        Success,
        NotFound,
        HasOrders,
        DatabaseError
    }
}