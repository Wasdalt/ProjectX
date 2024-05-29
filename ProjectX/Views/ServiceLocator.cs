using System;
using System.Collections.Generic;
using ProjectX.Models.ExitProgram;

namespace ProjectX.Views;

// public static class ServiceLocator
// {
//     
//     private static readonly Dictionary<Type, object> Services = new();
//
//     public static void RegisterService<T>(T service) where T : class
//     {
//         Services[typeof(T)] = service;
//     }
//
//     public static T GetService<T>() where T : class
//     {
//         return Services.TryGetValue(typeof(T), out var service) ? (T)service : throw new InvalidOperationException($"Service of type {typeof(T)} is not registered.");
//     }
//     
//     
//     private static readonly Lazy<IShutdownService> _shutdownService = new(() => new ShutdownService());
//
//     public static IShutdownService ShutdownService => _shutdownService.Value;
// }
public static class ServiceLocator
{
    private static readonly Dictionary<Type, object> _services = new();

    public static void Register<T>(T service) where T : class
    {
        _services[typeof(T)] = service;
    }

    public static T Resolve<T>()
    {
        var type = typeof(T);
        return (_services.ContainsKey(type) ? (T)_services[type] : default)!;
    }

    private static readonly Lazy<IShutdownService> _shutdownService = new(() => new ShutdownService());

    public static IShutdownService ShutdownService => _shutdownService.Value;
}
