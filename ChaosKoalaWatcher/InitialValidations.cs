using CustomTypes;
using System;
using System.Configuration;
using System.Dynamic;
using System.IO;
using Utils;

namespace ChaosKoalaWatcher
{
    static class InitialValidations
    {
        private static Monad<dynamic> ValidationFailureCase(this string val)
        {
            Console.Beep();
            val.WriteLine();
            Console.ReadKey();
            return Monad.None<dynamic>();
        }

        public static Monad<dynamic> Validate(string[] args)
        {
            if (!args.HasLength() || !(args.Length <= 2))
            {
                return
                    "Validation Failure: Two parameters are required:  (1) the Directory path and (2) the filename pattern (filter)."
                    .ValidationFailureCase();
            }

            string path = args[0];
            string filter = args[1];
            if (!path.HasLength())
            {
                return "Validation Failure:  A path parameter is required as the first parameter."
                    .ValidationFailureCase();
            }

            if (!filter.HasLength())
            {
                return "Validation Failure:  A filter parameter is required as the second parameter."
                    .ValidationFailureCase();
            }

            bool isUNC = IsUNCPath(path);
            if (!isUNC)
            {
                string fullPath = GetFullPath(path);
                if (fullPath.HasLength())
                    path = fullPath;
                else
                {
                    return "Validation Failure:  A valid (well-formed) path parameter is required."
                        .ValidationFailureCase();
                }
            }

            string strDelay = ConfigurationManager.AppSettings["DelayForChecking"];
            var delay = strDelay.ToInt();
            if (!delay.HasValue() || delay.Value < 1)
            {
                "Validation Failure: There must be a specified delay in the config file (in milliseconds)."
                    .ValidationFailureCase();
            }

            dynamic retVal = new ExpandoObject();
            retVal.Path = path;
            retVal.Filter = filter;
            retVal.Delay = delay.Value;

            return Monad.Create<dynamic>(retVal);
        }

        private static string GetFullPath(string path)
        {
            string fullPath = string.Empty;

            try
            {
                //if the following statement throws an exception, the path is invalid.
                //so we need to trap it to avoid an error throwing here.
                //if we have no path to return, we will know it is invalid anyway.
                fullPath = Path.GetFullPath(path);
            }
            finally { }

            return fullPath;
        }

        private static bool IsUNCPath(string path)
        {
            Uri uri;
            bool isUNC =
                Uri.TryCreate(path, UriKind.RelativeOrAbsolute, out uri) &&
                    !uri.IsNull() && uri.IsWellFormedOriginalString() && uri.IsUnc;
            return isUNC;
        }


    }
}
