using System;
using System.Text;

namespace db
{
    public static class globals
    {
        //
        private static db _database = null;
        public static db database
        {
            get {
                if (_database == null) 
                {
                    _database = new db();
                }
                return _database; 
            }
            set { _database = value; }
        }
        
        //
    }
}
