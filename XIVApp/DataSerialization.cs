using System;
using System.Collections.Generic;

namespace XIVApp
{
    [Serializable]
    public class DataSerialization
    {
        // prop here
        //public string something1 = "test1";
        //public string somethign2 = "test2";
        //public int someint = 55;
        public DataSerialization()
        {
            // Constructor
        }

        public List<string> Settings = new List<String>();

    }
}



// http://www.jonasjohn.de/snippets/csharp/xmlserializer-example.htm
// http://blogs.msdn.com/b/sowmy/archive/2008/10/04/serializing-internal-types-using-xmlserializer.aspx
// 