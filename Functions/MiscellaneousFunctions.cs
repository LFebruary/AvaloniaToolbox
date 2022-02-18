using System.Reflection;

namespace AvaloniaToolbox.Functions
{
    public static partial class MiscellaneousFunctions
    {
        private static Assembly _executingAssembly => Assembly.GetExecutingAssembly();

        private static object[] _customAttributes => _executingAssembly.GetCustomAttributes(false);

        private static T? GetAttribute<T>() where T: Attribute
        {
            foreach (object o in _customAttributes)
	        {
		        if (o.GetType() == typeof(T))
                {
                    return (T)o;
                }
	        }
            return null;
        }

        public static string CopyrightText => GetAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? "NAN";
        public static string VersionSuffix
        {
            get
            {
                if (CustomDebug.IsDebug)
                {
                    if (_rawVersionText?.Count(i => i == '.') == 3)
                    {
                        if (_rawVersionText.Count(i => i == '.') == 3)
                        {
                            int index = _rawVersionText.LastIndexOf('.');
                            if (index != -1)
                            {
                                return $"-rev{_rawVersionText.Substring(index + 1)}";
                            }
                        }
                    }
                }

                return string.Empty;
            }
        }
        public static string VersionText        => string.IsNullOrWhiteSpace(Version) ? "NAN" : $"Version {Version}{VersionSuffix}";
        private static string? _rawVersionText => GetAttribute<AssemblyFileVersionAttribute>()?.Version;
        public static string? Version
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_rawVersionText) == false)
                {
                    if (_rawVersionText.Count(i => i == '.') == 3)
                    {
                        int index = _rawVersionText.LastIndexOf('.');
                        if (index != -1)
                        {
                            return _rawVersionText.Substring(0, index);
                        }
                    }
                }

                return _rawVersionText;
            }
        }
        public static string DescriptionText    => GetAttribute<AssemblyDescriptionAttribute>()?.Description ?? "NAN";
        public static string AuthorText         => GetAttribute<AssemblyCompanyAttribute>()?.Company ?? "NAN";
    }
}
