﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kentico.KInspector.Core;

using Ninject;
using Ninject.Extensions.Conventions;

namespace Kentico.KInspector.Modules.Export
{
    public class ExportModuleLoader
    {
        private static readonly IDictionary<string, IExportModule> mModules = new Dictionary<string, IExportModule>(StringComparer.InvariantCultureIgnoreCase);
        

        /// <summary>
        /// Loads all the modules from the assemblies that are in the same directory as an executing assembly.
        /// Thanks to this loader, you can add your own DLL with <see cref="IModule"/> implementations.
        /// </summary>
        public static ICollection<IExportModule> Modules
        {
            get
            {
                if (mModules.Count == 0)
                {
                    LoadModules();
                }

                return mModules.Values;
            }
        }


        private static void LoadModules()
        {
            var kernel = new StandardKernel();
            kernel.Bind(c => c
                    .FromAssembliesMatching("*")
                        .SelectAllClasses()
                        .InheritedFrom<IExportModule>()
                        .BindAllInterfaces());

            foreach (var module in kernel.GetAll<IExportModule>())
            {
                string name = module.ModuleMetaData.ModuleCodeName;
                if (mModules.ContainsKey(name))
                {
                    throw new ArgumentException("Module with code name '{0}' already exists!", name);
                }

                mModules.Add(name, module);
            }
        }


        public static IExportModule GetModule(string moduleName)
        {
            if (mModules.Count == 0)
            {
                LoadModules();
            }

            return mModules[moduleName];
        }
    }
}
