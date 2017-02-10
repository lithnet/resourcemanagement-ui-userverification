using System;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using Microsoft.IdentityManagement.SmsServiceProvider;

namespace Lithnet.ResourceManagement.UI.UserVerification
{
    internal static class SmsProvider
    {
        private static ISmsServiceProvider provider;

        private static ISmsServiceProvider Provider
        {
            get
            {
                if (provider == null)
                {
                    LoadSmsProvider();
                }

                return provider;
            }
        }

        public static void SendSms(string mobileNumber, string message)
        {
            SmsProvider.Provider.SendSms(mobileNumber, message, Guid.NewGuid(), null);
        }

        public static void LoadSmsProvider()
        {
            Trace.Write("Loading SMS provider");

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;


            string path = Environment.ExpandEnvironmentVariables(AppConfigurationSection.CurrentConfig.SmsServiceProviderDll);

            Trace.WriteLine($"Attempting to load provider from {path}");

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"The SMS provider DLL was not found at {path}. If the file exists in another location, specify that in the smsServiceProviderDll section of the web.config file");
            }

            Assembly assembly = Assembly.LoadFile(path);

            Trace.WriteLine($"Loaded assembly");

            Type[] types = assembly.GetExportedTypes();

            Trace.WriteLine($"Got types");

            foreach (Type t in types)
            {
                if (typeof(ISmsServiceProvider).IsAssignableFrom(t))
                {
                    Trace.WriteLine($"Found type that implements ISmsServiceProvider");
                    SmsProvider.provider = (ISmsServiceProvider)Activator.CreateInstance(t);
                    Trace.WriteLine($"Provider loaded");
                    return;
                }
            }

            Trace.WriteLine($"Did not find any types that implement ISmsServiceProvider");

            throw new InvalidOperationException("The specified SMS provider did not contain an ISmsServiceProvider interface");
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Trace.WriteLine($"Attempting to resolve assembly {args.Name}");

            string folderPath = Path.GetDirectoryName(Environment.ExpandEnvironmentVariables(AppConfigurationSection.CurrentConfig.SmsServiceProviderDll));
            string assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
            if (!File.Exists(assemblyPath))
            {
                Trace.WriteLine($"Assembly not found at path {assemblyPath}");
                return null;
            }

            Trace.WriteLine($"Assembly found at path {assemblyPath}");
            return Assembly.LoadFrom(assemblyPath);
        }

    }
}