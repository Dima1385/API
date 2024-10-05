namespace Dashboard.BLL.Services.RoleService
{
    public interface IMapper
    {
        TDestination Map<TDestination>(object source);
        TDestination Map<TSource, TDestination>(TSource source, TDestination destination);
    }
}
