using BuildAProject.Console.IOC.Registries;
using StructureMap;

namespace BuildAProject.Console.IOC
{
  public static class Bootstrap
  {
    public static IContainer CreateContainer()
    {
      var container = new Container(x =>
      {
        x.AddRegistry<BuildManagementRegistry>();
      });

      return container;
    }
  }
}
