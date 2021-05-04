using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Reflection;

namespace AutoConfig
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAutoConfig(this IServiceCollection services, IConfiguration configuration)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembie in assemblies)
            {
                assembie.GetTypes().ToList().ForEach(t =>
                {
                    var ac = t.GetCustomAttribute<AutoConfigAttribute>();
                    if (ac != null)
                    {
                        var key = ac.GetSectionKey();
                        services.AddOptions();
                        var config = configuration;
                        if (!string.IsNullOrEmpty(key))
                        {
                            config = configuration.GetSection(key);
                        }

                        var changeTokenSourceInterfaceType = typeof(IOptionsChangeTokenSource<>).MakeGenericType(t);
                        var changeTokenSourceImpType = typeof(ConfigurationChangeTokenSource<>).MakeGenericType(t);
                        var name = Options.DefaultName;
                        var changeTokenInstance = Activator.CreateInstance(changeTokenSourceImpType, new object[] { name, config });
                        services.AddSingleton(changeTokenSourceInterfaceType, changeTokenInstance);

                        //var configOptionsInterfaceType = typeof(IConfigureOptions<>).MakeGenericType(t);
                        //var namedOptionsImpType = typeof(ConfigureNamedOptions<>).MakeGenericType(t);
                        //var parameterExp = Expression.Parameter(t);
                        //var bindMethod = typeof(ConfigurationBinder).GetMethod("Bind", new Type[] { typeof(IConfiguration), typeof(object) });
                        //var bindAction = Expression.Lambda(Expression.Call(bindMethod, Expression.Constant(config), parameterExp), parameterExp).Compile();
                        //var namedOptionsInstance = Activator.CreateInstance(namedOptionsImpType, new object[] { name, bindAction});
                        //services.AddSingleton(configOptionsInterfaceType, namedOptionsInstance);

                        //使用NamedConfigureFromConfigurationOptions简化
                        var configOptionsInterfaceType = typeof(IConfigureOptions<>).MakeGenericType(t);
                        var namedOptionsImpType = typeof(NamedConfigureFromConfigurationOptions<>).MakeGenericType(t);
                        var namedOptionsInstance = Activator.CreateInstance(namedOptionsImpType, new object[] { name, config, null });
                        services.AddSingleton(configOptionsInterfaceType, namedOptionsInstance);
                    }
                });
            }
        }
    }
}