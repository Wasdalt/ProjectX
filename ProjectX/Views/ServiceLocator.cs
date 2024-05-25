using System;
using System.Collections.Generic;

namespace ProjectX.Views;

public static class ServiceLocator
{
    private static readonly Dictionary<Type, object> Services = new();

    public static void RegisterService<T>(T service) where T : class
    {
        Services[typeof(T)] = service;
    }

    public static T GetService<T>() where T : class
    {
        return Services.TryGetValue(typeof(T), out var service) ? (T)service : throw new InvalidOperationException($"Service of type {typeof(T)} is not registered.");
    }
}
