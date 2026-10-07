namespace BusinessLogic.CategoryServices
{
    public enum CategoryDeleteResult
    {
        Success,
        NotFound,
        HasProducts,
        DatabaseError
    }
}