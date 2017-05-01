using StructureMap;

namespace Case00026950.Handler
{
    public sealed class IocContainer : Registry
    {
        public IocContainer()
        {
            Scan(scanner =>
            {
                scanner.Assembly("Case00026950.Handler");
                scanner.SingleImplementationsOfInterface();
                scanner.WithDefaultConventions();
            });
        }
    }
}
