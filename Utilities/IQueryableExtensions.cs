namespace KalumManagement.Utilities
{
    public static class IQueryableExtensions
    {
        // metodo estatico
        public static int Registers { get; set; }

        // metodo para traer la Data
        public static IQueryable<T> Pagination<T>(this IQueryable<T> queryable, int number)
        {
            return queryable.Skip(Registers * number).Take(Registers);
        }
    }
}