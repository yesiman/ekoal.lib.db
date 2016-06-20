using System;
using System.Collections.Generic;
using System.Text;

namespace db
{
    public static class toolBox
    {
        public static String doubleCot(String str)
        {
            str = str.Replace("'", "''");
            return str.Replace('"', '\"');
        }
    }
}
