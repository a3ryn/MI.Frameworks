using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NuGetPackageCreator
{
    public class NuGetPackageConfig
    {
        /// <summary>
        /// Creates the <see cref="NuGetPackageConfig"/> settings that drive package creation
        /// </summary>
        /// <param name="a">The primary assembly to be packaged</param>
        /// <param name="outdir">The directory to which the produced package will be published</param>
        /// <param name="templateResource">The name of the .nuspec embedded resource to use</param>
        /// <returns></returns>
        public static NuGetPackageConfig Create(Assembly a, string outdir, string templateResource)
        {
            return new NuGetPackageConfig
            {
                NuspecTemplateName = templateResource,
                OutputDirectory = outdir,
                Version = GetSemanticVersion(a),
                WorkingDirectory = GetWorkingDirectory(templateResource),
                InputFiles = GetInputFiles(a)

            };

        }

        static string GetSemanticVersion(Assembly a)
        {
            var version = a.GetName().Version;
            return string.Join(".", version.Major, version.Minor, version.Build);
        }

        static IReadOnlyList<string> GetInputFiles(Assembly a)
        {
            var assemblyPath = a.CodeBase.Replace("file:///", String.Empty);
            var pdbPath = Path.ChangeExtension(assemblyPath, ".pdb");
            var docPath = Path.ChangeExtension(assemblyPath, ".xml");
            return new[] { assemblyPath, pdbPath, docPath };
        }

        static string GetWorkingDirectory(string label)
        {
            var dir = Path.Combine(Path.GetTempPath(), label);
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
            Directory.CreateDirectory(dir);
            return dir;
        }


        public string WorkingDirectory { get; set; }
        public string OutputDirectory { get; set; }
        public string Version { get; set; }
        public IReadOnlyList<string> InputFiles { get; set; }
        public string NuspecTemplateName { get; set; }
        public string OutputNuspecPath => Path.Combine(WorkingDirectory, NuspecTemplateName);
    }

    static class NugetPackageBuilder
    {
        static string GetResourceText(string partialName)
        {
            var ass = Assembly.GetExecutingAssembly();
            var resname = ass.GetManifestResourceNames().First(x => x.Contains(partialName));
            using (var stream = ass.GetManifestResourceStream(resname))
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();

        }

        static string CreateNuspecText(NuGetPackageConfig config) =>
            GetResourceText(config.NuspecTemplateName).Replace("$VERSION$", config.Version);

        public static void BuildPackage(NuGetPackageConfig config)
        {
            var workdir = config.WorkingDirectory;
            if (Directory.Exists(workdir))
                Directory.Delete(workdir, true);
            Directory.CreateDirectory(workdir);


            var nuspec = CreateNuspecText(config);
            File.WriteAllText(config.OutputNuspecPath, nuspec);

            var libdir = Path.Combine(workdir, @"lib\net461");
            Directory.CreateDirectory(libdir);
            foreach (var file in config.InputFiles)
            {
                try
                {
                    File.Copy(file, Path.Combine(libdir, Path.GetFileName(file)));
                }
                catch (FileNotFoundException)
                {
                    if (file.EndsWith("xml"))
                        continue;
                    throw;
                }
            }

            var p = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    FileName = "nuget.exe",
                    Arguments =
                        $"pack \"{config.OutputNuspecPath}\" -Verbosity detailed -OutputDirectory \"{config.OutputDirectory}\""
                }
            };
            p.Start();
            p.WaitForExit();
        }

    }
}
