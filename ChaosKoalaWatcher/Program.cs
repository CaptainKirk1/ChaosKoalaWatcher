using Domain;
using System;
using System.Collections.Concurrent;
using System.Timers;
using Utils;

namespace ChaosKoalaWatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            var initialValues = InitialValidations.Validate(args);
            if (!initialValues.HasValue()) return; //get out if validations failed

            var vals = initialValues.Value;
            string path = vals.Path;
            string filter = vals.Filter;
            int delay = vals.Delay;

            $"Currently watching {path} for file pattern {filter}.".WriteLine();

            //instantiate dependencies/composition root for poor man's injection, as IOC container would be overarchitecting
            var sync = new Sync();
            var IO = new IOAdapters(path, filter);
            var fileInfoBag = IO.GetInitializedDirectoryListAsDictionary();
            var currentFilesBag = new ConcurrentDictionary<string, bool>();
            var renamedFilesBag = new ConcurrentDictionary<string, string>();
            var watcher = new Watcher(sync);

            //start file watcher and start timer
            watcher.Start(path, filter);
            var timer = new Timer(delay);
            // Hook up the Elapsed event for the timer, inject dependencies into implementation
            timer.Elapsed += (source, e) => new Implementation(sync, IO, fileInfoBag, currentFilesBag);
            timer.AutoReset = true;
            timer.Enabled = true;

            Console.ReadKey();
        }

    }
}
