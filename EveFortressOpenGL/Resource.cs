using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveFortressClient
{
    public class Resource<T>
    {
        public bool FileChanged { get; set; }
        public Func<string, T> Recalculate { get; set; }

        string fullPath;
        public string FullPath
        {
            get
            {
                return fullPath;
            }
            set
            {
                watcher.Path = Path.GetFullPath(Path.GetDirectoryName(value));
                fullPath = value;
            }
        }

        T value;
        public T Value 
        { 
            get
            {
                if (FileChanged)
                {
                    value = Recalculate(FullPath);
                    FileChanged = false;
                }
                return value;
            }
        }

        FileSystemWatcher watcher = new FileSystemWatcher();

        public Resource(string fullPath, Func<string, T> recalculate)
        {
            FullPath = fullPath;
            Recalculate = recalculate;
            value = recalculate(fullPath);
            watcher.Changed += watcher_Changed;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = Path.GetFileName(FullPath);

            watcher.EnableRaisingEvents = true;
        }

        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            FileChanged = true;
        }

        public static implicit operator T(Resource<T> resource)
        {
            return resource.Value;
        }
    }
}
