using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimBridge
{
    class Settings
    {
        private object _lock;
        private string _path;
        private Dictionary<string, object> _data;
        private JsonSerializer _serializer;

        public Settings()
        {
            _lock = new object();
            _data = new Dictionary<string, object>();
            _serializer = new JsonSerializer();
            _serializer.Formatting = Formatting.Indented;
        }

        public Settings(string path) : this()
        {
            _path = path;
            if (File.Exists(path))
            {
                Load(path);
            }
        }

        public bool Load()
        {
            if (!string.IsNullOrEmpty(_path) && File.Exists(_path))
            {
                Load(_path);
                return true;
            }
            return false;
        }

        public bool Save()
        {
            if (!string.IsNullOrEmpty(_path))
            {
                Save(_path);
                return true;
            }
            return false;
        }

        public void Load(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open))
            using (var sr = new StreamReader(fs))
            {
                _data = (Dictionary<string, object>)_serializer.Deserialize(sr, typeof(Dictionary<string, object>));
            }
        }

        public void Save(string path)
        {
            using (var fs = new FileStream(path, FileMode.Create))
            using (var sw = new StreamWriter(fs))
            {
                _serializer.Serialize(sw, _data);
            }
        }

        public T Get<T>(string key)
        {
            lock (_lock)
            {
                object value;
                var result = _data.TryGetValue(key, out value);
                if (result)
                {
                    return (T)value;
                }
                else
                {
                    throw new KeyNotFoundException(string.Format("Key '{0}' not found in configuration.", key));
                }
            }
        }

        public T Get<T>(string key, T defaultValue, bool addDefaultValue = false)
        {
            lock (_lock)
            {
                object value;
                var result = _data.TryGetValue(key, out value);
                if (result)
                {
                    return (T)value;
                }
                else
                {
                    if (addDefaultValue)
                    {
                        _data.Add(key, defaultValue);
                    }
                    return defaultValue;
                }
            }
        }

        public void Set<T>(string key, T value)
        {
            lock (_lock)
            {
                var result = _data.ContainsKey(key);
                if (!result)
                {
                    _data.Add(key, value);
                }
                else
                {
                    _data[key] = value;
                }
            }
        }
    }
}
