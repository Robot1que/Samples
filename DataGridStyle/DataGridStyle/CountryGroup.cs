using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DataGridStyle
{
    [DebuggerDisplay("GroupName = {GroupName}")]
    public class CountryGroup
    {
        private readonly string _groupName;
        public string GroupName
        {
            get
            {
                return this._groupName;
            }
        }
        

        private readonly CountryItem[] _countries;
        public CountryItem[] Countries
        {
            get
            {
                return this._countries;
            }
        }

        public CountryGroup(string groupName, CountryItem[] countries)
        {
            this._groupName = groupName;
            this._countries = countries;
        }
    }
}
