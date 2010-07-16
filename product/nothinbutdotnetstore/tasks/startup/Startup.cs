using System;
using System.Collections.Generic;
using System.Web.Compilation;
using nothinbutdotnetstore.infrastructure.containers;
using nothinbutdotnetstore.infrastructure.containers.basic;
using nothinbutdotnetstore.infrastructure.logging;
using nothinbutdotnetstore.infrastructure.logging.simple;
using nothinbutdotnetstore.tasks.stubs;
using nothinbutdotnetstore.web.core;
using nothinbutdotnetstore.web.core.stubs;

namespace nothinbutdotnetstore.tasks.startup
{
    public class Startup
    {
        static Dictionary<Type, DependencyFactory> factories = new Dictionary<Type, DependencyFactory>();

        public static void run()
        {
            new ConfigureCoreServices(factories).run();
            configure_front_controller(factories);
            configure_service_layer(factories);
        }

        static void add_to_factories<TypeToRegister>(object creation)
        {
            factories.Add(typeof(TypeToRegister), create_factory(creation));
        }

        static void configure_service_layer(Dictionary<Type, DependencyFactory> factories)
        {
            add_to_factories<CatalogTasks>(new StubCatalogTasks());
        }

        static void configure_front_controller(Dictionary<Type, DependencyFactory> factories)
        {
            add_to_factories<FrontController>(
                new DefaultFrontController(new DefaultCommandRegistry(new StubFakeCommandSet())));
            add_to_factories<RequestFactory>(new StubRequestFactory());
            add_to_factories<ResponseEngine>(
                new DefaultResponseEngine(new DefaultViewFactory(new DefaultViewRegistry(null))));

            DefaultViewFactory.page_factory = BuildManager.CreateInstanceFromVirtualPath;
        }


        static SingletonFactory create_factory(object dependency)
        {
            return new SingletonFactory(new BasicDependencyFactory(() => dependency));
        }
    }
}