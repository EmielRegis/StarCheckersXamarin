using System;
using System.IO;
using System.Xml.Serialization;

namespace StarCheckersWindows
{
    class XmlManager<T>
    {
        public Type Type { set; get; }

        public XmlManager()
        {
            Type = typeof (T);
        }

        public T Load(string path)
        {
            T instance;

			#if ANDROID
			using (TextReader reader = new StreamReader(StarCheckersWindows.Game1.Activity.Assets.Open(path)))
			{
				XmlSerializer xml = new XmlSerializer(Type);
				instance = (T) xml.Deserialize(reader);
				return instance;
			}
			#else
            using (TextReader reader = new StreamReader(path))
            {
                XmlSerializer xml = new XmlSerializer(Type);
                instance = (T) xml.Deserialize(reader);
                return instance;
            }
			#endif
        }

        public void Save(string path, object obj)
        {
            using (TextWriter writer = new StreamWriter(path))
            {
                XmlSerializer xml = new XmlSerializer(Type);
                xml.Serialize(writer, obj);
            }
        }
    }
}
