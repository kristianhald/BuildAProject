using System;
using System.IO;

// The implementation of this class has been taken from http://malvinly.com/2012/04/08/executing-code-in-a-new-application-domain/
// A little help with teh domain setup here http://stackoverflow.com/questions/1838619/relocating-app-config-file-to-a-custom-path
using System.Reflection;

namespace AppDomainExecutor
{
  [Serializable]
  public class NewAppDomain
  {
    private readonly string baseDirectory;

    public NewAppDomain(string baseDirectory)
    {
      if (String.IsNullOrWhiteSpace(baseDirectory))
      {
        throw new ArgumentNullException("baseDirectory");
      }

      if (!Directory.Exists(baseDirectory))
      {
        throw new ArgumentException("The base directory '{0}' does not exist.");
      }

      this.baseDirectory = baseDirectory;
    }

    public void Execute(Action action)
    {
      AppDomain domain = null;

      try
      {
        domain = AppDomain.CreateDomain("New App Domain: " + Guid.NewGuid());

        var domainDelegate = (AppDomainDelegate)domain.CreateInstanceAndUnwrap(
            typeof(AppDomainDelegate).Assembly.FullName,
            typeof(AppDomainDelegate).FullName);

        domainDelegate.Execute(action);
      }
      finally
      {
        if (domain != null)
          AppDomain.Unload(domain);
      }
    }

    public void Execute<T>(T parameter, Action<T> action)
    {
      AppDomain domain = null;

      try
      {
        domain = AppDomain.CreateDomain("New App Domain: " + Guid.NewGuid());

        var domainDelegate = (AppDomainDelegate)domain.CreateInstanceAndUnwrap(
            typeof(AppDomainDelegate).Assembly.FullName,
            typeof(AppDomainDelegate).FullName);

        domainDelegate.Execute(parameter, action);
      }
      finally
      {
        if (domain != null)
          AppDomain.Unload(domain);
      }
    }

    public T Execute<T>(Func<T> action)
    {
      AppDomain domain = null;

      try
      {
        domain = AppDomain.CreateDomain("New App Domain: " + Guid.NewGuid());

        var domainDelegate = (AppDomainDelegate)domain.CreateInstanceAndUnwrap(
            typeof(AppDomainDelegate).Assembly.FullName,
            typeof(AppDomainDelegate).FullName);

        return domainDelegate.Execute(action);
      }
      finally
      {
        if (domain != null)
          AppDomain.Unload(domain);
      }
    }

    public TResult Execute<T, TResult>(T parameter, Func<T, TResult> action, string appConfigFilePath = null)
    {
      AppDomain domain = null;

      try
      {
        var domainSetup = new AppDomainSetup
                          {
                            ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
                            ConfigurationFile = appConfigFilePath
                          };

        domain = AppDomain.CreateDomain(
          "New App Domain: " + Guid.NewGuid(),
          null,
          domainSetup);

        domain.AssemblyResolve += DomainOnAssemblyResolve;

        var domainDelegate = (AppDomainDelegate)domain.CreateInstanceAndUnwrap(
            typeof(AppDomainDelegate).Assembly.FullName,
            typeof(AppDomainDelegate).FullName);

        return domainDelegate.Execute(parameter, action);
      }
      finally
      {
        if (domain != null)
          AppDomain.Unload(domain);
      }
    }

    private Assembly DomainOnAssemblyResolve(object sender, ResolveEventArgs args)
    {
      var fileNameWithoutExtension = GetFileNameWithoutExtension(args.Name);

      var dllDomainFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileNameWithoutExtension + ".dll");
      if (File.Exists(dllDomainFilePath))
      {
        return Assembly.LoadFrom(dllDomainFilePath);
      }

      var exeDomainFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileNameWithoutExtension + ".exe");
      if (File.Exists(exeDomainFilePath))
      {
        return Assembly.LoadFrom(exeDomainFilePath);
      }

      var dllBaseFilePath = Path.Combine(baseDirectory, fileNameWithoutExtension + ".dll");
      if (File.Exists(dllBaseFilePath))
      {
        return Assembly.LoadFrom(dllBaseFilePath);
      }

      var exeBaseFilePath = Path.Combine(baseDirectory, fileNameWithoutExtension + ".exe");
      if (File.Exists(exeBaseFilePath))
      {
        return Assembly.LoadFrom(exeBaseFilePath);
      }

      return null;
    }

    private string GetFileNameWithoutExtension(string name)
    {
      // Gets the first part 'FluentNHibernate, Version=1.4.0.0, Culture=neutral, PublicKeyToken=8aa435e3cb308880'
      return name.Split(',')[0];
    }

    private class AppDomainDelegate : MarshalByRefObject
    {
      public void Execute(Action action)
      {
        action();
      }

      public void Execute<T>(T parameter, Action<T> action)
      {
        action(parameter);
      }

      public T Execute<T>(Func<T> action)
      {
        return action();
      }

      public TResult Execute<T, TResult>(T parameter, Func<T, TResult> action)
      {
        return action(parameter);
      }
    }
  }
}
