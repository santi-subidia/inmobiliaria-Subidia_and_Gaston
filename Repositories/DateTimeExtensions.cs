namespace Inmobiliaria.Repositories
{
    public static class DateTimeExtensions
    {
        public static DateOnly DateOnly(this DateTime dt) => new DateOnly(dt.Year, dt.Month, dt.Day);
    }
}
