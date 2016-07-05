using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGetPackageCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            NugetPackageBuilder.BuildPackage(
                NuGetPackageConfig.Create(
                    typeof(Shared.Core.Common.Caching.ICache).Assembly, 
                    Environment.GetEnvironmentVariable("NUGETLIB"),
                    "Shared.Core.Common.nuspec"));

            NugetPackageBuilder.BuildPackage(
                NuGetPackageConfig.Create(
                    typeof(Shared.Frameworks.Caching.CacheManager).Assembly,
                    Environment.GetEnvironmentVariable("NUGETLIB"),
                    "Shared.Frameworks.Caching.nuspec"));

            NugetPackageBuilder.BuildPackage(
                NuGetPackageConfig.Create(
                    typeof(Shared.Frameworks.DataAccess.DataAdapter).Assembly,
                    Environment.GetEnvironmentVariable("NUGETLIB"),
                    "Shared.Frameworks.DataAccess.nuspec"));
            */
        }
    }
}
